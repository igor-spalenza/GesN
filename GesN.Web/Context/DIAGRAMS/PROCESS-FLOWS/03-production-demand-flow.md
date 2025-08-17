# 🏭 FLUXOGRAMA - PROCESSO DE PRODUÇÃO

## 🎯 Visão Geral
Fluxograma completo do processo de criação automática de demandas a partir de OrderItems e gerenciamento de produção através de ProductComposition e ProductionOrder. Mostra como 1 OrderItem pode gerar 1:N Demands dependendo do tipo de produto.

## 🔄 Fluxo de Criação Automática de Demandas

```mermaid
flowchart TD
    %% === TRIGGER INICIAL ===
    A[⚡ TRIGGER:<br/>OrderItem criado/editado] --> B{🎯 Identificar<br/>Product.ProductType}
    
    %% === FLUXOS POR TIPO DE PRODUTO ===
    
    %% PRODUTO SIMPLES
    B -->|Simple| C1[📦 Produto Simples]
    C1 --> C2[🏭 Criar 1 Demand<br/>Quantity = OrderItem.Quantity]
    C2 --> C3[📋 DemandStatus: Pending<br/>ProductionOrderId: null]
    C3 --> C4[✅ Demand Simples criada]
    
    %% PRODUTO COMPOSTO  
    B -->|Composite| D1[🧩 Produto Composto]
    D1 --> D2[🏭 Criar 1 Demand<br/>Quantity = OrderItem.Quantity]
    D2 --> D3[📄 Analisar ProductConfiguration<br/>JSON do OrderItem]
    D3 --> D4[🔍 Para cada componente configurado]
    D4 --> D5[🧩 Criar ProductComposition<br/>vinculada à Demand]
    D5 --> D6[📋 Status: Pending<br/>HierarchyName + ComponentName]
    D6 --> D7{🔄 Mais componentes?}
    D7 -->|Sim| D4
    D7 -->|Não| D8[✅ Demand Composta<br/>+ ProductComposition criadas]
    
    %% PRODUTO GRUPO
    B -->|Group| E1[📦 Grupo de Produtos]
    E1 --> E2[🔍 Analisar configuração<br/>do grupo no OrderItem]
    E2 --> E3[📊 Explodir grupo em<br/>produtos concretos]
    E3 --> E4[📋 Para cada produto concreto]
    E4 --> E5{🎯 Produto concreto<br/>é Simple ou Composite?}
    
    E5 -->|Simple| E6[🏭 Criar Demand Simple<br/>para este produto]
    E5 -->|Composite| E7[🏭 Criar Demand Composite<br/>+ ProductComposition]
    
    E6 --> E8[✅ Demand criada]
    E7 --> E8
    E8 --> E9{🔄 Mais produtos<br/>no grupo?}
    E9 -->|Sim| E4
    E9 -->|Não| E10[✅ N Demands criadas<br/>para o grupo]
    
    %% === CONSOLIDAÇÃO ===
    C4 --> F[📊 Todas Demands criadas<br/>com Status: Pending]
    D8 --> F
    E10 --> F
    
    F --> G[🔔 Notificar sistema<br/>de novas demandas]
    G --> H[📋 Aguardar OrderEntry<br/>Status: SentToProduction]
    
    %% === AGRUPAMENTO EM PRODUCTION ORDER ===
    H --> I[⚡ TRIGGER:<br/>OrderStatus = SentToProduction]
    I --> J[🏭 Criar ProductionOrder]
    J --> K[📊 Agrupar Demands relacionadas]
    K --> L[🔄 Para cada Demand do pedido]
    L --> M[🔗 Vincular à ProductionOrder<br/>DemandStatus: Confirmed]
    M --> N{🔄 Mais Demands?}
    N -->|Sim| L
    N -->|Não| O[✅ ProductionOrder criada<br/>Status: Scheduled]
    
    O --> P[🎯 Produção pode iniciar]
    
    %% === STYLING POR TIPO ===
    
    %% TRIGGER E GERAL = Cinza
    classDef triggerStyle fill:#e5e7eb,stroke:#6b7280,stroke-width:2px,color:black
    class A,B,F,G,H,I,P triggerStyle
    
    %% PRODUTO SIMPLES = Verde claro
    classDef simpleStyle fill:#a7f3d0,stroke:#00a86b,stroke-width:2px,color:black
    class C1,C2,C3,C4,E6,E8 simpleStyle
    
    %% PRODUTO COMPOSTO = Verde médio  
    classDef compositeStyle fill:#6ee7b7,stroke:#00a86b,stroke-width:2px,color:black
    class D1,D2,D3,D4,D5,D6,D7,D8,E7 compositeStyle
    
    %% PRODUTO GRUPO = Verde escuro
    classDef groupStyle fill:#10b981,stroke:#00a86b,stroke-width:2px,color:white
    class E1,E2,E3,E4,E5,E9,E10 groupStyle
    
    %% PRODUCTION ORDER = Dourado
    classDef productionStyle fill:#fba81d,stroke:#fba81d,stroke-width:2px,color:black
    class J,K,L,M,N,O productionStyle
```

