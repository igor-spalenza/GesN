# ðŸ­ DIAGRAMA DE ESTADOS - CICLO DE VIDA DA DEMANDA

## ðŸŽ¯ VisÃ£o Geral
Diagrama de estados completo mostrando o ciclo de vida de uma Demand (Demanda de ProduÃ§Ã£o), desde sua criaÃ§Ã£o automÃ¡tica a partir de OrderItem atÃ© a entrega final, incluindo sincronizaÃ§Ã£o com ProductComposition e ProductionOrder.

## ðŸ”„ Diagrama Principal de Estados

```mermaid
stateDiagram-v2
    [*] --> Pending : ðŸ†• CriaÃ§Ã£o automÃ¡tica<br/>OrderItem criado/editado
    
    %% === ESTADOS PRINCIPAIS ===
    Pending --> Confirmed : âœ… OrderEntry confirmada<br/>Auto: Sistema confirma todas Demands
    Confirmed --> InProduction : âš™ï¸ ProductionOrder inicia<br/>Manual: Supervisor/operador
    InProduction --> Finalizando : ðŸ”§ ProduÃ§Ã£o quase completa<br/>Auto: Ãšltimas ProductComposition
    Finalizando --> Ready : ðŸ“¦ ProduÃ§Ã£o finalizada<br/>Auto: Todas tarefas completas
    Ready --> Delivered : ðŸšš Produto entregue<br/>Auto: OrderEntry delivered
    
    %% === CANCELAMENTOS ===
    Pending --> Cancelled : âŒ OrderItem cancelado<br/>Auto: PropagaÃ§Ã£o automÃ¡tica
    Confirmed --> Cancelled : âŒ OrderEntry cancelada<br/>Auto: PropagaÃ§Ã£o automÃ¡tica  
    InProduction --> Cancelled : âŒ Problema na produÃ§Ã£o<br/>Manual: Supervisor cancela
    
    %% === ESTADOS FINAIS ===
    Delivered --> [*] : ðŸŽ‰ Demanda completamente finalizada
    Cancelled --> [*] : ðŸš« Demanda cancelada
    
    %% === STYLING POR FASE ===
    
    %% CRIAÃ‡ÃƒO E CONFIRMAÃ‡ÃƒO
    classDef pending fill:#fef3c7,stroke:#f59e0b,stroke-width:3px,color:black
    class Pending pending
    
    classDef confirmed fill:#d1fae5,stroke:#10b981,stroke-width:3px,color:black
    class Confirmed confirmed
    
    %% PRODUÃ‡ÃƒO
    classDef production fill:#fed7aa,stroke:#f97316,stroke-width:3px,color:black
    class InProduction,Finalizando production
    
    %% FINALIZAÃ‡ÃƒO
    classDef ready fill:#dbeafe,stroke:#3b82f6,stroke-width:3px,color:black
    class Ready ready
    
    classDef delivered fill:#e0e7ff,stroke:#6366f1,stroke-width:3px,color:black
    class Delivered delivered
    
    %% CANCELAMENTO
    classDef cancelled fill:#fecaca,stroke:#ef4444,stroke-width:3px,color:black
    class Cancelled cancelled
```

## ðŸ“‹ Detalhamento dos Estados

### **ðŸŸ¡ PENDING (Pendente)**
```
ðŸ“Œ Estado Inicial
â”œâ”€â”€ Origem: OrderItem criado automaticamente
â”œâ”€â”€ DescriÃ§Ã£o: Demanda criada mas nÃ£o confirmada
â”œâ”€â”€ Permitido: Aguardar confirmaÃ§Ã£o do pedido
â”œâ”€â”€ Bloqueado: NÃ£o inicia produÃ§Ã£o
â””â”€â”€ PrÃ³ximo Estado: Confirmed ou Cancelled

CriaÃ§Ã£o AutomÃ¡tica:
â”œâ”€â”€ ðŸ”— OrderItemId: Vinculada ao item origem
â”œâ”€â”€ ðŸ“¦ ProductId: Produto a ser produzido
â”œâ”€â”€ ðŸ”¢ Quantity: Quantidade solicitada
â”œâ”€â”€ ðŸ“… RequiredDate: OrderEntry.DeliveryDate
â””â”€â”€ ðŸ­ ProductionOrderId: null (ainda nÃ£o agrupada)
```

