# ðŸ’³ DIAGRAMA DE ESTADOS - CICLO DE VIDA DAS CONTAS FINANCEIRAS

## ðŸŽ¯ VisÃ£o Geral
Diagrama de estados completo mostrando o ciclo de vida das contas financeiras (AccountReceivable e AccountPayable), desde sua criaÃ§Ã£o automÃ¡tica atÃ© a liquidaÃ§Ã£o total, incluindo controle de vencimentos, inadimplÃªncia e conciliaÃ§Ã£o bancÃ¡ria.

## ðŸ’° Diagrama de Estados - Contas a Receber (AccountReceivable)

```mermaid
stateDiagram-v2
    [*] --> Pending : ðŸ†• CriaÃ§Ã£o automÃ¡tica<br/>OrderEntry confirmada
    
    %% === ESTADOS PRINCIPAIS ===
    Pending --> PartiallyPaid : ðŸ’° Pagamento parcial<br/>Manual: UsuÃ¡rio registra
    PartiallyPaid --> Paid : ðŸ’° Pagamento total<br/>Manual: LiquidaÃ§Ã£o final
    Pending --> Paid : ðŸ’° Pagamento total direto<br/>Manual: QuitaÃ§Ã£o integral
    
    %% === VENCIMENTO ===
    Pending --> Overdue : â° Vencimento sem pagamento<br/>Auto: Job diÃ¡rio verifica
    PartiallyPaid --> Overdue : â° Vencimento parcial<br/>Auto: Saldo em atraso
    Overdue --> Paid : ðŸ’° Pagamento apÃ³s vencimento<br/>Manual: Com juros/multa
    
    %% === CANCELAMENTOS ===
    Pending --> Cancelled : âŒ Pedido cancelado<br/>Auto: OrderEntry cancelled
    PartiallyPaid --> Cancelled : âŒ Acordo cancelamento<br/>Manual: NegociaÃ§Ã£o
    Overdue --> Cancelled : âŒ Perda definitiva<br/>Manual: Baixa por perda
    
    %% === ESTADOS FINAIS ===
    Paid --> [*] : ðŸŽ‰ Conta totalmente quitada
    Cancelled --> [*] : ðŸš« Conta cancelada
    
    %% === STYLING POR SITUAÃ‡ÃƒO ===
    
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

## ðŸ’¸ Diagrama de Estados - Contas a Pagar (AccountPayable)

```mermaid
stateDiagram-v2
    [*] --> Pending : ðŸ†• CriaÃ§Ã£o automÃ¡tica<br/>PurchaseOrder recebida
    
    %% === ESTADOS PRINCIPAIS ===
    Pending --> PartiallyPaid : ðŸ’³ Pagamento parcial<br/>Manual: UsuÃ¡rio efetua
    PartiallyPaid --> Paid : ðŸ’³ Pagamento total<br/>Manual: LiquidaÃ§Ã£o final
    Pending --> Paid : ðŸ’³ Pagamento total direto<br/>Manual: QuitaÃ§Ã£o integral
    
    %% === VENCIMENTO ===
    Pending --> Overdue : â° Vencimento sem pagamento<br/>Auto: Job diÃ¡rio verifica
    PartiallyPaid --> Overdue : â° Vencimento parcial<br/>Auto: Saldo em atraso
    Overdue --> Paid : ðŸ’³ Pagamento apÃ³s vencimento<br/>Manual: Com multa
    
    %% === CANCELAMENTOS ===
    Pending --> Cancelled : âŒ Compra cancelada<br/>Auto: PurchaseOrder cancelled
    PartiallyPaid --> Cancelled : âŒ Acordo cancelamento<br/>Manual: NegociaÃ§Ã£o
    Overdue --> Cancelled : âŒ Dispensa pagamento<br/>Manual: Acordo fornecedor
    
    %% === ESTADOS FINAIS ===
    Paid --> [*] : ðŸŽ‰ Conta totalmente paga
    Cancelled --> [*] : ðŸš« Conta cancelada
    
    %% === STYLING POR SITUAÃ‡ÃƒO ===
    
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

