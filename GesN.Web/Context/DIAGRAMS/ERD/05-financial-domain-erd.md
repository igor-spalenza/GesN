# ðŸ’³ ERD - DOMÃNIO FINANCEIRO

## ðŸŽ¯ VisÃ£o Geral
Diagrama Entity-Relationship completo do DomÃ­nio Financeiro, mostrando o controle de fluxo de caixa atravÃ©s de contas a receber (originadas das vendas) e contas a pagar (originadas das compras), com rastreamento detalhado de transaÃ§Ãµes e anÃ¡lise de lucratividade.

## ðŸ—„ï¸ Diagrama de Entidades e Relacionamentos

```mermaid
erDiagram
    %% === DOMÃNIO FINANCEIRO ===
    
    %% === CONTA A RECEBER ===
    ACCOUNT_RECEIVABLE {
        string Id PK "GUID Ãºnico"
        string OrderEntryId FK "Pedido origem"
        string CustomerId FK "Cliente"
        string ReceivableNumber "NÃºmero sequencial"
        decimal TotalAmount "Valor total a receber"
        decimal PaidAmount "Valor jÃ¡ recebido"
        decimal RemainingAmount "Valor restante"
        datetime DueDate "Data vencimento"
        datetime IssueDate "Data emissÃ£o"
        string AccountStatus "Pending|PartiallyPaid|Paid|Overdue|Cancelled"
        string PaymentTerms "CondiÃ§Ãµes pagamento"
        string PaymentMethod "MÃ©todo pagamento"
        int InstallmentNumber "NÃºmero da parcela"
        int TotalInstallments "Total de parcelas"
        decimal InterestRate "Taxa de juros"
        decimal DiscountRate "Taxa de desconto"
        decimal FineAmount "Valor de multa"
        string Description "DescriÃ§Ã£o da conta"
        string Notes "ObservaÃ§Ãµes"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criaÃ§Ã£o"
        datetime ModifiedDate "Data de modificaÃ§Ã£o"
        string CreatedBy "UsuÃ¡rio criador"
    }

    %% === CONTA A PAGAR ===
    ACCOUNT_PAYABLE {
        string Id PK "GUID Ãºnico"
        string PurchaseOrderId FK "Ordem compra origem"
        string SupplierId FK "Fornecedor"
        string PayableNumber "NÃºmero sequencial"
        decimal TotalAmount "Valor total a pagar"
        decimal PaidAmount "Valor jÃ¡ pago"
        decimal RemainingAmount "Valor restante"
        datetime DueDate "Data vencimento"
        datetime IssueDate "Data emissÃ£o"
        string AccountStatus "Pending|PartiallyPaid|Paid|Overdue|Cancelled"
        string PaymentTerms "CondiÃ§Ãµes pagamento"
        string PaymentMethod "MÃ©todo pagamento"
        int InstallmentNumber "NÃºmero da parcela"
        int TotalInstallments "Total de parcelas"
        decimal InterestRate "Taxa de juros"
        decimal DiscountRate "Taxa de desconto"
        decimal FineAmount "Valor de multa"
        string Description "DescriÃ§Ã£o da conta"
        string Notes "ObservaÃ§Ãµes"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criaÃ§Ã£o"
        datetime ModifiedDate "Data de modificaÃ§Ã£o"
        string CreatedBy "UsuÃ¡rio criador"
    }

    %% === TRANSAÃ‡ÃƒO FINANCEIRA ===
    TRANSACTION {
        string Id PK "GUID Ãºnico"
        string TransactionNumber "NÃºmero sequencial"
        string AccountReceivableId FK "Conta receber (opcional)"
        string AccountPayableId FK "Conta pagar (opcional)"
        string BankAccountId FK "Conta bancÃ¡ria"
        decimal Amount "Valor da transaÃ§Ã£o"
        datetime TransactionDate "Data da transaÃ§Ã£o"
        string TransactionType "Credit|Debit"
        string Category "Receita|Despesa|Transferencia"
        string SubCategory "Subcategoria"
        string PaymentMethod "Dinheiro|CartÃ£o|PIX|Boleto|TransferÃªncia"
        string Description "DescriÃ§Ã£o da transaÃ§Ã£o"
        string ReferenceNumber "NÃºmero referÃªncia"
        bool IsReconciled "Conciliado?"
        datetime ReconciledDate "Data conciliaÃ§Ã£o"
        string ReconciledBy "Conciliado por"
        decimal ExchangeRate "Taxa de cÃ¢mbio"
        string Currency "Moeda"
        string Notes "ObservaÃ§Ãµes"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criaÃ§Ã£o"
        datetime ModifiedDate "Data de modificaÃ§Ã£o"
        string CreatedBy "UsuÃ¡rio criador"
    }

    %% === CONTA BANCÃRIA ===
    BANK_ACCOUNT {
        string Id PK "GUID Ãºnico"
        string AccountName "Nome da conta"
        string BankName "Nome do banco"
        string AccountNumber "NÃºmero da conta"
        string Agency "AgÃªncia"
        string AccountType "Corrente|PoupanÃ§a|AplicaÃ§Ã£o"
        decimal CurrentBalance "Saldo atual"
        decimal AvailableBalance "Saldo disponÃ­vel"
        string Currency "Moeda"
        bool IsDefault "Conta padrÃ£o?"
        string Notes "ObservaÃ§Ãµes"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criaÃ§Ã£o"
        datetime ModifiedDate "Data de modificaÃ§Ã£o"
    }

    %% === MÃ‰TODO DE PAGAMENTO ===
    PAYMENT_METHOD {
        string Id PK "GUID Ãºnico"
        string Name "Nome do mÃ©todo"
        string PaymentType "Cash|Card|Transfer|Check|PIX|Boleto"
        bool IsActive "Ativo?"
        decimal Fee "Taxa/Tarifa"
        int DaysToReceive "Dias para receber"
        string Description "DescriÃ§Ã£o"
        string Notes "ObservaÃ§Ãµes"
        datetime CreatedDate "Data de criaÃ§Ã£o"
        datetime ModifiedDate "Data de modificaÃ§Ã£o"
    }

    %% === CATEGORIA FINANCEIRA ===
    FINANCIAL_CATEGORY {
        string Id PK "GUID Ãºnico"
        string Name "Nome da categoria"
        string CategoryType "Income|Expense|Transfer"
        string ParentCategoryId FK "Categoria pai"
        string Description "DescriÃ§Ã£o"
        bool IsActive "Ativa?"
        string Notes "ObservaÃ§Ãµes"
        datetime CreatedDate "Data de criaÃ§Ã£o"
        datetime ModifiedDate "Data de modificaÃ§Ã£o"
    }

    %% === FLUXO DE CAIXA (VISÃƒO) ===
    CASH_FLOW_VIEW {
        string Id PK "GUID Ãºnico"
        datetime PeriodDate "Data do perÃ­odo"
        string PeriodType "Daily|Weekly|Monthly|Yearly"
        decimal OpeningBalance "Saldo inicial"
        decimal TotalIncome "Total receitas"
        decimal TotalExpense "Total despesas"
        decimal NetFlow "Fluxo lÃ­quido"
        decimal ClosingBalance "Saldo final"
        decimal ProjectedIncome "Receita projetada"
        decimal ProjectedExpense "Despesa projetada"
        decimal ProjectedBalance "Saldo projetado"
        datetime GeneratedDate "Data de geraÃ§Ã£o"
        string GeneratedBy "Gerado por"
    }

    %% === INTEGRAÃ‡Ã•ES COM OUTROS DOMÃNIOS ===

    %% VENDAS (ORIGEM DAS CONTAS A RECEBER)
    ORDER_ENTRY {
        string Id PK "GUID Ãºnico"
        string CustomerId FK "Cliente"
        decimal TotalValue "Valor total"
        string OrderStatus "Status do pedido"
        string PaymentTerms "CondiÃ§Ãµes pagamento"
        datetime DeliveryDate "Data entrega"
    }

    CUSTOMER {
        string Id PK "GUID Ãºnico"
        string Name "Nome cliente"
        string Document "CPF/CNPJ"
        string Email "Email"
        string Phone "Telefone"
    }

    %% COMPRAS (ORIGEM DAS CONTAS A PAGAR)
    PURCHASE_ORDER {
        string Id PK "GUID Ãºnico"
        string SupplierId FK "Fornecedor"
        decimal TotalValue "Valor total"
        string PurchaseStatus "Status da compra"
        string PaymentTerms "CondiÃ§Ãµes pagamento"
        datetime ActualDeliveryDate "Data entrega"
    }

    SUPPLIER {
        string Id PK "GUID Ãºnico"
        string Name "Nome fornecedor"
        string Document "CNPJ"
        string Email "Email"
        string PaymentTerms "CondiÃ§Ãµes padrÃ£o"
    }

    %% ==========================================
    %% RELACIONAMENTOS PRINCIPAIS
    %% ==========================================

    %% CONTAS E TRANSAÃ‡Ã•ES
    ACCOUNT_RECEIVABLE ||--o{ TRANSACTION : "possui recebimentos"
    ACCOUNT_PAYABLE ||--o{ TRANSACTION : "possui pagamentos"
    BANK_ACCOUNT ||--o{ TRANSACTION : "movimenta conta"
    PAYMENT_METHOD ||--o{ TRANSACTION : "utiliza mÃ©todo"
    FINANCIAL_CATEGORY ||--o{ TRANSACTION : "categoriza"

    %% HIERARQUIA DE CATEGORIAS
    FINANCIAL_CATEGORY ||--o{ FINANCIAL_CATEGORY : "categoria pai"

    %% ==========================================
    %% INTEGRAÃ‡Ã•ES COM OUTROS DOMÃNIOS
    %% ==========================================

    %% VENDAS â†’ FINANCEIRO (Customer-Supplier)
    ORDER_ENTRY ||--o{ ACCOUNT_RECEIVABLE : "gera contas a receber"
    CUSTOMER ||--o{ ACCOUNT_RECEIVABLE : "deve pagar"

    %% COMPRAS â†’ FINANCEIRO (Customer-Supplier)
    PURCHASE_ORDER ||--o{ ACCOUNT_PAYABLE : "gera contas a pagar"
    SUPPLIER ||--o{ ACCOUNT_PAYABLE : "deve receber"

    %% ==========================================
    %% STYLING POR DOMÃNIO
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

## ðŸ“‹ Detalhes das Entidades

### **ðŸ’° ACCOUNT_RECEIVABLE (Contas a Receber)**
- **PropÃ³sito**: Controlar valores que a empresa tem direito de receber de clientes
- **Origem**: Gerada automaticamente quando OrderEntry Ã© confirmada
- **Status Flow**: Pending â†’ PartiallyPaid â†’ Paid (ou Overdue se vencer)
- **CaracterÃ­sticas**: Parcelamento, juros, multas, descontos

### **ðŸ’¸ ACCOUNT_PAYABLE (Contas a Pagar)**
- **PropÃ³sito**: Controlar obrigaÃ§Ãµes financeiras com fornecedores
- **Origem**: Gerada automaticamente quando PurchaseOrder Ã© recebida totalmente
- **Status Flow**: Pending â†’ PartiallyPaid â†’ Paid (ou Overdue se vencer)
- **CaracterÃ­sticas**: Parcelamento, juros, multas, descontos

### **ðŸ”„ TRANSACTION (TransaÃ§Ã£o Financeira)**
- **PropÃ³sito**: Registrar movimentaÃ§Ãµes de dinheiro (entradas e saÃ­das)
- **Tipos**: Credit (entrada) ou Debit (saÃ­da)
- **Relacionamentos**: Vinculada a AccountReceivable OU AccountPayable
- **CaracterÃ­sticas**: ConciliaÃ§Ã£o bancÃ¡ria, categorizaÃ§Ã£o, mÃ©todos de pagamento

### **ðŸ¦ BANK_ACCOUNT (Conta BancÃ¡ria)**
- **PropÃ³sito**: Controlar contas bancÃ¡rias da empresa
- **CaracterÃ­sticas**: Saldo atual/disponÃ­vel, tipo de conta, moeda
- **IntegraÃ§Ã£o**: Todas Transaction devem ter uma BankAccount

### **ðŸ’³ PAYMENT_METHOD (MÃ©todo de Pagamento)**
- **PropÃ³sito**: Definir formas de pagamento aceitas/utilizadas
- **CaracterÃ­sticas**: Taxas, prazos para recebimento, tipo
- **Exemplos**: Dinheiro, PIX, CartÃ£o, Boleto, TransferÃªncia

### **ðŸ“Š FINANCIAL_CATEGORY (Categoria Financeira)**
- **PropÃ³sito**: Categorizar receitas e despesas para relatÃ³rios
- **Estrutura**: HierÃ¡rquica (categoria pai/filha)
- **Tipos**: Income (receita), Expense (despesa), Transfer (transferÃªncia)

### **ðŸ“ˆ CASH_FLOW_VIEW (VisÃ£o de Fluxo de Caixa)**
- **PropÃ³sito**: VisÃ£o consolidada do fluxo de caixa por perÃ­odo
- **CaracterÃ­sticas**: Saldos inicial/final, projeÃ§Ãµes, anÃ¡lises
- **PerÃ­odos**: DiÃ¡rio, semanal, mensal, anual

## ðŸ”„ Fluxos de IntegraÃ§Ã£o AutomÃ¡tica

### **ðŸ’° Vendas â†’ Contas a Receber**

#### **GeraÃ§Ã£o AutomÃ¡tica**
```
Quando OrderEntry.OrderStatus = "Confirmed":

