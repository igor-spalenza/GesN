# ðŸ›’ ERD - DOMÃNIO DE COMPRAS

## ðŸŽ¯ VisÃ£o Geral
Diagrama Entity-Relationship completo do DomÃ­nio de Compras, mostrando a gestÃ£o de fornecedores, ingredientes, ordens de compra e controle de estoque. Inclui o fluxo de criaÃ§Ã£o manual de compras com apoio de IA para interpretaÃ§Ã£o de notas fiscais e controle automÃ¡tico de estoque mÃ­nimo.

## ðŸ—„ï¸ Diagrama de Entidades e Relacionamentos

```mermaid
erDiagram
    %% === DOMÃNIO DE COMPRAS ===
    
    %% === FORNECEDOR ===
    SUPPLIER {
        string Id PK "GUID Ãºnico"
        string Name "RazÃ£o Social"
        string TradeName "Nome Fantasia"
        string Document "CNPJ"
        string Email "Email principal"
        string Phone "Telefone principal"
        string ContactPerson "Pessoa de contato"
        string Address "EndereÃ§o completo"
        string City "Cidade"
        string State "Estado"
        string ZipCode "CEP"
        string PaymentTerms "CondiÃ§Ãµes pagamento"
        int DeliveryDays "Prazo entrega (dias)"
        decimal MinimumOrderValue "Valor mÃ­nimo pedido"
        string SupplierRating "A|B|C|D"
        string BankAccount "Conta bancÃ¡ria"
        string Notes "ObservaÃ§Ãµes"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criaÃ§Ã£o"
        datetime ModifiedDate "Data de modificaÃ§Ã£o"
    }

    %% === INGREDIENTE ===
    INGREDIENT {
        string Id PK "GUID Ãºnico"
        string Name "Nome do ingrediente"
        string Description "DescriÃ§Ã£o detalhada"
        string Category "Categoria ingrediente"
        string UnitOfMeasure "Unidade medida padrÃ£o"
        decimal MinimumStockLevel "Estoque mÃ­nimo"
        decimal MaximumStockLevel "Estoque mÃ¡ximo"
        decimal StandardCost "Custo padrÃ£o"
        string StorageRequirements "Requisitos armazenagem"
        int ShelfLifeDays "Validade (dias)"
        string IngredientCode "CÃ³digo interno"
        string Specifications "EspecificaÃ§Ãµes tÃ©cnicas"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criaÃ§Ã£o"
        datetime ModifiedDate "Data de modificaÃ§Ã£o"
    }

    %% === CONTROLE DE ESTOQUE ===
    INGREDIENT_STOCK {
        string Id PK "GUID Ãºnico"
        string IngredientId FK "Ingrediente"
        decimal CurrentQuantity "Quantidade atual"
        decimal ReservedQuantity "Quantidade reservada"
        decimal AvailableQuantity "Quantidade disponÃ­vel"
        string UnitOfMeasure "Unidade de medida"
        decimal AverageCost "Custo mÃ©dio"
        datetime LastPurchaseDate "Ãšltima compra"
        decimal LastPurchaseCost "Custo Ãºltima compra"
        string StorageLocation "Local armazenagem"
        datetime ExpirationDate "Data vencimento"
        string LotNumber "NÃºmero do lote"
        datetime LastUpdated "Ãšltima atualizaÃ§Ã£o"
        string UpdatedBy "Atualizado por"
    }

    %% === ORDEM DE COMPRA ===
    PURCHASE_ORDER {
        string Id PK "GUID Ãºnico"
        string OrderNumber "NÃºmero sequencial"
        string SupplierId FK "Fornecedor"
        datetime OrderDate "Data do pedido"
        datetime RequestedDeliveryDate "Data entrega solicitada"
        datetime ActualDeliveryDate "Data entrega real"
        string PurchaseStatus "Draft|Sent|PartiallyReceived|FullyReceived|Cancelled"
        decimal TotalValue "Valor total"
        decimal TotalReceived "Valor recebido"
        string PaymentTerms "CondiÃ§Ãµes pagamento"
        string DeliveryAddress "EndereÃ§o entrega"
        string PurchaseType "Manual|AutoSuggested|Emergency"
        string RequestedBy "Solicitado por"
        string ApprovedBy "Aprovado por"
        datetime ApprovalDate "Data aprovaÃ§Ã£o"
        string Notes "ObservaÃ§Ãµes"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criaÃ§Ã£o"
        datetime ModifiedDate "Data de modificaÃ§Ã£o"
        string CreatedBy "UsuÃ¡rio criador"
    }

    %% === ITEM DA ORDEM DE COMPRA ===
    PURCHASE_ORDER_ITEM {
        string Id PK "GUID Ãºnico"
        string PurchaseOrderId FK "Ordem compra"
        string IngredientId FK "Ingrediente"
        decimal QuantityOrdered "Quantidade pedida"
        decimal QuantityReceived "Quantidade recebida"
        decimal UnitCost "Custo unitÃ¡rio"
        decimal TotalCost "Custo total"
        string UnitOfMeasure "Unidade medida"
        string ItemStatus "Pending|PartiallyReceived|FullyReceived|Cancelled"
        datetime ExpectedDeliveryDate "Data entrega prevista"
        datetime ActualDeliveryDate "Data entrega real"
        string QualityNotes "ObservaÃ§Ãµes qualidade"
        string LotNumber "NÃºmero do lote"
        datetime ExpirationDate "Data vencimento"
        string Notes "ObservaÃ§Ãµes"
        datetime CreatedDate "Data de criaÃ§Ã£o"
        datetime ModifiedDate "Data de modificaÃ§Ã£o"
    }

    %% === RELACIONAMENTO FORNECEDOR x INGREDIENTE ===
    SUPPLIER_INGREDIENT {
        string Id PK "GUID Ãºnico"
        string SupplierId FK "Fornecedor"
        string IngredientId FK "Ingrediente"
        decimal PreferredUnitCost "Custo preferencial"
        string PreferredUnitOfMeasure "Unidade preferencial"
        int LeadTimeDays "Prazo entrega (dias)"
        decimal MinimumOrderQuantity "Quantidade mÃ­nima"
        bool IsPreferredSupplier "Fornecedor preferencial?"
        string SupplierProductCode "CÃ³digo produto fornecedor"
        string QualityRating "AvaliaÃ§Ã£o qualidade"
        datetime LastPurchaseDate "Ãšltima compra"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criaÃ§Ã£o"
        datetime ModifiedDate "Data de modificaÃ§Ã£o"
    }

    %% === DOCUMENTO FISCAL (IA) ===
    FISCAL_DOCUMENT {
        string Id PK "GUID Ãºnico"
        string PurchaseOrderId FK "Ordem compra relacionada"
        string DocumentNumber "NÃºmero documento"
        string DocumentType "NotaFiscal|Recibo|Fatura"
        datetime DocumentDate "Data do documento"
        string SupplierId FK "Fornecedor emissor"
        decimal TotalValue "Valor total documento"
        string DocumentStatus "Processing|Processed|Error|Validated"
        string BinaryData "Documento digitalizado (base64)"
        string AIExtractedData "Dados extraÃ­dos pela IA (JSON)"
        string ValidationErrors "Erros de validaÃ§Ã£o"
        bool IsAIProcessed "Processado pela IA?"
        datetime ProcessedDate "Data processamento"
        string ProcessedBy "Processado por"
        string Notes "ObservaÃ§Ãµes"
        datetime CreatedDate "Data de criaÃ§Ã£o"
    }

    %% === INTEGRAÃ‡Ã•ES COM OUTROS DOMÃNIOS ===

    %% PRODUÃ‡ÃƒO (CONSUMO DE INGREDIENTES)
    INGREDIENT_CONSUMPTION {
        string Id PK "GUID Ãºnico"
        string DemandId FK "Demanda produÃ§Ã£o"
        string IngredientId FK "Ingrediente consumido"
        decimal QuantityConsumed "Quantidade consumida"
        datetime ConsumptionDate "Data consumo"
        string Notes "ObservaÃ§Ãµes"
    }

    %% PRODUTO (RECEITAS)
    PRODUCT_INGREDIENT {
        string Id PK "GUID Ãºnico"
        string ProductId FK "Produto"
        string IngredientId FK "Ingrediente"
        decimal Quantity "Quantidade necessÃ¡ria"
        string UnitOfMeasure "Unidade medida"
        string Notes "ObservaÃ§Ãµes"
    }

    %% FINANCEIRO (CONTAS A PAGAR)
    ACCOUNT_PAYABLE {
        string Id PK "GUID Ãºnico"
        string PurchaseOrderId FK "Ordem compra origem"
        string SupplierId FK "Fornecedor"
        decimal TotalAmount "Valor total a pagar"
        decimal PaidAmount "Valor jÃ¡ pago"
        datetime DueDate "Data vencimento"
        string AccountStatus "Pending|PartiallyPaid|Paid|Overdue"
        datetime CreatedDate "Data de criaÃ§Ã£o"
    }

    %% ==========================================
    %% RELACIONAMENTOS PRINCIPAIS
    %% ==========================================

    %% FLUXO PRINCIPAL DE COMPRAS
    SUPPLIER ||--o{ PURCHASE_ORDER : "recebe pedidos"
    PURCHASE_ORDER ||--o{ PURCHASE_ORDER_ITEM : "contÃ©m itens"
    PURCHASE_ORDER_ITEM }o--|| INGREDIENT : "especifica ingrediente"
    INGREDIENT ||--|| INGREDIENT_STOCK : "controla estoque"

    %% RELACIONAMENTOS AUXILIARES
    SUPPLIER ||--o{ SUPPLIER_INGREDIENT : "fornece ingredientes"
    INGREDIENT ||--o{ SUPPLIER_INGREDIENT : "fornecido por"
    PURCHASE_ORDER ||--o{ FISCAL_DOCUMENT : "possui documentos"

    %% ==========================================
    %% INTEGRAÃ‡Ã•ES COM OUTROS DOMÃNIOS
    %% ==========================================

    %% PRODUÃ‡ÃƒO â†’ COMPRAS (Consumo)
    INGREDIENT ||--o{ INGREDIENT_CONSUMPTION : "consumido na produÃ§Ã£o"
    INGREDIENT_CONSUMPTION }o--|| INGREDIENT_STOCK : "reduz estoque"

    %% PRODUTO â†’ COMPRAS (Receitas)
    INGREDIENT ||--o{ PRODUCT_INGREDIENT : "compÃµe produtos"

    %% COMPRAS â†’ FINANCEIRO (Contas a Pagar)
    PURCHASE_ORDER ||--o{ ACCOUNT_PAYABLE : "gera contas a pagar"
    SUPPLIER ||--o{ ACCOUNT_PAYABLE : "deve receber"

    %% ==========================================
    %% STYLING POR DOMÃNIO
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

    %% PRODUÃ‡ÃƒO = Dourado (#fba81d)
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

## ðŸ“‹ Detalhes das Entidades

### **ðŸ¢ SUPPLIER**
- **PropÃ³sito**: GestÃ£o de fornecedores de ingredientes e matÃ©rias-primas
- **CaracterÃ­sticas**: Dados contratuais, condiÃ§Ãµes comerciais, rating de desempenho
- **Relacionamentos**: 1:N com PurchaseOrder, N:N com Ingredient via SupplierIngredient

### **ðŸ¥˜ INGREDIENT**
- **PropÃ³sito**: CatÃ¡logo de ingredientes e matÃ©rias-primas utilizados na produÃ§Ã£o
- **CaracterÃ­sticas**: EspecificaÃ§Ãµes tÃ©cnicas, armazenagem, validade, custos
- **Controle**: NÃ­veis mÃ­nimo/mÃ¡ximo de estoque, unidade de medida padrÃ£o

### **ðŸ“Š INGREDIENT_STOCK**
- **PropÃ³sito**: Controle em tempo real do estoque de ingredientes
- **CaracterÃ­sticas**: Quantidade atual/reservada/disponÃ­vel, custos, lotes, validades
- **IntegraÃ§Ã£o**: Atualizado automaticamente por recebimentos e consumos

### **ðŸ“„ PURCHASE_ORDER**
- **PropÃ³sito**: Documento de compra formalizado com fornecedor
- **Status Flow**: Draft â†’ Sent â†’ PartiallyReceived â†’ FullyReceived
- **Tipos**: Manual, AutoSuggested (por IA), Emergency

### **ðŸ“¦ PURCHASE_ORDER_ITEM**
- **PropÃ³sito**: Item especÃ­fico dentro de uma ordem de compra
- **CaracterÃ­sticas**: Quantidades pedidas/recebidas, custos, prazos, qualidade
- **Controle**: Status individual por item, lotes, validades

### **ðŸ”— SUPPLIER_INGREDIENT**
- **PropÃ³sito**: Relacionamento N:N entre fornecedores e ingredientes
- **CaracterÃ­sticas**: Custos preferenciais, prazos, quantidades mÃ­nimas, ratings
- **Utilidade**: SugestÃµes automÃ¡ticas de compra, comparaÃ§Ã£o de fornecedores

### **ðŸ¤– FISCAL_DOCUMENT (IntegraÃ§Ã£o com IA)**
- **PropÃ³sito**: DigitalizaÃ§Ã£o e interpretaÃ§Ã£o automÃ¡tica de documentos fiscais
- **IA Features**: ExtraÃ§Ã£o de dados, validaÃ§Ã£o automÃ¡tica, detecÃ§Ã£o de erros
- **Dados**: Documento em binÃ¡rio + dados extraÃ­dos em JSON

## ðŸ”„ Fluxo de CriaÃ§Ã£o de Compras com IA

### **ðŸ“‹ Processo Manual com Apoio de IA**

#### **1. Upload e Processamento**
```
1. UsuÃ¡rio faz upload da nota fiscal (PDF/imagem)
2. Sistema armazena em FISCAL_DOCUMENT:
   - BinaryData (documento digitalizado)
   - DocumentStatus: "Processing"
   - IsAIProcessed: false

