# ðŸ›’ FLUXOGRAMA - PROCESSO DE COMPRAS

## ðŸŽ¯ VisÃ£o Geral
Fluxograma completo do processo de criaÃ§Ã£o manual de compras com apoio de IA para interpretaÃ§Ã£o de notas fiscais, incluindo extraÃ§Ã£o automÃ¡tica de dados, validaÃ§Ã£o, conferÃªncia pelo usuÃ¡rio e persistÃªncia final no sistema. TambÃ©m abrange sugestÃµes automÃ¡ticas de compra baseadas em estoque mÃ­nimo.

## ðŸ¤– Fluxo Principal: CriaÃ§Ã£o Manual com IA

```mermaid
flowchart TD
    %% === INÃCIO DO PROCESSO ===
    A[ðŸš€ UsuÃ¡rio inicia<br/>nova compra] --> B{ðŸ“‹ Tipo de criaÃ§Ã£o}
    
    B -->|Manual tradicional| Manual[ðŸ“ FormulÃ¡rio manual]
    B -->|Com IA| C[ðŸ“„ Upload nota fiscal]
    
    %% === PROCESSAMENTO DE IA ===
    C --> D[ðŸ’¾ Salvar documento<br/>em FISCAL_DOCUMENT]
    D --> E[ðŸ¤– IA inicia processamento<br/>DocumentStatus: Processing]
    
    E --> F[ðŸ” Extrair dados do documento]
    F --> G[ðŸ“Š OCR + Machine Learning]
    G --> H[ðŸ§  Identificar campos-chave]
    
    H --> I[ðŸ“‹ Dados extraÃ­dos]
    I --> J{âœ… Processamento<br/>bem-sucedido?}
    
    J -->|Erro| K[âŒ DocumentStatus: Error<br/>Mostrar erro ao usuÃ¡rio]
    K --> L[ðŸ”„ UsuÃ¡rio pode tentar novamente<br/>ou ir para modo manual]
    L --> B
    
    J -->|Sucesso| M[âœ… DocumentStatus: Processed<br/>AIExtractedData preenchida]
    
    %% === IDENTIFICAÃ‡ÃƒO E MAPEAMENTO ===
    M --> N[ðŸ” Identificar fornecedor<br/>por CNPJ/Nome]
    N --> O{ðŸ¢ Fornecedor existe<br/>no sistema?}
    
    O -->|NÃ£o| P[âž• Criar registro temporÃ¡rio<br/>de Supplier]
    O -->|Sim| Q[âœ… Supplier identificado]
    P --> Q
    
    Q --> R[ðŸ¥˜ Para cada item extraÃ­do]
    R --> S[ðŸ” Mapear ingrediente<br/>por nome/cÃ³digo]
    S --> T{ðŸ§ª Ingrediente existe<br/>no sistema?}
    
    T -->|NÃ£o| U[âš ï¸ Marcar para revisÃ£o<br/>manual obrigatÃ³ria]
    T -->|Sim| V[âœ… Ingredient mapeado]
    
    U --> W[ðŸ“Š Adicionar Ã  lista<br/>de itens para revisÃ£o]
    V --> W
    W --> X{ðŸ”„ Mais itens<br/>extraÃ­dos?}
    
    X -->|Sim| R
    X -->|NÃ£o| Y[ðŸ“‹ Gerar formulÃ¡rio<br/>prÃ©-preenchido]
    
    %% === CONFERÃŠNCIA PELO USUÃRIO ===
    Y --> Z[ðŸ‘€ Exibir formulÃ¡rio<br/>com dados extraÃ­dos]
    Z --> AA[ðŸ” UsuÃ¡rio revisa dados]
    
    AA --> BB{ðŸ¢ Fornecedor correto?}
    BB -->|NÃ£o| CC[âœï¸ Corrigir/selecionar fornecedor]
    CC --> DD
    BB -->|Sim| DD[ðŸ“¦ Revisar itens]
    
    DD --> EE[ðŸ“‹ Para cada item]
    EE --> FF{ðŸ§ª Ingrediente<br/>mapeado corretamente?}
    
    FF -->|NÃ£o| GG[ðŸ” Buscar ingrediente correto<br/>ou criar novo]
    FF -->|Sim| HH[ðŸ’± Validar quantidade<br/>e unidade de medida]
    
    GG --> HH
    HH --> II[ðŸ’° Validar preÃ§o unitÃ¡rio]
    II --> JJ{ðŸ”„ Mais itens<br/>para revisar?}
    
    JJ -->|Sim| EE
    JJ -->|NÃ£o| KK[ðŸ“ Adicionar observaÃ§Ãµes<br/>de qualidade/entrega]
    
    KK --> LL[âœ… UsuÃ¡rio confirma<br/>dados revisados]
    
    %% === PERSISTÃŠNCIA FINAL ===
    LL --> MM[ðŸ’¾ Criar PurchaseOrder]
    MM --> NN[ðŸ“¦ Criar PurchaseOrderItem<br/>para cada item]
    NN --> OO[ðŸ”— Vincular FISCAL_DOCUMENT<br/>Ã  PurchaseOrder]
    OO --> PP[ðŸ“ˆ PurchaseStatus: Draft]
    PP --> QQ[ðŸŽ‰ Compra criada<br/>pronta para envio]
    
    %% === FLUXO MANUAL TRADICIONAL ===
    Manual --> ManualForm[ðŸ“ FormulÃ¡rio em branco]
    ManualForm --> ManualFill[ðŸ‘¤ UsuÃ¡rio preenche tudo]
    ManualFill --> MM
    
    %% === STYLING ===
    
    classDef startStyle fill:#e5e7eb,stroke:#6b7280,stroke-width:2px,color:black
    class A,B startStyle
    
    classDef aiStyle fill:#8b5cf6,stroke:#7c3aed,stroke-width:2px,color:white
    class C,D,E,F,G,H,I,J,M aiStyle
    
    classDef errorStyle fill:#fecaca,stroke:#ef4444,stroke-width:2px,color:black
    class K,L errorStyle
    
    classDef mappingStyle fill:#fed7aa,stroke:#f97316,stroke-width:2px,color:black
    class N,O,P,Q,R,S,T,U,V,W,X,Y mappingStyle
    
    classDef userStyle fill:#dbeafe,stroke:#3b82f6,stroke-width:2px,color:black
    class Z,AA,BB,CC,DD,EE,FF,GG,HH,II,JJ,KK,LL userStyle
    
    classDef persistStyle fill:#0562aa,stroke:#0562aa,stroke-width:2px,color:white
    class MM,NN,OO,PP,QQ persistStyle
    
    classDef manualStyle fill:#d1fae5,stroke:#10b981,stroke-width:2px,color:black
    class Manual,ManualForm,ManualFill manualStyle
```

