# ðŸ­ ERD - DOMÃNIO DE PRODUÃ‡ÃƒO

## ðŸŽ¯ VisÃ£o Geral
Diagrama Entity-Relationship completo do DomÃ­nio de ProduÃ§Ã£o, mostrando como as demandas sÃ£o geradas automaticamente a partir de OrderItems e como sÃ£o gerenciadas atravÃ©s de ProductComposition e ProductionOrder. Este domÃ­nio traduz vendas em tarefas de produÃ§Ã£o executÃ¡veis.

## ðŸ—„ï¸ Diagrama de Entidades e Relacionamentos

```mermaid
erDiagram
    %% === DOMÃNIO DE PRODUÃ‡ÃƒO ===
    
    %% === DEMANDA DE PRODUÃ‡ÃƒO ===
    DEMAND {
        string Id PK "GUID Ãºnico"
        string OrderItemId FK "Item pedido origem"
        string ProductId FK "Produto a produzir"
        string ProductionOrderId FK "Ordem produÃ§Ã£o (opcional)"
        int Quantity "Quantidade a produzir"
        datetime RequiredDate "Data limite entrega"
        datetime StartDate "Data inÃ­cio produÃ§Ã£o"
        datetime CompletionDate "Data conclusÃ£o"
        string DemandStatus "Pending|Confirmed|InProduction|Finalizando|Ready|Delivered|Cancelled"
        string Priority "Low|Normal|High|Urgent"
        decimal EstimatedCost "Custo estimado"
        decimal ActualCost "Custo real"
        int EstimatedTimeMinutes "Tempo estimado (min)"
        int ActualTimeMinutes "Tempo real (min)"
        string Notes "ObservaÃ§Ãµes"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criaÃ§Ã£o"
        datetime ModifiedDate "Data de modificaÃ§Ã£o"
        string CreatedBy "UsuÃ¡rio criador"
    }

    %% === COMPOSIÃ‡ÃƒO DO PRODUTO ===
    PRODUCT_COMPOSITION {
        string Id PK "GUID Ãºnico"
        string DemandId FK "Demanda pai"
        string ProductComponentId FK "Componente escolhido"
        string HierarchyName "Nome hierarquia"
        string ComponentName "Nome componente"
        int Quantity "Quantidade componente"
        string Status "Pending|InProgress|Completed|Cancelled"
        datetime StartTime "InÃ­cio processamento"
        datetime CompletionTime "Fim processamento"
        decimal ComponentCost "Custo do componente"
        int ProcessingTimeMinutes "Tempo processamento"
        string WorkstationId "EstaÃ§Ã£o de trabalho"
        string OperatorId "Operador responsÃ¡vel"
        string QualityNotes "ObservaÃ§Ãµes qualidade"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criaÃ§Ã£o"
        datetime ModifiedDate "Data de modificaÃ§Ã£o"
    }

    %% === ORDEM DE PRODUÃ‡ÃƒO ===
    PRODUCTION_ORDER {
        string Id PK "GUID Ãºnico"
        string OrderNumber "NÃºmero sequencial"
        string BatchNumber "NÃºmero do lote"
        datetime ScheduledDate "Data agendada"
        datetime StartDate "Data inÃ­cio"
        datetime CompletionDate "Data conclusÃ£o"
        string ProductionStatus "Draft|Scheduled|InProgress|Completed|Cancelled"
        string ProductionType "Regular|Express|Batch"
        decimal TotalEstimatedCost "Custo total estimado"
        decimal TotalActualCost "Custo total real"
        int TotalEstimatedTime "Tempo total estimado"
        int TotalActualTime "Tempo total real"
        string SupervisorId "Supervisor responsÃ¡vel"
        string Notes "ObservaÃ§Ãµes gerais"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criaÃ§Ã£o"
        datetime ModifiedDate "Data de modificaÃ§Ã£o"
        string CreatedBy "UsuÃ¡rio criador"
    }

    %% === INTEGRAÃ‡Ã•ES COM OUTROS DOMÃNIOS ===

    %% VENDAS (ORIGEM DAS DEMANDAS)
    ORDER_ITEM {
        string Id PK "GUID Ãºnico"
        string OrderEntryId FK "Pedido pai"
        string ProductId FK "Produto"
        int Quantity "Quantidade pedida"
        string ProductConfiguration "JSON configuraÃ§Ã£o"
        string ItemStatus "Pending|Confirmed|InProduction|Completed"
        datetime CreatedDate "Data de criaÃ§Ã£o"
    }

    ORDER_ENTRY {
        string Id PK "GUID Ãºnico"
        string CustomerId FK "Cliente"
        datetime DeliveryDate "Data entrega"
        string OrderStatus "Pending|Confirmed|InProduction|Ready|Delivered"
        decimal TotalValue "Valor total"
    }

    %% PRODUTO (ESPECIFICAÃ‡Ã•ES)
    PRODUCT {
        string Id PK "GUID Ãºnico"
        string ProductType "Simple|Composite|Group"
        string Name "Nome produto"
        int AssemblyTime "Tempo montagem"
        string AssemblyInstructions "InstruÃ§Ãµes"
        decimal Cost "Custo base"
    }

    %% COMPONENTES (CONFIGURAÃ‡ÃƒO)
    PRODUCT_COMPONENT {
        string Id PK "GUID Ãºnico"
        string Name "Nome componente"
        string ProductComponentHierarchyId FK "Hierarquia"
        decimal AdditionalCost "Custo adicional"
        string StateCode "Active|Inactive"
    }

    %% COMPRAS (CONSUMO DE INGREDIENTES)
    INGREDIENT_STOCK {
        string Id PK "GUID Ãºnico"
        string IngredientId FK "Ingrediente"
        decimal CurrentQuantity "Qtd atual estoque"
        decimal MinimumLevel "NÃ­vel mÃ­nimo"
        string UnitOfMeasure "Unidade medida"
        datetime LastUpdated "Ãšltima atualizaÃ§Ã£o"
    }

    INGREDIENT_CONSUMPTION {
        string Id PK "GUID Ãºnico"
        string DemandId FK "Demanda consumidora"
        string IngredientId FK "Ingrediente consumido"
        decimal QuantityConsumed "Quantidade consumida"
        datetime ConsumptionDate "Data do consumo"
        string Notes "ObservaÃ§Ãµes"
    }

    %% ==========================================
    %% RELACIONAMENTOS PRINCIPAIS
    %% ==========================================

    %% FLUXO DE PRODUÃ‡ÃƒO
    DEMAND ||--o{ PRODUCT_COMPOSITION : "detalha tarefas"
    PRODUCTION_ORDER ||--o{ DEMAND : "agrupa demandas"

    %% ==========================================
    %% INTEGRAÃ‡Ã•ES COM OUTROS DOMÃNIOS
    %% ==========================================

    %% VENDAS â†’ PRODUÃ‡ÃƒO (AutomÃ¡tico)
    ORDER_ITEM ||--o{ DEMAND : "gera automaticamente"
    ORDER_ENTRY ||--o{ ORDER_ITEM : "contÃ©m"

    %% PRODUTO â†’ PRODUÃ‡ÃƒO (Consulta)
    PRODUCT ||--o{ DEMAND : "especifica produÃ§Ã£o"
    PRODUCT_COMPONENT ||--o{ PRODUCT_COMPOSITION : "define componente"

    %% PRODUÃ‡ÃƒO â†’ COMPRAS (Consumo)
    DEMAND ||--o{ INGREDIENT_CONSUMPTION : "consome ingredientes"
    INGREDIENT_STOCK ||--o{ INGREDIENT_CONSUMPTION : "fornece estoque"

    %% ==========================================
    %% STYLING POR DOMÃNIO
    %% ==========================================
    
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
    
    PRODUCTION_ORDER {
        background-color "#fba81d"
        color "black"
        border-color "#fba81d"
    }
    
    INGREDIENT_CONSUMPTION {
        background-color "#fba81d"
        color "black"
        border-color "#fba81d"
    }

    %% VENDAS = Laranja (#f36b21)
    ORDER_ITEM {
        background-color "#f36b21"
        color "white"
        border-color "#f36b21"
    }
    
    ORDER_ENTRY {
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
    
    PRODUCT_COMPONENT {
        background-color "#00a86b"
        color "white"
        border-color "#00a86b"
    }

    %% COMPRAS = Azul (#0562aa)
    INGREDIENT_STOCK {
        background-color "#0562aa"
        color "white"
        border-color "#0562aa"
    }
```