**Dados Iniciais da Demand:**
```mermaid
flowchart TD
    A[OrderItem criado/editado] --> B{ðŸŽ¯ Product.ProductType}
    
    B -->|Simple| C[ðŸ­ Criar Demand Simple<br/>Quantity = OrderItem.Quantity]
    
    B -->|Composite| D[ðŸ­ Criar Demand Composite<br/>+ ProductComposition]
    D --> E[ðŸ§© Para cada componente<br/>em ProductConfiguration]
    E --> F[ðŸ“ Criar ProductComposition<br/>Status: Pending]
    
    B -->|Group| G[ðŸ­ Explodir em N Demands<br/>um por produto concreto]
    
    C --> H[ðŸ”„ Demand Status: Pending]
    F --> H
    G --> H
    
    classDef demandStyle fill:#fef3c7,stroke:#f59e0b,stroke-width:2px,color:black
    class A,B,C,D,G,H demandStyle
    
    classDef compositionStyle fill:#fed7aa,stroke:#f97316,stroke-width:2px,color:black
    class E,F compositionStyle
```

**Relacionamento com ProductComposition:**
```
Para ProductType.Composite:

Demand 1:N ProductComposition
â”œâ”€â”€ HierarchyName: "Massa"
â”‚   â””â”€â”€ ComponentName: "Massa de Chocolate"
â”œâ”€â”€ HierarchyName: "Recheio"  
â”‚   â”œâ”€â”€ ComponentName: "Recheio Brigadeiro"
â”‚   â””â”€â”€ ComponentName: "Recheio Morango"
â””â”€â”€ HierarchyName: "Cobertura"
    â””â”€â”€ ComponentName: "Cobertura Chantilly"

Todas ProductComposition iniciam com Status: Pending
```

### **ðŸŸ¢ CONFIRMED (Confirmada)**
```
ðŸ“Œ Estado de AprovaÃ§Ã£o
â”œâ”€â”€ Trigger: OrderEntry.OrderStatus = "Confirmed"
â”œâ”€â”€ DescriÃ§Ã£o: Demanda aprovada para produÃ§Ã£o
â”œâ”€â”€ Permitido: Agrupar em ProductionOrder
â”œâ”€â”€ Bloqueado: Ainda nÃ£o pode iniciar produÃ§Ã£o fÃ­sica
â””â”€â”€ PrÃ³ximo Estado: InProduction ou Cancelled

SincronizaÃ§Ã£o AutomÃ¡tica:
â”œâ”€â”€ ðŸ”„ Todas Demands do OrderEntry â†’ Confirmed
â”œâ”€â”€ ðŸ­ DisponÃ­veis para agrupamento em ProductionOrder
â””â”€â”€ ðŸ“Š Estimativas de custo e tempo calculadas
```

**AÃ§Ãµes no Estado Confirmed:**
- âœ… Aguardar agrupamento em ProductionOrder
- âœ… CÃ¡lculo de estimativas (tempo/custo)
- âœ… ValidaÃ§Ã£o de ingredientes disponÃ­veis
- âœ… Cancelar se OrderEntry for cancelada
- â›” Iniciar produÃ§Ã£o sem ProductionOrder

**Agrupamento em ProductionOrder:**
```mermaid
flowchart TD
    A[OrderEntry: SentToProduction] --> B[ðŸ” Buscar Demands Confirmed<br/>do mesmo pedido]
    B --> C[ðŸ­ Criar ProductionOrder]
    C --> D[ðŸ”— Vincular Demands Ã  PO]
    D --> E[ðŸ“ˆ Demand.ProductionOrderId = PO.Id]
    E --> F[ðŸ“Š PO Status: Scheduled]
    
    classDef productionOrderStyle fill:#fba81d,stroke:#fba81d,stroke-width:2px,color:black
    class A,B,C,D,E,F productionOrderStyle
```