## ðŸ¤– Detalhamento do Processamento de IA

### **ðŸ“„ Estrutura de Dados ExtraÃ­dos (AIExtractedData JSON):**

```json
{
  "supplier": {
    "name": "Fornecedor ABC Ltda",
    "cnpj": "12.345.678/0001-99",
    "address": "Rua das Flores, 123, Centro",
    "phone": "(11) 99999-9999",
    "confidence": 0.95
  },
  "document": {
    "number": "000123456",
    "date": "2025-01-15",
    "totalValue": 1250.75,
    "confidence": 0.98
  },
  "items": [
    {
      "description": "Farinha de Trigo Especial 25kg",
      "quantity": 10,
      "unitOfMeasure": "saco",
      "unitCost": 35.50,
      "totalCost": 355.00,
      "confidence": 0.92,
      "mappedIngredientId": "ingredient-farinha-trigo-id",
      "mappingConfidence": 0.88
    },
    {
      "description": "AÃ§Ãºcar Cristal 50kg", 
      "quantity": 5,
      "unitOfMeasure": "saco",
      "unitCost": 95.15,
      "totalCost": 475.75,
      "confidence": 0.94,
      "mappedIngredientId": null,
      "mappingConfidence": 0.0
    }
  ],
  "processingStats": {
    "processingTime": "2.3s",
    "ocrQuality": "high",
    "documentType": "nota_fiscal",
    "totalConfidence": 0.93
  }
}
```

