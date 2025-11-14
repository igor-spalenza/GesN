# ðŸ’³ FLUXOGRAMA - PROCESSO FINANCEIRO

## ðŸŽ¯ VisÃ£o Geral
Fluxograma completo dos processos financeiros, mostrando o controle de fluxo de caixa atravÃ©s de contas a receber (originadas das vendas) e contas a pagar (originadas das compras), com rastreamento detalhado de transaÃ§Ãµes e anÃ¡lise de lucratividade.

## ðŸ’° Fluxo de Contas a Receber

```mermaid
flowchart TD
    %% === ORIGEM: VENDAS ===
    A[ðŸ›ï¸ OrderEntry confirmada<br/>no DomÃ­nio Vendas] --> B[ðŸ“Š Analisar PaymentTerms<br/>do pedido]
    
    B --> C{ðŸ’³ Tipo de<br/>pagamento?}
    
    %% PAGAMENTO Ã€ VISTA
    C -->|Ã€ Vista| D1[ðŸ’° Pagamento Ã  vista]
    D1 --> D2[ðŸ’¾ Criar 1 AccountReceivable<br/>DueDate = DeliveryDate]
    D2 --> D3[ðŸ’µ TotalAmount = OrderEntry.TotalValue<br/>InstallmentNumber = 1]
    
    %% PAGAMENTO PARCELADO
    C -->|Parcelado| E1[ðŸ“‹ Pagamento parcelado]
    E1 --> E2[ðŸ”¢ Dividir valor total<br/>pelo nÃºmero de parcelas]
    E2 --> E3[ðŸ“… Para cada parcela]
    E3 --> E4[ðŸ’¾ Criar AccountReceivable<br/>DueDate = DeliveryDate + (30 * N)]
    E4 --> E5[ðŸ’µ TotalAmount = Valor/Parcelas<br/>InstallmentNumber = N]
    E5 --> E6{ðŸ”„ Mais parcelas?}
    E6 -->|Sim| E3
    E6 -->|NÃ£o| F
    
    D3 --> F[ðŸ“ˆ AccountStatus: Pending<br/>Aguardar recebimento]
    
    %% === PROCESSO DE RECEBIMENTO ===
    F --> G[â° Vencimento se aproxima<br/>ou cliente paga]
    G --> H[ðŸ‘¤ UsuÃ¡rio registra<br/>recebimento]
    
    H --> I[ðŸ” Localizar AccountReceivable<br/>pelo cliente/pedido]
    I --> J[ðŸ’° Informar valor recebido<br/>e forma de pagamento]
    J --> K[ðŸ¦ Selecionar conta bancÃ¡ria<br/>de destino]
    
    K --> L[ðŸ’¾ Criar Transaction]
    L --> M[ðŸ“Š Atualizar AccountReceivable<br/>PaidAmount += Valor]
    M --> N[ðŸ§® Calcular RemainingAmount<br/>= TotalAmount - PaidAmount]
    
    N --> O{ðŸ’¯ Valor pago<br/>completamente?}
    
    O -->|Sim| P1[âœ… AccountStatus: Paid]
    O -->|NÃ£o| P2[âš ï¸ AccountStatus: PartiallyPaid]
    
    P1 --> Q[ðŸŽ‰ Conta recebida<br/>com sucesso]
    P2 --> Q
    
    %% === CONTROLE DE INADIMPLÃŠNCIA ===
    F --> R[ðŸ“… Job automÃ¡tico verifica<br/>vencimentos diÃ¡rios]
    R --> S{â° Conta vencida<br/>e nÃ£o paga?}
    
    S -->|Sim| T[ðŸ“ˆ AccountStatus: Overdue]
    T --> U[ðŸ“§ Notificar cliente<br/>sobre vencimento]
    U --> V[ðŸ“Š Aplicar juros/multa<br/>se configurado]
    
    S -->|NÃ£o| Q
    V --> Q
    
    %% === STYLING ===
    
    classDef salesOriginStyle fill:#f36b21,stroke:#f36b21,stroke-width:2px,color:white
    class A salesOriginStyle
    
    classDef paymentTypeStyle fill:#fed7aa,stroke:#f97316,stroke-width:2px,color:black
    class B,C paymentTypeStyle
    
    classDef cashStyle fill:#d1fae5,stroke:#10b981,stroke-width:2px,color:black
    class D1,D2,D3 cashStyle
    
    classDef installmentStyle fill:#fef3c7,stroke:#f59e0b,stroke-width:2px,color:black
    class E1,E2,E3,E4,E5,E6 installmentStyle
    
    classDef receivingStyle fill:#dbeafe,stroke:#3b82f6,stroke-width:2px,color:black
    class F,G,H,I,J,K,L,M,N,O,P1,P2,Q receivingStyle
    
    classDef overdueStyle fill:#fecaca,stroke:#ef4444,stroke-width:2px,color:black
    class R,S,T,U,V overdueStyle
```

