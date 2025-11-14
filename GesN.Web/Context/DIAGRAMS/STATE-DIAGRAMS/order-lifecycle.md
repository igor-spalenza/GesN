# ðŸ“‹ DIAGRAMA DE ESTADOS - CICLO DE VIDA DO PEDIDO

## ðŸŽ¯ VisÃ£o Geral
Diagrama de estados completo mostrando o ciclo de vida de uma OrderEntry (Pedido de Venda), incluindo todas as transiÃ§Ãµes possÃ­veis, condiÃ§Ãµes para mudanÃ§a de estado, e impactos automÃ¡ticos em outros domÃ­nios (ProduÃ§Ã£o e Financeiro).

## ðŸ”„ Diagrama Principal de Estados

```mermaid
stateDiagram-v2
    [*] --> Pending : ðŸ†• CriaÃ§Ã£o inicial<br/>Modal rÃ¡pido + aba dinÃ¢mica
    
    %% === ESTADOS PRINCIPAIS ===
    Pending --> Confirmed : âœ… UsuÃ¡rio confirma pedido<br/>ValidaÃ§Ãµes: Customer + â‰¥1 OrderItem
    Confirmed --> SentToProduction : ðŸ­ Sistema envia p/ produÃ§Ã£o<br/>Auto: Gera ProductionOrder
    SentToProduction --> InProduction : âš™ï¸ ProduÃ§Ã£o inicia<br/>Manual: Supervisor confirma
    InProduction --> ReadyForDelivery : ðŸ“¦ ProduÃ§Ã£o concluÃ­da<br/>Auto: Todas Demands "Ready"
    ReadyForDelivery --> Delivered : ðŸšš Produto entregue<br/>Manual: ConfirmaÃ§Ã£o entrega
    Delivered --> Invoiced : ðŸ’° Pagamento recebido<br/>Auto: AccountReceivable "Paid"
    
    %% === CANCELAMENTOS ===
    Pending --> Cancelled : âŒ Cancelar antes confirmaÃ§Ã£o<br/>Manual: UsuÃ¡rio cancela
    Confirmed --> Cancelled : âŒ Cancelar apÃ³s confirmaÃ§Ã£o<br/>Manual: Cancela + reverte AR
    SentToProduction --> Cancelled : âŒ Cancelar durante produÃ§Ã£o<br/>Manual: Cancela + reverte produÃ§Ã£o
    
    %% === ESTADOS FINAIS ===
    Invoiced --> [*] : ðŸŽ‰ Processo completamente finalizado
    Cancelled --> [*] : ðŸš« Processo cancelado
    
    %% === STYLING POR FASE ===
    
    %% CRIAÃ‡ÃƒO E CONFIRMAÃ‡ÃƒO
    classDef creation fill:#fef3c7,stroke:#f59e0b,stroke-width:3px,color:black
    class Pending creation
    
    classDef confirmed fill:#d1fae5,stroke:#10b981,stroke-width:3px,color:black
    class Confirmed confirmed
    
    %% PRODUÃ‡ÃƒO
    classDef production fill:#fed7aa,stroke:#f97316,stroke-width:3px,color:black
    class SentToProduction,InProduction production
    
    %% ENTREGA E FINALIZAÃ‡ÃƒO
    classDef delivery fill:#dbeafe,stroke:#3b82f6,stroke-width:3px,color:black
    class ReadyForDelivery,Delivered delivery
    
    classDef finalized fill:#e0e7ff,stroke:#6366f1,stroke-width:3px,color:black
    class Invoiced finalized
    
    %% CANCELAMENTO
    classDef cancelled fill:#fecaca,stroke:#ef4444,stroke-width:3px,color:black
    class Cancelled cancelled
```

## ðŸ“‹ Detalhamento dos Estados

### **ðŸŸ¡ PENDING (Pendente)**
```
ðŸ“Œ Estado Inicial
â”œâ”€â”€ DescriÃ§Ã£o: OrderEntry criada mas nÃ£o confirmada
â”œâ”€â”€ Permitido: EdiÃ§Ã£o livre de itens e dados
â”œâ”€â”€ Bloqueado: NÃ£o gera produÃ§Ã£o nem financeiro
â””â”€â”€ PrÃ³ximo Estado: Confirmed ou Cancelled
```

