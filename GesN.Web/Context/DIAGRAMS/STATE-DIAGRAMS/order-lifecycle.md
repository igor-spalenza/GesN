# 📋 DIAGRAMA DE ESTADOS - CICLO DE VIDA DO PEDIDO

## 🎯 Visão Geral
Diagrama de estados completo mostrando o ciclo de vida de uma OrderEntry (Pedido de Venda), incluindo todas as transições possíveis, condições para mudança de estado, e impactos automáticos em outros domínios (Produção e Financeiro).

## 🔄 Diagrama Principal de Estados

```mermaid
stateDiagram-v2
    [*] --> Pending : 🆕 Criação inicial<br/>Modal rápido + aba dinâmica
    
    %% === ESTADOS PRINCIPAIS ===
    Pending --> Confirmed : ✅ Usuário confirma pedido<br/>Validações: Customer + ≥1 OrderItem
    Confirmed --> SentToProduction : 🏭 Sistema envia p/ produção<br/>Auto: Gera ProductionOrder
    SentToProduction --> InProduction : ⚙️ Produção inicia<br/>Manual: Supervisor confirma
    InProduction --> ReadyForDelivery : 📦 Produção concluída<br/>Auto: Todas Demands "Ready"
    ReadyForDelivery --> Delivered : 🚚 Produto entregue<br/>Manual: Confirmação entrega
    Delivered --> Invoiced : 💰 Pagamento recebido<br/>Auto: AccountReceivable "Paid"
    
    %% === CANCELAMENTOS ===
    Pending --> Cancelled : ❌ Cancelar antes confirmação<br/>Manual: Usuário cancela
    Confirmed --> Cancelled : ❌ Cancelar após confirmação<br/>Manual: Cancela + reverte AR
    SentToProduction --> Cancelled : ❌ Cancelar durante produção<br/>Manual: Cancela + reverte produção
    
    %% === ESTADOS FINAIS ===
    Invoiced --> [*] : 🎉 Processo completamente finalizado
    Cancelled --> [*] : 🚫 Processo cancelado
    
    %% === STYLING POR FASE ===
    
    %% CRIAÇÃO E CONFIRMAÇÃO
    classDef creation fill:#fef3c7,stroke:#f59e0b,stroke-width:3px,color:black
    class Pending creation
    
    classDef confirmed fill:#d1fae5,stroke:#10b981,stroke-width:3px,color:black
    class Confirmed confirmed
    
    %% PRODUÇÃO
    classDef production fill:#fed7aa,stroke:#f97316,stroke-width:3px,color:black
    class SentToProduction,InProduction production
    
    %% ENTREGA E FINALIZAÇÃO
    classDef delivery fill:#dbeafe,stroke:#3b82f6,stroke-width:3px,color:black
    class ReadyForDelivery,Delivered delivery
    
    classDef finalized fill:#e0e7ff,stroke:#6366f1,stroke-width:3px,color:black
    class Invoiced finalized
    
    %% CANCELAMENTO
    classDef cancelled fill:#fecaca,stroke:#ef4444,stroke-width:3px,color:black
    class Cancelled cancelled
```

## 📋 Detalhamento dos Estados

### **🟡 PENDING (Pendente)**
```
📌 Estado Inicial
├── Descrição: OrderEntry criada mas não confirmada
├── Permitido: Edição livre de itens e dados
├── Bloqueado: Não gera produção nem financeiro
└── Próximo Estado: Confirmed ou Cancelled
```

**Ações Disponíveis:**
- ✅ Adicionar/remover OrderItem
- ✅ Editar quantidades e configurações
- ✅ Alterar Customer
- ✅ Modificar datas e endereço
- ✅ Cancelar pedido
- ✅ Confirmar pedido

**Validações para Confirmação:**
- ✅ Customer deve estar selecionado
- ✅ Pelo menos 1 OrderItem ativo
- ✅ DeliveryDate ≥ OrderDate
- ✅ Todos OrderItem com Product ativo
- ✅ Configurações válidas (para Composite/Group)

### **🟢 CONFIRMED (Confirmado)**
```
📌 Estado de Aprovação
├── Descrição: Pedido confirmado pelo cliente
├── Permitido: Visualização e envio para produção
├── Bloqueado: Edição de itens e dados críticos
└── Próximo Estado: SentToProduction ou Cancelled

Integrações Automáticas:
├── 💰 Gerar AccountReceivable no Financeiro
├── 🏭 Demands ficam disponíveis para produção  
└── 📧 Notificar produção sobre novo pedido
```

**Ações Disponíveis:**
- ✅ Enviar para produção
- ✅ Cancelar (com reversão)
- ✅ Visualizar detalhes
- ⛔ Editar itens
- ⛔ Alterar Customer

**Impactos da Confirmação:**
```mermaid
flowchart LR
    A[OrderEntry: Confirmed] --> B[💰 AccountReceivable<br/>Status: Pending]
    A --> C[🏭 Todas Demands<br/>Status: Confirmed]
    A --> D[📧 Notificação<br/>para Produção]
    
    classDef orderStyle fill:#d1fae5,stroke:#10b981,stroke-width:2px,color:black
    class A orderStyle
    
    classDef financeStyle fill:#083e61,stroke:#083e61,stroke-width:2px,color:white
    class B financeStyle
    
    classDef productionStyle fill:#fba81d,stroke:#fba81d,stroke-width:2px,color:black
    class C,D productionStyle
```