## ðŸ“‹ Detalhes das Entidades

### **ðŸŽ¯ DEMAND (Demanda de ProduÃ§Ã£o)**
- **PropÃ³sito**: Ordem de produÃ§Ã£o gerada automaticamente a partir de OrderItem
- **Relacionamento**: N:1 com OrderItem (origem), 1:N com ProductComposition (detalhamento)
- **Status Flow**: Pending â†’ Confirmed â†’ InProduction â†’ Finalizando â†’ Ready â†’ Delivered
- **CaracterÃ­sticas**: Quantidade, data limite, custos estimados/reais, tempos

### **ðŸ§© PRODUCT_COMPOSITION (Tarefa de ProduÃ§Ã£o)**
- **PropÃ³sito**: Detalha componentes especÃ­ficos de uma demanda (especialmente para ProductType.Composite)
- **Relacionamento**: N:1 com Demand, N:1 com ProductComponent
- **CaracterÃ­sticas**: Quantidade, tempos de processamento, estaÃ§Ã£o de trabalho, operador
- **Rastreamento**: Status individual por componente (Pending â†’ InProgress â†’ Completed)

### **ðŸ“‹ PRODUCTION_ORDER (Ordem de ProduÃ§Ã£o)**
- **PropÃ³sito**: Agrupa mÃºltiplas demandas para otimizaÃ§Ã£o de recursos e cronograma
- **Relacionamento**: 1:N com Demand
- **CaracterÃ­sticas**: Lote, agendamento, custos totais, supervisor responsÃ¡vel
- **Ciclo**: Draft â†’ Scheduled â†’ InProgress â†’ Completed

