# 💳 ERD - DOMÍNIO FINANCEIRO

## 🎯 Visão Geral
Diagrama Entity-Relationship completo do Domínio Financeiro, mostrando o controle de fluxo de caixa através de contas a receber (originadas das vendas) e contas a pagar (originadas das compras), com rastreamento detalhado de transações e análise de lucratividade.

## 🗄️ Diagrama de Entidades e Relacionamentos

```mermaid
erDiagram
    %% === DOMÍNIO FINANCEIRO ===
    
    %% === CONTA A RECEBER ===
    ACCOUNT_RECEIVABLE {
        string Id PK "GUID único"
        string OrderEntryId FK "Pedido origem"
        string CustomerId FK "Cliente"
        string ReceivableNumber "Número sequencial"
        decimal TotalAmount "Valor total a receber"
        decimal PaidAmount "Valor já recebido"
        decimal RemainingAmount "Valor restante"
        datetime DueDate "Data vencimento"
        datetime IssueDate "Data emissão"
        string AccountStatus "Pending|PartiallyPaid|Paid|Overdue|Cancelled"
        string PaymentTerms "Condições pagamento"
        string PaymentMethod "Método pagamento"
        int InstallmentNumber "Número da parcela"
        int TotalInstallments "Total de parcelas"
        decimal InterestRate "Taxa de juros"
        decimal DiscountRate "Taxa de desconto"
        decimal FineAmount "Valor de multa"
        string Description "Descrição da conta"
        string Notes "Observações"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criação"
        datetime ModifiedDate "Data de modificação"
        string CreatedBy "Usuário criador"
    }

    %% === CONTA A PAGAR ===
    ACCOUNT_PAYABLE {
        string Id PK "GUID único"
        string PurchaseOrderId FK "Ordem compra origem"
        string SupplierId FK "Fornecedor"
        string PayableNumber "Número sequencial"
        decimal TotalAmount "Valor total a pagar"
        decimal PaidAmount "Valor já pago"
        decimal RemainingAmount "Valor restante"
        datetime DueDate "Data vencimento"
        datetime IssueDate "Data emissão"
        string AccountStatus "Pending|PartiallyPaid|Paid|Overdue|Cancelled"
        string PaymentTerms "Condições pagamento"
        string PaymentMethod "Método pagamento"
        int InstallmentNumber "Número da parcela"
        int TotalInstallments "Total de parcelas"
        decimal InterestRate "Taxa de juros"
        decimal DiscountRate "Taxa de desconto"
        decimal FineAmount "Valor de multa"
        string Description "Descrição da conta"
        string Notes "Observações"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criação"
        datetime ModifiedDate "Data de modificação"
        string CreatedBy "Usuário criador"
    }

    %% === TRANSAÇÃO FINANCEIRA ===
    TRANSACTION {
        string Id PK "GUID único"
        string TransactionNumber "Número sequencial"
        string AccountReceivableId FK "Conta receber (opcional)"
        string AccountPayableId FK "Conta pagar (opcional)"
        string BankAccountId FK "Conta bancária"
        decimal Amount "Valor da transação"
        datetime TransactionDate "Data da transação"
        string TransactionType "Credit|Debit"
        string Category "Receita|Despesa|Transferencia"
        string SubCategory "Subcategoria"
        string PaymentMethod "Dinheiro|Cartão|PIX|Boleto|Transferência"
        string Description "Descrição da transação"
        string ReferenceNumber "Número referência"
        bool IsReconciled "Conciliado?"
        datetime ReconciledDate "Data conciliação"
        string ReconciledBy "Conciliado por"
        decimal ExchangeRate "Taxa de câmbio"
        string Currency "Moeda"
        string Notes "Observações"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criação"
        datetime ModifiedDate "Data de modificação"
        string CreatedBy "Usuário criador"
    }

    %% === CONTA BANCÁRIA ===
    BANK_ACCOUNT {
        string Id PK "GUID único"
        string AccountName "Nome da conta"
        string BankName "Nome do banco"
        string AccountNumber "Número da conta"
        string Agency "Agência"
        string AccountType "Corrente|Poupança|Aplicação"
        decimal CurrentBalance "Saldo atual"
        decimal AvailableBalance "Saldo disponível"
        string Currency "Moeda"
        bool IsDefault "Conta padrão?"
        string Notes "Observações"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criação"
        datetime ModifiedDate "Data de modificação"
    }

    %% === MÉTODO DE PAGAMENTO ===
    PAYMENT_METHOD {
        string Id PK "GUID único"
        string Name "Nome do método"
        string PaymentType "Cash|Card|Transfer|Check|PIX|Boleto"
        bool IsActive "Ativo?"
        decimal Fee "Taxa/Tarifa"
        int DaysToReceive "Dias para receber"
        string Description "Descrição"
        string Notes "Observações"
        datetime CreatedDate "Data de criação"
        datetime ModifiedDate "Data de modificação"
    }

    %% === CATEGORIA FINANCEIRA ===
    FINANCIAL_CATEGORY {
        string Id PK "GUID único"
        string Name "Nome da categoria"
        string CategoryType "Income|Expense|Transfer"
        string ParentCategoryId FK "Categoria pai"
        string Description "Descrição"
        bool IsActive "Ativa?"
        string Notes "Observações"
        datetime CreatedDate "Data de criação"
        datetime ModifiedDate "Data de modificação"
    }

    %% === FLUXO DE CAIXA (VISÃO) ===
    CASH_FLOW_VIEW {
        string Id PK "GUID único"
        datetime PeriodDate "Data do período"
        string PeriodType "Daily|Weekly|Monthly|Yearly"
        decimal OpeningBalance "Saldo inicial"
        decimal TotalIncome "Total receitas"
        decimal TotalExpense "Total despesas"
        decimal NetFlow "Fluxo líquido"
        decimal ClosingBalance "Saldo final"
        decimal ProjectedIncome "Receita projetada"
        decimal ProjectedExpense "Despesa projetada"
        decimal ProjectedBalance "Saldo projetado"
        datetime GeneratedDate "Data de geração"
        string GeneratedBy "Gerado por"
    }

    %% === INTEGRAÇÕES COM OUTROS DOMÍNIOS ===

    %% VENDAS (ORIGEM DAS CONTAS A RECEBER)
    ORDER_ENTRY {
        string Id PK "GUID único"
        string CustomerId FK "Cliente"
        decimal TotalValue "Valor total"
        string OrderStatus "Status do pedido"
        string PaymentTerms "Condições pagamento"
        datetime DeliveryDate "Data entrega"
    }

    CUSTOMER {
        string Id PK "GUID único"
        string Name "Nome cliente"
        string Document "CPF/CNPJ"
        string Email "Email"
        string Phone "Telefone"
    }

    %% COMPRAS (ORIGEM DAS CONTAS A PAGAR)
    PURCHASE_ORDER {
        string Id PK "GUID único"
        string SupplierId FK "Fornecedor"
        decimal TotalValue "Valor total"
        string PurchaseStatus "Status da compra"
        string PaymentTerms "Condições pagamento"
        datetime ActualDeliveryDate "Data entrega"
    }

    SUPPLIER {
        string Id PK "GUID único"
        string Name "Nome fornecedor"
        string Document "CNPJ"
        string Email "Email"
        string PaymentTerms "Condições padrão"
    }

    %% ==========================================
    %% RELACIONAMENTOS PRINCIPAIS
    %% ==========================================

    %% CONTAS E TRANSAÇÕES
    ACCOUNT_RECEIVABLE ||--o{ TRANSACTION : "possui recebimentos"
    ACCOUNT_PAYABLE ||--o{ TRANSACTION : "possui pagamentos"
    BANK_ACCOUNT ||--o{ TRANSACTION : "movimenta conta"
    PAYMENT_METHOD ||--o{ TRANSACTION : "utiliza método"
    FINANCIAL_CATEGORY ||--o{ TRANSACTION : "categoriza"

    %% HIERARQUIA DE CATEGORIAS
    FINANCIAL_CATEGORY ||--o{ FINANCIAL_CATEGORY : "categoria pai"

    %% ==========================================
    %% INTEGRAÇÕES COM OUTROS DOMÍNIOS
    %% ==========================================

    %% VENDAS → FINANCEIRO (Customer-Supplier)
    ORDER_ENTRY ||--o{ ACCOUNT_RECEIVABLE : "gera contas a receber"
    CUSTOMER ||--o{ ACCOUNT_RECEIVABLE : "deve pagar"

    %% COMPRAS → FINANCEIRO (Customer-Supplier)
    PURCHASE_ORDER ||--o{ ACCOUNT_PAYABLE : "gera contas a pagar"
    SUPPLIER ||--o{ ACCOUNT_PAYABLE : "deve receber"

    %% ==========================================
    %% STYLING POR DOMÍNIO
    %% ==========================================
    
    %% FINANCEIRO = Azul Escuro (#083e61)
    ACCOUNT_RECEIVABLE {
        background-color "#083e61"
        color "white"
        border-color "#083e61"
    }
    
    ACCOUNT_PAYABLE {
        background-color "#083e61"
        color "white"
        border-color "#083e61"
    }
    
    TRANSACTION {
        background-color "#083e61"
        color "white"
        border-color "#083e61"
    }
    
    BANK_ACCOUNT {
        background-color "#083e61"
        color "white"
        border-color "#083e61"
    }
    
    PAYMENT_METHOD {
        background-color "#083e61"
        color "white"
        border-color "#083e61"
    }
    
    FINANCIAL_CATEGORY {
        background-color "#083e61"
        color "white"
        border-color "#083e61"
    }
    
    CASH_FLOW_VIEW {
        background-color "#083e61"
        color "white"
        border-color "#083e61"
    }

    %% VENDAS = Laranja (#f36b21)
    ORDER_ENTRY {
        background-color "#f36b21"
        color "white"
        border-color "#f36b21"
    }
    
    CUSTOMER {
        background-color "#f36b21"
        color "white"
        border-color "#f36b21"
    }

    %% COMPRAS = Azul (#0562aa)
    PURCHASE_ORDER {
        background-color "#0562aa"
        color "white"
        border-color "#0562aa"
    }
    
    SUPPLIER {
        background-color "#0562aa"
        color "white"
        border-color "#0562aa"
    }
```

