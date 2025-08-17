# 🛒 ERD - DOMÍNIO DE COMPRAS

## 🎯 Visão Geral
Diagrama Entity-Relationship completo do Domínio de Compras, mostrando a gestão de fornecedores, ingredientes, ordens de compra e controle de estoque. Inclui o fluxo de criação manual de compras com apoio de IA para interpretação de notas fiscais e controle automático de estoque mínimo.

## 🗄️ Diagrama de Entidades e Relacionamentos

```mermaid
erDiagram
    %% === DOMÍNIO DE COMPRAS ===
    
    %% === FORNECEDOR ===
    SUPPLIER {
        string Id PK "GUID único"
        string Name "Razão Social"
        string TradeName "Nome Fantasia"
        string Document "CNPJ"
        string Email "Email principal"
        string Phone "Telefone principal"
        string ContactPerson "Pessoa de contato"
        string Address "Endereço completo"
        string City "Cidade"
        string State "Estado"
        string ZipCode "CEP"
        string PaymentTerms "Condições pagamento"
        int DeliveryDays "Prazo entrega (dias)"
        decimal MinimumOrderValue "Valor mínimo pedido"
        string SupplierRating "A|B|C|D"
        string BankAccount "Conta bancária"
        string Notes "Observações"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criação"
        datetime ModifiedDate "Data de modificação"
    }

    %% === INGREDIENTE ===
    INGREDIENT {
        string Id PK "GUID único"
        string Name "Nome do ingrediente"
        string Description "Descrição detalhada"
        string Category "Categoria ingrediente"
        string UnitOfMeasure "Unidade medida padrão"
        decimal MinimumStockLevel "Estoque mínimo"
        decimal MaximumStockLevel "Estoque máximo"
        decimal StandardCost "Custo padrão"
        string StorageRequirements "Requisitos armazenagem"
        int ShelfLifeDays "Validade (dias)"
        string IngredientCode "Código interno"
        string Specifications "Especificações técnicas"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criação"
        datetime ModifiedDate "Data de modificação"
    }

    %% === CONTROLE DE ESTOQUE ===
    INGREDIENT_STOCK {
        string Id PK "GUID único"
        string IngredientId FK "Ingrediente"
        decimal CurrentQuantity "Quantidade atual"
        decimal ReservedQuantity "Quantidade reservada"
        decimal AvailableQuantity "Quantidade disponível"
        string UnitOfMeasure "Unidade de medida"
        decimal AverageCost "Custo médio"
        datetime LastPurchaseDate "Última compra"
        decimal LastPurchaseCost "Custo última compra"
        string StorageLocation "Local armazenagem"
        datetime ExpirationDate "Data vencimento"
        string LotNumber "Número do lote"
        datetime LastUpdated "Última atualização"
        string UpdatedBy "Atualizado por"
    }

    %% === ORDEM DE COMPRA ===
    PURCHASE_ORDER {
        string Id PK "GUID único"
        string OrderNumber "Número sequencial"
        string SupplierId FK "Fornecedor"
        datetime OrderDate "Data do pedido"
        datetime RequestedDeliveryDate "Data entrega solicitada"
        datetime ActualDeliveryDate "Data entrega real"
        string PurchaseStatus "Draft|Sent|PartiallyReceived|FullyReceived|Cancelled"
        decimal TotalValue "Valor total"
        decimal TotalReceived "Valor recebido"
        string PaymentTerms "Condições pagamento"
        string DeliveryAddress "Endereço entrega"
        string PurchaseType "Manual|AutoSuggested|Emergency"
        string RequestedBy "Solicitado por"
        string ApprovedBy "Aprovado por"
        datetime ApprovalDate "Data aprovação"
        string Notes "Observações"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criação"
        datetime ModifiedDate "Data de modificação"
        string CreatedBy "Usuário criador"
    }

    %% === ITEM DA ORDEM DE COMPRA ===
    PURCHASE_ORDER_ITEM {
        string Id PK "GUID único"
        string PurchaseOrderId FK "Ordem compra"
        string IngredientId FK "Ingrediente"
        decimal QuantityOrdered "Quantidade pedida"
        decimal QuantityReceived "Quantidade recebida"
        decimal UnitCost "Custo unitário"
        decimal TotalCost "Custo total"
        string UnitOfMeasure "Unidade medida"
        string ItemStatus "Pending|PartiallyReceived|FullyReceived|Cancelled"
        datetime ExpectedDeliveryDate "Data entrega prevista"
        datetime ActualDeliveryDate "Data entrega real"
        string QualityNotes "Observações qualidade"
        string LotNumber "Número do lote"
        datetime ExpirationDate "Data vencimento"
        string Notes "Observações"
        datetime CreatedDate "Data de criação"
        datetime ModifiedDate "Data de modificação"
    }

    %% === RELACIONAMENTO FORNECEDOR x INGREDIENTE ===
    SUPPLIER_INGREDIENT {
        string Id PK "GUID único"
        string SupplierId FK "Fornecedor"
        string IngredientId FK "Ingrediente"
        decimal PreferredUnitCost "Custo preferencial"
        string PreferredUnitOfMeasure "Unidade preferencial"
        int LeadTimeDays "Prazo entrega (dias)"
        decimal MinimumOrderQuantity "Quantidade mínima"
        bool IsPreferredSupplier "Fornecedor preferencial?"
        string SupplierProductCode "Código produto fornecedor"
        string QualityRating "Avaliação qualidade"
        datetime LastPurchaseDate "Última compra"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criação"
        datetime ModifiedDate "Data de modificação"
    }

    %% === DOCUMENTO FISCAL (IA) ===
    FISCAL_DOCUMENT {
        string Id PK "GUID único"
        string PurchaseOrderId FK "Ordem compra relacionada"
        string DocumentNumber "Número documento"
        string DocumentType "NotaFiscal|Recibo|Fatura"
        datetime DocumentDate "Data do documento"
        string SupplierId FK "Fornecedor emissor"
        decimal TotalValue "Valor total documento"
        string DocumentStatus "Processing|Processed|Error|Validated"
        string BinaryData "Documento digitalizado (base64)"
        string AIExtractedData "Dados extraídos pela IA (JSON)"
        string ValidationErrors "Erros de validação"
        bool IsAIProcessed "Processado pela IA?"
        datetime ProcessedDate "Data processamento"
        string ProcessedBy "Processado por"
        string Notes "Observações"
        datetime CreatedDate "Data de criação"
    }

    %% === INTEGRAÇÕES COM OUTROS DOMÍNIOS ===

    %% PRODUÇÃO (CONSUMO DE INGREDIENTES)
    INGREDIENT_CONSUMPTION {
        string Id PK "GUID único"
        string DemandId FK "Demanda produção"
        string IngredientId FK "Ingrediente consumido"
        decimal QuantityConsumed "Quantidade consumida"
        datetime ConsumptionDate "Data consumo"
        string Notes "Observações"
    }

    %% PRODUTO (RECEITAS)
    PRODUCT_INGREDIENT {
        string Id PK "GUID único"
        string ProductId FK "Produto"
        string IngredientId FK "Ingrediente"
        decimal Quantity "Quantidade necessária"
        string UnitOfMeasure "Unidade medida"
        string Notes "Observações"
    }

    %% FINANCEIRO (CONTAS A PAGAR)
    ACCOUNT_PAYABLE {
        string Id PK "GUID único"
        string PurchaseOrderId FK "Ordem compra origem"
        string SupplierId FK "Fornecedor"
        decimal TotalAmount "Valor total a pagar"
        decimal PaidAmount "Valor já pago"
        datetime DueDate "Data vencimento"
        string AccountStatus "Pending|PartiallyPaid|Paid|Overdue"
        datetime CreatedDate "Data de criação"
    }

    %% ==========================================
    %% RELACIONAMENTOS PRINCIPAIS
    %% ==========================================

    %% FLUXO PRINCIPAL DE COMPRAS
    SUPPLIER ||--o{ PURCHASE_ORDER : "recebe pedidos"
    PURCHASE_ORDER ||--o{ PURCHASE_ORDER_ITEM : "contém itens"
    PURCHASE_ORDER_ITEM }o--|| INGREDIENT : "especifica ingrediente"
    INGREDIENT ||--|| INGREDIENT_STOCK : "controla estoque"

    %% RELACIONAMENTOS AUXILIARES
    SUPPLIER ||--o{ SUPPLIER_INGREDIENT : "fornece ingredientes"
    INGREDIENT ||--o{ SUPPLIER_INGREDIENT : "fornecido por"
    PURCHASE_ORDER ||--o{ FISCAL_DOCUMENT : "possui documentos"

    %% ==========================================
    %% INTEGRAÇÕES COM OUTROS DOMÍNIOS
    %% ==========================================

    %% PRODUÇÃO → COMPRAS (Consumo)
    INGREDIENT ||--o{ INGREDIENT_CONSUMPTION : "consumido na produção"
    INGREDIENT_CONSUMPTION }o--|| INGREDIENT_STOCK : "reduz estoque"

    %% PRODUTO → COMPRAS (Receitas)
    INGREDIENT ||--o{ PRODUCT_INGREDIENT : "compõe produtos"

    %% COMPRAS → FINANCEIRO (Contas a Pagar)
    PURCHASE_ORDER ||--o{ ACCOUNT_PAYABLE : "gera contas a pagar"
    SUPPLIER ||--o{ ACCOUNT_PAYABLE : "deve receber"

    %% ==========================================
    %% STYLING POR DOMÍNIO
    %% ==========================================
    
    %% COMPRAS = Azul (#0562aa)
    SUPPLIER {
        background-color "#0562aa"
        color "white"
        border-color "#0562aa"
    }
    
    INGREDIENT {
        background-color "#0562aa"
        color "white"
        border-color "#0562aa"
    }
    
    INGREDIENT_STOCK {
        background-color "#0562aa"
        color "white"
        border-color "#0562aa"
    }
    
    PURCHASE_ORDER {
        background-color "#0562aa"
        color "white"
        border-color "#0562aa"
    }
    
    PURCHASE_ORDER_ITEM {
        background-color "#0562aa"
        color "white"
        border-color "#0562aa"
    }
    
    SUPPLIER_INGREDIENT {
        background-color "#0562aa"
        color "white"
        border-color "#0562aa"
    }
    
    FISCAL_DOCUMENT {
        background-color "#0562aa"
        color "white"
        border-color "#0562aa"
    }

    %% PRODUÇÃO = Dourado (#fba81d)
    INGREDIENT_CONSUMPTION {
        background-color "#fba81d"
        color "black"
        border-color "#fba81d"
    }

    %% PRODUTO = Verde (#00a86b)
    PRODUCT_INGREDIENT {
        background-color "#00a86b"
        color "white"
        border-color "#00a86b"
    }

    %% FINANCEIRO = Azul Escuro (#083e61)
    ACCOUNT_PAYABLE {
        background-color "#083e61"
        color "white"
        border-color "#083e61"
    }
```