### **ðŸ”— Entidades de IntegraÃ§Ã£o**

#### **ORDER_ITEM** *(Origem do DomÃ­nio de Vendas)*
- **Relacionamento**: 1:N com Demand
- **Regra CrÃ­tica**: Toda alteraÃ§Ã£o em OrderItem deve propagar para Demands relacionadas

#### **PRODUCT** *(EspecificaÃ§Ã£o do DomÃ­nio de Produto)*
- **Relacionamento**: 1:N com Demand
- **Dados Utilizados**: AssemblyTime, AssemblyInstructions, Cost

#### **INGREDIENT_CONSUMPTION** *(IntegraÃ§Ã£o com DomÃ­nio de Compras)*
- **Relacionamento**: N:1 com Demand, N:1 com IngredientStock
- **PropÃ³sito**: Registrar consumo de ingredientes durante produÃ§Ã£o

## ðŸ”„ Fluxos de CriaÃ§Ã£o AutomÃ¡tica de Demandas

### **ðŸ“Š Regra de GeraÃ§Ã£o: 1 OrderItem â†’ 1:N Demand**

#### **ProductType.Simple**
```
OrderItem: 50x "Coxinha Comum"
â†“
Demand: 50x "Coxinha Comum" (DemandStatus: Pending)
```

#### **ProductType.Composite**
```
OrderItem: 1x "Bolo p/ 20 pessoas" 
    ProductConfiguration: {
        "massa": "branca",
        "recheio": ["brigadeiro", "morango"], 
        "cobertura": "chantilly"
    }
â†“
Demand: 1x "Bolo p/ 20 pessoas" (DemandStatus: Pending)
â”œâ”€â”€ ProductComposition: Massa Branca (Status: Pending)
â”œâ”€â”€ ProductComposition: Recheio Brigadeiro (Status: Pending)  
â”œâ”€â”€ ProductComposition: Recheio Morango (Status: Pending)
â””â”€â”€ ProductComposition: Cobertura Chantilly (Status: Pending)
```

#### **ProductType.Group**
```
OrderItem: 1x "Kit Festa 50 pessoas"
    ConfiguraÃ§Ã£o: {
        "bolo": 1x "Bolo p/ 50 pessoas",
        "salgados": 500x "Coxinha",
        "doces": 200x "Cajuzinho"
    }
â†“
Demand 1: 1x "Bolo p/ 50 pessoas" + ProductComposition detalhada
Demand 2: 500x "Coxinha" 
Demand 3: 200x "Cajuzinho"
```

### **ðŸ“ˆ SincronizaÃ§Ã£o de Status**