## ðŸ“‹ Detalhamento dos Estados

### **ðŸŸ¡ PENDING (Pendente)**

#### **ðŸ’° AccountReceivable:**
```
ðŸ“Œ Estado Inicial para Contas a Receber
â”œâ”€â”€ Origem: OrderEntry.OrderStatus = "Confirmed"
â”œâ”€â”€ DescriÃ§Ã£o: Valor devido pelo cliente
â”œâ”€â”€ Permitido: Aguardar pagamento ou registrar recebimento
â”œâ”€â”€ Bloqueado: NÃ£o pode ser alterado diretamente
â””â”€â”€ PrÃ³ximo Estado: PartiallyPaid, Paid, Overdue ou Cancelled

CriaÃ§Ã£o AutomÃ¡tica:
â”œâ”€â”€ ðŸ”— OrderEntryId: Pedido origem
â”œâ”€â”€ ðŸ‘¤ CustomerId: Cliente devedor  
â”œâ”€â”€ ðŸ’µ TotalAmount: OrderEntry.TotalValue
â”œâ”€â”€ ðŸ“… DueDate: DeliveryDate + PaymentTerms
â””â”€â”€ ðŸ“„ InstallmentNumber: 1..N (se parcelado)
```

#### **ðŸ’¸ AccountPayable:**
```
ðŸ“Œ Estado Inicial para Contas a Pagar
â”œâ”€â”€ Origem: PurchaseOrder.PurchaseStatus = "FullyReceived"
â”œâ”€â”€ DescriÃ§Ã£o: Valor devido ao fornecedor
â”œâ”€â”€ Permitido: Agendar ou efetuar pagamento
â”œâ”€â”€ Bloqueado: NÃ£o pode ser alterado diretamente
â””â”€â”€ PrÃ³ximo Estado: PartiallyPaid, Paid, Overdue ou Cancelled

CriaÃ§Ã£o AutomÃ¡tica:
â”œâ”€â”€ ðŸ”— PurchaseOrderId: Compra origem
â”œâ”€â”€ ðŸ¢ SupplierId: Fornecedor credor
â”œâ”€â”€ ðŸ’µ TotalAmount: PurchaseOrder.TotalValue  
â”œâ”€â”€ ðŸ“… DueDate: ActualDeliveryDate + PaymentTerms
â””â”€â”€ ðŸ“„ PaymentMethod: Baseado no fornecedor
```

**CÃ¡lculo de DueDate:**
```mermaid
flowchart TD
    A[Account criada] --> B[ðŸ“Š Analisar PaymentTerms]
    B --> C{ðŸ’³ Tipo de<br/>pagamento?}
    
    C -->|Ã€ vista| D[ðŸ“… DueDate = ReferenceDate]
    C -->|15 dias| E[ðŸ“… DueDate = ReferenceDate + 15]
    C -->|30 dias| F[ðŸ“… DueDate = ReferenceDate + 30]
    C -->|Parcelado| G[ðŸ“… DueDate = ReferenceDate + (30 * N)]
    
    D --> H[âœ… DueDate calculada]
    E --> H
    F --> H  
    G --> H
    
    classDef calcStyle fill:#fed7aa,stroke:#f97316,stroke-width:2px,color:black
    class A,B,C,D,E,F,G,H calcStyle
```

### **ðŸ”µ PARTIALLY_PAID (Parcialmente Paga)**