## 📋 Detalhes das Entidades

### **💰 ACCOUNT_RECEIVABLE (Contas a Receber)**
- **Propósito**: Controlar valores que a empresa tem direito de receber de clientes
- **Origem**: Gerada automaticamente quando OrderEntry é confirmada
- **Status Flow**: Pending → PartiallyPaid → Paid (ou Overdue se vencer)
- **Características**: Parcelamento, juros, multas, descontos

### **💸 ACCOUNT_PAYABLE (Contas a Pagar)**
- **Propósito**: Controlar obrigações financeiras com fornecedores
- **Origem**: Gerada automaticamente quando PurchaseOrder é recebida totalmente
- **Status Flow**: Pending → PartiallyPaid → Paid (ou Overdue se vencer)
- **Características**: Parcelamento, juros, multas, descontos

### **🔄 TRANSACTION (Transação Financeira)**
- **Propósito**: Registrar movimentações de dinheiro (entradas e saídas)
- **Tipos**: Credit (entrada) ou Debit (saída)
- **Relacionamentos**: Vinculada a AccountReceivable OU AccountPayable
- **Características**: Conciliação bancária, categorização, métodos de pagamento

### **🏦 BANK_ACCOUNT (Conta Bancária)**
- **Propósito**: Controlar contas bancárias da empresa
- **Características**: Saldo atual/disponível, tipo de conta, moeda
- **Integração**: Todas Transaction devem ter uma BankAccount

