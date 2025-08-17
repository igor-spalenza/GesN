# 💳 DIAGRAMA DE ESTADOS - CICLO DE VIDA DAS CONTAS FINANCEIRAS

## 🎯 Visão Geral
Diagrama de estados completo mostrando o ciclo de vida das contas financeiras (AccountReceivable e AccountPayable), desde sua criação automática até a liquidação total, incluindo controle de vencimentos, inadimplência e conciliação bancária.

## 💰 Diagrama de Estados - Contas a Receber (AccountReceivable)

```mermaid
stateDiagram-v2
    [*] --> Pending : 🆕 Criação automática<br/>OrderEntry confirmada
    
    %% === ESTADOS PRINCIPAIS ===
    Pending --> PartiallyPaid : 💰 Pagamento parcial<br/>Manual: Usuário registra
    PartiallyPaid --> Paid : 💰 Pagamento total<br/>Manual: Liquidação final
    Pending --> Paid : 💰 Pagamento total direto<br/>Manual: Quitação integral
    
    %% === VENCIMENTO ===
    Pending --> Overdue : ⏰ Vencimento sem pagamento<br/>Auto: Job diário verifica
    PartiallyPaid --> Overdue : ⏰ Vencimento parcial<br/>Auto: Saldo em atraso
    Overdue --> Paid : 💰 Pagamento após vencimento<br/>Manual: Com juros/multa
    
    %% === CANCELAMENTOS ===
    Pending --> Cancelled : ❌ Pedido cancelado<br/>Auto: OrderEntry cancelled
    PartiallyPaid --> Cancelled : ❌ Acordo cancelamento<br/>Manual: Negociação
    Overdue --> Cancelled : ❌ Perda definitiva<br/>Manual: Baixa por perda
    
    %% === ESTADOS FINAIS ===
    Paid --> [*] : 🎉 Conta totalmente quitada
    Cancelled --> [*] : 🚫 Conta cancelada
    
    %% === STYLING POR SITUAÇÃO ===
    
    classDef pending fill:#fef3c7,stroke:#f59e0b,stroke-width:3px,color:black
    class Pending pending
    
    classDef partial fill:#dbeafe,stroke:#3b82f6,stroke-width:3px,color:black
    class PartiallyPaid partial
    
    classDef paid fill:#d1fae5,stroke:#10b981,stroke-width:3px,color:black
    class Paid paid
    
    classDef overdue fill:#fecaca,stroke:#ef4444,stroke-width:3px,color:black
    class Overdue overdue
    
    classDef cancelled fill:#e5e7eb,stroke:#6b7280,stroke-width:3px,color:black
    class Cancelled cancelled
```

## 💸 Diagrama de Estados - Contas a Pagar (AccountPayable)

```mermaid
stateDiagram-v2
    [*] --> Pending : 🆕 Criação automática<br/>PurchaseOrder recebida
    
    %% === ESTADOS PRINCIPAIS ===
    Pending --> PartiallyPaid : 💳 Pagamento parcial<br/>Manual: Usuário efetua
    PartiallyPaid --> Paid : 💳 Pagamento total<br/>Manual: Liquidação final
    Pending --> Paid : 💳 Pagamento total direto<br/>Manual: Quitação integral
    
    %% === VENCIMENTO ===
    Pending --> Overdue : ⏰ Vencimento sem pagamento<br/>Auto: Job diário verifica
    PartiallyPaid --> Overdue : ⏰ Vencimento parcial<br/>Auto: Saldo em atraso
    Overdue --> Paid : 💳 Pagamento após vencimento<br/>Manual: Com multa
    
    %% === CANCELAMENTOS ===
    Pending --> Cancelled : ❌ Compra cancelada<br/>Auto: PurchaseOrder cancelled
    PartiallyPaid --> Cancelled : ❌ Acordo cancelamento<br/>Manual: Negociação
    Overdue --> Cancelled : ❌ Dispensa pagamento<br/>Manual: Acordo fornecedor
    
    %% === ESTADOS FINAIS ===
    Paid --> [*] : 🎉 Conta totalmente paga
    Cancelled --> [*] : 🚫 Conta cancelada
    
    %% === STYLING POR SITUAÇÃO ===
    
    classDef pending fill:#fef3c7,stroke:#f59e0b,stroke-width:3px,color:black
    class Pending pending
    
    classDef partial fill:#dbeafe,stroke:#3b82f6,stroke-width:3px,color:black
    class PartiallyPaid partial
    
    classDef paid fill:#d1fae5,stroke:#10b981,stroke-width:3px,color:black
    class Paid paid
    
    classDef overdue fill:#fecaca,stroke:#ef4444,stroke-width:3px,color:black
    class Overdue overdue
    
    classDef cancelled fill:#e5e7eb,stroke:#6b7280,stroke-width:3px,color:black
    class Cancelled cancelled
```