**AÃ§Ãµes DisponÃ­veis:**
- âœ… Adicionar/remover OrderItem
- âœ… Editar quantidades e configuraÃ§Ãµes
- âœ… Alterar Customer
- âœ… Modificar datas e endereÃ§o
- âœ… Cancelar pedido
- âœ… Confirmar pedido

**ValidaÃ§Ãµes para ConfirmaÃ§Ã£o:**
- âœ… Customer deve estar selecionado
- âœ… Pelo menos 1 OrderItem ativo
- âœ… DeliveryDate â‰¥ OrderDate
- âœ… Todos OrderItem com Product ativo
- âœ… ConfiguraÃ§Ãµes vÃ¡lidas (para Composite/Group)

### **ðŸŸ¢ CONFIRMED (Confirmado)**
```
ðŸ“Œ Estado de AprovaÃ§Ã£o
â”œâ”€â”€ DescriÃ§Ã£o: Pedido confirmado pelo cliente
â”œâ”€â”€ Permitido: VisualizaÃ§Ã£o e envio para produÃ§Ã£o
â”œâ”€â”€ Bloqueado: EdiÃ§Ã£o de itens e dados crÃ­ticos
â””â”€â”€ PrÃ³ximo Estado: SentToProduction ou Cancelled

IntegraÃ§Ãµes AutomÃ¡ticas:
â”œâ”€â”€ ðŸ’° Gerar AccountReceivable no Financeiro
â”œâ”€â”€ ðŸ­ Demands ficam disponÃ­veis para produÃ§Ã£o  
â””â”€â”€ ðŸ“§ Notificar produÃ§Ã£o sobre novo pedido
```

**AÃ§Ãµes DisponÃ­veis:**
- âœ… Enviar para produÃ§Ã£o
- âœ… Cancelar (com reversÃ£o)
- âœ… Visualizar detalhes
- â›” Editar itens
- â›” Alterar Customer

**Impactos da ConfirmaÃ§Ã£o:**
```mermaid
flowchart LR
    A[OrderEntry: Confirmed] --> B[ðŸ’° AccountReceivable<br/>Status: Pending]
    A --> C[ðŸ­ Todas Demands<br/>Status: Confirmed]
    A --> D[ðŸ“§ NotificaÃ§Ã£o<br/>para ProduÃ§Ã£o]
    
    classDef orderStyle fill:#d1fae5,stroke:#10b981,stroke-width:2px,color:black
    class A orderStyle
    
    classDef financeStyle fill:#083e61,stroke:#083e61,stroke-width:2px,color:white
    class B financeStyle
    
    classDef productionStyle fill:#fba81d,stroke:#fba81d,stroke-width:2px,color:black
    class C,D productionStyle
```

### **ðŸŸ  SENT_TO_PRODUCTION (Enviado para ProduÃ§Ã£o)**
```
ðŸ“Œ Estado de ProduÃ§Ã£o Agendada
â”œâ”€â”€ DescriÃ§Ã£o: Pedido enviado para fila de produÃ§Ã£o
â”œâ”€â”€ Permitido: Acompanhar status de produÃ§Ã£o
â”œâ”€â”€ Bloqueado: EdiÃ§Ãµes e cancelamento simples
â””â”€â”€ PrÃ³ximo Estado: InProduction ou Cancelled

IntegraÃ§Ãµes AutomÃ¡ticas:
â”œâ”€â”€ ðŸ­ Criar ProductionOrder agrupando Demands
â”œâ”€â”€ ðŸ“Š Reservar ingredientes no estoque
â””â”€â”€ â° Agendar produÃ§Ã£o baseada em RequiredDate
```

**AÃ§Ãµes DisponÃ­veis:**
- âœ… Acompanhar progresso produÃ§Ã£o
- âœ… Cancelar (com impacto na produÃ§Ã£o)
- â›” Editar qualquer dado
- â›” Adicionar/remover itens

**CriaÃ§Ã£o de ProductionOrder:**
```mermaid
flowchart TD
    A[OrderEntry: SentToProduction] --> B[ðŸ” Localizar todas Demands<br/>relacionadas]
    B --> C[ðŸ­ Criar ProductionOrder]
    C --> D[ðŸ”— Vincular Demands Ã  PO]
    D --> E[ðŸ“ˆ Demands: Confirmed]
    E --> F[ðŸ“Š ProductionOrder: Scheduled]
    
    classDef productionStyle fill:#fba81d,stroke:#fba81d,stroke-width:2px,color:black
    class A,B,C,D,E,F productionStyle
```