### **ðŸ” Algoritmo de Mapeamento de Ingredientes:**

```mermaid
flowchart TD
    A[ðŸ“ Item extraÃ­do da nota] --> B[ðŸ” Buscar por nome exato<br/>no INGREDIENT]
    B --> C{âœ… Encontrou<br/>match exato?}
    
    C -->|Sim| D[ðŸŽ¯ Confidence = 1.0<br/>Mapeamento confirmado]
    
    C -->|NÃ£o| E[ðŸ”¤ Buscar por similaridade<br/>Levenshtein Distance]
    E --> F{ðŸ“Š Similaridade > 80%?}
    
    F -->|Sim| G[ðŸŽ¯ Confidence = 0.8-0.95<br/>Mapeamento sugerido]
    
    F -->|NÃ£o| H[ðŸ” Buscar por palavras-chave<br/>na descriÃ§Ã£o]
    H --> I{ðŸ”‘ Palavras-chave<br/>encontradas?}
    
    I -->|Sim| J[ðŸŽ¯ Confidence = 0.6-0.8<br/>Mapeamento possÃ­vel]
    
    I -->|NÃ£o| K[â“ Confidence = 0.0<br/>Mapeamento manual necessÃ¡rio]
    
    D --> L[âœ… Adicionar Ã  lista<br/>com confianÃ§a]
    G --> L
    J --> L
    K --> L
    
    classDef searchStyle fill:#fed7aa,stroke:#f97316,stroke-width:2px,color:black
    class A,B,C,E,F,H,I searchStyle
    
    classDef highConfStyle fill:#d1fae5,stroke:#10b981,stroke-width:2px,color:black
    class D,G highConfStyle
    
    classDef lowConfStyle fill:#fef3c7,stroke:#f59e0b,stroke-width:2px,color:black
    class J lowConfStyle
    
    classDef noConfStyle fill:#fecaca,stroke:#ef4444,stroke-width:2px,color:black
    class K noConfStyle
    
    classDef resultStyle fill:#dbeafe,stroke:#3b82f6,stroke-width:2px,color:black
    class L resultStyle
```

## ðŸ“‹ Processo de ConferÃªncia e ValidaÃ§Ã£o

### **ðŸ‘¤ Interface de ConferÃªncia:**