1. Sistema analisa PaymentTerms:
   - Ã€ vista: 1 AccountReceivable
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

#### **Exemplo PrÃ¡tico**
```
OrderEntry: R$ 1.000,00 - Pagamento em 3x

Gera 3 AccountReceivable:
â”œâ”€â”€ Parcela 1: R$ 333,33 - Vencimento: 30 dias
â”œâ”€â”€ Parcela 2: R$ 333,33 - Vencimento: 60 dias  
â””â”€â”€ Parcela 3: R$ 333,34 - Vencimento: 90 dias
```

### **ðŸ’¸ Compras â†’ Contas a Pagar**

#### **GeraÃ§Ã£o AutomÃ¡tica**
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

### **ðŸ”„ Registro de TransaÃ§Ãµes**

#### **Recebimento de Cliente**
```
UsuÃ¡rio registra recebimento:

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
   SenÃ£o:
     AccountStatus = "PartiallyPaid"
```

#### **Pagamento a Fornecedor**
```
UsuÃ¡rio registra pagamento:

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

## ðŸ“Š AnÃ¡lises e RelatÃ³rios Financeiros

### **ðŸ’¹ Fluxo de Caixa Realizado**
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

### **ðŸ“ˆ Fluxo de Caixa Projetado**
```sql
-- Entradas Projetadas (Contas a Receber)
SELECT 
    ar.DueDate,
    SUM(ar.RemainingAmount) AS EntradasProjetadas