### **ðŸ”´ IN_PRODUCTION (Em ProduÃ§Ã£o)**
```
ðŸ“Œ Estado de ProduÃ§Ã£o Ativa
â”œâ”€â”€ DescriÃ§Ã£o: ProduÃ§Ã£o efetivamente iniciada
â”œâ”€â”€ Permitido: Acompanhar progresso em tempo real
â”œâ”€â”€ Bloqueado: Cancelamento sÃ³ com supervisor
â””â”€â”€ PrÃ³ximo Estado: ReadyForDelivery ou Cancelled

IntegraÃ§Ãµes AutomÃ¡ticas:
â”œâ”€â”€ âš™ï¸ ProductionOrder: InProgress
â”œâ”€â”€ ðŸ§© ProductComposition: InProgress â†’ Completed
â””â”€â”€ ðŸ¥˜ Consumo automÃ¡tico de ingredientes
```

**AÃ§Ãµes DisponÃ­veis:**
- âœ… Monitorar ProductComposition
- âœ… Ver tempo estimado vs real
- âœ… Cancelar (com aprovaÃ§Ã£o supervisor)
- â›” Qualquer ediÃ§Ã£o

**Monitoramento de Progresso:**
```mermaid
flowchart LR
    A[OrderEntry: InProduction] --> B[ðŸ“Š Dashboard Tempo Real]
    B --> C[â° Tempo Estimado vs Real]
    B --> D[ðŸ§© ProductComposition Status]
    B --> E[ðŸ’° Custo Estimado vs Real]
    B --> F[ðŸ“ˆ % Progresso Geral]
    
    classDef monitorStyle fill:#fed7aa,stroke:#f97316,stroke-width:2px,color:black
    class A,B,C,D,E,F monitorStyle
```

### **ðŸ”µ READY_FOR_DELIVERY (Pronto para Entrega)**
```
ðŸ“Œ Estado de Produto Finalizado
â”œâ”€â”€ DescriÃ§Ã£o: ProduÃ§Ã£o concluÃ­da, aguardando entrega
â”œâ”€â”€ Permitido: Agendar/confirmar entrega
â”œâ”€â”€ Bloqueado: AlteraÃ§Ãµes de produÃ§Ã£o
â””â”€â”€ PrÃ³ximo Estado: Delivered

IntegraÃ§Ãµes AutomÃ¡ticas:
â”œâ”€â”€ ðŸ­ ProductionOrder: Completed
â”œâ”€â”€ ðŸ“¦ Todas Demands: Ready
â””â”€â”€ ðŸ“§ Notificar cliente sobre conclusÃ£o
```

**AÃ§Ãµes DisponÃ­veis:**
- âœ… Agendar entrega
- âœ… Confirmar entrega
- âœ… Gerar etiquetas/documentos
- â›” Alterar produÃ§Ã£o

**PreparaÃ§Ã£o para Entrega:**
```mermaid
flowchart TD
    A[Todas Demands: Ready] --> B[OrderEntry: ReadyForDelivery]
    B --> C[ðŸ“§ Notificar Cliente]
    B --> D[ðŸ“… Agendar Entrega]
    B --> E[ðŸ“‹ Preparar Documentos]
    B --> F[ðŸ“¦ Separar para LogÃ­stica]
    
    classDef readyStyle fill:#dbeafe,stroke:#3b82f6,stroke-width:2px,color:black
    class A,B,C,D,E,F readyStyle
```

### **ðŸŸ£ DELIVERED (Entregue)**
```
ðŸ“Œ Estado de Entrega Confirmada
â”œâ”€â”€ DescriÃ§Ã£o: Produto entregue ao cliente
â”œâ”€â”€ Permitido: Processar pagamento
â”œâ”€â”€ Bloqueado: AlteraÃ§Ãµes de produto
â””â”€â”€ PrÃ³ximo Estado: Invoiced

IntegraÃ§Ãµes AutomÃ¡ticas:
â”œâ”€â”€ ðŸ“… Registrar data/hora entrega real
â”œâ”€â”€ ðŸ’° Liberar AccountReceivable para cobranÃ§a
â””â”€â”€ ðŸ“Š Atualizar mÃ©tricas de entrega
```