```mermaid
flowchart TD
    A[ðŸ“‹ FormulÃ¡rio prÃ©-preenchido] --> B[ðŸ¢ SeÃ§Ã£o Fornecedor]
    B --> C[ðŸ“¦ SeÃ§Ã£o Itens]
    
    %% FORNECEDOR
    B --> B1[ðŸ‘€ Nome: Fornecedor ABC Ltda âœ…]
    B1 --> B2[ðŸ†” CNPJ: 12.345.678/0001-99 âœ…]
    B2 --> B3[ðŸ“ž Contato: (11) 99999-9999 âš ï¸ Validar]
    B3 --> B4{ðŸ“ Dados do fornecedor<br/>precisam correÃ§Ã£o?}
    
    B4 -->|Sim| B5[âœï¸ Editar dados<br/>ou criar novo fornecedor]
    B4 -->|NÃ£o| C
    B5 --> C
    
    %% ITENS
    C --> C1[ðŸ“‹ Lista de itens extraÃ­dos]
    C1 --> C2[ðŸ“¦ Para cada item]
    
    C2 --> C3[ðŸŽ¯ Status do mapeamento]
    C3 --> C4{ðŸ§ª Ingrediente<br/>mapeado automaticamente?}
    
    C4 -->|âœ… Sim, confidence > 80%| C5[ðŸ‘€ Revisar mapeamento<br/>Farinha Trigo âœ…]
    C4 -->|âš ï¸ Sim, confidence < 80%| C6[ðŸ” Validar mapeamento<br/>sugerido]
    C4 -->|âŒ NÃ£o mapeado| C7[ðŸ” Buscar ingrediente<br/>ou criar novo]
    
    C5 --> C8[ðŸ’± Validar quantidade<br/>e unidade]
    C6 --> C8
    C7 --> C8
    
    C8 --> C9[ðŸ’° Validar preÃ§o<br/>unitÃ¡rio]
    C9 --> C10{ðŸ”„ Mais itens?}
    
    C10 -->|Sim| C2
    C10 -->|NÃ£o| D[ðŸ“ ObservaÃ§Ãµes finais]
    
    D --> E[âœ… Confirmar e salvar]
    
    classDef formStyle fill:#dbeafe,stroke:#3b82f6,stroke-width:2px,color:black
    class A,B,C,C1,C2,D,E formStyle
    
    classDef supplierStyle fill:#fed7aa,stroke:#f97316,stroke-width:2px,color:black
    class B1,B2,B3,B4,B5 supplierStyle
    
    classDef itemValidStyle fill:#d1fae5,stroke:#10b981,stroke-width:2px,color:black
    class C3,C5,C8,C9 itemValidStyle
    
    classDef itemWarnStyle fill:#fef3c7,stroke:#f59e0b,stroke-width:2px,color:black
    class C6 itemWarnStyle
    
    classDef itemErrorStyle fill:#fecaca,stroke:#ef4444,stroke-width:2px,color:black
    class C7 itemErrorStyle
```

## ðŸ”„ Fluxo de SugestÃµes AutomÃ¡ticas

### **ðŸ“Š Sistema de Alerta de Estoque MÃ­nimo:**

```mermaid
flowchart TD
    A[â° Job automÃ¡tico<br/>executa diariamente] --> B[ðŸ” Verificar ingredientes<br/>abaixo do estoque mÃ­nimo]
    
    B --> C[ðŸ“Š Query estoque crÃ­tico]
    C --> D[ðŸ“‹ Para cada ingrediente<br/>em situaÃ§Ã£o crÃ­tica]
    
    D --> E[ðŸ¢ Identificar fornecedor<br/>preferencial]
    E --> F{âœ… Fornecedor preferencial<br/>existe?}
    
    F -->|NÃ£o| G[âš ï¸ Usar fornecedor<br/>com menor custo]
    F -->|Sim| H[ðŸŽ¯ Usar fornecedor preferencial]
    
    G --> I[ðŸ“ Calcular quantidade<br/>sugerida]
    H --> I
    
    I --> J[ðŸ§® FÃ³rmula sugestÃ£o:<br/>MaxStock - CurrentStock]
    J --> K[ðŸ’° Calcular custo estimado<br/>baseado no histÃ³rico]
    K --> L[ðŸ“‹ Gerar sugestÃ£o<br/>de compra]
    
    L --> M{ðŸ”„ Mais ingredientes<br/>crÃ­ticos?}
    M -->|Sim| D
    M -->|NÃ£o| N[ðŸ“§ Notificar usuÃ¡rios<br/>responsÃ¡veis]
    
    N --> O[ðŸ“Š Exibir dashboard<br/>de sugestÃµes]
    O --> P{ðŸ‘¤ UsuÃ¡rio decide<br/>criar compra?}
    
    P -->|Sim| Q[ðŸ“ Gerar PurchaseOrder<br/>prÃ©-preenchida]
    P -->|NÃ£o| R[â° Aguardar prÃ³xima<br/>verificaÃ§Ã£o]
    
    Q --> S[âœ… UsuÃ¡rio pode ajustar<br/>e confirmar]
    
    classDef autoStyle fill:#8b5cf6,stroke:#7c3aed,stroke-width:2px,color:white
    class A,B,C,D autoStyle
    
    classDef supplierStyle fill:#fed7aa,stroke:#f97316,stroke-width:2px,color:black
    class E,F,G,H supplierStyle
    
    classDef calcStyle fill:#fba81d,stroke:#fba81d,stroke-width:2px,color:black
    class I,J,K,L calcStyle
    
    classDef notifyStyle fill:#dbeafe,stroke:#3b82f6,stroke-width:2px,color:black
    class M,N,O,P notifyStyle
    
    classDef resultStyle fill:#0562aa,stroke:#0562aa,stroke-width:2px,color:white
    class Q,R,S resultStyle
```