### **🟠 SENT_TO_PRODUCTION (Enviado para Produção)**
```
📌 Estado de Produção Agendada
├── Descrição: Pedido enviado para fila de produção
├── Permitido: Acompanhar status de produção
├── Bloqueado: Edições e cancelamento simples
└── Próximo Estado: InProduction ou Cancelled

Integrações Automáticas:
├── 🏭 Criar ProductionOrder agrupando Demands
├── 📊 Reservar ingredientes no estoque
└── ⏰ Agendar produção baseada em RequiredDate
```

**Ações Disponíveis:**
- ✅ Acompanhar progresso produção
- ✅ Cancelar (com impacto na produção)
- ⛔ Editar qualquer dado
- ⛔ Adicionar/remover itens

**Criação de ProductionOrder:**
```mermaid
flowchart TD
    A[OrderEntry: SentToProduction] --> B[🔍 Localizar todas Demands<br/>relacionadas]
    B --> C[🏭 Criar ProductionOrder]
    C --> D[🔗 Vincular Demands à PO]
    D --> E[📈 Demands: Confirmed]
    E --> F[📊 ProductionOrder: Scheduled]
    
    classDef productionStyle fill:#fba81d,stroke:#fba81d,stroke-width:2px,color:black
    class A,B,C,D,E,F productionStyle
```

### **🔴 IN_PRODUCTION (Em Produção)**
```
📌 Estado de Produção Ativa
├── Descrição: Produção efetivamente iniciada
├── Permitido: Acompanhar progresso em tempo real
├── Bloqueado: Cancelamento só com supervisor
└── Próximo Estado: ReadyForDelivery ou Cancelled

Integrações Automáticas:
├── ⚙️ ProductionOrder: InProgress
├── 🧩 ProductComposition: InProgress → Completed
└── 🥘 Consumo automático de ingredientes
```

**Ações Disponíveis:**
- ✅ Monitorar ProductComposition
- ✅ Ver tempo estimado vs real
- ✅ Cancelar (com aprovação supervisor)
- ⛔ Qualquer edição

**Monitoramento de Progresso:**
```mermaid
flowchart LR
    A[OrderEntry: InProduction] --> B[📊 Dashboard Tempo Real]
    B --> C[⏰ Tempo Estimado vs Real]
    B --> D[🧩 ProductComposition Status]
    B --> E[💰 Custo Estimado vs Real]
    B --> F[📈 % Progresso Geral]
    
    classDef monitorStyle fill:#fed7aa,stroke:#f97316,stroke-width:2px,color:black
    class A,B,C,D,E,F monitorStyle
```

### **🔵 READY_FOR_DELIVERY (Pronto para Entrega)**
```
📌 Estado de Produto Finalizado
├── Descrição: Produção concluída, aguardando entrega
├── Permitido: Agendar/confirmar entrega
├── Bloqueado: Alterações de produção
└── Próximo Estado: Delivered

Integrações Automáticas:
├── 🏭 ProductionOrder: Completed
├── 📦 Todas Demands: Ready
└── 📧 Notificar cliente sobre conclusão
```

**Ações Disponíveis:**
- ✅ Agendar entrega
- ✅ Confirmar entrega
- ✅ Gerar etiquetas/documentos
- ⛔ Alterar produção

**Preparação para Entrega:**
```mermaid
flowchart TD
    A[Todas Demands: Ready] --> B[OrderEntry: ReadyForDelivery]
    B --> C[📧 Notificar Cliente]
    B --> D[📅 Agendar Entrega]
    B --> E[📋 Preparar Documentos]
    B --> F[📦 Separar para Logística]
    
    classDef readyStyle fill:#dbeafe,stroke:#3b82f6,stroke-width:2px,color:black
    class A,B,C,D,E,F readyStyle
```

### **🟣 DELIVERED (Entregue)**
```
📌 Estado de Entrega Confirmada
├── Descrição: Produto entregue ao cliente
├── Permitido: Processar pagamento
├── Bloqueado: Alterações de produto
└── Próximo Estado: Invoiced

Integrações Automáticas:
├── 📅 Registrar data/hora entrega real
├── 💰 Liberar AccountReceivable para cobrança
└── 📊 Atualizar métricas de entrega
```

**Ações Disponíveis:**
- ✅ Processar pagamento
- ✅ Gerar comprovante entrega
- ✅ Avaliar satisfação cliente
- ⛔ Alterar produto/produção

**Confirmação de Entrega:**
```mermaid
flowchart TD
    A[👤 Confirmar Entrega] --> B[📅 Registrar Data/Hora]
    B --> C[✍️ Coletar Assinatura<br/>ou Confirmação]
    C --> D[OrderEntry: Delivered]
    D --> E[💰 AccountReceivable<br/>disponível para cobrança]
    
    classDef deliveredStyle fill:#dbeafe,stroke:#3b82f6,stroke-width:2px,color:black
    class A,B,C,D,E deliveredStyle
```