**AÃ§Ãµes DisponÃ­veis:**
- âœ… Processar pagamento
- âœ… Gerar comprovante entrega
- âœ… Avaliar satisfaÃ§Ã£o cliente
- â›” Alterar produto/produÃ§Ã£o

**ConfirmaÃ§Ã£o de Entrega:**
```mermaid
flowchart TD
    A[ðŸ‘¤ Confirmar Entrega] --> B[ðŸ“… Registrar Data/Hora]
    B --> C[âœï¸ Coletar Assinatura<br/>ou ConfirmaÃ§Ã£o]
    C --> D[OrderEntry: Delivered]
    D --> E[ðŸ’° AccountReceivable<br/>disponÃ­vel para cobranÃ§a]
    
    classDef deliveredStyle fill:#dbeafe,stroke:#3b82f6,stroke-width:2px,color:black
    class A,B,C,D,E deliveredStyle
```

### **ðŸŸ¦ INVOICED (Faturado)**
```
ðŸ“Œ Estado Final - Pago
â”œâ”€â”€ DescriÃ§Ã£o: Pagamento totalmente recebido
â”œâ”€â”€ Permitido: Consulta e anÃ¡lise
â”œâ”€â”€ Bloqueado: Qualquer alteraÃ§Ã£o
â””â”€â”€ PrÃ³ximo Estado: [Finalizado]

IntegraÃ§Ãµes AutomÃ¡ticas:
â”œâ”€â”€ ðŸ’° AccountReceivable: Paid
â”œâ”€â”€ ðŸ“Š Calcular lucratividade final
â””â”€â”€ ðŸ“ˆ Atualizar mÃ©tricas de negÃ³cio
```

**AÃ§Ãµes DisponÃ­veis:**
- âœ… Consultar dados histÃ³ricos
- âœ… Analisar lucratividade
- âœ… Gerar relatÃ³rios
- â›” Qualquer alteraÃ§Ã£o

### **âŒ CANCELLED (Cancelado)**
```
ðŸ“Œ Estado Final - Cancelado
â”œâ”€â”€ DescriÃ§Ã£o: Pedido cancelado em qualquer fase
â”œâ”€â”€ Permitido: Consulta e auditoria
â”œâ”€â”€ Bloqueado: ReativaÃ§Ã£o
â””â”€â”€ PrÃ³ximo Estado: [Finalizado]

IntegraÃ§Ãµes de ReversÃ£o:
â”œâ”€â”€ ðŸ’° Cancelar AccountReceivable pendentes
â”œâ”€â”€ ðŸ­ Cancelar Demands e ProductionOrder
â””â”€â”€ ðŸ“Š Registrar motivo do cancelamento
```

## âš¡ TransiÃ§Ãµes AutomÃ¡ticas vs Manuais

### **ðŸ¤– TransiÃ§Ãµes AutomÃ¡ticas:**
```
Confirmed â†’ SentToProduction
â”œâ”€â”€ Trigger: Sistema agenda produÃ§Ã£o
â”œâ”€â”€ CondiÃ§Ã£o: Todas validaÃ§Ãµes OK
â””â”€â”€ Tempo: Imediato apÃ³s confirmaÃ§Ã£o

InProduction â†’ ReadyForDelivery  
â”œâ”€â”€ Trigger: Todas Demands = "Ready"
â”œâ”€â”€ CondiÃ§Ã£o: ProductionOrder completed
â””â”€â”€ Tempo: AutomÃ¡tico quando Ãºltima tarefa completa

Delivered â†’ Invoiced
â”œâ”€â”€ Trigger: AccountReceivable = "Paid"  
â”œâ”€â”€ CondiÃ§Ã£o: Pagamento total recebido
â””â”€â”€ Tempo: Imediato apÃ³s pagamento
```

### **ðŸ‘¤ TransiÃ§Ãµes Manuais:**
```
Pending â†’ Confirmed
â”œâ”€â”€ Trigger: UsuÃ¡rio clica "Confirmar"
â”œâ”€â”€ Interface: BotÃ£o de confirmaÃ§Ã£o
â””â”€â”€ ValidaÃ§Ã£o: Dados obrigatÃ³rios preenchidos

SentToProduction â†’ InProduction
â”œâ”€â”€ Trigger: Supervisor inicia produÃ§Ã£o
â”œâ”€â”€ Interface: Dashboard de produÃ§Ã£o
â””â”€â”€ ValidaÃ§Ã£o: Ingredientes disponÃ­veis

ReadyForDelivery â†’ Delivered
â”œâ”€â”€ Trigger: ConfirmaÃ§Ã£o de entrega
â”œâ”€â”€ Interface: App mÃ³vel ou web
â””â”€â”€ ValidaÃ§Ã£o: Assinatura ou confirmaÃ§Ã£o
```