#### **Processo de Pagamento Parcial:**
```mermaid
flowchart TD
    A[ðŸ’° UsuÃ¡rio registra<br/>recebimento/pagamento] --> B[ðŸ” Localizar Account<br/>por cliente/fornecedor]
    B --> C[ðŸ’µ Informar valor<br/>recebido/pago]
    C --> D[ðŸ’³ Selecionar mÃ©todo<br/>de pagamento]
    D --> E[ðŸ¦ Selecionar conta<br/>bancÃ¡ria]
    
    E --> F[ðŸ’¾ Criar Transaction<br/>vinculada Ã  Account]
    F --> G[ðŸ“Š PaidAmount += Valor]
    G --> H[ðŸ§® RemainingAmount = Total - Paid]
    H --> I{ðŸ’¯ RemainingAmount = 0?}
    
    I -->|Sim| J[ðŸ“ˆ Status: Paid]
    I -->|NÃ£o| K[ðŸ“ˆ Status: PartiallyPaid]
    
    J --> L[ðŸŽ‰ Account liquidada]
    K --> M[â° Aguardar prÃ³ximo<br/>pagamento]
    
    classDef paymentStyle fill:#dbeafe,stroke:#3b82f6,stroke-width:2px,color:black
    class A,B,C,D,E,F,G,H,I,J,K,L,M paymentStyle
```

**Controle de Parcelas:**
```
Para AccountReceivable parcelada:

Pedido R$ 3.000 em 3x:
â”œâ”€â”€ Parcela 1: R$ 1.000 (venc: 30 dias) â†’ PartiallyPaid
â”œâ”€â”€ Parcela 2: R$ 1.000 (venc: 60 dias) â†’ PartiallyPaid  
â””â”€â”€ Parcela 3: R$ 1.000 (venc: 90 dias) â†’ Paid (final)

Cada parcela Ã© uma AccountReceivable separada
Status individual por parcela
```

### **ðŸŸ¢ PAID (Paga)**
```
ðŸ“Œ Estado Final de Sucesso
â”œâ”€â”€ Trigger: RemainingAmount = 0
â”œâ”€â”€ DescriÃ§Ã£o: Conta totalmente liquidada
â”œâ”€â”€ Permitido: Consulta e anÃ¡lise histÃ³rica
â”œâ”€â”€ Bloqueado: Qualquer alteraÃ§Ã£o
â””â”€â”€ PrÃ³ximo Estado: [Finalizado]

AtualizaÃ§Ãµes AutomÃ¡ticas:
â”œâ”€â”€ ðŸ“Š Atualizar mÃ©tricas de cobranÃ§a/pagamento
â”œâ”€â”€ ðŸ’¹ Calcular lucratividade (para AR)
â”œâ”€â”€ ðŸ¤ Avaliar relacionamento cliente/fornecedor
â””â”€â”€ ðŸ“ˆ Atualizar fluxo de caixa realizado
```

**AnÃ¡lise de Lucratividade (AccountReceivable):**
```mermaid
flowchart TD
    A[AccountReceivable: Paid] --> B[ðŸ“Š Buscar OrderEntry<br/>relacionada]
    B --> C[ðŸ’° Receita = TotalAmount]
    C --> D[ðŸ­ Buscar custos de produÃ§Ã£o<br/>via Demands]
    D --> E[ðŸ›’ Buscar custos de compras<br/>via ingredientes]
    E --> F[ðŸ’¼ Alocar custos operacionais]
    F --> G[ðŸ§® Lucro = Receita - Custos]
    G --> H[ðŸ“ˆ Margem = Lucro / Receita]
    H --> I[ðŸ’¾ Salvar anÃ¡lise<br/>para relatÃ³rios]
    
    classDef profitabilityStyle fill:#d1fae5,stroke:#10b981,stroke-width:2px,color:black
    class A,B,C,D,E,F,G,H,I profitabilityStyle
```

### **ðŸ”´ OVERDUE (Vencida)**
```
ðŸ“Œ Estado de InadimplÃªncia
â”œâ”€â”€ Trigger: DueDate < hoje E RemainingAmount > 0
â”œâ”€â”€ DescriÃ§Ã£o: Conta vencida nÃ£o paga
â”œâ”€â”€ Permitido: AÃ§Ãµes de cobranÃ§a ou negociaÃ§Ã£o
â”œâ”€â”€ Bloqueado: Novos crÃ©ditos (AR) ou atraso acumulado (AP)
â””â”€â”€ PrÃ³ximo Estado: Paid ou Cancelled

AÃ§Ãµes AutomÃ¡ticas no Vencimento:
â”œâ”€â”€ ðŸ“§ Notificar responsÃ¡veis
â”œâ”€â”€ ðŸ’° Aplicar juros/multa (se configurado)
â”œâ”€â”€ ðŸš¨ Gerar alertas crÃ­ticos
â””â”€â”€ ðŸ“Š Atualizar score de inadimplÃªncia
```

