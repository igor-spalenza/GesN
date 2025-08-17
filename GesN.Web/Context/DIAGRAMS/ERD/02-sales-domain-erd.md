# 💰 ERD - DOMÍNIO DE VENDAS

## 🎯 Visão Geral
Diagrama Entity-Relationship completo do Domínio de Vendas, mostrando o fluxo de Customer → OrderEntry → OrderItem → Product e suas integrações com os domínios de Produção e Financeiro. Este domínio é responsável por capturar e gerenciar transações comerciais.

## 🗄️ Diagrama de Entidades e Relacionamentos

```mermaid
erDiagram
    %% === DOMÍNIO DE VENDAS ===
    
    %% === CLIENTE ===
    CUSTOMER {
        string Id PK "GUID único"
        string Name "Nome/Razão Social"
        string Document "CPF/CNPJ"
        string Email "Email principal"
        string Phone "Telefone principal"
        string Address "Endereço completo"
        string City "Cidade"
        string State "Estado"
        string ZipCode "CEP"
        string CustomerType "Individual|Company"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criação"
        datetime ModifiedDate "Data de modificação"
        string Notes "Observações"
    }

    %% === PEDIDO PRINCIPAL ===
    ORDER_ENTRY {
        string Id PK "GUID único"
        string OrderNumber "Número sequencial"
        string CustomerId FK "Cliente"
        datetime OrderDate "Data do pedido"
        datetime DeliveryDate "Data de entrega"
        datetime RequestedDate "Data solicitada"
        string OrderType "Delivery|Pickup"
        string DeliveryAddress "Endereço de entrega"
        decimal TotalValue "Valor total calculado"
        string OrderStatus "Pending|Confirmed|InProduction|ReadyForDelivery|Delivered|Invoiced|Cancelled"
        string PaymentTerms "Condições de pagamento"
        string PaymentMethod "Método de pagamento"
        string Notes "Observações"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criação"
        datetime ModifiedDate "Data de modificação"
        string CreatedBy "Usuário criador"
    }

    %% === ITEM DO PEDIDO ===
    ORDER_ITEM {
        string Id PK "GUID único"
        string OrderEntryId FK "Pedido pai"
        string ProductId FK "Produto"
        int Quantity "Quantidade solicitada"
        decimal UnitPrice "Preço unitário"
        decimal TotalPrice "Preço total (Qty * Unit)"
        string ProductConfiguration "JSON com configurações"
        string ItemStatus "Pending|Confirmed|InProduction|Completed"
        string Notes "Observações do item"
        int LineNumber "Número da linha"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criação"
        datetime ModifiedDate "Data de modificação"
    }

    %% === INTEGRAÇÕES COM OUTROS DOMÍNIOS ===

    %% PRODUTO (DOMÍNIO DE PRODUTO)
    PRODUCT {
        string Id PK "GUID único"
        string ProductType "Simple|Composite|Group"
        string Name "Nome do produto"
        decimal Price "Preço base"
        decimal Cost "Custo do produto"
        string CategoryId FK "Categoria"
        string StateCode "Active|Inactive"
    }

    %% DEMANDA (DOMÍNIO DE PRODUÇÃO)
    DEMAND {
        string Id PK "GUID único"
        string OrderItemId FK "Item do pedido origem"
        string ProductId FK "Produto a ser produzido"
        int Quantity "Quantidade a produzir"
        datetime RequiredDate "Data limite"
        string DemandStatus "Pending|Confirmed|InProduction|Completed|Cancelled"
        string Notes "Observações"
        datetime CreatedDate "Data de criação"
        datetime ModifiedDate "Data de modificação"
    }

    %% COMPOSIÇÃO DO PRODUTO (DOMÍNIO DE PRODUÇÃO)
    PRODUCT_COMPOSITION {
        string Id PK "GUID único"
        string DemandId FK "Demanda pai"
        string ProductComponentId FK "Componente"
        string HierarchyName "Nome da hierarquia"
        int Quantity "Quantidade do componente"
        string Status "Pending|InProgress|Completed"
        datetime StartTime "Início da produção"
        datetime CompletionTime "Fim da produção"
        string Notes "Observações"
    }

    %% CONTA A RECEBER (DOMÍNIO FINANCEIRO)
    ACCOUNT_RECEIVABLE {
        string Id PK "GUID único"
        string OrderEntryId FK "Pedido origem"
        string CustomerId FK "Cliente"
        decimal TotalAmount "Valor total a receber"
        decimal PaidAmount "Valor já recebido"
        datetime DueDate "Data de vencimento"
        string AccountStatus "Pending|PartiallyPaid|Paid|Overdue|Cancelled"
        string PaymentTerms "Condições de pagamento"
        datetime CreatedDate "Data de criação"
        datetime ModifiedDate "Data de modificação"
    }

    %% ==========================================
    %% RELACIONAMENTOS DIRETOS
    %% ==========================================

    %% FLUXO PRINCIPAL DE VENDAS
    CUSTOMER ||--o{ ORDER_ENTRY : "faz pedidos"
    ORDER_ENTRY ||--o{ ORDER_ITEM : "contém itens"
    ORDER_ITEM }o--|| PRODUCT : "referencia produto"

    %% ==========================================
    %% INTEGRAÇÕES COM OUTROS DOMÍNIOS
    %% ==========================================

    %% VENDAS → PRODUÇÃO (Customer-Supplier)
    ORDER_ITEM ||--o{ DEMAND : "gera demandas"
    DEMAND ||--o{ PRODUCT_COMPOSITION : "detalha composição"

    %% VENDAS → FINANCEIRO (Customer-Supplier)
    ORDER_ENTRY ||--o{ ACCOUNT_RECEIVABLE : "gera contas a receber"
    CUSTOMER ||--o{ ACCOUNT_RECEIVABLE : "deve pagar"

    %% ==========================================
    %% STYLING POR DOMÍNIO
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

    %% PRODUÇÃO = Dourado (#fba81d)
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

## 📋 Detalhes das Entidades

### **👤 CUSTOMER**
- **Propósito**: Representar clientes pessoa física ou jurídica
- **Tipos**: Individual (CPF) ou Company (CNPJ) 
- **Características**: Dados de contato, endereçamento, observações
- **Relacionamentos**: 1:N com OrderEntry, 1:N com AccountReceivable

### **📄 ORDER_ENTRY**
- **Propósito**: Cabeçalho do pedido de venda
- **Status Flow**: Pending → Confirmed → InProduction → ReadyForDelivery → Delivered → Invoiced
- **Características**: Datas (pedido/entrega), endereço, condições de pagamento
- **Relacionamentos**: N:1 com Customer, 1:N com OrderItem, 1:N com AccountReceivable

### **📦 ORDER_ITEM**
- **Propósito**: Item específico dentro de um pedido
- **Características**: Quantidade, preços, configuração de produtos
- **Integração Crítica**: Gera automaticamente registros de Demand
- **Relacionamentos**: N:1 com OrderEntry, N:1 com Product, 1:N com Demand

### **🔗 Entidades de Integração**

#### **PRODUCT** *(Referência do Domínio de Produto)*
- **Relacionamento**: 1:N com OrderItem
- **Tipos**: Simple, Composite, Group (impacta criação de Demands)

#### **DEMAND** *(Gerada no Domínio de Produção)*
- **Relacionamento**: N:1 com OrderItem (origem)
- **Propósito**: Traduzir item de venda em ordem de produção
- **Status Inicial**: Sempre "Pending" na criação

#### **PRODUCT_COMPOSITION** *(Detalhamento de Produção)*
- **Relacionamento**: N:1 com Demand
- **Propósito**: Detalhar componentes específicos para produtos compostos

#### **ACCOUNT_RECEIVABLE** *(Gerada no Domínio Financeiro)*
- **Relacionamento**: N:1 com OrderEntry (origem)
- **Propósito**: Controlar valores a receber do cliente

## 🔄 Fluxos de Integração Críticos

### **📊 Criação de OrderItem → Demand (Automática)**

**Regra de Negócio**:
```
1 OrderItem pode gerar 1:N Demand dependendo do ProductType:

