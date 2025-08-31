# ðŸ’° ERD - DOMÃNIO DE VENDAS

## ðŸŽ¯ VisÃ£o Geral
Diagrama Entity-Relationship completo do DomÃ­nio de Vendas, mostrando o fluxo de Customer â†’ OrderEntry â†’ OrderItem â†’ Product e suas integraÃ§Ãµes com os domÃ­nios de ProduÃ§Ã£o e Financeiro. Este domÃ­nio Ã© responsÃ¡vel por capturar e gerenciar transaÃ§Ãµes comerciais.

## ðŸ—„ï¸ Diagrama de Entidades e Relacionamentos

```mermaid
erDiagram
    %% === DOMÃNIO DE VENDAS ===
    
    %% === CLIENTE ===
    CUSTOMER {
        string Id PK "GUID Ãºnico"
        string Name "Nome/RazÃ£o Social"
        string Document "CPF/CNPJ"
        string Email "Email principal"
        string Phone "Telefone principal"
        string Address "EndereÃ§o completo"
        string City "Cidade"
        string State "Estado"
        string ZipCode "CEP"
        string CustomerType "Individual|Company"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criaÃ§Ã£o"
        datetime ModifiedDate "Data de modificaÃ§Ã£o"
        string Notes "ObservaÃ§Ãµes"
    }

    %% === PEDIDO PRINCIPAL ===
    ORDER_ENTRY {
        string Id PK "GUID Ãºnico"
        string OrderNumber "NÃºmero sequencial"
        string CustomerId FK "Cliente"
        datetime OrderDate "Data do pedido"
        datetime DeliveryDate "Data de entrega"
        datetime RequestedDate "Data solicitada"
        string OrderType "Delivery|Pickup"
        string DeliveryAddress "EndereÃ§o de entrega"
        decimal TotalValue "Valor total calculado"
        string OrderStatus "Pending|Confirmed|InProduction|ReadyForDelivery|Delivered|Invoiced|Cancelled"
        string PaymentTerms "CondiÃ§Ãµes de pagamento"
        string PaymentMethod "MÃ©todo de pagamento"
        string Notes "ObservaÃ§Ãµes"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criaÃ§Ã£o"
        datetime ModifiedDate "Data de modificaÃ§Ã£o"
        string CreatedBy "UsuÃ¡rio criador"
    }

    %% === ITEM DO PEDIDO ===
    ORDER_ITEM {
        string Id PK "GUID Ãºnico"
        string OrderEntryId FK "Pedido pai"
        string ProductId FK "Produto"
        int Quantity "Quantidade solicitada"
        decimal UnitPrice "PreÃ§o unitÃ¡rio"
        decimal TotalPrice "PreÃ§o total (Qty * Unit)"
        string ProductConfiguration "JSON com configuraÃ§Ãµes"
        string ItemStatus "Pending|Confirmed|InProduction|Completed"
        string Notes "ObservaÃ§Ãµes do item"
        int LineNumber "NÃºmero da linha"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criaÃ§Ã£o"
        datetime ModifiedDate "Data de modificaÃ§Ã£o"
    }

    %% === INTEGRAÃ‡Ã•ES COM OUTROS DOMÃNIOS ===

    %% PRODUTO (DOMÃNIO DE PRODUTO)
    PRODUCT {
        string Id PK "GUID Ãºnico"
        string ProductType "Simple|Composite|Group"
        string Name "Nome do produto"
        decimal Price "PreÃ§o base"
        decimal Cost "Custo do produto"
        string CategoryId FK "Categoria"
        string StateCode "Active|Inactive"
    }

    %% DEMANDA (DOMÃNIO DE PRODUÃ‡ÃƒO)
    DEMAND {
        string Id PK "GUID Ãºnico"
        string OrderItemId FK "Item do pedido origem"
        string ProductId FK "Produto a ser produzido"
        int Quantity "Quantidade a produzir"
        datetime RequiredDate "Data limite"
        string DemandStatus "Pending|Confirmed|InProduction|Completed|Cancelled"
        string Notes "ObservaÃ§Ãµes"
        datetime CreatedDate "Data de criaÃ§Ã£o"
        datetime ModifiedDate "Data de modificaÃ§Ã£o"
    }

    %% COMPOSIÃ‡ÃƒO DO PRODUTO (DOMÃNIO DE PRODUÃ‡ÃƒO)
    PRODUCT_COMPOSITION {
        string Id PK "GUID Ãºnico"
        string DemandId FK "Demanda pai"
        string ProductComponentId FK "Componente"
        string HierarchyName "Nome da hierarquia"
        int Quantity "Quantidade do componente"
        string Status "Pending|InProgress|Completed"
        datetime StartTime "InÃ­cio da produÃ§Ã£o"
        datetime CompletionTime "Fim da produÃ§Ã£o"
        string Notes "ObservaÃ§Ãµes"
    }

    %% CONTA A RECEBER (DOMÃNIO FINANCEIRO)
    ACCOUNT_RECEIVABLE {
        string Id PK "GUID Ãºnico"
        string OrderEntryId FK "Pedido origem"
        string CustomerId FK "Cliente"
        decimal TotalAmount "Valor total a receber"
        decimal PaidAmount "Valor jÃ¡ recebido"
        datetime DueDate "Data de vencimento"
        string AccountStatus "Pending|PartiallyPaid|Paid|Overdue|Cancelled"
        string PaymentTerms "CondiÃ§Ãµes de pagamento"
        datetime CreatedDate "Data de criaÃ§Ã£o"
        datetime ModifiedDate "Data de modificaÃ§Ã£o"
    }

    %% ==========================================
    %% RELACIONAMENTOS DIRETOS
    %% ==========================================

    %% FLUXO PRINCIPAL DE VENDAS
    CUSTOMER ||--o{ ORDER_ENTRY : "faz pedidos"
    ORDER_ENTRY ||--o{ ORDER_ITEM : "contÃ©m itens"
    ORDER_ITEM }o--|| PRODUCT : "referencia produto"

    %% ==========================================
    %% INTEGRAÃ‡Ã•ES COM OUTROS DOMÃNIOS
    %% ==========================================

    %% VENDAS â†’ PRODUÃ‡ÃƒO (Customer-Supplier)
    ORDER_ITEM ||--o{ DEMAND : "gera demandas"
    DEMAND ||--o{ PRODUCT_COMPOSITION : "detalha composiÃ§Ã£o"

    %% VENDAS â†’ FINANCEIRO (Customer-Supplier)
    ORDER_ENTRY ||--o{ ACCOUNT_RECEIVABLE : "gera contas a receber"
    CUSTOMER ||--o{ ACCOUNT_RECEIVABLE : "deve pagar"

    %% ==========================================
    %% STYLING POR DOMÃNIO
    %% ==========================================
    
    %% VENDAS = Laranja (#f36b21)
    CUSTOMER {
        background-color "#f36b21"
        color "white"
        border-color "#f36b21"
    }
    
    ORDER_ENTRY {
        background-color "#f36b21"
        color "white"
        border-color "#f36b21"
    }
    
    ORDER_ITEM {
        background-color "#f36b21"
        color "white"
        border-color "#f36b21"
    }

    %% PRODUTO = Verde (#00a86b)
    PRODUCT {
        background-color "#00a86b"
        color "white"
        border-color "#00a86b"
    }

    %% PRODUÃ‡ÃƒO = Dourado (#fba81d)
    DEMAND {
        background-color "#fba81d"
        color "black"
        border-color "#fba81d"
    }
    
    PRODUCT_COMPOSITION {
        background-color "#fba81d"
        color "black"
        border-color "#fba81d"
    }

    %% FINANCEIRO = Azul Escuro (#083e61)
    ACCOUNT_RECEIVABLE {
        background-color "#083e61"
        color "white"
        border-color "#083e61"
    }
```