### **ðŸ”´ IN_PRODUCTION (Em ProduÃ§Ã£o)**
```
ðŸ“Œ Estado de ProduÃ§Ã£o Ativa
â”œâ”€â”€ Trigger: ProductionOrder iniciada manualmente
â”œâ”€â”€ DescriÃ§Ã£o: ProduÃ§Ã£o fÃ­sica em andamento
â”œâ”€â”€ Permitido: Executar ProductComposition sequencialmente
â”œâ”€â”€ Bloqueado: Editar configuraÃ§Ãµes
â””â”€â”€ PrÃ³ximo Estado: Finalizando ou Cancelled

ExecuÃ§Ã£o por Tipo:
â”œâ”€â”€ ðŸ“¦ Simple: ProduÃ§Ã£o direta sem decomposiÃ§Ã£o
â”œâ”€â”€ ðŸ§© Composite: Executar ProductComposition em ordem
â””â”€â”€ ðŸ“Š Tracking em tempo real de progresso
```

**Fluxo de ExecuÃ§Ã£o por Tipo:**

#### **Simple Product:**
```mermaid
flowchart TD
    A[Demand Simple: InProduction] --> B[â° StartTime = now()]
    B --> C[ðŸ­ Executar produÃ§Ã£o direta]
    C --> D[ðŸ¥˜ Consumir ingredientes]
    D --> E[â° CompletionTime = now()]
    E --> F[ðŸ“ˆ Status: Finalizando]
    
    classDef simpleStyle fill:#a7f3d0,stroke:#00a86b,stroke-width:2px,color:black
    class A,B,C,D,E,F simpleStyle
```

#### **Composite Product:**
```mermaid
flowchart TD
    A[Demand Composite: InProduction] --> B[ðŸ“‹ Listar ProductComposition<br/>ordenadas por HierarchyName]
    B --> C[ðŸ”§ Para cada ProductComposition]
    
    C --> D[ðŸ“ˆ Status: Pending â†’ InProgress]
    D --> E[â° StartTime = now()]
    E --> F[ðŸ­ Executar tarefa especÃ­fica]
    F --> G[ðŸ¥˜ Consumir ingredientes]
    G --> H[â° CompletionTime = now()]
    H --> I[ðŸ“ˆ Status: InProgress â†’ Completed]
    
    I --> J{ðŸ”„ Mais ProductComposition?}
    J -->|Sim| C
    J -->|NÃ£o| K[âœ… Todas Completed]
    K --> L[ðŸ“ˆ Demand Status: Finalizando]
    
    classDef compositeStyle fill:#6ee7b7,stroke:#00a86b,stroke-width:2px,color:black
    class A,B,C,D,E,F,G,H,I,J,K,L compositeStyle
```

**Monitoramento em Tempo Real:**
```
Dashboard mostra para cada Demand InProduction:
â”œâ”€â”€ â° Tempo decorrido vs estimado
â”œâ”€â”€ ðŸ’° Custo acumulado vs estimado  
â”œâ”€â”€ ðŸ“Š % progresso (ProductComposition completed)
â”œâ”€â”€ ðŸ§‘â€ðŸ­ Operador(es) responsÃ¡vel(is)
â”œâ”€â”€ ðŸ¥˜ Ingredientes consumidos
â””â”€â”€ ðŸš¨ Alertas de atraso ou problemas
```