## ðŸ’¸ Fluxo de Contas a Pagar

```mermaid
flowchart TD
    %% === ORIGEM: COMPRAS ===
    A[ðŸ“¦ PurchaseOrder totalmente<br/>recebida no DomÃ­nio Compras] --> B[ðŸ“Š Analisar PaymentTerms<br/>do fornecedor]
    
    B --> C[ðŸ“… Calcular DueDate<br/>= ActualDeliveryDate + PaymentTerms]
    C --> D[ðŸ’¾ Criar AccountPayable]
    D --> E[ðŸ’µ TotalAmount = PurchaseOrder.TotalValue<br/>AccountStatus: Pending]
    
    %% === PLANEJAMENTO DE PAGAMENTO ===
    E --> F[ðŸ“‹ Conta criada<br/>aguardando pagamento]
    F --> G[ðŸ“Š Dashboard de contas a pagar<br/>mostra vencimentos]
    
    G --> H[â° Vencimento se aproxima<br/>ou usuÃ¡rio decide pagar]
    H --> I[ðŸ‘¤ UsuÃ¡rio inicia<br/>processo de pagamento]
    
    %% === PROCESSO DE PAGAMENTO ===
    I --> J[ðŸ” Localizar AccountPayable<br/>pelo fornecedor/compra]
    J --> K[ðŸ’° Definir valor a pagar<br/>(total ou parcial)]
    K --> L[ðŸ’³ Selecionar mÃ©todo<br/>de pagamento]
    L --> M[ðŸ¦ Selecionar conta bancÃ¡ria<br/>de origem]
    
    M --> N{ðŸ’¯ Valor pago<br/>completamente?}
    
    N -->|Sim| O1[ðŸ’¾ Criar Transaction<br/>Amount = TotalAmount]
    N -->|NÃ£o| O2[ðŸ’¾ Criar Transaction<br/>Amount = ValorParcial]
    
    O1 --> P1[ðŸ“Š PaidAmount = TotalAmount<br/>RemainingAmount = 0]
    O2 --> P2[ðŸ“Š PaidAmount += ValorParcial<br/>RemainingAmount = Total - Paid]
    
    P1 --> Q1[âœ… AccountStatus: Paid]
    P2 --> Q2[âš ï¸ AccountStatus: PartiallyPaid]
    
    Q1 --> R[ðŸŽ‰ Pagamento realizado<br/>com sucesso]
    Q2 --> R
    
    %% === CONTROLE DE VENCIMENTOS ===
    F --> S[ðŸ“… Job automÃ¡tico verifica<br/>vencimentos diÃ¡rios]
    S --> T{â° Conta vencida<br/>e nÃ£o paga?}
    
    T -->|Sim| U[ðŸ“ˆ AccountStatus: Overdue]
    U --> V[ðŸ“§ Alertar responsÃ¡vel<br/>financeiro]
    V --> W[ðŸ“Š Aplicar multa/juros<br/>se contrato prevÃª]
    
    T -->|NÃ£o| R
    W --> R
    
    %% === STYLING ===
    
    classDef purchaseOriginStyle fill:#0562aa,stroke:#0562aa,stroke-width:2px,color:white
    class A purchaseOriginStyle
    
    classDef creationStyle fill:#fed7aa,stroke:#f97316,stroke-width:2px,color:black
    class B,C,D,E creationStyle
    
    classDef planningStyle fill:#fef3c7,stroke:#f59e0b,stroke-width:2px,color:black
    class F,G,H,I planningStyle
    
    classDef paymentStyle fill:#dbeafe,stroke:#3b82f6,stroke-width:2px,color:black
    class J,K,L,M,N,O1,O2,P1,P2,Q1,Q2,R paymentStyle
    
    classDef overdueStyle fill:#fecaca,stroke:#ef4444,stroke-width:2px,color:black
    class S,T,U,V,W overdueStyle
```