## 📋 Detalhamento da Geração por Tipo de Produto

### **🔷 Geração para Produto Simples**

#### **Regra de Conversão 1:1:**
```
1 OrderItem (Simple) → 1 Demand

Exemplo:
OrderItem: 50x "Coxinha Comum"
↓
Demand: {
  OrderItemId: OrderItem.Id,
  ProductId: "coxinha-comum",
  Quantity: 50,
  RequiredDate: OrderEntry.DeliveryDate,
  DemandStatus: "Pending",
  ProductionOrderId: null
}
```

#### **Características:**
- ✅ Processo mais direto
- ✅ Sem ProductComposition necessária
- ✅ Tempo de processamento: ~50ms
- ✅ Não requer configuração adicional

### **🔶 Geração para Produto Composto**

#### **Processo Detalhado:**
```mermaid
flowchart TD
    A[🧩 OrderItem Composto] --> B[📄 Ler ProductConfiguration<br/>JSON]
    B --> C[🔍 Parse configuração<br/>por hierarquia]
    C --> D[📋 Para cada hierarquia]
    
    D --> E[🏷️ Extrair HierarchyName]
    E --> F[🧩 Para cada componente<br/>selecionado na hierarquia]
    F --> G[🔧 Criar ProductComposition]
    G --> H[📊 Definir propriedades]
    H --> I{🔄 Mais componentes<br/>na hierarquia?}
    
    I -->|Sim| F
    I -->|Não| J{🔄 Mais hierarquias?}
    J -->|Sim| D
    J -->|Não| K[✅ Todas ProductComposition<br/>criadas]
    
    classDef compositeStyle fill:#6ee7b7,stroke:#00a86b,stroke-width:2px,color:black
    class A,B,C,D,E,F,G,H,I,J,K compositeStyle
```

#### **Estrutura ProductConfiguration (JSON):**
```json
{
  "massa": {
    "hierarchyId": "hierarchy-massa-id",
    "componentId": "component-chocolate-id",
    "componentName": "Massa de Chocolate",
    "quantity": 1,
    "additionalCost": 0.00
  },
  "recheio": {
    "hierarchyId": "hierarchy-recheio-id", 
    "components": [
      {
        "componentId": "component-brigadeiro-id",
        "componentName": "Recheio de Brigadeiro",
        "quantity": 1,
        "additionalCost": 0.00
      },
      {
        "componentId": "component-morango-id", 
        "componentName": "Recheio de Morango",
        "quantity": 1,
        "additionalCost": 5.00
      }
    ]
  },
  "cobertura": {
    "hierarchyId": "hierarchy-cobertura-id",
    "componentId": "component-chantilly-id",
    "componentName": "Cobertura Chantilly",
    "quantity": 1,
    "additionalCost": 0.00
  }
}
```

#### **ProductComposition Resultante:**
```
Para o exemplo acima, gera 4 ProductComposition:

1. ProductComposition {
     DemandId: demand-id,
     ProductComponentId: "component-chocolate-id",
     HierarchyName: "Massa",
     ComponentName: "Massa de Chocolate", 
     Quantity: 1,
     Status: "Pending"
   }

2. ProductComposition {
     DemandId: demand-id,
     ProductComponentId: "component-brigadeiro-id",
     HierarchyName: "Recheio",
     ComponentName: "Recheio de Brigadeiro",
     Quantity: 1, 
     Status: "Pending"
   }

3. ProductComposition {
     DemandId: demand-id,
     ProductComponentId: "component-morango-id",
     HierarchyName: "Recheio", 
     ComponentName: "Recheio de Morango",
     Quantity: 1,
     Status: "Pending"
   }

4. ProductComposition {
     DemandId: demand-id,
     ProductComponentId: "component-chantilly-id",
     HierarchyName: "Cobertura",
     ComponentName: "Cobertura Chantilly",
     Quantity: 1,
     Status: "Pending"
   }
```

### **🔸 Geração para Grupo de Produtos**