## ðŸ“‹ Detalhes das Entidades

### **ðŸ‘¤ CUSTOMER**
- **PropÃ³sito**: Representar clientes pessoa fÃ­sica ou jurÃ­dica
- **Tipos**: Individual (CPF) ou Company (CNPJ) 
- **CaracterÃ­sticas**: Dados de contato, endereÃ§amento, observaÃ§Ãµes
- **Relacionamentos**: 1:N com OrderEntry, 1:N com AccountReceivable

### **ðŸ“„ ORDER_ENTRY**
- **PropÃ³sito**: CabeÃ§alho do pedido de venda
- **Status Flow**: Pending â†’ Confirmed â†’ InProduction â†’ ReadyForDelivery â†’ Delivered â†’ Invoiced
- **CaracterÃ­sticas**: Datas (pedido/entrega), endereÃ§o, condiÃ§Ãµes de pagamento
- **Relacionamentos**: N:1 com Customer, 1:N com OrderItem, 1:N com AccountReceivable

### **ðŸ“¦ ORDER_ITEM**
- **PropÃ³sito**: Item especÃ­fico dentro de um pedido
- **CaracterÃ­sticas**: Quantidade, preÃ§os, configuraÃ§Ã£o de produtos
- **IntegraÃ§Ã£o CrÃ­tica**: Gera automaticamente registros de Demand
- **Relacionamentos**: N:1 com OrderEntry, N:1 com Product, 1:N com Demand

### **ðŸ”— Entidades de IntegraÃ§Ã£o**