3. IA processa documento:
   - Extrai dados: fornecedor, itens, quantidades, valores
   - Valida dados extraÃ­dos
   - Preenche AIExtractedData (JSON)
   - DocumentStatus: "Processed"
   - IsAIProcessed: true
```

#### **2. GeraÃ§Ã£o AutomÃ¡tica de FormulÃ¡rio**
```
Sistema cria formulÃ¡rio prÃ©-preenchido:

PurchaseOrder (prÃ©-preenchido):
â”œâ”€â”€ SupplierId (identificado por CNPJ/Nome)
â”œâ”€â”€ OrderDate (data do documento)
â”œâ”€â”€ TotalValue (valor total extraÃ­do)
â”œâ”€â”€ PaymentTerms (extraÃ­do se disponÃ­vel)

PurchaseOrderItem[] (prÃ©-preenchidos):
â”œâ”€â”€ Item 1: IngredientId (identificado por nome/cÃ³digo)
â”‚          QuantityOrdered (extraÃ­do)
â”‚          UnitCost (extraÃ­do)
â”œâ”€â”€ Item 2: [...]
â””â”€â”€ Item N: [...]
```

#### **3. ConferÃªncia e EdiÃ§Ã£o pelo UsuÃ¡rio**
```
UsuÃ¡rio revisa e ajusta:
âœ“ Confirma fornecedor identificado
âœ“ Valida ingredientes mapeados
âœ“ Ajusta quantidades se necessÃ¡rio
âœ“ Corrige custos se necessÃ¡rio
âœ“ Adiciona observaÃ§Ãµes de qualidade
```

#### **4. PersistÃªncia Final**
```
Sistema persiste:
1. PurchaseOrder com status "Draft"
2. PurchaseOrderItem[] vinculados
3. FISCAL_DOCUMENT vinculado Ã  PurchaseOrder
4. ValidaÃ§Ãµes de negÃ³cio aplicadas
5. PurchaseOrder pronto para envio
```

### **âš¡ SugestÃµes AutomÃ¡ticas de Compra**

#### **Algoritmo de Estoque MÃ­nimo**
```
SELECT i.*, s.CurrentQuantity, s.MinimumStockLevel
FROM INGREDIENT i
JOIN INGREDIENT_STOCK s ON i.Id = s.IngredientId  
WHERE s.CurrentQuantity <= s.MinimumStockLevel
  AND i.StateCode = 'Active'