## 📋 Detalhes das Entidades

### **🏢 SUPPLIER**
- **Propósito**: Gestão de fornecedores de ingredientes e matérias-primas
- **Características**: Dados contratuais, condições comerciais, rating de desempenho
- **Relacionamentos**: 1:N com PurchaseOrder, N:N com Ingredient via SupplierIngredient

### **🥘 INGREDIENT**
- **Propósito**: Catálogo de ingredientes e matérias-primas utilizados na produção
- **Características**: Especificações técnicas, armazenagem, validade, custos
- **Controle**: Níveis mínimo/máximo de estoque, unidade de medida padrão

### **📊 INGREDIENT_STOCK**
- **Propósito**: Controle em tempo real do estoque de ingredientes
- **Características**: Quantidade atual/reservada/disponível, custos, lotes, validades
- **Integração**: Atualizado automaticamente por recebimentos e consumos

### **📄 PURCHASE_ORDER**
- **Propósito**: Documento de compra formalizado com fornecedor
- **Status Flow**: Draft → Sent → PartiallyReceived → FullyReceived
- **Tipos**: Manual, AutoSuggested (por IA), Emergency

### **📦 PURCHASE_ORDER_ITEM**
- **Propósito**: Item específico dentro de uma ordem de compra
- **Características**: Quantidades pedidas/recebidas, custos, prazos, qualidade
- **Controle**: Status individual por item, lotes, validades