#### **Processo de Explosão:**
```mermaid
flowchart TD
    A[📦 OrderItem Group] --> B[📊 Analisar configuração<br/>do grupo]
    B --> C[🔍 Para cada item configurado]
    
    C --> D{🎯 Item é produto<br/>Simple ou Composite?}
    
    D -->|Simple| E1[📦 Criar Demand Simple]
    E1 --> E2[✅ Demand: ProductId + Quantity]
    
    D -->|Composite| F1[🧩 Criar Demand Composite]
    F1 --> F2[📄 Processar configuração<br/>específica do item]
    F2 --> F3[🧩 Gerar ProductComposition<br/>para o item]
    
    E2 --> G[📊 Adicionar à lista<br/>de Demands]
    F3 --> G
    
    G --> H{🔄 Mais itens<br/>no grupo?}
    H -->|Sim| C
    H -->|Não| I[✅ N Demands criadas]
    
    classDef groupStyle fill:#10b981,stroke:#00a86b,stroke-width:2px,color:white
    class A,B,C,D,G,H,I groupStyle
    
    classDef simpleStyle fill:#a7f3d0,stroke:#00a86b,stroke-width:2px,color:black
    class E1,E2 simpleStyle
    
    classDef compositeStyle fill:#6ee7b7,stroke:#00a86b,stroke-width:2px,color:black
    class F1,F2,F3 compositeStyle
```

#### **Exemplo Prático Completo:**
```
OrderItem: 1x "Kit Festa 50 pessoas"

Configuração final do grupo:
├── 1x Bolo p/ 50 pessoas (Composite)
│   ├── Massa: Chocolate
│   ├── Recheio: Brigadeiro + Morango
│   └── Cobertura: Chantilly
├── 400x Coxinha (Simple)
├── 50x Torta de Frango (Simple) 
├── 150x Cajuzinho (Simple)
├── 25x Torta de Morango (Simple)
└── 3x Refrigerante 2L (Simple)

Demands geradas:

1. Demand (Composite): 1x Bolo p/ 50 pessoas
   ├── ProductComposition: Massa Chocolate
   ├── ProductComposition: Recheio Brigadeiro
   ├── ProductComposition: Recheio Morango
   └── ProductComposition: Cobertura Chantilly

2. Demand (Simple): 400x Coxinha
3. Demand (Simple): 50x Torta de Frango  
4. Demand (Simple): 150x Cajuzinho
5. Demand (Simple): 25x Torta de Morango
6. Demand (Simple): 3x Refrigerante 2L

Total: 6 Demands (1 Composite + 5 Simple)
```

## 🏭 Processo de Execução de Produção

### **📋 Agrupamento em ProductionOrder**

```mermaid
flowchart TD
    A[⚡ OrderStatus = SentToProduction] --> B[🔍 Localizar todas Demands<br/>do OrderEntry]
    B --> C[🏭 Criar ProductionOrder]
    
    C --> D[📊 Para cada Demand encontrada]
    D --> E[🔗 Atualizar DemandId.ProductionOrderId]
    E --> F[📈 DemandStatus: Pending → Confirmed]
    F --> G{🔄 Mais Demands?}
    
    G -->|Sim| D
    G -->|Não| H[📋 ProductionOrder.Status: Scheduled]
    H --> I[⏰ Calcular tempo total<br/>estimado de produção]
    I --> J[💰 Calcular custo total<br/>estimado]
    J --> K[✅ ProductionOrder pronta<br/>para iniciar produção]
    
    classDef productionStyle fill:#fba81d,stroke:#fba81d,stroke-width:2px,color:black
    class A,B,C,D,E,F,G,H,I,J,K productionStyle
```

### **⚙️ Execução de Demandas**

```mermaid
flowchart TD
    A[🚀 Produção inicia<br/>ProductionOrder] --> B[📋 Selecionar Demand<br/>Status: Confirmed]
    B --> C[📈 DemandStatus: Confirmed → InProduction]
    
    C --> D{🎯 Demand é Simple<br/>ou Composite?}
    
    %% SIMPLE
    D -->|Simple| E1[📦 Produção direta]
    E1 --> E2[⏰ Registrar StartTime]
    E2 --> E3[🏭 Executar produção]
    E3 --> E4[⏰ Registrar CompletionTime]
    E4 --> E5[📈 DemandStatus: InProduction → Ready]
    
    %% COMPOSITE  
    D -->|Composite| F1[🧩 Listar ProductComposition<br/>da Demand]
    F1 --> F2[📊 Ordenar por HierarchyName<br/>conforme AssemblyOrder]
    F2 --> F3[🔧 Para cada ProductComposition]
    
    F3 --> F4[📈 Status: Pending → InProgress]
    F4 --> F5[⏰ StartTime = now()]
    F5 --> F6[🏭 Executar tarefa específica]
    F6 --> F7[⏰ CompletionTime = now()]
    F7 --> F8[📈 Status: InProgress → Completed]
    F8 --> F9{🔄 Mais ProductComposition?}
    
    F9 -->|Sim| F3
    F9 -->|Não| F10[✔️ Verificar se todas<br/>estão Completed]
    F10 --> F11[📈 DemandStatus: InProduction → Ready]
    
    %% CONSOLIDAÇÃO
    E5 --> G[✅ Demand concluída]
    F11 --> G
    
    G --> H{🔄 Mais Demands<br/>na ProductionOrder?}
    H -->|Sim| B
    H -->|Não| I[📈 ProductionOrder.Status:<br/>InProgress → Completed]
    I --> J[🎉 Produção finalizada]
    
    %% STYLING
    classDef productionStyle fill:#fba81d,stroke:#fba81d,stroke-width:2px,color:black
    class A,B,C,D,G,H,I,J productionStyle
    
    classDef simpleStyle fill:#a7f3d0,stroke:#00a86b,stroke-width:2px,color:black
    class E1,E2,E3,E4,E5 simpleStyle
    
    classDef compositeStyle fill:#6ee7b7,stroke:#00a86b,stroke-width:2px,color:black
    class F1,F2,F3,F4,F5,F6,F7,F8,F9,F10,F11 compositeStyle
```