### **ðŸ“ Algoritmo de CÃ¡lculo de Quantidade:**

```sql
-- Query para ingredientes crÃ­ticos
SELECT 
    i.Id,
    i.Name,
    s.CurrentQuantity,
    s.MinimumStockLevel,
    s.MaximumStockLevel,
    (s.MaximumStockLevel - s.CurrentQuantity) AS SuggestedQuantity,
    si.PreferredUnitCost,
    si.SupplierId AS PreferredSupplierId
FROM INGREDIENT i
JOIN INGREDIENT_STOCK s ON i.Id = s.IngredientId
LEFT JOIN SUPPLIER_INGREDIENT si ON i.Id = si.IngredientId 
    AND si.IsPreferredSupplier = 1
WHERE s.CurrentQuantity <= s.MinimumStockLevel
    AND i.StateCode = 'Active'
ORDER BY (s.CurrentQuantity / s.MinimumStockLevel) ASC
```

## ðŸšš Processo de Recebimento

### **ðŸ“¦ Fluxo de Recebimento de Compra:**

```mermaid
flowchart TD
    A[ðŸ“¦ Mercadoria chega] --> B[ðŸ” Localizar PurchaseOrder<br/>pelo nÃºmero]
    B --> C[ðŸ“‹ Verificar itens<br/>contra PurchaseOrderItem]
    
    C --> D[ðŸ“Š Para cada item recebido]
    D --> E[âš–ï¸ Conferir quantidade<br/>fÃ­sica vs pedida]
    E --> F[ðŸ§ª Verificar qualidade]
    F --> G[ðŸ“… Verificar validade]
    
    G --> H{âœ… Item conforme<br/>especificaÃ§Ã£o?}
    
    H -->|Sim| I[âœ… QuantityReceived += Qty]
    H -->|NÃ£o| J[âŒ Registrar discrepÃ¢ncia<br/>em QualityNotes]
    
    I --> K[ðŸ“ˆ Atualizar ItemStatus]
    J --> K
    
    K --> L{ðŸ”„ Mais itens<br/>para conferir?}
    L -->|Sim| D
    L -->|NÃ£o| M[ðŸ“Š Verificar status geral<br/>da PurchaseOrder]
    
    M --> N{ðŸ“‹ Todos itens<br/>totalmente recebidos?}
    
    N -->|Sim| O[ðŸ“ˆ PurchaseStatus:<br/>FullyReceived]
    N -->|NÃ£o| P[ðŸ“ˆ PurchaseStatus:<br/>PartiallyReceived]
    
    O --> Q[ðŸ­ Atualizar INGREDIENT_STOCK<br/>automaticamente]
    P --> Q
    
    Q --> R[ðŸ’° Gerar ACCOUNT_PAYABLE<br/>no Financeiro]
    R --> S[ðŸŽ‰ Recebimento concluÃ­do]
    
    classDef receiveStyle fill:#0562aa,stroke:#0562aa,stroke-width:2px,color:white
    class A,B,C,D,E,F,G receiveStyle
    
    classDef qualityStyle fill:#fef3c7,stroke:#f59e0b,stroke-width:2px,color:black
    class H,I,J,K qualityStyle
    
    classDef statusStyle fill:#fed7aa,stroke:#f97316,stroke-width:2px,color:black
    class L,M,N,O,P statusStyle
    
    classDef integrationStyle fill:#d1fae5,stroke:#10b981,stroke-width:2px,color:black
    class Q,R,S integrationStyle
```