### **💳 PAYMENT_METHOD (Método de Pagamento)**
- **Propósito**: Definir formas de pagamento aceitas/utilizadas
- **Características**: Taxas, prazos para recebimento, tipo
- **Exemplos**: Dinheiro, PIX, Cartão, Boleto, Transferência

### **📊 FINANCIAL_CATEGORY (Categoria Financeira)**
- **Propósito**: Categorizar receitas e despesas para relatórios
- **Estrutura**: Hierárquica (categoria pai/filha)
- **Tipos**: Income (receita), Expense (despesa), Transfer (transferência)

### **📈 CASH_FLOW_VIEW (Visão de Fluxo de Caixa)**
- **Propósito**: Visão consolidada do fluxo de caixa por período
- **Características**: Saldos inicial/final, projeções, análises
- **Períodos**: Diário, semanal, mensal, anual

## 🔄 Fluxos de Integração Automática

### **💰 Vendas → Contas a Receber**

#### **Geração Automática**
```
Quando OrderEntry.OrderStatus = "Confirmed":

1. Sistema analisa PaymentTerms:
   - À vista: 1 AccountReceivable
   - Parcelado: N AccountReceivable (uma por parcela)

2. Para cada parcela:
   AccountReceivable {
     OrderEntryId: OrderEntry.Id,
     CustomerId: OrderEntry.CustomerId,
     TotalAmount: OrderEntry.TotalValue / TotalInstallments,
     DueDate: OrderEntry.DeliveryDate + (30 * InstallmentNumber),
     AccountStatus: "Pending",
     InstallmentNumber: N,
     TotalInstallments: X
   }
```