```

#### **GeraÃ§Ã£o de SugestÃµes**
```
Para cada ingrediente abaixo do mÃ­nimo:
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

## ðŸ“Š Controle de Estoque AutomÃ¡tico

### **ðŸ“ˆ Entrada de Estoque (Recebimento)**
```
Quando PurchaseOrderItem Ã© marcado como "FullyReceived":

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

### **ðŸ“‰ SaÃ­da de Estoque (Consumo na ProduÃ§Ã£o)**
```
Quando INGREDIENT_CONSUMPTION Ã© registrado:

UPDATE INGREDIENT_STOCK 
SET CurrentQuantity = CurrentQuantity - @QuantityConsumed,
    AvailableQuantity = AvailableQuantity - @QuantityConsumed,
    LastUpdated = GETDATE()
WHERE IngredientId = @IngredientId

-- VerificaÃ§Ã£o de estoque mÃ­nimo
IF (CurrentQuantity <= MinimumStockLevel)
    INSERT INTO PURCHASE_SUGGESTION (...)
```

### **ðŸ”’ Reserva de Estoque (ProduÃ§Ã£o Planejada)**
```
Quando Demand Ã© confirmada:

UPDATE INGREDIENT_STOCK 
SET ReservedQuantity = ReservedQuantity + @RequiredQuantity,
    AvailableQuantity = AvailableQuantity - @RequiredQuantity
