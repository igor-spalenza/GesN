# 🏭 ERD - DOMÍNIO DE PRODUÇÃO

## 🎯 Visão Geral
Diagrama Entity-Relationship completo do Domínio de Produção, mostrando como as demandas são geradas automaticamente a partir de OrderItems e como são gerenciadas através de ProductComposition e ProductionOrder. Este domínio traduz vendas em tarefas de produção executáveis.

## 🗄️ Diagrama de Entidades e Relacionamentos

```mermaid
erDiagram
    %% === DOMÍNIO DE PRODUÇÃO ===
    
    %% === DEMANDA DE PRODUÇÃO ===
    DEMAND {
        string Id PK "GUID único"
        string OrderItemId FK "Item pedido origem"
        string ProductId FK "Produto a produzir"
        string ProductionOrderId FK "Ordem produção (opcional)"
        int Quantity "Quantidade a produzir"
        datetime RequiredDate "Data limite entrega"
        datetime StartDate "Data início produção"
        datetime CompletionDate "Data conclusão"
        string DemandStatus "Pending|Confirmed|InProduction|Finalizando|Ready|Delivered|Cancelled"
        string Priority "Low|Normal|High|Urgent"
        decimal EstimatedCost "Custo estimado"
        decimal ActualCost "Custo real"
        int EstimatedTimeMinutes "Tempo estimado (min)"
        int ActualTimeMinutes "Tempo real (min)"
        string Notes "Observações"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criação"
        datetime ModifiedDate "Data de modificação"
        string CreatedBy "Usuário criador"
    }

    %% === COMPOSIÇÃO DO PRODUTO ===
    PRODUCT_COMPOSITION {
        string Id PK "GUID único"
        string DemandId FK "Demanda pai"
        string ProductComponentId FK "Componente escolhido"
        string HierarchyName "Nome hierarquia"
        string ComponentName "Nome componente"
        int Quantity "Quantidade componente"
        string Status "Pending|InProgress|Completed|Cancelled"
        datetime StartTime "Início processamento"
        datetime CompletionTime "Fim processamento"
        decimal ComponentCost "Custo do componente"
        int ProcessingTimeMinutes "Tempo processamento"
        string WorkstationId "Estação de trabalho"
        string OperatorId "Operador responsável"
        string QualityNotes "Observações qualidade"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criação"
        datetime ModifiedDate "Data de modificação"
    }

    %% === ORDEM DE PRODUÇÃO ===
    PRODUCTION_ORDER {
        string Id PK "GUID único"
        string OrderNumber "Número sequencial"
        string BatchNumber "Número do lote"
        datetime ScheduledDate "Data agendada"
        datetime StartDate "Data início"
        datetime CompletionDate "Data conclusão"
        string ProductionStatus "Draft|Scheduled|InProgress|Completed|Cancelled"
        string ProductionType "Regular|Express|Batch"
        decimal TotalEstimatedCost "Custo total estimado"
        decimal TotalActualCost "Custo total real"
        int TotalEstimatedTime "Tempo total estimado"
        int TotalActualTime "Tempo total real"
        string SupervisorId "Supervisor responsável"
        string Notes "Observações gerais"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criação"
        datetime ModifiedDate "Data de modificação"
        string CreatedBy "Usuário criador"
    }

    %% === INTEGRAÇÕES COM OUTROS DOMÍNIOS ===

    %% VENDAS (ORIGEM DAS DEMANDAS)
    ORDER_ITEM {
        string Id PK "GUID único"
        string OrderEntryId FK "Pedido pai"
        string ProductId FK "Produto"
        int Quantity "Quantidade pedida"
        string ProductConfiguration "JSON configuração"
        string ItemStatus "Pending|Confirmed|InProduction|Completed"
        datetime CreatedDate "Data de criação"
    }

    ORDER_ENTRY {
        string Id PK "GUID único"
        string CustomerId FK "Cliente"
        datetime DeliveryDate "Data entrega"
        string OrderStatus "Pending|Confirmed|InProduction|Ready|Delivered"
        decimal TotalValue "Valor total"
    }

    %% PRODUTO (ESPECIFICAÇÕES)
    PRODUCT {
        string Id PK "GUID único"
        string ProductType "Simple|Composite|Group"
        string Name "Nome produto"
        int AssemblyTime "Tempo montagem"
        string AssemblyInstructions "Instruções"
        decimal Cost "Custo base"
    }

    %% COMPONENTES (CONFIGURAÇÃO)
    PRODUCT_COMPONENT {
        string Id PK "GUID único"
        string Name "Nome componente"
        string ProductComponentHierarchyId FK "Hierarquia"
        decimal AdditionalCost "Custo adicional"
        string StateCode "Active|Inactive"
    }

    %% COMPRAS (CONSUMO DE INGREDIENTES)
    INGREDIENT_STOCK {
        string Id PK "GUID único"
        string IngredientId FK "Ingrediente"
        decimal CurrentQuantity "Qtd atual estoque"
        decimal MinimumLevel "Nível mínimo"
        string UnitOfMeasure "Unidade medida"
        datetime LastUpdated "Última atualização"
    }

    INGREDIENT_CONSUMPTION {
        string Id PK "GUID único"
        string DemandId FK "Demanda consumidora"
        string IngredientId FK "Ingrediente consumido"
        decimal QuantityConsumed "Quantidade consumida"
        datetime ConsumptionDate "Data do consumo"
        string Notes "Observações"
    }

    %% ==========================================
    %% RELACIONAMENTOS PRINCIPAIS
    %% ==========================================

    %% FLUXO DE PRODUÇÃO
    DEMAND ||--o{ PRODUCT_COMPOSITION : "detalha tarefas"
    PRODUCTION_ORDER ||--o{ DEMAND : "agrupa demandas"

    %% ==========================================
    %% INTEGRAÇÕES COM OUTROS DOMÍNIOS
    %% ==========================================

    %% VENDAS → PRODUÇÃO (Automático)
    ORDER_ITEM ||--o{ DEMAND : "gera automaticamente"
    ORDER_ENTRY ||--o{ ORDER_ITEM : "contém"

    %% PRODUTO → PRODUÇÃO (Consulta)
    PRODUCT ||--o{ DEMAND : "especifica produção"
    PRODUCT_COMPONENT ||--o{ PRODUCT_COMPOSITION : "define componente"

    %% PRODUÇÃO → COMPRAS (Consumo)
    DEMAND ||--o{ INGREDIENT_CONSUMPTION : "consome ingredientes"
    INGREDIENT_STOCK ||--o{ INGREDIENT_CONSUMPTION : "fornece estoque"

    %% ==========================================
    %% STYLING POR DOMÍNIO
    %% ==========================================
    
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

## 📋 Detalhes das Entidades

### **🎯 DEMAND (Demanda de Produção)**
- **Propósito**: Ordem de produção gerada automaticamente a partir de OrderItem
- **Relacionamento**: N:1 com OrderItem (origem), 1:N com ProductComposition (detalhamento)
- **Status Flow**: Pending → Confirmed → InProduction → Finalizando → Ready → Delivered
- **Características**: Quantidade, data limite, custos estimados/reais, tempos

### **🧩 PRODUCT_COMPOSITION (Tarefa de Produção)**
- **Propósito**: Detalha componentes específicos de uma demanda (especialmente para ProductType.Composite)
- **Relacionamento**: N:1 com Demand, N:1 com ProductComponent
- **Características**: Quantidade, tempos de processamento, estação de trabalho, operador
- **Rastreamento**: Status individual por componente (Pending → InProgress → Completed)

### **📋 PRODUCTION_ORDER (Ordem de Produção)**
- **Propósito**: Agrupa múltiplas demandas para otimização de recursos e cronograma
- **Relacionamento**: 1:N com Demand
- **Características**: Lote, agendamento, custos totais, supervisor responsável
- **Ciclo**: Draft → Scheduled → InProgress → Completed

### **🔗 Entidades de Integração**

#### **ORDER_ITEM** *(Origem do Domínio de Vendas)*
- **Relacionamento**: 1:N com Demand
- **Regra Crítica**: Toda alteração em OrderItem deve propagar para Demands relacionadas

#### **PRODUCT** *(Especificação do Domínio de Produto)*
- **Relacionamento**: 1:N com Demand
- **Dados Utilizados**: AssemblyTime, AssemblyInstructions, Cost

#### **INGREDIENT_CONSUMPTION** *(Integração com Domínio de Compras)*
- **Relacionamento**: N:1 com Demand, N:1 com IngredientStock
- **Propósito**: Registrar consumo de ingredientes durante produção

## 🔄 Fluxos de Criação Automática de Demandas

### **📊 Regra de Geração: 1 OrderItem → 1:N Demand**

#### **ProductType.Simple**
```
OrderItem: 50x "Coxinha Comum"
↓
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
↓
Demand: 1x "Bolo p/ 20 pessoas" (DemandStatus: Pending)
├── ProductComposition: Massa Branca (Status: Pending)
├── ProductComposition: Recheio Brigadeiro (Status: Pending)  
├── ProductComposition: Recheio Morango (Status: Pending)
└── ProductComposition: Cobertura Chantilly (Status: Pending)
```

#### **ProductType.Group**
```
OrderItem: 1x "Kit Festa 50 pessoas"
    Configuração: {
        "bolo": 1x "Bolo p/ 50 pessoas",
        "salgados": 500x "Coxinha",
        "doces": 200x "Cajuzinho"
    }