## 📊 Estados e Transições de Status

### **🎯 Demand Status Flow:**
```mermaid
stateDiagram-v2
    [*] --> Pending : OrderItem criado
    Pending --> Confirmed : OrderStatus = SentToProduction
    Confirmed --> InProduction : Produção inicia
    InProduction --> Finalizando : Componentes prontos
    Finalizando --> Ready : Finalização/embalagem
    Ready --> Delivered : Produto entregue
    
    Pending --> Cancelled : OrderItem cancelado
    Confirmed --> Cancelled : Pedido cancelado
    InProduction --> Cancelled : Problemas produção
    
    Delivered --> [*] : Processo concluído
    Cancelled --> [*] : Processo cancelado
    
    %% Styling
    classDef pending fill:#fef3c7,stroke:#f59e0b,stroke-width:2px
    classDef confirmed fill:#d1fae5,stroke:#10b981,stroke-width:2px
    classDef production fill:#fed7aa,stroke:#f97316,stroke-width:2px
    classDef ready fill:#dbeafe,stroke:#3b82f6,stroke-width:2px
    classDef cancelled fill:#fecaca,stroke:#ef4444,stroke-width:2px
    
    class Pending pending
    class Confirmed confirmed
    class InProduction,Finalizando production
    class Ready,Delivered ready
    class Cancelled cancelled
```

### **🔧 ProductComposition Status Flow:**
```mermaid
stateDiagram-v2
    [*] --> Pending : Demand criada
    Pending --> InProgress : Tarefa iniciada
    InProgress --> Completed : Tarefa finalizada
    
    Pending --> Cancelled : Demand cancelada
    InProgress --> Cancelled : Problema na tarefa
    
    Completed --> [*] : Tarefa concluída
    Cancelled --> [*] : Tarefa cancelada
```

## 🚨 Regras de Negócio e Validações

### **Criação de Demands:**
- ✅ OrderItem deve ter Product ativo
- ✅ ProductConfiguration deve ser válida (para Composite)
- ✅ RequiredDate = OrderEntry.DeliveryDate - Product.AssemblyTime
- ✅ Uma Demand só pode ter uma ProductionOrder

### **ProductComposition:**
- ✅ Só criada para ProductType.Composite
- ✅ HierarchyName preservado mesmo se hierarquia for alterada
- ✅ Quantidade deve respeitar configuração do OrderItem
- ✅ Status individual por componente

### **ProductionOrder:**
- ✅ Só pode agrupar Demands com status Confirmed
- ✅ Todas Demands devem ter mesmo RequiredDate (±1 dia)
- ✅ Capacidade máxima de produção respeitada
- ✅ Ingredientes disponíveis validados

### **Consumo de Ingredientes:**
- ✅ Validar estoque antes de iniciar produção
- ✅ Reservar ingredientes ao confirmar Demand
- ✅ Consumir ingredientes ao completar ProductComposition
- ✅ Atualizar IngredientStock automaticamente

## 🎯 Eventos de Domínio Gerados

- **DemandCreated**: Nova demanda gerada automaticamente
- **ProductCompositionCreated**: Tarefa específica criada
- **DemandStatusChanged**: Mudança de status de demanda
- **ProductionOrderCreated**: Agrupamento de demandas
- **ProductionStarted**: Início de processamento
- **ComponentCompleted**: Componente específico finalizado
- **DemandCompleted**: Demanda totalmente finalizada
- **IngredientConsumed**: Consumo de ingrediente registrado

---

**Arquivo**: `03-production-demand-flow.md`  
**Domínio**: Produção (#fba81d)  
**Tipo**: Process Flowchart  
**Foco**: Geração Automática de Demands + Execução de Produção