**Job de VerificaÃ§Ã£o de Vencimentos:**
```mermaid
flowchart TD
    A[â° Job diÃ¡rio executa<br/>Ã s 06:00] --> B[ðŸ” Buscar contas com<br/>DueDate < hoje]
    B --> C[ðŸ“‹ Para cada conta<br/>com saldo pendente]
    
    C --> D{ðŸ’° Tem saldo<br/>RemainingAmount > 0?}
    D -->|NÃ£o| E[âœ… Conta jÃ¡ paga<br/>pular]
    D -->|Sim| F[ðŸ“ˆ Status: Overdue]
    
    F --> G[ðŸ“§ Enviar notificaÃ§Ã£o<br/>ao responsÃ¡vel]
    G --> H[ðŸ’° Calcular juros/multa<br/>se configurado]
    H --> I[ðŸ“Š Atualizar mÃ©tricas<br/>de inadimplÃªncia]
    
    E --> J{ðŸ”„ Mais contas?}
    I --> J
    J -->|Sim| C
    J -->|NÃ£o| K[ðŸ“ˆ RelatÃ³rio diÃ¡rio<br/>de vencimentos]
    
    classDef jobStyle fill:#8b5cf6,stroke:#7c3aed,stroke-width:2px,color:white
    class A,B,C jobStyle
    
    classDef overdueStyle fill:#fecaca,stroke:#ef4444,stroke-width:2px,color:black
    class D,F,G,H,I,K overdueStyle
    
    classDef skipStyle fill:#e5e7eb,stroke:#6b7280,stroke-width:2px,color:black
    class E,J skipStyle
```

**CÃ¡lculo de Juros e Multa:**
```
ConfiguraÃ§Ã£o por tipo de conta:

AccountReceivable (cliente inadimplente):
â”œâ”€â”€ ðŸ’° Multa: 2% sobre valor em atraso
â”œâ”€â”€ ðŸ“ˆ Juros: 1% ao mÃªs pro-rata
â”œâ”€â”€ ðŸ“… Base: dias em atraso
â””â”€â”€ ðŸ§® Valor atualizado = Original + Multa + Juros

AccountPayable (fornecedor):  
â”œâ”€â”€ ðŸ’° Multa: Conforme contrato (se aplicÃ¡vel)
â”œâ”€â”€ ðŸ“ˆ Juros: Conforme contrato
â”œâ”€â”€ ðŸ“Š Impacto: Rating da empresa com fornecedor
â””â”€â”€ ðŸ¤ Relacionamento: Pode impactar futuros pedidos
```

### **âŒ CANCELLED (Cancelada)**
```
ðŸ“Œ Estado Final de Cancelamento
â”œâ”€â”€ Trigger: Cancelamento da origem ou acordo
â”œâ”€â”€ DescriÃ§Ã£o: Conta cancelada por motivo especÃ­fico
â”œâ”€â”€ Permitido: Consulta e auditoria
â”œâ”€â”€ Bloqueado: ReativaÃ§Ã£o
â””â”€â”€ PrÃ³ximo Estado: [Finalizado]

Motivos de Cancelamento:
â”œâ”€â”€ ðŸš« OrderEntry/PurchaseOrder cancelada (automÃ¡tico)
â”œâ”€â”€ ðŸ¤ Acordo de cancelamento entre partes
â”œâ”€â”€ ðŸ’” Perda definitiva (baixa por perda)
â””â”€â”€ ðŸ“„ Erro na criaÃ§Ã£o (estorno)
```

**Impactos do Cancelamento:**