#### **OrderEntry.OrderStatus â†’ Demand.DemandStatus**
```
OrderEntry "SentToProduction" â†’ Todas Demands relacionadas:
1. DemandStatus: Pending â†’ Confirmed
2. CriaÃ§Ã£o de ProductionOrder agrupando as Demands
3. ProductionOrder.ProductionStatus: Draft â†’ Scheduled
```

#### **Demand.DemandStatus â†’ OrderItem.ItemStatus**
```
Demand "Completed" â†’ OrderItem "Completed"
Todas Demands de um OrderItem "Completed" â†’ OrderItem "Completed"
```

## ðŸ—ï¸ Processo de ExecuÃ§Ã£o de ProduÃ§Ã£o

### **1. Planejamento (ProductionOrder)**
```
1. Sistema agrupa Demands por:
   - Data de entrega (RequiredDate)
   - Tipo de produto (otimizaÃ§Ã£o de setup)
   - Prioridade (Priority)

2. Cria ProductionOrder com:
   - Agendamento otimizado
   - AlocaÃ§Ã£o de recursos
   - Estimativas de tempo e custo
```

### **2. ExecuÃ§Ã£o (ProductComposition)**
```
1. Para cada Demand na ProductionOrder:
   a. Lista ProductComposition relacionadas
   b. Ordena por AssemblyOrder (hierarquia)
   c. Executa componente por componente:
      - Status: Pending â†’ InProgress
      - Registra StartTime
      - Aloca workstation e operator
      - Consome ingredientes (IngredientConsumption)
      - Registra CompletionTime
      - Status: InProgress â†’ Completed

2. Quando todas ProductComposition estÃ£o "Completed":
   - Demand.DemandStatus: InProduction â†’ Ready
```

### **3. Controle de Qualidade**
```
1. QualityNotes em ProductComposition
2. ValidaÃ§Ã£o de especificaÃ§Ãµes
3. AprovaÃ§Ã£o final da Demand
4. DemandStatus: Ready â†’ Delivered
```

## ðŸ“Š MÃ©tricas de ProduÃ§Ã£o

### **Por Demand**
- **EficiÃªncia Temporal**: ActualTimeMinutes vs EstimatedTimeMinutes
- **EficiÃªncia de Custo**: ActualCost vs EstimatedCost
- **Taxa de Qualidade**: QualityNotes vs ProductComposition completadas

### **Por ProductionOrder**
- **Throughput**: Demands processadas por perÃ­odo
- **UtilizaÃ§Ã£o de Recursos**: Tempo de workstation ocupado
- **Cumprimento de Prazo**: Delivered vs RequiredDate

## ðŸŽ¯ Eventos de DomÃ­nio Gerados

- **DemandCreated**: Nova demanda gerada a partir de OrderItem
- **DemandStatusChanged**: MudanÃ§a de status de demanda
- **ProductionStarted**: InÃ­cio de processamento de ProductComposition
- **ComponentCompleted**: Componente especÃ­fico finalizado
- **DemandCompleted**: Toda a demanda finalizada
- **IngredientConsumed**: Consumo de ingrediente registrado
- **ProductionOrderCompleted**: Lote de produÃ§Ã£o finalizado

## ðŸš¨ Alertas e ValidaÃ§Ãµes

### **Alertas CrÃ­ticos**
- **Demand Atrasada**: RequiredDate < hoje E DemandStatus â‰  Ready|Delivered
- **Componente Parado**: ProductComposition InProgress > 2 horas sem update
- **Ingrediente Insuficiente**: Tentativa de consumo > IngredientStock disponÃ­vel

### **ValidaÃ§Ãµes de NegÃ³cio**
- Demand sÃ³ pode ser cancelada se OrderItem for cancelado
- ProductComposition sÃ³ pode ser criada para ProductType.Composite
- IngredientConsumption deve respeitar estoque disponÃ­vel
- ProductionOrder sÃ³ pode agrupar Demands com status Confirmed

---

**Arquivo**: `03-production-domain-erd.md`  
**DomÃ­nio**: ProduÃ§Ã£o (#fba81d)  
**Tipo**: Entity-Relationship Diagram  
**NÃ­vel**: Detalhado + Fluxos AutomÃ¡ticos + IntegraÃ§Ãµes