WHERE IngredientId = @IngredientId
```

## ðŸŽ¯ Eventos de DomÃ­nio Gerados

- **PurchaseOrderCreated**: Nova ordem de compra criada
- **PurchaseOrderSent**: Ordem enviada ao fornecedor
- **ItemReceived**: Item especÃ­fico recebido
- **StockUpdated**: Estoque de ingrediente atualizado
- **LowStockAlert**: Ingrediente abaixo do estoque mÃ­nimo
- **FiscalDocumentProcessed**: Documento fiscal processado pela IA
- **SupplierEvaluated**: AvaliaÃ§Ã£o de fornecedor atualizada

## ðŸš¨ Alertas e ValidaÃ§Ãµes

### **Alertas CrÃ­ticos**
- **Estoque CrÃ­tico**: CurrentQuantity < MinimumStockLevel
- **Vencimento PrÃ³ximo**: ExpirationDate < 30 dias
- **Fornecedor Atrasado**: ActualDeliveryDate > RequestedDeliveryDate
- **IA Error**: DocumentStatus = "Error" em FiscalDocument

### **ValidaÃ§Ãµes de NegÃ³cio**
- Quantidade recebida nÃ£o pode exceder quantidade pedida
- Ingrediente deve estar ativo para ser incluÃ­do em PurchaseOrder
- Fornecedor deve estar ativo para receber PurchaseOrder
- Custo unitÃ¡rio deve ser > 0
- Validade do ingrediente deve ser futura

---

**Arquivo**: `04-purchasing-domain-erd.md`  
**DomÃ­nio**: Compras (#0562aa)  
**Tipo**: Entity-Relationship Diagram  
**NÃ­vel**: Detalhado + IA Integration + Controle AutomÃ¡tico