## ðŸ“Š Fluxo de AnÃ¡lise e Controle de Caixa

```mermaid
flowchart TD
    %% === ENTRADAS DE DADOS ===
    A[ðŸ’° TransaÃ§Ãµes de Recebimento] --> C[ðŸ“Š ConsolidaÃ§Ã£o<br/>Fluxo de Caixa]
    B[ðŸ’¸ TransaÃ§Ãµes de Pagamento] --> C
    
    %% === ANÃLISE REALIZADA ===
    C --> D[ðŸ“ˆ CÃ¡lculo do Fluxo Realizado]
    D --> E[ðŸ“‹ Por perÃ­odo:<br/>Entradas - SaÃ­das = Saldo]
    E --> F[ðŸ’¹ Gerar CASH_FLOW_VIEW<br/>com dados realizados]
    
    %% === PROJEÃ‡ÃƒO ===
    C --> G[ðŸ”® AnÃ¡lise de ProjeÃ§Ã£o]
    G --> H[ðŸ“… AccountReceivable<br/>Status: Pending/PartiallyPaid]
    G --> I[ðŸ“… AccountPayable<br/>Status: Pending/PartiallyPaid]
    
    H --> J[ðŸ’° Somar entradas projetadas<br/>por data de vencimento]
    I --> K[ðŸ’¸ Somar saÃ­das projetadas<br/>por data de vencimento]
    
    J --> L[ðŸ“Š ProjeÃ§Ã£o de Fluxo<br/>por perÃ­odo futuro]
    K --> L
    
    L --> M[ðŸ’¹ Atualizar CASH_FLOW_VIEW<br/>com projeÃ§Ãµes]
    
    %% === ALERTAS E DECISÃ•ES ===
    F --> N[ðŸŽ¯ AnÃ¡lise de Alertas]
    M --> N
    
    N --> O{âš ï¸ Saldo projetado<br/>negativo?}
    O -->|Sim| P[ðŸš¨ Alerta de caixa baixo]
    O -->|NÃ£o| Q[âœ… Fluxo saudÃ¡vel]
    
    P --> R[ðŸ“§ Notificar gestores<br/>financeiros]
    Q --> S[ðŸ“Š Dashboard atualizado]
    R --> S
    
    %% === RELATÃ“RIOS ===
    S --> T[ðŸ“ˆ Gerar relatÃ³rios<br/>de performance]
    T --> U[ðŸ’¼ AnÃ¡lise de lucratividade<br/>por domÃ­nio]
    U --> V[ðŸ“Š Dashboard executivo]
    
    %% === STYLING ===
    
    classDef inputStyle fill:#e5e7eb,stroke:#6b7280,stroke-width:2px,color:black
    class A,B inputStyle
    
    classDef consolidationStyle fill:#fed7aa,stroke:#f97316,stroke-width:2px,color:black
    class C,D,E,F consolidationStyle
    
    classDef projectionStyle fill:#8b5cf6,stroke:#7c3aed,stroke-width:2px,color:white
    class G,H,I,J,K,L,M projectionStyle
    
    classDef alertStyle fill:#fecaca,stroke:#ef4444,stroke-width:2px,color:black
    class N,O,P,R alertStyle
    
    classDef healthyStyle fill:#d1fae5,stroke:#10b981,stroke-width:2px,color:black
    class Q,S healthyStyle
    
    classDef reportStyle fill:#dbeafe,stroke:#3b82f6,stroke-width:2px,color:black
    class T,U,V reportStyle
```

## ðŸ”„ Processo de ConciliaÃ§Ã£o BancÃ¡ria