## ðŸŽ¯ Estados e ValidaÃ§Ãµes

### **ðŸ“ˆ Ciclo de Status da PurchaseOrder:**

```mermaid
stateDiagram-v2
    [*] --> Draft : CriaÃ§Ã£o inicial
    Draft --> Sent : Enviada ao fornecedor
    Sent --> PartiallyReceived : Recebimento parcial
    PartiallyReceived --> FullyReceived : Recebimento total
    FullyReceived --> [*] : Processo concluÃ­do
    
    Draft --> Cancelled : Cancelamento antes envio
    Sent --> Cancelled : Cancelamento apÃ³s envio
    PartiallyReceived --> Cancelled : Cancelamento parcial
    
    Cancelled --> [*] : Processo cancelado
```

### **ðŸ§ª ValidaÃ§Ãµes CrÃ­ticas:**

#### **Durante Processamento IA:**
- âœ… Documento deve ser PDF ou imagem (JPG/PNG)
- âœ… Tamanho mÃ¡ximo: 10MB
- âœ… Qualidade OCR mÃ­nima: 70%
- âœ… CNPJ do fornecedor deve ser vÃ¡lido

#### **Durante ConferÃªncia:**
- âœ… Fornecedor deve existir ou ser criado
- âœ… Ingredientes devem estar ativos
- âœ… Quantidades > 0
- âœ… PreÃ§os unitÃ¡rios > 0
- âœ… Unidades de medida consistentes

#### **Durante Recebimento:**
- âœ… Quantidade recebida â‰¤ quantidade pedida
- âœ… Validade deve ser futura
- âœ… Qualidade dentro dos padrÃµes
- âœ… Estoque suficiente para armazenagem

## ðŸŽ¯ Eventos de DomÃ­nio Gerados

- **FiscalDocumentUploaded**: Documento fiscal enviado
- **FiscalDocumentProcessed**: IA processou documento
- **PurchaseOrderCreated**: Nova ordem de compra criada
- **PurchaseOrderSent**: Ordem enviada ao fornecedor
- **ItemReceived**: Item especÃ­fico recebido
- **StockUpdated**: Estoque atualizado automaticamente
- **LowStockAlert**: Alerta de estoque mÃ­nimo
- **SupplierEvaluated**: AvaliaÃ§Ã£o de fornecedor

## ðŸš¨ Alertas e Monitoramento

### **Alertas CrÃ­ticos:**
- ðŸš¨ **IA Processing Error**: Falha no processamento de documento
- ðŸš¨ **Estoque CrÃ­tico**: Ingrediente abaixo de 10% do mÃ­nimo
- ðŸš¨ **Fornecedor Atrasado**: Entrega > 3 dias do prazo
- ðŸš¨ **Qualidade Rejeitada**: Item reprovado na conferÃªncia

### **MÃ©tricas de Performance:**
- **Accuracy IA**: % de dados extraÃ­dos corretamente
- **Tempo Processamento**: MÃ©dia de tempo para processar documentos
- **Taxa ConferÃªncia**: % de itens que precisam correÃ§Ã£o manual
- **Pontualidade Fornecedores**: % entregas no prazo

---

**Arquivo**: `04-purchasing-flow.md`  
**DomÃ­nio**: Compras (#0562aa)  
**Tipo**: Process Flowchart  
**Foco**: IA Integration + Manual Creation + Stock Management