#### **AccountReceivable Cancelada:**
```mermaid
flowchart TD
    A[OrderEntry cancelada] --> B[ðŸ“ˆ AccountReceivable: Cancelled]
    B --> C[ðŸ”„ Reverter Transactions<br/>jÃ¡ registradas]
    C --> D[ðŸ¦ Estornar valores<br/>em conta bancÃ¡ria]
    D --> E[ðŸ“Š Atualizar fluxo<br/>de caixa projetado]
    E --> F[ðŸ“§ Notificar cliente<br/>sobre cancelamento]
    
    classDef cancelStyle fill:#fecaca,stroke:#ef4444,stroke-width:2px,color:black
    class A,B,C,D,E,F cancelStyle
```

#### **AccountPayable Cancelada:**
```mermaid
flowchart TD
    A[PurchaseOrder cancelada] --> B[ðŸ“ˆ AccountPayable: Cancelled]
    B --> C[ðŸ¤ Verificar se parcialmente<br/>paga ao fornecedor]
    C --> D{ðŸ’° Tem pagamentos<br/>jÃ¡ efetuados?}
    
    D -->|NÃ£o| E[âœ… Cancelamento simples]
    D -->|Sim| F[ðŸ”„ Negociar com fornecedor<br/>devoluÃ§Ã£o ou crÃ©dito]
    
    E --> G[ðŸ“Š Remover do fluxo<br/>de caixa projetado]
    F --> H[ðŸ’³ Registrar acordo<br/>ou ajuste financeiro]
    H --> G
    
    classDef cancelStyle fill:#fecaca,stroke:#ef4444,stroke-width:2px,color:black
    class A,B,C,D,E,F,G,H cancelStyle
```

## ðŸ”„ SincronizaÃ§Ã£o entre DomÃ­nios

### **ðŸ›ï¸ Vendas â†’ AccountReceivable:**
```
OrderEntry Status Changes â†’ AccountReceivable Actions:

OrderEntry: Confirmed
â”œâ”€â”€ ðŸ†• Criar AccountReceivable(s)
â”œâ”€â”€ ðŸ“… Calcular DueDate(s)
â”œâ”€â”€ ðŸ’µ Definir TotalAmount por parcela
â””â”€â”€ ðŸ“ˆ Status inicial: Pending

OrderEntry: Delivered  
â”œâ”€â”€ ðŸ”“ Liberar para cobranÃ§a
â”œâ”€â”€ ðŸ“§ Notificar financeiro
â””â”€â”€ â° Iniciar tracking de vencimento

OrderEntry: Cancelled
â”œâ”€â”€ ðŸ“ˆ AccountReceivable: Cancelled
â”œâ”€â”€ ðŸ”„ Reverter Transactions
â””â”€â”€ ðŸ“Š Ajustar projeÃ§Ãµes
```

### **ðŸ›’ Compras â†’ AccountPayable:**
```
PurchaseOrder Status Changes â†’ AccountPayable Actions:

PurchaseOrder: FullyReceived
â”œâ”€â”€ ðŸ†• Criar AccountPayable
â”œâ”€â”€ ðŸ“… DueDate = ActualDeliveryDate + PaymentTerms
â”œâ”€â”€ ðŸ’µ TotalAmount = PurchaseOrder.TotalValue
â””â”€â”€ ðŸ“ˆ Status inicial: Pending

PurchaseOrder: Cancelled
â”œâ”€â”€ ðŸ“ˆ AccountPayable: Cancelled
â”œâ”€â”€ ðŸ”„ Tratar pagamentos jÃ¡ efetuados
â””â”€â”€ ðŸ“Š Ajustar projeÃ§Ãµes
```

## ðŸ’³ IntegraÃ§Ã£o com TransaÃ§Ãµes BancÃ¡rias