↓
Demand 1: 1x "Bolo p/ 50 pessoas" + ProductComposition detalhada
Demand 2: 500x "Coxinha" 
Demand 3: 200x "Cajuzinho"
```

### **📈 Sincronização de Status**

#### **OrderEntry.OrderStatus → Demand.DemandStatus**
```
OrderEntry "SentToProduction" → Todas Demands relacionadas:
1. DemandStatus: Pending → Confirmed
2. Criação de ProductionOrder agrupando as Demands
3. ProductionOrder.ProductionStatus: Draft → Scheduled
```

#### **Demand.DemandStatus → OrderItem.ItemStatus**
```
Demand "Completed" → OrderItem "Completed"
Todas Demands de um OrderItem "Completed" → OrderItem "Completed"
```

## 🏗️ Processo de Execução de Produção

### **1. Planejamento (ProductionOrder)**
```
1. Sistema agrupa Demands por:
   - Data de entrega (RequiredDate)
   - Tipo de produto (otimização de setup)
   - Prioridade (Priority)

2. Cria ProductionOrder com:
   - Agendamento otimizado
   - Alocação de recursos
   - Estimativas de tempo e custo
```

### **2. Execução (ProductComposition)**
```
1. Para cada Demand na ProductionOrder:
   a. Lista ProductComposition relacionadas
   b. Ordena por AssemblyOrder (hierarquia)
   c. Executa componente por componente:
      - Status: Pending → InProgress
      - Registra StartTime
      - Aloca workstation e operator
      - Consome ingredientes (IngredientConsumption)
      - Registra CompletionTime
      - Status: InProgress → Completed