```mermaid
flowchart TD
    %% === IMPORT DE DADOS ===
    A[ðŸ¦ Extrato bancÃ¡rio<br/>importado/manual] --> B[ðŸ” Para cada movimentaÃ§Ã£o<br/>do extrato]
    
    B --> C[ðŸ“Š Identificar tipo<br/>CrÃ©dito ou DÃ©bito]
    C --> D[ðŸ’° Buscar Transaction<br/>correspondente no sistema]
    
    D --> E{ðŸ” Transaction<br/>encontrada?}
    
    %% CONCILIAÃ‡ÃƒO AUTOMÃTICA
    E -->|Sim| F[âœ… Marcar como conciliada<br/>IsReconciled = true]
    F --> G[ðŸ“… ReconciledDate = hoje<br/>ReconciledBy = usuÃ¡rio]
    
    %% MOVIMENTAÃ‡ÃƒO NÃƒO IDENTIFICADA
    E -->|NÃ£o| H[â“ MovimentaÃ§Ã£o<br/>nÃ£o identificada]
    H --> I[ðŸ‘¤ UsuÃ¡rio decide aÃ§Ã£o]
    I --> J{ðŸŽ¯ AÃ§Ã£o do usuÃ¡rio}
    
    J -->|Criar Transaction| K[ðŸ’¾ Criar Transaction manual<br/>com dados do extrato]
    J -->|Ignorar| L[â­ï¸ Pular movimentaÃ§Ã£o]
    J -->|Marcar pendente| M[â° Deixar para anÃ¡lise<br/>posterior]
    
    K --> N[âœ… Transaction criada<br/>e conciliada]
    
    %% CONTINUAÃ‡ÃƒO
    G --> O{ðŸ”„ Mais movimentaÃ§Ãµes<br/>no extrato?}
    L --> O
    M --> O
    N --> O
    
    O -->|Sim| B
    O -->|NÃ£o| P[ðŸ“Š RelatÃ³rio de<br/>conciliaÃ§Ã£o]
    
    P --> Q[âœ… ConciliaÃ§Ãµes realizadas]
    P --> R[â“ MovimentaÃ§Ãµes pendentes]
    P --> S[ðŸ’¹ Saldo conciliado]
    
    %% === STYLING ===
    
    classDef importStyle fill:#fed7aa,stroke:#f97316,stroke-width:2px,color:black
    class A,B,C,D importStyle
    
    classDef autoStyle fill:#d1fae5,stroke:#10b981,stroke-width:2px,color:black
    class E,F,G autoStyle
    
    classDef manualStyle fill:#fef3c7,stroke:#f59e0b,stroke-width:2px,color:black
    class H,I,J,K,L,M,N manualStyle
    
    classDef reportStyle fill:#dbeafe,stroke:#3b82f6,stroke-width:2px,color:black
    class O,P,Q,R,S reportStyle
```

## ðŸ“ˆ AnÃ¡lise de Lucratividade

### **ðŸ’¹ CÃ¡lculo de Lucratividade por Pedido:**

```mermaid
flowchart TD
    A[ðŸ“‹ OrderEntry finalizada] --> B[ðŸ’° Receita Total<br/>= OrderEntry.TotalValue]
    B --> C[ðŸ“Š Buscar custos associados]
    
    C --> D[ðŸ­ Custo de ProduÃ§Ã£o<br/>= Î£ Demand.ActualCost]
    C --> E[ðŸ›’ Custo de Ingredientes<br/>= Î£ IngredientConsumption]
    C --> F[ðŸ’¼ Custos Operacionais<br/>= Overhead alocado]
    
    D --> G[ðŸ§® Custo Total<br/>= ProduÃ§Ã£o + Ingredientes + Operacional]
    E --> G
    F --> G
    
    G --> H[ðŸ’µ Lucro Bruto<br/>= Receita - Custo Total]
    H --> I[ðŸ“Š Margem Bruta<br/>= (Lucro / Receita) * 100]
    
    I --> J[ðŸ“ˆ Salvar anÃ¡lise<br/>para relatÃ³rios]
    
    classDef revenueStyle fill:#d1fae5,stroke:#10b981,stroke-width:2px,color:black
    class A,B revenueStyle
    
    classDef costStyle fill:#fecaca,stroke:#ef4444,stroke-width:2px,color:black
    class C,D,E,F,G costStyle
    
    classDef profitStyle fill:#dbeafe,stroke:#3b82f6,stroke-width:2px,color:black
    class H,I,J profitStyle
```

## ðŸŽ¯ Estados e ValidaÃ§Ãµes

### **ðŸ“ˆ Ciclo de Status AccountReceivable:**

```mermaid
stateDiagram-v2
    [*] --> Pending : OrderEntry confirmada
    Pending --> PartiallyPaid : Pagamento parcial
    PartiallyPaid --> Paid : Pagamento total
    Pending --> Paid : Pagamento total direto
    
    Pending --> Overdue : Vencimento sem pagamento
    PartiallyPaid --> Overdue : Vencimento parcial
    Overdue --> Paid : Pagamento apÃ³s vencimento
    
    Pending --> Cancelled : Pedido cancelado
    PartiallyPaid --> Cancelled : Pedido cancelado
    Overdue --> Cancelled : Cancelamento por acordo
    
    Paid --> [*] : Processo finalizado
    Cancelled --> [*] : Processo cancelado
```