## 📋 Detalhamento dos Estados

### **🟡 PENDING (Pendente)**

#### **💰 AccountReceivable:**
```
📌 Estado Inicial para Contas a Receber
├── Origem: OrderEntry.OrderStatus = "Confirmed"
├── Descrição: Valor devido pelo cliente
├── Permitido: Aguardar pagamento ou registrar recebimento
├── Bloqueado: Não pode ser alterado diretamente
└── Próximo Estado: PartiallyPaid, Paid, Overdue ou Cancelled

Criação Automática:
├── 🔗 OrderEntryId: Pedido origem
├── 👤 CustomerId: Cliente devedor  
├── 💵 TotalAmount: OrderEntry.TotalValue
├── 📅 DueDate: DeliveryDate + PaymentTerms
└── 📄 InstallmentNumber: 1..N (se parcelado)
```

#### **💸 AccountPayable:**
```
📌 Estado Inicial para Contas a Pagar
├── Origem: PurchaseOrder.PurchaseStatus = "FullyReceived"
├── Descrição: Valor devido ao fornecedor
├── Permitido: Agendar ou efetuar pagamento
├── Bloqueado: Não pode ser alterado diretamente
└── Próximo Estado: PartiallyPaid, Paid, Overdue ou Cancelled

Criação Automática:
├── 🔗 PurchaseOrderId: Compra origem
├── 🏢 SupplierId: Fornecedor credor
├── 💵 TotalAmount: PurchaseOrder.TotalValue  
├── 📅 DueDate: ActualDeliveryDate + PaymentTerms
└── 📄 PaymentMethod: Baseado no fornecedor
```

**Cálculo de DueDate:**
```mermaid
flowchart TD
    A[Account criada] --> B[📊 Analisar PaymentTerms]
    B --> C{💳 Tipo de<br/>pagamento?}
    
    C -->|À vista| D[📅 DueDate = ReferenceDate]
    C -->|15 dias| E[📅 DueDate = ReferenceDate + 15]
    C -->|30 dias| F[📅 DueDate = ReferenceDate + 30]
    C -->|Parcelado| G[📅 DueDate = ReferenceDate + (30 * N)]
    
    D --> H[✅ DueDate calculada]
    E --> H
    F --> H  
    G --> H
    
    classDef calcStyle fill:#fed7aa,stroke:#f97316,stroke-width:2px,color:black
    class A,B,C,D,E,F,G,H calcStyle
```

### **🔵 PARTIALLY_PAID (Parcialmente Paga)**

#### **Processo de Pagamento Parcial:**
```mermaid
flowchart TD
    A[💰 Usuário registra<br/>recebimento/pagamento] --> B[🔍 Localizar Account<br/>por cliente/fornecedor]
    B --> C[💵 Informar valor<br/>recebido/pago]
    C --> D[💳 Selecionar método<br/>de pagamento]
    D --> E[🏦 Selecionar conta<br/>bancária]
    
    E --> F[💾 Criar Transaction<br/>vinculada à Account]
    F --> G[📊 PaidAmount += Valor]
    G --> H[🧮 RemainingAmount = Total - Paid]
    H --> I{💯 RemainingAmount = 0?}
    
    I -->|Sim| J[📈 Status: Paid]
    I -->|Não| K[📈 Status: PartiallyPaid]
    
    J --> L[🎉 Account liquidada]
    K --> M[⏰ Aguardar próximo<br/>pagamento]
    
    classDef paymentStyle fill:#dbeafe,stroke:#3b82f6,stroke-width:2px,color:black
    class A,B,C,D,E,F,G,H,I,J,K,L,M paymentStyle
```

**Controle de Parcelas:**
```
Para AccountReceivable parcelada:

Pedido R$ 3.000 em 3x:
├── Parcela 1: R$ 1.000 (venc: 30 dias) → PartiallyPaid
├── Parcela 2: R$ 1.000 (venc: 60 dias) → PartiallyPaid  
└── Parcela 3: R$ 1.000 (venc: 90 dias) → Paid (final)

Cada parcela é uma AccountReceivable separada
Status individual por parcela
```