### **Registro de TransaÃ§Ã£o:**
```mermaid
flowchart TD
    A[ðŸ’° Pagamento recebido/efetuado] --> B[ðŸ’¾ Criar Transaction]
    B --> C[ðŸ”— Vincular Ã  Account<br/>Receivable ou Payable]
    C --> D[ðŸ¦ Definir BankAccount<br/>origem/destino]
    D --> E[ðŸ“Š Atualizar saldos<br/>da Account]
    E --> F[ðŸ’¹ Atualizar saldo<br/>da BankAccount]
    F --> G[ðŸ“ˆ Verificar se Account<br/>estÃ¡ totalmente paga]
    G --> H{ðŸ’¯ RemainingAmount = 0?}
    
    H -->|Sim| I[ðŸ“ˆ Account Status: Paid]
    H -->|NÃ£o| J[ðŸ“ˆ Account Status: PartiallyPaid]
    
    I --> K[ðŸŽ‰ Transaction concluÃ­da]
    J --> K
    
    classDef transactionStyle fill:#083e61,stroke:#083e61,stroke-width:2px,color:white
    class A,B,C,D,E,F,G,H,I,J,K transactionStyle
```

### **ConciliaÃ§Ã£o BancÃ¡ria:**
```
Transaction.IsReconciled = false â†’ Pendente conciliaÃ§Ã£o

Processo de conciliaÃ§Ã£o:
â”œâ”€â”€ ðŸ“„ Import extrato bancÃ¡rio
â”œâ”€â”€ ðŸ” Match automÃ¡tico por valor/data
â”œâ”€â”€ âœ… Marcar Transaction.IsReconciled = true
â”œâ”€â”€ ðŸ“… Transaction.ReconciledDate = hoje
â””â”€â”€ ðŸ“Š Gerar relatÃ³rio de conciliaÃ§Ã£o
```

## ðŸš¨ ValidaÃ§Ãµes e Regras de NegÃ³cio

### **ValidaÃ§Ãµes CrÃ­ticas:**
```
AccountReceivable:
â”œâ”€â”€ âœ… DueDate â‰¥ OrderEntry.DeliveryDate
â”œâ”€â”€ âœ… TotalAmount = OrderEntry.TotalValue (soma parcelas)
â”œâ”€â”€ âœ… PaidAmount â‰¤ TotalAmount
â”œâ”€â”€ âœ… CustomerId = OrderEntry.CustomerId

AccountPayable:
â”œâ”€â”€ âœ… DueDate â‰¥ PurchaseOrder.ActualDeliveryDate  
â”œâ”€â”€ âœ… TotalAmount = PurchaseOrder.TotalValue
â”œâ”€â”€ âœ… PaidAmount â‰¤ TotalAmount
â”œâ”€â”€ âœ… SupplierId = PurchaseOrder.SupplierId

Transaction:
â”œâ”€â”€ âœ… Amount > 0
â”œâ”€â”€ âœ… Deve referenciar AccountReceivable OU AccountPayable
â”œâ”€â”€ âœ… TransactionDate â‰¤ hoje
â”œâ”€â”€ âœ… BankAccount deve existir e estar ativa
```

### **Regras de InadimplÃªncia:**
```
AccountReceivable Overdue:
â”œâ”€â”€ ðŸš« Cliente nÃ£o pode fazer novos pedidos
â”œâ”€â”€ âš ï¸ Alertas escalados por tempo de atraso
â”œâ”€â”€ ðŸ“ž AÃ§Ãµes de cobranÃ§a automatizadas
â””â”€â”€ ðŸ’° Juros/multa aplicados conforme configuraÃ§Ã£o

AccountPayable Overdue:
â”œâ”€â”€ ðŸš¨ Alerta crÃ­tico para financeiro
â”œâ”€â”€ ðŸ“‰ Impacto no rating com fornecedor
â”œâ”€â”€ ðŸ¤ Pode afetar relacionamento comercial
â””â”€â”€ ðŸ’° Multas contratuais se aplicÃ¡veis
```

## ðŸŽ¯ Eventos de DomÃ­nio e Alertas