### **ðŸŸ  FINALIZANDO (Finalizando)**
```
ðŸ“Œ Estado de FinalizaÃ§Ã£o
â”œâ”€â”€ Trigger: ProduÃ§Ã£o fÃ­sica completa
â”œâ”€â”€ DescriÃ§Ã£o: Embalagem, controle qualidade final
â”œâ”€â”€ Permitido: Ãšltimos ajustes e validaÃ§Ãµes
â”œâ”€â”€ Bloqueado: Alterar produÃ§Ã£o principal
â””â”€â”€ PrÃ³ximo Estado: Ready

Atividades de FinalizaÃ§Ã£o:
â”œâ”€â”€ ðŸ“¦ Embalagem final do produto
â”œâ”€â”€ ðŸ” Controle de qualidade final
â”œâ”€â”€ ðŸ·ï¸ Etiquetagem e identificaÃ§Ã£o
â”œâ”€â”€ ðŸ“Š Registro de custos e tempos reais
â””â”€â”€ âœ… AprovaÃ§Ã£o final para entrega
```

**ValidaÃ§Ãµes de FinalizaÃ§Ã£o:**
- âœ… Todas ProductComposition com Status "Completed"
- âœ… Controle de qualidade aprovado
- âœ… Produto corretamente embalado
- âœ… Etiquetas e documentaÃ§Ã£o prontas
- âœ… Custos reais registrados

**Processo de FinalizaÃ§Ã£o:**
```mermaid
flowchart TD
    A[Demand: Finalizando] --> B[ðŸ“¦ Embalagem final]
    B --> C[ðŸ” Controle qualidade]
    C --> D{âœ… Qualidade aprovada?}
    
    D -->|NÃ£o| E[ðŸ”„ Voltar para correÃ§Ã£o<br/>Status: InProduction]
    D -->|Sim| F[ðŸ·ï¸ Etiquetagem]
    
    F --> G[ðŸ“Š Registrar custos/tempos reais]
    G --> H[âœ… AprovaÃ§Ã£o final]
    H --> I[ðŸ“ˆ Status: Ready]
    
    E --> A
    
    classDef finalizingStyle fill:#fed7aa,stroke:#f97316,stroke-width:2px,color:black
    class A,B,C,D,F,G,H,I finalizingStyle
    
    classDef errorStyle fill:#fecaca,stroke:#ef4444,stroke-width:2px,color:black
    class E errorStyle
```

### **ðŸ”µ READY (Pronta)**
```
ðŸ“Œ Estado de Produto Finalizado
â”œâ”€â”€ Trigger: FinalizaÃ§Ã£o aprovada automaticamente
â”œâ”€â”€ DescriÃ§Ã£o: Produto pronto para entrega
â”œâ”€â”€ Permitido: ExpediÃ§Ã£o e entrega
â”œâ”€â”€ Bloqueado: AlteraÃ§Ãµes de produÃ§Ã£o
â””â”€â”€ PrÃ³ximo Estado: Delivered

IntegraÃ§Ã£o com OrderEntry:
â”œâ”€â”€ ðŸ” Verificar se todas Demands do pedido estÃ£o Ready
â”œâ”€â”€ ðŸ“ˆ Se sim: OrderEntry Status â†’ ReadyForDelivery
â””â”€â”€ ðŸ“§ Notificar cliente sobre conclusÃ£o
```

**VerificaÃ§Ã£o de ConclusÃ£o do Pedido:**
```mermaid
flowchart TD
    A[Demand: Ready] --> B[ðŸ” Verificar outras Demands<br/>do mesmo OrderEntry]
    B --> C{ðŸŽ¯ Todas Demands<br/>estÃ£o Ready?}
    
    C -->|NÃ£o| D[â° Aguardar outras Demands]
    C -->|Sim| E[ðŸ“ˆ OrderEntry Status:<br/>ReadyForDelivery]
    
    E --> F[ðŸ“§ Notificar cliente<br/>sobre conclusÃ£o]
    F --> G[ðŸ“¦ Preparar para logÃ­stica]
    
    classDef readyStyle fill:#dbeafe,stroke:#3b82f6,stroke-width:2px,color:black
    class A,B,C,D,E,F,G readyStyle
```