2. Quando todas ProductComposition estão "Completed":
   - Demand.DemandStatus: InProduction → Ready
```

### **3. Controle de Qualidade**
```
1. QualityNotes em ProductComposition
2. Validação de especificações
3. Aprovação final da Demand
4. DemandStatus: Ready → Delivered
```

## 📊 Métricas de Produção

### **Por Demand**
- **Eficiência Temporal**: ActualTimeMinutes vs EstimatedTimeMinutes
- **Eficiência de Custo**: ActualCost vs EstimatedCost
- **Taxa de Qualidade**: QualityNotes vs ProductComposition completadas

### **Por ProductionOrder**
- **Throughput**: Demands processadas por período
- **Utilização de Recursos**: Tempo de workstation ocupado
- **Cumprimento de Prazo**: Delivered vs RequiredDate

## 🎯 Eventos de Domínio Gerados

- **DemandCreated**: Nova demanda gerada a partir de OrderItem
- **DemandStatusChanged**: Mudança de status de demanda
- **ProductionStarted**: Início de processamento de ProductComposition
- **ComponentCompleted**: Componente específico finalizado
- **DemandCompleted**: Toda a demanda finalizada
- **IngredientConsumed**: Consumo de ingrediente registrado
- **ProductionOrderCompleted**: Lote de produção finalizado

## 🚨 Alertas e Validações

### **Alertas Críticos**
- **Demand Atrasada**: RequiredDate < hoje E DemandStatus ≠ Ready|Delivered
- **Componente Parado**: ProductComposition InProgress > 2 horas sem update
- **Ingrediente Insuficiente**: Tentativa de consumo > IngredientStock disponível

### **Validações de Negócio**
- Demand só pode ser cancelada se OrderItem for cancelado
- ProductComposition só pode ser criada para ProductType.Composite
- IngredientConsumption deve respeitar estoque disponível
- ProductionOrder só pode agrupar Demands com status Confirmed

---

**Arquivo**: `03-production-domain-erd.md`  
**Domínio**: Produção (#fba81d)  
**Tipo**: Entity-Relationship Diagram  
**Nível**: Detalhado + Fluxos Automáticos + Integrações