#### **Exemplo Prático**
```
OrderEntry: R$ 1.000,00 - Pagamento em 3x

Gera 3 AccountReceivable:
├── Parcela 1: R$ 333,33 - Vencimento: 30 dias
├── Parcela 2: R$ 333,33 - Vencimento: 60 dias  
└── Parcela 3: R$ 333,34 - Vencimento: 90 dias
```

### **💸 Compras → Contas a Pagar**

#### **Geração Automática**
```
Quando PurchaseOrder.PurchaseStatus = "FullyReceived":

1. Sistema cria AccountPayable:
   AccountPayable {
     PurchaseOrderId: PurchaseOrder.Id,
     SupplierId: PurchaseOrder.SupplierId,
     TotalAmount: PurchaseOrder.TotalValue,
     DueDate: PurchaseOrder.ActualDeliveryDate + Supplier.PaymentTerms,
     AccountStatus: "Pending"
   }
```

### **🔄 Registro de Transações**

#### **Recebimento de Cliente**
```
Usuário registra recebimento:

1. Localiza AccountReceivable
2. Cria Transaction:
   Transaction {
     AccountReceivableId: Account.Id,
     Amount: ValorRecebido,
     TransactionType: "Credit",
     TransactionDate: DataRecebimento,
     PaymentMethod: MetodoEscolhido,
     BankAccountId: ContaDestino
   }

3. Atualiza AccountReceivable:
   PaidAmount += ValorRecebido
   RemainingAmount = TotalAmount - PaidAmount
   
   Se RemainingAmount = 0:
     AccountStatus = "Paid"
   Senão:
     AccountStatus = "PartiallyPaid"
```

#### **Pagamento a Fornecedor**
```
Usuário registra pagamento:

1. Localiza AccountPayable
2. Cria Transaction:
   Transaction {
     AccountPayableId: Account.Id,
     Amount: ValorPago,
     TransactionType: "Debit",
     TransactionDate: DataPagamento,
     PaymentMethod: MetodoEscolhido,
     BankAccountId: ContaOrigem
   }

3. Atualiza AccountPayable:
   PaidAmount += ValorPago
   RemainingAmount = TotalAmount - PaidAmount
   AccountStatus = "Paid"
```

## 📊 Análises e Relatórios Financeiros