### **🟢 PAID (Paga)**
```
📌 Estado Final de Sucesso
├── Trigger: RemainingAmount = 0
├── Descrição: Conta totalmente liquidada
├── Permitido: Consulta e análise histórica
├── Bloqueado: Qualquer alteração
└── Próximo Estado: [Finalizado]

Atualizações Automáticas:
├── 📊 Atualizar métricas de cobrança/pagamento
├── 💹 Calcular lucratividade (para AR)
├── 🤝 Avaliar relacionamento cliente/fornecedor
└── 📈 Atualizar fluxo de caixa realizado
```

**Análise de Lucratividade (AccountReceivable):**
```mermaid
flowchart TD
    A[AccountReceivable: Paid] --> B[📊 Buscar OrderEntry<br/>relacionada]
    B --> C[💰 Receita = TotalAmount]
    C --> D[🏭 Buscar custos de produção<br/>via Demands]
    D --> E[🛒 Buscar custos de compras<br/>via ingredientes]
    E --> F[💼 Alocar custos operacionais]
    F --> G[🧮 Lucro = Receita - Custos]
    G --> H[📈 Margem = Lucro / Receita]
    H --> I[💾 Salvar análise<br/>para relatórios]
    
    classDef profitabilityStyle fill:#d1fae5,stroke:#10b981,stroke-width:2px,color:black
    class A,B,C,D,E,F,G,H,I profitabilityStyle
```

### **🔴 OVERDUE (Vencida)**
```
📌 Estado de Inadimplência
├── Trigger: DueDate < hoje E RemainingAmount > 0
├── Descrição: Conta vencida não paga
├── Permitido: Ações de cobrança ou negociação
├── Bloqueado: Novos créditos (AR) ou atraso acumulado (AP)
└── Próximo Estado: Paid ou Cancelled

Ações Automáticas no Vencimento:
├── 📧 Notificar responsáveis
├── 💰 Aplicar juros/multa (se configurado)
├── 🚨 Gerar alertas críticos
└── 📊 Atualizar score de inadimplência
```

**Job de Verificação de Vencimentos:**
```mermaid
flowchart TD
    A[⏰ Job diário executa<br/>às 06:00] --> B[🔍 Buscar contas com<br/>DueDate < hoje]
    B --> C[📋 Para cada conta<br/>com saldo pendente]
    
    C --> D{💰 Tem saldo<br/>RemainingAmount > 0?}
    D -->|Não| E[✅ Conta já paga<br/>pular]
    D -->|Sim| F[📈 Status: Overdue]
    
    F --> G[📧 Enviar notificação<br/>ao responsável]
    G --> H[💰 Calcular juros/multa<br/>se configurado]
    H --> I[📊 Atualizar métricas<br/>de inadimplência]
    
    E --> J{🔄 Mais contas?}
    I --> J
    J -->|Sim| C
    J -->|Não| K[📈 Relatório diário<br/>de vencimentos]
    
    classDef jobStyle fill:#8b5cf6,stroke:#7c3aed,stroke-width:2px,color:white
    class A,B,C jobStyle
    
    classDef overdueStyle fill:#fecaca,stroke:#ef4444,stroke-width:2px,color:black
    class D,F,G,H,I,K overdueStyle
    
    classDef skipStyle fill:#e5e7eb,stroke:#6b7280,stroke-width:2px,color:black
    class E,J skipStyle
```

**Cálculo de Juros e Multa:**
```
Configuração por tipo de conta:

AccountReceivable (cliente inadimplente):
├── 💰 Multa: 2% sobre valor em atraso
├── 📈 Juros: 1% ao mês pro-rata
├── 📅 Base: dias em atraso
└── 🧮 Valor atualizado = Original + Multa + Juros

AccountPayable (fornecedor):  
├── 💰 Multa: Conforme contrato (se aplicável)
├── 📈 Juros: Conforme contrato
├── 📊 Impacto: Rating da empresa com fornecedor
└── 🤝 Relacionamento: Pode impactar futuros pedidos
```

### **❌ CANCELLED (Cancelada)**
```
📌 Estado Final de Cancelamento
├── Trigger: Cancelamento da origem ou acordo
├── Descrição: Conta cancelada por motivo específico
├── Permitido: Consulta e auditoria
├── Bloqueado: Reativação
└── Próximo Estado: [Finalizado]

Motivos de Cancelamento:
├── 🚫 OrderEntry/PurchaseOrder cancelada (automático)
├── 🤝 Acordo de cancelamento entre partes
├── 💔 Perda definitiva (baixa por perda)
└── 📄 Erro na criação (estorno)
```