### **🟦 INVOICED (Faturado)**
```
📌 Estado Final - Pago
├── Descrição: Pagamento totalmente recebido
├── Permitido: Consulta e análise
├── Bloqueado: Qualquer alteração
└── Próximo Estado: [Finalizado]

Integrações Automáticas:
├── 💰 AccountReceivable: Paid
├── 📊 Calcular lucratividade final
└── 📈 Atualizar métricas de negócio
```

**Ações Disponíveis:**
- ✅ Consultar dados históricos
- ✅ Analisar lucratividade
- ✅ Gerar relatórios
- ⛔ Qualquer alteração

### **❌ CANCELLED (Cancelado)**
```
📌 Estado Final - Cancelado
├── Descrição: Pedido cancelado em qualquer fase
├── Permitido: Consulta e auditoria
├── Bloqueado: Reativação
└── Próximo Estado: [Finalizado]

Integrações de Reversão:
├── 💰 Cancelar AccountReceivable pendentes
├── 🏭 Cancelar Demands e ProductionOrder
└── 📊 Registrar motivo do cancelamento
```

## ⚡ Transições Automáticas vs Manuais

### **🤖 Transições Automáticas:**
```
Confirmed → SentToProduction
├── Trigger: Sistema agenda produção
├── Condição: Todas validações OK
└── Tempo: Imediato após confirmação

InProduction → ReadyForDelivery  
├── Trigger: Todas Demands = "Ready"
├── Condição: ProductionOrder completed
└── Tempo: Automático quando última tarefa completa

Delivered → Invoiced
├── Trigger: AccountReceivable = "Paid"  
├── Condição: Pagamento total recebido
└── Tempo: Imediato após pagamento
```

### **👤 Transições Manuais:**
```
Pending → Confirmed
├── Trigger: Usuário clica "Confirmar"
├── Interface: Botão de confirmação
└── Validação: Dados obrigatórios preenchidos

SentToProduction → InProduction
├── Trigger: Supervisor inicia produção
├── Interface: Dashboard de produção
└── Validação: Ingredientes disponíveis

ReadyForDelivery → Delivered
├── Trigger: Confirmação de entrega
├── Interface: App móvel ou web
└── Validação: Assinatura ou confirmação
```

## 🚨 Validações e Restrições por Estado

### **Restrições de Edição:**
```
Estado               │ Customer │ OrderItem │ Datas │ Endereço │ Cancelar
═══════════════════════════════════════════════════════════════════════════
Pending              │    ✅    │     ✅    │   ✅   │    ✅    │    ✅
Confirmed            │    ⛔    │     ⛔    │   ⚠️    │    ✅    │    ✅
SentToProduction     │    ⛔    │     ⛔    │   ⛔    │    ⚠️    │    ⚠️
InProduction         │    ⛔    │     ⛔    │   ⛔    │    ⛔    │    ⚠️
ReadyForDelivery     │    ⛔    │     ⛔    │   ⛔    │    ✅    │    ⛔
Delivered            │    ⛔    │     ⛔    │   ⛔    │    ⛔    │    ⛔
Invoiced             │    ⛔    │     ⛔    │   ⛔    │    ⛔    │    ⛔

Legenda: ✅ Permitido | ⛔ Bloqueado | ⚠️ Com restrições
```

### **Impactos de Cancelamento por Estado:**
```
Pending → Cancelled:
├── ⚡ Ação: Exclusão simples
├── 🔄 Reversão: Nenhuma necessária
└── 📊 Impacto: Apenas OrderEntry

Confirmed → Cancelled:
├── ⚡ Ação: Cancelamento com reversão
├── 🔄 Reversão: Cancelar AccountReceivable
└── 📊 Impacto: Vendas + Financeiro

SentToProduction → Cancelled:
├── ⚡ Ação: Cancelamento complexo
├── 🔄 Reversão: Cancelar produção + AR
└── 📊 Impacto: Vendas + Produção + Financeiro
```

## 🎯 Eventos de Domínio por Transição

```
OrderStatusChanged:
├── From: EstadoAnterior
├── To: NovoEstado  
├── Timestamp: DataHora da mudança
├── UserId: Usuário responsável
├── Reason: Motivo da mudança
└── AdditionalData: Dados específicos

OrderConfirmed → Gera:
├── AccountReceivableCreated
├── DemandStatusChanged (múltiplos)
└── ProductionNotificationSent

OrderSentToProduction → Gera:
├── ProductionOrderCreated
├── IngredientReserved (múltiplos)
└── ProductionScheduled

OrderCompleted → Gera:
├── CustomerNotificationSent
├── DeliveryScheduled
└── ProfitabilityCalculated
```

---

**Arquivo**: `order-lifecycle.md`  
**Domínio**: Vendas (#f36b21)  
**Tipo**: State Diagram  
**Foco**: Ciclo Completo OrderEntry + Integrações Automáticas