#### **PRODUCT** *(ReferÃªncia do DomÃ­nio de Produto)*
- **Relacionamento**: 1:N com OrderItem
- **Tipos**: Simple, Composite, Group (impacta criaÃ§Ã£o de Demands)

#### **DEMAND** *(Gerada no DomÃ­nio de ProduÃ§Ã£o)*
- **Relacionamento**: N:1 com OrderItem (origem)
- **PropÃ³sito**: Traduzir item de venda em ordem de produÃ§Ã£o
- **Status Inicial**: Sempre "Pending" na criaÃ§Ã£o

#### **PRODUCT_COMPOSITION** *(Detalhamento de ProduÃ§Ã£o)*
- **Relacionamento**: N:1 com Demand
- **PropÃ³sito**: Detalhar componentes especÃ­ficos para produtos compostos

#### **ACCOUNT_RECEIVABLE** *(Gerada no DomÃ­nio Financeiro)*
- **Relacionamento**: N:1 com OrderEntry (origem)
- **PropÃ³sito**: Controlar valores a receber do cliente

## ðŸ”„ Fluxos de IntegraÃ§Ã£o CrÃ­ticos

### **ðŸ“Š CriaÃ§Ã£o de OrderItem â†’ Demand (AutomÃ¡tica)**

**Regra de NegÃ³cio**:
```
1 OrderItem pode gerar 1:N Demand dependendo do ProductType:

- ProductType.Simple: 1 OrderItem â†’ 1 Demand
- ProductType.Composite: 1 OrderItem â†’ 1 Demand + N ProductComposition
- ProductType.Group: 1 OrderItem â†’ N Demand (um por produto concreto no grupo)
```

**Processo**:
1. OrderItem criado/editado
2. Sistema identifica Product.ProductType
3. **Simple**: Cria 1 Demand com quantity = OrderItem.Quantity
4. **Composite**: Cria 1 Demand + ProductComposition baseada em configuraÃ§Ã£o
5. **Group**: Explode grupo e cria 1 Demand por produto concreto
6. Todas Demands iniciam com DemandStatus = "Pending"

### **ðŸ’° CriaÃ§Ã£o de OrderEntry â†’ AccountReceivable (AutomÃ¡tica)**

**Processo**:
1. OrderEntry confirmado (OrderStatus = "Confirmed")
2. Sistema cria AccountReceivable:
   - TotalAmount = OrderEntry.TotalValue
   - DueDate baseada em PaymentTerms
   - AccountStatus = "Pending"
3. Se pagamento parcelado: cria mÃºltiplas AccountReceivable

### **ðŸ“ˆ SincronizaÃ§Ã£o de Status**

**OrderEntry.OrderStatus â†” Demand.DemandStatus**:
```
OrderEntry "Confirmed" â†’ Todas Demands passam para "Confirmed"
OrderEntry "InProduction" â†’ Demands passam para "InProduction"
OrderEntry "Cancelled" â†’ Demands passam para "Cancelled"
```

## ðŸ“Š Regras de ValidaÃ§Ã£o CrÃ­ticas

### **OrderEntry**
- NÃ£o pode ser confirmado sem Customer e pelo menos 1 OrderItem
- TotalValue = Î£(OrderItem.TotalPrice)
- DeliveryDate >= OrderDate
- Status "Cancelled" cancela todas Demands e AccountReceivables relacionadas

### **OrderItem**
- Product deve estar ativo (StateCode = "Active")
- Quantity > 0
- UnitPrice deve ser >= Product.Cost (validaÃ§Ã£o de margem)
- ProductConfiguration obrigatÃ³ria para ProductType.Composite

### **IntegraÃ§Ãµes**
- Demand sÃ³ pode ser cancelada se OrderItem for cancelado
- AccountReceivable sÃ³ pode ser cancelada se OrderEntry for cancelado
- ProductComposition gerada automaticamente baseada em ProductConfiguration

## ðŸŽ¯ Eventos de DomÃ­nio Gerados

- **OrderCreated**: Nova OrderEntry criada
- **OrderConfirmed**: OrderEntry confirmada â†’ Gera Demands + AccountReceivable
- **OrderItemAdded**: Novo OrderItem â†’ Gera Demand correspondente
- **OrderItemUpdated**: OrderItem modificado â†’ Atualiza Demand relacionada
- **OrderCancelled**: OrderEntry cancelada â†’ Cancela Demands + AccountReceivables
- **OrderDelivered**: OrderEntry entregue â†’ Atualiza status de produÃ§Ã£o

---

**Arquivo**: `02-sales-domain-erd.md`  
**DomÃ­nio**: Vendas (#f36b21)  
**Tipo**: Entity-Relationship Diagram  
**NÃ­vel**: Detalhado + IntegraÃ§Ãµes entre DomÃ­nios