### **💹 Fluxo de Caixa Realizado**
```sql
SELECT 
    t.TransactionDate,
    SUM(CASE WHEN t.TransactionType = 'Credit' THEN t.Amount ELSE 0 END) AS Entradas,
    SUM(CASE WHEN t.TransactionType = 'Debit' THEN t.Amount ELSE 0 END) AS Saidas,
    SUM(CASE WHEN t.TransactionType = 'Credit' THEN t.Amount ELSE -t.Amount END) AS SaldoDiario
FROM TRANSACTION t
WHERE t.TransactionDate BETWEEN @DataInicio AND @DataFim
GROUP BY t.TransactionDate
ORDER BY t.TransactionDate
```

### **📈 Fluxo de Caixa Projetado**
```sql
-- Entradas Projetadas (Contas a Receber)
SELECT 
    ar.DueDate,
    SUM(ar.RemainingAmount) AS EntradasProjetadas
FROM ACCOUNT_RECEIVABLE ar
WHERE ar.AccountStatus IN ('Pending', 'PartiallyPaid')
  AND ar.DueDate BETWEEN @DataInicio AND @DataFim
GROUP BY ar.DueDate

-- Saídas Projetadas (Contas a Pagar)  
SELECT 
    ap.DueDate,
    SUM(ap.RemainingAmount) AS SaidasProjetadas
FROM ACCOUNT_PAYABLE ap
WHERE ap.AccountStatus IN ('Pending', 'PartiallyPaid')
  AND ap.DueDate BETWEEN @DataInicio AND @DataFim
GROUP BY ap.DueDate
```

### **🎯 Indicadores de Performance**

#### **Receitas por Domínio**
```sql
-- Receitas de Vendas
SELECT 'Vendas' AS Origem, SUM(ar.TotalAmount) AS Receita
FROM ACCOUNT_RECEIVABLE ar
WHERE ar.AccountStatus = 'Paid'
  AND ar.CreatedDate BETWEEN @DataInicio AND @DataFim

-- Custos de Compras
SELECT 'Compras' AS Origem, SUM(ap.TotalAmount) AS Custo
FROM ACCOUNT_PAYABLE ap  
WHERE ap.AccountStatus = 'Paid'
  AND ap.CreatedDate BETWEEN @DataInicio AND @DataFim
```

#### **Análise de Inadimplência**
```sql
SELECT 
    c.Name AS Cliente,
    COUNT(ar.Id) AS ContasVencidas,
    SUM(ar.RemainingAmount) AS ValorVencido,
    AVG(DATEDIFF(day, ar.DueDate, GETDATE())) AS MediaDiasAtraso
FROM ACCOUNT_RECEIVABLE ar
JOIN CUSTOMER c ON ar.CustomerId = c.Id
WHERE ar.AccountStatus = 'Overdue'
GROUP BY c.Id, c.Name
ORDER BY ValorVencido DESC
```

## 🎯 Eventos de Domínio Gerados

- **AccountReceivableCreated**: Nova conta a receber gerada
- **PaymentReceived**: Pagamento de cliente recebido
- **PaymentMade**: Pagamento a fornecedor efetuado
- **AccountOverdue**: Conta vencida detectada
- **CashFlowAlert**: Alerta de fluxo de caixa baixo
- **BankAccountReconciled**: Conciliação bancária realizada
- **ProfitabilityCalculated**: Lucratividade recalculada

## 🚨 Alertas e Validações

### **Alertas Críticos**
- **Contas Vencidas**: DueDate < hoje E AccountStatus ≠ Paid
- **Fluxo de Caixa Baixo**: Saldo projetado < limite configurado
- **Inadimplência Alta**: % contas vencidas > limite aceitável
- **Descasamento**: Transaction sem conciliação > 7 dias

### **Validações de Negócio**
- Transaction deve ter AccountReceivable OU AccountPayable
- Valor de Transaction não pode exceder saldo da conta
- AccountReceivable não pode ser paga além do valor total
- Data de Transaction não pode ser futura (exceto projeções)
- BankAccount deve ter saldo suficiente para débitos

---

**Arquivo**: `05-financial-domain-erd.md`  
**Domínio**: Financeiro (#083e61)  
**Tipo**: Entity-Relationship Diagram  
**Nível**: Detalhado + Fluxos Automáticos + Análises