## ðŸš¨ ValidaÃ§Ãµes e RestriÃ§Ãµes por Estado

### **RestriÃ§Ãµes de EdiÃ§Ã£o:**
```
Estado               â”‚ Customer â”‚ OrderItem â”‚ Datas â”‚ EndereÃ§o â”‚ Cancelar
â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•
Pending              â”‚    âœ…    â”‚     âœ…    â”‚   âœ…   â”‚    âœ…    â”‚    âœ…
Confirmed            â”‚    â›”    â”‚     â›”    â”‚   âš ï¸    â”‚    âœ…    â”‚    âœ…
SentToProduction     â”‚    â›”    â”‚     â›”    â”‚   â›”    â”‚    âš ï¸    â”‚    âš ï¸
InProduction         â”‚    â›”    â”‚     â›”    â”‚   â›”    â”‚    â›”    â”‚    âš ï¸
ReadyForDelivery     â”‚    â›”    â”‚     â›”    â”‚   â›”    â”‚    âœ…    â”‚    â›”
Delivered            â”‚    â›”    â”‚     â›”    â”‚   â›”    â”‚    â›”    â”‚    â›”
Invoiced             â”‚    â›”    â”‚     â›”    â”‚   â›”    â”‚    â›”    â”‚    â›”

Legenda: âœ… Permitido | â›” Bloqueado | âš ï¸ Com restriÃ§Ãµes
```

### **Impactos de Cancelamento por Estado:**
```
Pending â†’ Cancelled:
â”œâ”€â”€ âš¡ AÃ§Ã£o: ExclusÃ£o simples
â”œâ”€â”€ ðŸ”„ ReversÃ£o: Nenhuma necessÃ¡ria
â””â”€â”€ ðŸ“Š Impacto: Apenas OrderEntry

Confirmed â†’ Cancelled:
â”œâ”€â”€ âš¡ AÃ§Ã£o: Cancelamento com reversÃ£o
â”œâ”€â”€ ðŸ”„ ReversÃ£o: Cancelar AccountReceivable
â””â”€â”€ ðŸ“Š Impacto: Vendas + Financeiro

SentToProduction â†’ Cancelled:
â”œâ”€â”€ âš¡ AÃ§Ã£o: Cancelamento complexo
â”œâ”€â”€ ðŸ”„ ReversÃ£o: Cancelar produÃ§Ã£o + AR
â””â”€â”€ ðŸ“Š Impacto: Vendas + ProduÃ§Ã£o + Financeiro
```

## ðŸŽ¯ Eventos de DomÃ­nio por TransiÃ§Ã£o

```
OrderStatusChanged:
â”œâ”€â”€ From: EstadoAnterior
â”œâ”€â”€ To: NovoEstado  
â”œâ”€â”€ Timestamp: DataHora da mudanÃ§a
â”œâ”€â”€ UserId: UsuÃ¡rio responsÃ¡vel
â”œâ”€â”€ Reason: Motivo da mudanÃ§a
â””â”€â”€ AdditionalData: Dados especÃ­ficos

OrderConfirmed â†’ Gera:
â”œâ”€â”€ AccountReceivableCreated
â”œâ”€â”€ DemandStatusChanged (mÃºltiplos)
â””â”€â”€ ProductionNotificationSent

OrderSentToProduction â†’ Gera:
â”œâ”€â”€ ProductionOrderCreated
â”œâ”€â”€ IngredientReserved (mÃºltiplos)
â””â”€â”€ ProductionScheduled

OrderCompleted â†’ Gera:
â”œâ”€â”€ CustomerNotificationSent
â”œâ”€â”€ DeliveryScheduled
â””â”€â”€ ProfitabilityCalculated
```

---

**Arquivo**: `order-lifecycle.md`  
**DomÃ­nio**: Vendas (#f36b21)  
**Tipo**: State Diagram  
**Foco**: Ciclo Completo OrderEntry + IntegraÃ§Ãµes AutomÃ¡ticas