### **ðŸŸ£ DELIVERED (Entregue)**
```
ðŸ“Œ Estado Final de Sucesso
â”œâ”€â”€ Trigger: OrderEntry marcada como Delivered
â”œâ”€â”€ DescriÃ§Ã£o: Produto entregue ao cliente
â”œâ”€â”€ Permitido: Consulta e anÃ¡lise
â”œâ”€â”€ Bloqueado: Qualquer alteraÃ§Ã£o
â””â”€â”€ PrÃ³ximo Estado: [Finalizado]

SincronizaÃ§Ã£o AutomÃ¡tica:
â”œâ”€â”€ ðŸ”„ Todas Demands do OrderEntry â†’ Delivered
â”œâ”€â”€ ðŸ“Š Atualizar mÃ©tricas de produÃ§Ã£o
â”œâ”€â”€ ðŸ’° Calcular lucratividade por Demand
â””â”€â”€ ðŸ“ˆ Atualizar histÃ³rico de performance
```

### **âŒ CANCELLED (Cancelada)**
```
ðŸ“Œ Estado Final de Cancelamento
â”œâ”€â”€ Trigger: OrderItem/OrderEntry cancelados ou problema produÃ§Ã£o
â”œâ”€â”€ DescriÃ§Ã£o: Demanda cancelada em qualquer fase
â”œâ”€â”€ Permitido: Consulta e auditoria
â”œâ”€â”€ Bloqueado: ReativaÃ§Ã£o
â””â”€â”€ PrÃ³ximo Estado: [Finalizado]

ReversÃµes NecessÃ¡rias:
â”œâ”€â”€ ðŸ¥˜ Reverter consumo de ingredientes (se aplicÃ¡vel)
â”œâ”€â”€ ðŸ­ Liberar recursos de produÃ§Ã£o
â”œâ”€â”€ ðŸ“Š Cancelar ProductComposition relacionadas
â””â”€â”€ ðŸ“ˆ Atualizar ProductionOrder (se agrupada)
```

## âš¡ SincronizaÃ§Ã£o entre Estados

### **ðŸ”„ SincronizaÃ§Ã£o com OrderEntry:**
```
OrderEntry Status Changed â†’ Demand Status Changes:

OrderEntry: Confirmed
â”œâ”€â”€ ðŸ”„ Todas Demands relacionadas: Pending â†’ Confirmed
â””â”€â”€ âš¡ AutomÃ¡tico e instantÃ¢neo

OrderEntry: SentToProduction  
â”œâ”€â”€ ðŸ­ Criar ProductionOrder
â”œâ”€â”€ ðŸ”— Agrupar Demands
â””â”€â”€ ðŸ“Š PO Status: Scheduled

OrderEntry: Cancelled
â”œâ”€â”€ ðŸ”„ Todas Demands relacionadas: [Any] â†’ Cancelled  
â”œâ”€â”€ ðŸ¥˜ Reverter consumos de ingrediente
â””â”€â”€ ðŸ­ Cancelar ProductionOrder se existir
```

### **ðŸ§© SincronizaÃ§Ã£o com ProductComposition:**
```
Demand: InProduction
â”œâ”€â”€ ðŸ“‹ Listar ProductComposition relacionadas
â”œâ”€â”€ ðŸ”„ Executar sequencialmente
â””â”€â”€ â° Tracking individual de cada tarefa

Todas ProductComposition: Completed
â”œâ”€â”€ ðŸ“ˆ Demand Status: InProduction â†’ Finalizando
â””â”€â”€ âš¡ VerificaÃ§Ã£o automÃ¡tica contÃ­nua
```

### **ðŸ­ SincronizaÃ§Ã£o com ProductionOrder:**
```
ProductionOrder: InProgress
â”œâ”€â”€ ðŸ”„ Todas Demands vinculadas podem iniciar
â””â”€â”€ ðŸ“Š Tracking consolidado de progresso

Todas Demands: Ready
â”œâ”€â”€ ðŸ“ˆ ProductionOrder Status: Completed
â””â”€â”€ ðŸ“Š Calcular mÃ©tricas finais da PO
```