**Impactos do Cancelamento:**

#### **AccountReceivable Cancelada:**
```mermaid
flowchart TD
    A[OrderEntry cancelada] --> B[📈 AccountReceivable: Cancelled]
    B --> C[🔄 Reverter Transactions<br/>já registradas]
    C --> D[🏦 Estornar valores<br/>em conta bancária]
    D --> E[📊 Atualizar fluxo<br/>de caixa projetado]
    E --> F[📧 Notificar cliente<br/>sobre cancelamento]
    
    classDef cancelStyle fill:#fecaca,stroke:#ef4444,stroke-width:2px,color:black
    class A,B,C,D,E,F cancelStyle
```

#### **AccountPayable Cancelada:**
```mermaid
flowchart TD
    A[PurchaseOrder cancelada] --> B[📈 AccountPayable: Cancelled]
    B --> C[🤝 Verificar se parcialmente<br/>paga ao fornecedor]
    C --> D{💰 Tem pagamentos<br/>já efetuados?}
    
    D -->|Não| E[✅ Cancelamento simples]
    D -->|Sim| F[🔄 Negociar com fornecedor<br/>devolução ou crédito]
    
    E --> G[📊 Remover do fluxo<br/>de caixa projetado]
    F --> H[💳 Registrar acordo<br/>ou ajuste financeiro]
    H --> G
    
    classDef cancelStyle fill:#fecaca,stroke:#ef4444,stroke-width:2px,color:black
    class A,B,C,D,E,F,G,H cancelStyle
```

## 🔄 Sincronização entre Domínios

### **🛍️ Vendas → AccountReceivable:**
```
OrderEntry Status Changes → AccountReceivable Actions:

OrderEntry: Confirmed
├── 🆕 Criar AccountReceivable(s)
├── 📅 Calcular DueDate(s)
├── 💵 Definir TotalAmount por parcela
└── 📈 Status inicial: Pending

OrderEntry: Delivered  
├── 🔓 Liberar para cobrança
├── 📧 Notificar financeiro
└── ⏰ Iniciar tracking de vencimento

OrderEntry: Cancelled
├── 📈 AccountReceivable: Cancelled
├── 🔄 Reverter Transactions
└── 📊 Ajustar projeções
```

### **🛒 Compras → AccountPayable:**
```
PurchaseOrder Status Changes → AccountPayable Actions:

PurchaseOrder: FullyReceived
├── 🆕 Criar AccountPayable
├── 📅 DueDate = ActualDeliveryDate + PaymentTerms
├── 💵 TotalAmount = PurchaseOrder.TotalValue
└── 📈 Status inicial: Pending

PurchaseOrder: Cancelled
├── 📈 AccountPayable: Cancelled
├── 🔄 Tratar pagamentos já efetuados
└── 📊 Ajustar projeções
```

## 💳 Integração com Transações Bancárias

### **Registro de Transação:**
```mermaid
flowchart TD
    A[💰 Pagamento recebido/efetuado] --> B[💾 Criar Transaction]
    B --> C[🔗 Vincular à Account<br/>Receivable ou Payable]
    C --> D[🏦 Definir BankAccount<br/>origem/destino]
    D --> E[📊 Atualizar saldos<br/>da Account]
    E --> F[💹 Atualizar saldo<br/>da BankAccount]
    F --> G[📈 Verificar se Account<br/>está totalmente paga]
    G --> H{💯 RemainingAmount = 0?}
    
    H -->|Sim| I[📈 Account Status: Paid]
    H -->|Não| J[📈 Account Status: PartiallyPaid]
    
    I --> K[🎉 Transaction concluída]
    J --> K
    
    classDef transactionStyle fill:#083e61,stroke:#083e61,stroke-width:2px,color:white
    class A,B,C,D,E,F,G,H,I,J,K transactionStyle
```

### **Conciliação Bancária:**
```
Transaction.IsReconciled = false → Pendente conciliação

Processo de conciliação:
├── 📄 Import extrato bancário
├── 🔍 Match automático por valor/data
├── ✅ Marcar Transaction.IsReconciled = true
├── 📅 Transaction.ReconciledDate = hoje
└── 📊 Gerar relatório de conciliação
```