FROM ACCOUNT_RECEIVABLE ar
WHERE ar.AccountStatus IN ('Pending', 'PartiallyPaid')
  AND ar.DueDate BETWEEN @DataInicio AND @DataFim
GROUP BY ar.DueDate

-- SaÃ­das Projetadas (Contas a Pagar)  
SELECT 
    ap.DueDate,
    SUM(ap.RemainingAmount) AS SaidasProjetadas
FROM ACCOUNT_PAYABLE ap
WHERE ap.AccountStatus IN ('Pending', 'PartiallyPaid')
  AND ap.DueDate BETWEEN @DataInicio AND @DataFim
GROUP BY ap.DueDate
```

### **ðŸŽ¯ Indicadores de Performance**

#### **Receitas por DomÃ­nio**
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

#### **AnÃ¡lise de InadimplÃªncia**
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

## ðŸŽ¯ Eventos de DomÃ­nio Gerados

- **AccountReceivableCreated**: Nova conta a receber gerada
- **PaymentReceived**: Pagamento de cliente recebido
- **PaymentMade**: Pagamento a fornecedor efetuado
- **AccountOverdue**: Conta vencida detectada
- **CashFlowAlert**: Alerta de fluxo de caixa baixo
- **BankAccountReconciled**: ConciliaÃ§Ã£o bancÃ¡ria realizada
- **ProfitabilityCalculated**: Lucratividade recalculada

## ðŸš¨ Alertas e ValidaÃ§Ãµes

### **Alertas CrÃ­ticos**
- **Contas Vencidas**: DueDate < hoje E AccountStatus â‰  Paid
- **Fluxo de Caixa Baixo**: Saldo projetado < limite configurado
- **InadimplÃªncia Alta**: % contas vencidas > limite aceitÃ¡vel
- **Descasamento**: Transaction sem conciliaÃ§Ã£o > 7 dias

### **ValidaÃ§Ãµes de NegÃ³cio**
- Transaction deve ter AccountReceivable OU AccountPayable
- Valor de Transaction nÃ£o pode exceder saldo da conta
- AccountReceivable nÃ£o pode ser paga alÃ©m do valor total
- Data de Transaction nÃ£o pode ser futura (exceto projeÃ§Ãµes)
- BankAccount deve ter saldo suficiente para dÃ©bitos

---

**Arquivo**: `05-financial-domain-erd.md`  
**DomÃ­nio**: Financeiro (#083e61)  
**Tipo**: Entity-Relationship Diagram  
**NÃ­vel**: Detalhado + Fluxos AutomÃ¡ticos + AnÃ¡lises