- ProductType.Simple: 1 OrderItem → 1 Demand
- ProductType.Composite: 1 OrderItem → 1 Demand + N ProductComposition
- ProductType.Group: 1 OrderItem → N Demand (um por produto concreto no grupo)
```

**Processo**:
1. OrderItem criado/editado
2. Sistema identifica Product.ProductType
3. **Simple**: Cria 1 Demand com quantity = OrderItem.Quantity
4. **Composite**: Cria 1 Demand + ProductComposition baseada em configuração
5. **Group**: Explode grupo e cria 1 Demand por produto concreto
6. Todas Demands iniciam com DemandStatus = "Pending"

### **💰 Criação de OrderEntry → AccountReceivable (Automática)**

**Processo**:
1. OrderEntry confirmado (OrderStatus = "Confirmed")
2. Sistema cria AccountReceivable:
   - TotalAmount = OrderEntry.TotalValue
   - DueDate baseada em PaymentTerms
   - AccountStatus = "Pending"
3. Se pagamento parcelado: cria múltiplas AccountReceivable

### **📈 Sincronização de Status**

**OrderEntry.OrderStatus ↔ Demand.DemandStatus**:
```
OrderEntry "Confirmed" → Todas Demands passam para "Confirmed"
OrderEntry "InProduction" → Demands passam para "InProduction"
OrderEntry "Cancelled" → Demands passam para "Cancelled"
```

## 📊 Regras de Validação Críticas

### **OrderEntry**
- Não pode ser confirmado sem Customer e pelo menos 1 OrderItem
- TotalValue = Σ(OrderItem.TotalPrice)
- DeliveryDate >= OrderDate
- Status "Cancelled" cancela todas Demands e AccountReceivables relacionadas

### **OrderItem**
- Product deve estar ativo (StateCode = "Active")
- Quantity > 0
- UnitPrice deve ser >= Product.Cost (validação de margem)
- ProductConfiguration obrigatória para ProductType.Composite

### **Integrações**
- Demand só pode ser cancelada se OrderItem for cancelado
- AccountReceivable só pode ser cancelada se OrderEntry for cancelado
- ProductComposition gerada automaticamente baseada em ProductConfiguration

## 🎯 Eventos de Domínio Gerados

- **OrderCreated**: Nova OrderEntry criada
- **OrderConfirmed**: OrderEntry confirmada → Gera Demands + AccountReceivable
- **OrderItemAdded**: Novo OrderItem → Gera Demand correspondente
- **OrderItemUpdated**: OrderItem modificado → Atualiza Demand relacionada
- **OrderCancelled**: OrderEntry cancelada → Cancela Demands + AccountReceivables
- **OrderDelivered**: OrderEntry entregue → Atualiza status de produção

---

**Arquivo**: `02-sales-domain-erd.md`  
**Domínio**: Vendas (#f36b21)  
**Tipo**: Entity-Relationship Diagram  
**Nível**: Detalhado + Integrações entre Domínios