## 🚨 Validações e Regras de Negócio

### **Validações Críticas:**
```
AccountReceivable:
├── ✅ DueDate ≥ OrderEntry.DeliveryDate
├── ✅ TotalAmount = OrderEntry.TotalValue (soma parcelas)
├── ✅ PaidAmount ≤ TotalAmount
├── ✅ CustomerId = OrderEntry.CustomerId

AccountPayable:
├── ✅ DueDate ≥ PurchaseOrder.ActualDeliveryDate  
├── ✅ TotalAmount = PurchaseOrder.TotalValue
├── ✅ PaidAmount ≤ TotalAmount
├── ✅ SupplierId = PurchaseOrder.SupplierId

Transaction:
├── ✅ Amount > 0
├── ✅ Deve referenciar AccountReceivable OU AccountPayable
├── ✅ TransactionDate ≤ hoje
├── ✅ BankAccount deve existir e estar ativa
```

### **Regras de Inadimplência:**
```
AccountReceivable Overdue:
├── 🚫 Cliente não pode fazer novos pedidos
├── ⚠️ Alertas escalados por tempo de atraso
├── 📞 Ações de cobrança automatizadas
└── 💰 Juros/multa aplicados conforme configuração

AccountPayable Overdue:
├── 🚨 Alerta crítico para financeiro
├── 📉 Impacto no rating com fornecedor
├── 🤝 Pode afetar relacionamento comercial
└── 💰 Multas contratuais se aplicáveis
```

## 🎯 Eventos de Domínio e Alertas

### **Eventos Gerados:**
```
AccountStatusChanged:
├── AccountId: ID da conta
├── AccountType: Receivable ou Payable
├── From: Status anterior
├── To: Novo status
├── Amount: Valor da transação (se aplicável)
├── RemainingAmount: Saldo restante
└── Timestamp: Data/hora da mudança

Eventos Específicos:
├── AccountCreated: Nova conta criada
├── PaymentReceived: Pagamento de cliente
├── PaymentMade: Pagamento a fornecedor  
├── AccountOverdue: Conta vencida
├── AccountPaid: Conta totalmente quitada
├── AccountCancelled: Conta cancelada
└── CashFlowUpdated: Fluxo de caixa atualizado
```

### **Sistema de Alertas:**
```
🚨 Alertas Críticos:
├── Conta vencida > 30 dias
├── Cliente com múltiplas contas vencidas
├── Fluxo de caixa negativo projetado
├── Fornecedor não pago no prazo

⚠️ Alertas de Atenção:
├── Conta vencendo em 3 dias
├── Pagamento recebido para conciliar
├── Cliente atingindo limite de crédito
├── Fornecedor com desconto por antecipação

💡 Alertas Informativos:
├── Recebimento antecipado de cliente
├── Oportunidade de desconto fornecedor
├── Meta de recebimento atingida
├── Fluxo de caixa positivo acima do esperado
```

## 📊 Métricas e KPIs

### **Indicadores de Recebimento:**
```
DSO (Days Sales Outstanding):
├── Fórmula: (AR médio / Vendas diárias)
├── Meta: ≤ 30 dias
├── Cálculo: Média móvel 12 meses

Taxa de Inadimplência:
├── Fórmula: (Valor vencido / Total AR) * 100
├── Meta: ≤ 5%
├── Segmentação: Por cliente, produto, região

Eficiência de Cobrança:
├── Fórmula: (Recebido no prazo / Total devido) * 100
├── Meta: ≥ 95%
├── Tracking: Mensal e acumulado
```

### **Indicadores de Pagamento:**
```
DPO (Days Payable Outstanding):
├── Fórmula: (AP médio / Compras diárias)
├── Estratégia: Maximizar sem prejudicar relacionamento
├── Balance: Fluxo de caixa vs desconto por antecipação

Pontualidade de Pagamentos:
├── Fórmula: (Pagos no prazo / Total devido) * 100
├── Meta: ≥ 98%
├── Impacto: Rating creditício da empresa

Economia com Descontos:
├── Fórmula: Σ descontos obtidos por antecipação
├── Oportunidade: vs custo do dinheiro
├── ROI: Desconto vs juros de caixa
```

---

**Arquivo**: `account-lifecycle.md`  
**Domínio**: Financeiro (#083e61)  
**Tipo**: State Diagram  
**Foco**: Ciclo Completo AR/AP + Inadimplência + Conciliação Bancária