### **🔗 SUPPLIER_INGREDIENT**
- **Propósito**: Relacionamento N:N entre fornecedores e ingredientes
- **Características**: Custos preferenciais, prazos, quantidades mínimas, ratings
- **Utilidade**: Sugestões automáticas de compra, comparação de fornecedores

### **🤖 FISCAL_DOCUMENT (Integração com IA)**
- **Propósito**: Digitalização e interpretação automática de documentos fiscais
- **IA Features**: Extração de dados, validação automática, detecção de erros
- **Dados**: Documento em binário + dados extraídos em JSON

## 🔄 Fluxo de Criação de Compras com IA

### **📋 Processo Manual com Apoio de IA**

#### **1. Upload e Processamento**
```
1. Usuário faz upload da nota fiscal (PDF/imagem)
2. Sistema armazena em FISCAL_DOCUMENT:
   - BinaryData (documento digitalizado)
   - DocumentStatus: "Processing"
   - IsAIProcessed: false

3. IA processa documento:
   - Extrai dados: fornecedor, itens, quantidades, valores
   - Valida dados extraídos
   - Preenche AIExtractedData (JSON)
   - DocumentStatus: "Processed"
   - IsAIProcessed: true
```

#### **2. Geração Automática de Formulário**
```
Sistema cria formulário pré-preenchido:

PurchaseOrder (pré-preenchido):
├── SupplierId (identificado por CNPJ/Nome)
├── OrderDate (data do documento)
├── TotalValue (valor total extraído)
├── PaymentTerms (extraído se disponível)

PurchaseOrderItem[] (pré-preenchidos):
├── Item 1: IngredientId (identificado por nome/código)
│          QuantityOrdered (extraído)
│          UnitCost (extraído)
├── Item 2: [...]
└── Item N: [...]
```