### **Eventos Gerados:**
```
AccountStatusChanged:
â”œâ”€â”€ AccountId: ID da conta
â”œâ”€â”€ AccountType: Receivable ou Payable
â”œâ”€â”€ From: Status anterior
â”œâ”€â”€ To: Novo status
â”œâ”€â”€ Amount: Valor da transaÃ§Ã£o (se aplicÃ¡vel)
â”œâ”€â”€ RemainingAmount: Saldo restante
â””â”€â”€ Timestamp: Data/hora da mudanÃ§a

Eventos EspecÃ­ficos:
â”œâ”€â”€ AccountCreated: Nova conta criada
â”œâ”€â”€ PaymentReceived: Pagamento de cliente
â”œâ”€â”€ PaymentMade: Pagamento a fornecedor  
â”œâ”€â”€ AccountOverdue: Conta vencida
â”œâ”€â”€ AccountPaid: Conta totalmente quitada
â”œâ”€â”€ AccountCancelled: Conta cancelada
â””â”€â”€ CashFlowUpdated: Fluxo de caixa atualizado
```

### **Sistema de Alertas:**
```
ðŸš¨ Alertas CrÃ­ticos:
â”œâ”€â”€ Conta vencida > 30 dias
â”œâ”€â”€ Cliente com mÃºltiplas contas vencidas
â”œâ”€â”€ Fluxo de caixa negativo projetado
â”œâ”€â”€ Fornecedor nÃ£o pago no prazo

âš ï¸ Alertas de AtenÃ§Ã£o:
â”œâ”€â”€ Conta vencendo em 3 dias
â”œâ”€â”€ Pagamento recebido para conciliar
â”œâ”€â”€ Cliente atingindo limite de crÃ©dito
â”œâ”€â”€ Fornecedor com desconto por antecipaÃ§Ã£o

ðŸ’¡ Alertas Informativos:
â”œâ”€â”€ Recebimento antecipado de cliente
â”œâ”€â”€ Oportunidade de desconto fornecedor
â”œâ”€â”€ Meta de recebimento atingida
â”œâ”€â”€ Fluxo de caixa positivo acima do esperado
```

## ðŸ“Š MÃ©tricas e KPIs

### **Indicadores de Recebimento:**
```
DSO (Days Sales Outstanding):
â”œâ”€â”€ FÃ³rmula: (AR mÃ©dio / Vendas diÃ¡rias)
â”œâ”€â”€ Meta: â‰¤ 30 dias
â”œâ”€â”€ CÃ¡lculo: MÃ©dia mÃ³vel 12 meses

Taxa de InadimplÃªncia:
â”œâ”€â”€ FÃ³rmula: (Valor vencido / Total AR) * 100
â”œâ”€â”€ Meta: â‰¤ 5%
â”œâ”€â”€ SegmentaÃ§Ã£o: Por cliente, produto, regiÃ£o

EficiÃªncia de CobranÃ§a:
â”œâ”€â”€ FÃ³rmula: (Recebido no prazo / Total devido) * 100
â”œâ”€â”€ Meta: â‰¥ 95%
â”œâ”€â”€ Tracking: Mensal e acumulado
```

### **Indicadores de Pagamento:**
```
DPO (Days Payable Outstanding):
â”œâ”€â”€ FÃ³rmula: (AP mÃ©dio / Compras diÃ¡rias)
â”œâ”€â”€ EstratÃ©gia: Maximizar sem prejudicar relacionamento
â”œâ”€â”€ Balance: Fluxo de caixa vs desconto por antecipaÃ§Ã£o

Pontualidade de Pagamentos:
â”œâ”€â”€ FÃ³rmula: (Pagos no prazo / Total devido) * 100
â”œâ”€â”€ Meta: â‰¥ 98%
â”œâ”€â”€ Impacto: Rating creditÃ­cio da empresa

Economia com Descontos:
â”œâ”€â”€ FÃ³rmula: Î£ descontos obtidos por antecipaÃ§Ã£o
â”œâ”€â”€ Oportunidade: vs custo do dinheiro
â”œâ”€â”€ ROI: Desconto vs juros de caixa
```

---

**Arquivo**: `account-lifecycle.md`  
**DomÃ­nio**: Financeiro (#083e61)  
**Tipo**: State Diagram  
**Foco**: Ciclo Completo AR/AP + InadimplÃªncia + ConciliaÃ§Ã£o BancÃ¡ria