### **ðŸ“‰ Ciclo de Status AccountPayable:**

```mermaid
stateDiagram-v2
    [*] --> Pending : PurchaseOrder recebida
    Pending --> PartiallyPaid : Pagamento parcial
    PartiallyPaid --> Paid : Pagamento total
    Pending --> Paid : Pagamento total direto
    
    Pending --> Overdue : Vencimento sem pagamento
    PartiallyPaid --> Overdue : Vencimento parcial
    Overdue --> Paid : Pagamento apÃ³s vencimento
    
    Pending --> Cancelled : Compra cancelada
    PartiallyPaid --> Cancelled : Acordo de cancelamento
    
    Paid --> [*] : Processo finalizado
    Cancelled --> [*] : Processo cancelado
```

## ðŸš¨ Regras de ValidaÃ§Ã£o e Alertas

### **ValidaÃ§Ãµes CrÃ­ticas:**

#### **AccountReceivable:**
- âœ… DueDate â‰¥ OrderEntry.DeliveryDate
- âœ… TotalAmount = OrderEntry.TotalValue (soma de parcelas)
- âœ… InstallmentNumber Ãºnico por OrderEntry
- âœ… PaidAmount â‰¤ TotalAmount

#### **AccountPayable:**
- âœ… DueDate â‰¥ PurchaseOrder.ActualDeliveryDate
- âœ… TotalAmount = PurchaseOrder.TotalValue
- âœ… SupplierId = PurchaseOrder.SupplierId
- âœ… PaidAmount â‰¤ TotalAmount

#### **Transaction:**
- âœ… Deve referenciar AccountReceivable OU AccountPayable
- âœ… Amount > 0
- âœ… TransactionDate â‰¤ hoje
- âœ… BankAccount deve existir e estar ativa

### **Alertas AutomÃ¡ticos:**

#### **ðŸš¨ Alertas CrÃ­ticos:**
- **Fluxo Negativo**: Saldo projetado < 0 nos prÃ³ximos 30 dias
- **Alta InadimplÃªncia**: % contas vencidas > 15%
- **ConcentraÃ§Ã£o de Risco**: 1 cliente representa > 30% do AR
- **Descasamento**: MovimentaÃ§Ã£o bancÃ¡ria nÃ£o conciliada > 7 dias

#### **âš ï¸ Alertas de AtenÃ§Ã£o:**
- **Vencimento PrÃ³ximo**: Contas vencendo em 3 dias
- **Pagamento Atrasado**: Fornecedor com prazo vencido
- **Baixa Lucratividade**: Margem < 20% em pedidos
- **Crescimento AR**: Contas a receber crescendo > 50% mÃªs

## ðŸŽ¯ Eventos de DomÃ­nio Gerados

- **AccountReceivableCreated**: Nova conta a receber gerada
- **PaymentReceived**: Pagamento de cliente recebido
- **PaymentMade**: Pagamento a fornecedor efetuado
- **AccountOverdue**: Conta vencida detectada
- **CashFlowAlert**: Alerta de fluxo baixo
- **ProfitabilityCalculated**: Lucratividade calculada
- **BankReconciled**: ConciliaÃ§Ã£o bancÃ¡ria realizada

## ðŸ“Š MÃ©tricas e KPIs

### **Indicadores de Recebimento:**
- **DSO (Days Sales Outstanding)**: Prazo mÃ©dio de recebimento
- **Taxa de InadimplÃªncia**: % valor vencido vs total AR
- **EficiÃªncia de CobranÃ§a**: % contas pagas no prazo

### **Indicadores de Pagamento:**
- **DPO (Days Payable Outstanding)**: Prazo mÃ©dio de pagamento
- **Desconto Obtido**: % economia em pagamentos antecipados
- **Pontualidade**: % pagamentos realizados no prazo

### **Indicadores de Lucratividade:**
- **Margem Bruta**: (Receita - Custo) / Receita
- **ROI por Pedido**: Retorno sobre investimento
- **ContribuiÃ§Ã£o por DomÃ­nio**: Receita lÃ­quida por Ã¡rea

---

**Arquivo**: `05-financial-flow.md`  
**DomÃ­nio**: Financeiro (#083e61)  
**Tipo**: Process Flowchart  
**Foco**: Contas a Receber vs Pagar + AnÃ¡lise de Lucratividade