#### **3. Conferência e Edição pelo Usuário**
```
Usuário revisa e ajusta:
✓ Confirma fornecedor identificado
✓ Valida ingredientes mapeados
✓ Ajusta quantidades se necessário
✓ Corrige custos se necessário
✓ Adiciona observações de qualidade
```

#### **4. Persistência Final**
```
Sistema persiste:
1. PurchaseOrder com status "Draft"
2. PurchaseOrderItem[] vinculados
3. FISCAL_DOCUMENT vinculado à PurchaseOrder
4. Validações de negócio aplicadas
5. PurchaseOrder pronto para envio
```

### **⚡ Sugestões Automáticas de Compra**

#### **Algoritmo de Estoque Mínimo**
```
SELECT i.*, s.CurrentQuantity, s.MinimumStockLevel
FROM INGREDIENT i
JOIN INGREDIENT_STOCK s ON i.Id = s.IngredientId  
WHERE s.CurrentQuantity <= s.MinimumStockLevel
  AND i.StateCode = 'Active'
```

#### **Geração de Sugestões**
```
Para cada ingrediente abaixo do mínimo:
1. Calcula quantidade sugerida:
   SuggestedQty = (MaximumStockLevel - CurrentQuantity)
   
2. Identifica fornecedor preferencial:
   SELECT TOP 1 si.SupplierId
   FROM SUPPLIER_INGREDIENT si
   WHERE si.IngredientId = @IngredientId
     AND si.IsPreferredSupplier = 1
   ORDER BY si.PreferredUnitCost ASC
   
3. Cria PurchaseOrderItem sugerido com:
   - Quantidade calculada
   - Custo preferencial
   - Prazo de entrega
```

## 📊 Controle de Estoque Automático

### **📈 Entrada de Estoque (Recebimento)**
```
Quando PurchaseOrderItem é marcado como "FullyReceived":

UPDATE INGREDIENT_STOCK 
SET CurrentQuantity = CurrentQuantity + @QuantityReceived,
    AvailableQuantity = AvailableQuantity + @QuantityReceived,
    AverageCost = (AverageCost * CurrentQuantity + @UnitCost * @QuantityReceived) 
                  / (CurrentQuantity + @QuantityReceived),
    LastPurchaseDate = @ActualDeliveryDate,
    LastPurchaseCost = @UnitCost,
    LastUpdated = GETDATE()
WHERE IngredientId = @IngredientId
```

### **📉 Saída de Estoque (Consumo na Produção)**
```
Quando INGREDIENT_CONSUMPTION é registrado:

UPDATE INGREDIENT_STOCK 
SET CurrentQuantity = CurrentQuantity - @QuantityConsumed,
    AvailableQuantity = AvailableQuantity - @QuantityConsumed,
    LastUpdated = GETDATE()
WHERE IngredientId = @IngredientId

-- Verificação de estoque mínimo
IF (CurrentQuantity <= MinimumStockLevel)
    INSERT INTO PURCHASE_SUGGESTION (...)
```

### **🔒 Reserva de Estoque (Produção Planejada)**
```
Quando Demand é confirmada:

UPDATE INGREDIENT_STOCK 
SET ReservedQuantity = ReservedQuantity + @RequiredQuantity,
    AvailableQuantity = AvailableQuantity - @RequiredQuantity
WHERE IngredientId = @IngredientId
```

## 🎯 Eventos de Domínio Gerados

- **PurchaseOrderCreated**: Nova ordem de compra criada
- **PurchaseOrderSent**: Ordem enviada ao fornecedor
- **ItemReceived**: Item específico recebido
- **StockUpdated**: Estoque de ingrediente atualizado
- **LowStockAlert**: Ingrediente abaixo do estoque mínimo
- **FiscalDocumentProcessed**: Documento fiscal processado pela IA
- **SupplierEvaluated**: Avaliação de fornecedor atualizada

## 🚨 Alertas e Validações

### **Alertas Críticos**
- **Estoque Crítico**: CurrentQuantity < MinimumStockLevel
- **Vencimento Próximo**: ExpirationDate < 30 dias
- **Fornecedor Atrasado**: ActualDeliveryDate > RequestedDeliveryDate
- **IA Error**: DocumentStatus = "Error" em FiscalDocument

### **Validações de Negócio**
- Quantidade recebida não pode exceder quantidade pedida
- Ingrediente deve estar ativo para ser incluído em PurchaseOrder
- Fornecedor deve estar ativo para receber PurchaseOrder
- Custo unitário deve ser > 0
- Validade do ingrediente deve ser futura

---

**Arquivo**: `04-purchasing-domain-erd.md`  
**Domínio**: Compras (#0562aa)  
**Tipo**: Entity-Relationship Diagram  
**Nível**: Detalhado + IA Integration + Controle Automático