## ðŸš¨ ValidaÃ§Ãµes e RestriÃ§Ãµes

### **RestriÃ§Ãµes por Estado:**
```
Estado          â”‚ Editar Config â”‚ Cancelar â”‚ Iniciar Prod â”‚ Entregar
â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•
Pending         â”‚      âš ï¸       â”‚    âœ…    â”‚      â›”      â”‚    â›”
Confirmed       â”‚      â›”       â”‚    âœ…    â”‚      âš ï¸      â”‚    â›”
InProduction    â”‚      â›”       â”‚    âš ï¸    â”‚      N/A     â”‚    â›”
Finalizando     â”‚      â›”       â”‚    âš ï¸    â”‚      N/A     â”‚    â›”
Ready           â”‚      â›”       â”‚    â›”    â”‚      N/A     â”‚    âœ…
Delivered       â”‚      â›”       â”‚    â›”    â”‚      N/A     â”‚    N/A

Legenda: âœ… Permitido | â›” Bloqueado | âš ï¸ Com restriÃ§Ãµes | N/A NÃ£o aplicÃ¡vel
```

### **CondiÃ§Ãµes para MudanÃ§a de Estado:**
```
Pending â†’ Confirmed:
â”œâ”€â”€ âœ… OrderEntry deve estar Confirmed
â”œâ”€â”€ âœ… Product deve estar ativo
â””â”€â”€ âœ… Ingredientes suficientes (warning se nÃ£o)

Confirmed â†’ InProduction:
â”œâ”€â”€ âœ… Deve estar vinculada a ProductionOrder
â”œâ”€â”€ âœ… ProductionOrder deve estar InProgress
â””â”€â”€ âœ… Recursos de produÃ§Ã£o disponÃ­veis

InProduction â†’ Finalizando:
â”œâ”€â”€ âœ… Todas ProductComposition devem estar Completed
â”œâ”€â”€ âœ… Tempos de produÃ§Ã£o registrados
â””â”€â”€ âœ… Ingredientes consumidos registrados

Finalizando â†’ Ready:
â”œâ”€â”€ âœ… Controle de qualidade aprovado
â”œâ”€â”€ âœ… Produto corretamente embalado
â””â”€â”€ âœ… Custos reais registrados
```

## ðŸŽ¯ Eventos de DomÃ­nio por TransiÃ§Ã£o

```
DemandStatusChanged:
â”œâ”€â”€ DemandId: ID da demanda
â”œâ”€â”€ From: Estado anterior
â”œâ”€â”€ To: Novo estado
â”œâ”€â”€ Timestamp: Data/hora da mudanÃ§a
â”œâ”€â”€ TriggeredBy: UsuÃ¡rio ou sistema
â”œâ”€â”€ Reason: Motivo da mudanÃ§a
â””â”€â”€ ProductionOrderId: PO relacionada (se aplicÃ¡vel)

Eventos EspecÃ­ficos:
â”œâ”€â”€ DemandCreated: Nova demanda gerada
â”œâ”€â”€ DemandConfirmed: Demanda confirmada para produÃ§Ã£o
â”œâ”€â”€ ProductionStarted: InÃ­cio da produÃ§Ã£o fÃ­sica
â”œâ”€â”€ ComponentCompleted: ProductComposition finalizada
â”œâ”€â”€ DemandFinalized: Demanda pronta para entrega
â”œâ”€â”€ DemandDelivered: Demanda entregue
â””â”€â”€ DemandCancelled: Demanda cancelada
```

---

**Arquivo**: `demand-lifecycle.md`  
**DomÃ­nio**: ProduÃ§Ã£o (#fba81d)  
**Tipo**: State Diagram  
**Foco**: Ciclo Completo Demand + SincronizaÃ§Ã£o com ProductComposition
