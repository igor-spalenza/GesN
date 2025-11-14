# ðŸ”„ SEQUENCE DIAGRAM - OrderEntry â†’ AccountReceivable Flow

## ðŸŽ¯ VisÃ£o Geral
Diagrama de sequÃªncia detalhado mostrando o fluxo automÃ¡tico de criaÃ§Ã£o de contas a receber a partir da confirmaÃ§Ã£o de pedidos de venda. Este fluxo fundamental conecta os domÃ­nios de Vendas e Financeiro, garantindo que toda receita seja devidamente registrada e acompanhada para cobranÃ§a.

## ðŸ“Š Complexidade do Fluxo
- **âš ï¸ MÃ©dia Complexidade**: Cross-domain integration, payment terms processing, installment calculations
- **ðŸ‘¥ Participantes**: 7+ system components
- **ðŸ”„ InteraÃ§Ãµes**: 15+ interactions per order
- **ðŸŒ Cross-Domain**: Sales â†’ Financial integration
- **ðŸ“‹ ValidaÃ§Ãµes**: Customer credit, payment terms, installment calculations

## ðŸŽ¯ Trigger Event
**OrderConfirmed** (Sales Domain) â†’ Automatic AccountReceivable creation

## ðŸ“ Sequence Diagram

```mermaid
sequenceDiagram
    participant UI as ðŸ‘¤ User Interface
    participant SC as ðŸŽ® Sales Controller
    participant SS as âš™ï¸ Sales Service
    participant SR as ðŸ—„ï¸ Sales Repository
    participant EB as ðŸ“¡ Event Bus
    participant FS as ðŸ’³ Financial Service
    participant FR as ðŸ—„ï¸ Financial Repository
    participant VS as âœ… Validation Service
    participant CS as ðŸ‘¤ Customer Service
    participant NS as ðŸ”” Notification Service
    participant DB as ðŸ’¾ Database
    
    Note over UI, DB: OrderEntry â†’ AccountReceivable Flow (Triggered by Order Confirmation)
    
    %% ==========================================
    %% SALES DOMAIN - ORDER CONFIRMATION
    %% ==========================================
    
    UI->>SC: POST /Order/{orderId}/Confirm
    Note over SC: Validate user authorization for order confirmation
    
    SC->>SS: ConfirmOrderAsync(orderId, confirmationData)
    activate SS
    
    SS->>SR: GetOrderWithCustomerAsync(orderId)
    activate SR
    SR->>DB: SELECT OrderEntry + Customer + PaymentTerms
    DB-->>SR: Order with customer details
    SR-->>SS: OrderEntry with Customer
    deactivate SR
    
    Note over SS: Validate order for confirmation
    SS->>VS: ValidateOrderForConfirmationAsync(order)
    activate VS
    
    VS->>CS: ValidateCustomerCreditAsync(customerId, orderTotal)
    activate CS
    CS->>CS: CheckCurrentDebt()
    CS->>CS: CalculateAvailableCredit()
    CS->>CS: AssessCreditRisk()
    CS-->>VS: CreditValidationResult (Approved)
    deactivate CS
    
    VS->>VS: ValidateDeliveryDateFeasibility()
    VS->>VS: ValidatePaymentTermsValidity()
    VS->>VS: CheckBusinessRuleCompliance()
    
    VS-->>SS: ValidationResult (Success)
    deactivate VS
    
    Note over SS: Update order status
    SS->>SR: UpdateOrderStatusAsync(orderId, "Confirmed")
    activate SR
    SR->>DB: UPDATE OrderEntry SET Status = 'Confirmed', ConfirmedDate = NOW()
    DB-->>SR: Success
    SR-->>SS: Order status updated
    deactivate SR
    
    Note over SS: Prepare financial data for AR creation
    SS->>SS: CalculateOrderFinancials(order)
    SS->>SS: DeterminePaymentTerms(customer.PaymentTerms)
    SS->>SS: PrepareAccountReceivableData(order, paymentTerms)
    
    %% ==========================================
    %% EVENT PUBLISHING - CROSS DOMAIN
    %% ==========================================
    
    Note over SS: Publish OrderConfirmed event
    SS->>EB: PublishAsync(OrderConfirmed event)
    activate EB
    
    Note over EB: Event contains order, customer, and payment data
    EB->>EB: RouteEventToFinancialDomain(orderConfirmedEvent)
    EB->>EB: RouteEventToProductionDomain(orderConfirmedEvent)
    deactivate EB
    
    SS-->>SC: Order confirmed successfully
    deactivate SS
    SC-->>UI: 200 OK - Order confirmed
    
    %% ==========================================
    %% FINANCIAL DOMAIN - ACCOUNT RECEIVABLE CREATION
    %% ==========================================
    
    Note over EB, DB: Financial Domain Processing (Background)
    
    EB->>FS: Handle OrderConfirmed event
    activate FS
    
    Note over FS: Extract order and customer data from event
    FS->>FS: ExtractOrderDataFromEvent(orderConfirmedEvent)
    
    Note over FS: Validate financial business rules
    FS->>VS: ValidateFinancialRulesAsync(orderData)
    activate VS
    
    VS->>VS: ValidateCustomerAccountStatus()
    VS->>VS: CheckCustomerCreditLimits()
    VS->>VS: ValidateRevenueRecognitionRules()
    VS->>VS: VerifyChartOfAccountsMapping()
    VS->>VS: CheckTaxCalculationRequirements()
    
    VS-->>FS: ValidationResult (Success)
    deactivate VS
    
    %% ==========================================
    %% PAYMENT TERMS AND INSTALLMENT PROCESSING
    %% ==========================================
    
    Note over FS: Process payment terms and create payment schedule
    alt Payment Terms = "Ã€ Vista" (Cash)
        FS->>FS: CreateCashPayment(orderData)
        FS->>FS: CalculateImmediateDueDate(orderDate, terms)
        FS->>FS: ApplyCashDiscountIfApplicable()
        
    else Payment Terms = "Parcelado" (Installments)
        FS->>FS: CreateInstallmentPayment(orderData)
        FS->>FS: CalculateInstallmentSchedule(totalAmount, installmentCount)
        
        Note over FS: Create installment breakdown
        loop For each Installment
            FS->>FS: CalculateInstallmentAmount(principal, interest, installmentNumber)
            FS->>FS: CalculateInstallmentDueDate(orderDate, installmentNumber)
            FS->>FS: CreateInstallmentData(amount, dueDate, installmentNumber)
        end
        
    else Payment Terms = "Prazo" (Credit Term)
        FS->>FS: CreateCreditTermPayment(orderData)
        FS->>FS: CalculateCreditDueDate(orderDate, creditDays)
        FS->>FS: ApplyCreditTermsAndConditions()
        
    else Payment Terms = "Customizado" (Custom)
        FS->>FS: CreateCustomPayment(orderData)
        FS->>FS: ApplyCustomPaymentRules(customer.CustomTerms)
        FS->>FS: ValidateCustomTermsCompliance()
    end
    
    %% ==========================================
    %% ACCOUNT RECEIVABLE CREATION
    %% ==========================================
    
    Note over FS: Create main AccountReceivable record
    FS->>FR: CreateAccountReceivableAsync(accountReceivableData)
    activate FR
    
    FR->>DB: BEGIN TRANSACTION
    
    FR->>DB: INSERT INTO AccountReceivable (main record)
    DB-->>FR: AccountReceivableId
    
    Note over FR: Create installment records if applicable
    opt Has Installment Schedule
        loop For each Installment
            FR->>DB: INSERT INTO PaymentInstallment
            DB-->>FR: InstallmentId
        end
    end
    
    Note over FR: Create payment tracking records
    FR->>DB: INSERT INTO PaymentTracking (initial status)
    DB-->>FR: PaymentTrackingId
    
    FR->>DB: COMMIT TRANSACTION
    DB-->>FR: Transaction committed successfully
    
    FR-->>FS: AccountReceivable created with installments
    deactivate FR
    
    %% ==========================================
    %% CUSTOMER CREDIT AND FINANCIAL UPDATES
    %% ==========================================
    
    Note over FS: Update customer credit utilization
    FS->>FS: UpdateCustomerCreditUtilization(customerId, orderAmount)
    
    FS->>FR: UpdateCustomerFinancialStatusAsync(customerId, creditUpdate)
    activate FR
    FR->>DB: UPDATE CustomerFinancial SET current_debt, credit_utilization
    DB-->>FR: Customer credit updated
    FR-->>FS: Customer financial status updated
    deactivate FR
    
    Note over FS: Update cash flow projections
    FS->>FS: UpdateCashFlowProjections(accountReceivable)
    
    FS->>FR: UpdateCashFlowAsync(inflowProjections)
    activate FR
    FR->>DB: UPDATE CashFlowProjection SET projected_inflows
    DB-->>FR: Cash flow projections updated
    FR-->>FS: Cash flow updated
    deactivate FR
    
    %% ==========================================
    %% REVENUE RECOGNITION AND ACCOUNTING
    %% ==========================================
    
    Note over FS: Handle revenue recognition based on delivery terms
    alt Delivery Terms = "FOB Destination"
        FS->>FS: DeferRevenueRecognition(accountReceivable)
        FS->>FS: CreateDeferredRevenueEntry()
        
    else Delivery Terms = "FOB Origin"
        FS->>FS: RecognizeRevenueImmediately(accountReceivable)
        FS->>FS: CreateRevenueJournalEntry()
        
    else Delivery Terms = "Custom Terms"
        FS->>FS: ApplyCustomRevenueRules(deliveryTerms)
        FS->>FS: CreateConditionalRevenueEntry()
    end
    
    FS->>FR: CreateAccountingEntriesAsync(revenueEntries)
    activate FR
    FR->>DB: INSERT INTO GeneralLedgerEntry (revenue recognition)
    DB-->>FR: Accounting entries created
    FR-->>FS: Revenue properly recognized
    deactivate FR
    
    %% ==========================================
    %% NOTIFICATIONS AND CUSTOMER COMMUNICATION
    %% ==========================================
    
    Note over FS: Generate customer notifications and documents
    FS->>NS: GenerateInvoiceAsync(accountReceivable)
    activate NS
    NS->>NS: CreateInvoiceDocument(orderData, paymentTerms)
    NS->>NS: SendInvoiceToCustomer(customerId, invoice)
    NS-->>FS: Invoice generated and sent
    deactivate NS
    
    opt Has Installment Schedule
        FS->>NS: CreatePaymentReminderScheduleAsync(installments)
        activate NS
        loop For each Installment
            NS->>NS: SchedulePaymentReminder(dueDate - 3 days)
        end
        NS-->>FS: Payment reminders scheduled
        deactivate NS
    end
    
    FS->>NS: NotifyFinanceTeamAsync(accountReceivableCreated)
    activate NS
    NS->>NS: SendARCreationNotification(financeTeam, arDetails)
    NS-->>FS: Finance team notified
    deactivate NS
    
    opt Large Order (> Threshold)
        FS->>NS: NotifyManagementAsync(largeOrderAlert)
        activate NS
        NS->>NS: SendHighValueOrderAlert(management, orderAmount)
        NS-->>FS: Management alerted
        deactivate NS
    end
    
    %% ==========================================
    %% CROSS-DOMAIN STATUS UPDATE
    %% ==========================================
    
    Note over FS: Notify Sales domain of successful AR creation
    FS->>EB: PublishAsync(AccountReceivableCreated event)
    activate EB
    EB->>SS: Handle AccountReceivableCreated event
    activate SS
    
    SS->>SR: UpdateOrderFinancialStatusAsync(orderId, "AR_Created")
    activate SR
    SR->>DB: UPDATE OrderEntry SET FinancialStatus = 'AR_Created'
    DB-->>SR: Financial status updated
    SR-->>SS: Order financial status updated
    deactivate SR
    
    SS-->>EB: Event handled successfully
    deactivate SS
    deactivate EB
    
    FS-->>EB: AccountReceivable creation completed
    deactivate FS
    
    %% ==========================================
    %% ERROR HANDLING SCENARIOS
    %% ==========================================
    
    Note over UI, DB: Error Handling Scenarios
    
    alt Customer Credit Limit Exceeded
        CS-->>VS: CreditValidationResult (Credit exceeded)
        VS-->>SS: ValidationResult (Credit limit exceeded)
        SS->>SS: LogCreditLimitError(customerId, orderAmount)
        SS->>NS: NotifyAsync(salesTeam, "Credit limit exceeded")
        SS-->>SC: 400 Bad Request - Credit limit exceeded
    end
    
    alt Invalid Payment Terms
        VS-->>FS: ValidationResult (Invalid payment terms)
        FS->>FS: LogPaymentTermsError(customerId, terms)
        FS->>EB: PublishAsync(ARCreationFailed event)
        FS->>NS: NotifyAsync(salesTeam, "Payment terms invalid")
    end
    
    alt Database Transaction Failure
        FR-->>FS: DatabaseError (Transaction failed)
        FS->>FS: LogDatabaseError(orderId, error)
        FS->>FS: InitiateRetryMechanism(orderConfirmedEvent)
        FS->>NS: NotifyAsync(techTeam, "AR creation failed")
    end
    
    alt Revenue Recognition Rule Violation
        FS->>FS: RevenueRecognitionError(orderId, violation)
        FS->>NS: NotifyAsync(accountingTeam, "Revenue recognition issue")
        FS->>FS: CreateManualReviewTask(accountReceivable)
    end
    
    alt Customer Account Inactive
        VS-->>FS: ValidationResult (Customer account inactive)
        FS->>FS: LogInactiveCustomerError(customerId)
        FS->>EB: PublishAsync(CustomerAccountIssue event)
        FS->>NS: NotifyAsync(salesTeam, "Customer account inactive")
    end
```

## ðŸŽ¯ Detailed Component Responsibilities

### **ðŸŽ® Sales Controller**
```
Responsibilities:
â”œâ”€â”€ ðŸ” Validate user authorization for order confirmation
â”œâ”€â”€ ðŸ“‹ HTTP request validation and sanitization  
â”œâ”€â”€ ðŸ’° Order confirmation workflow coordination
â”œâ”€â”€ ðŸ“Š Return appropriate HTTP response codes
â””â”€â”€ ðŸ” Log confirmation-related activities

Authorization Validation:
â”œâ”€â”€ ðŸ‘¤ User role verification
â”œâ”€â”€ ðŸ’° Order amount vs authorization limits
â”œâ”€â”€ ðŸ¢ Customer account access permissions
â”œâ”€â”€ ðŸ“… Business hours confirmation rules
â””â”€â”€ ðŸš¨ Fraud detection checks
```

### **âš™ï¸ Sales Service**
```
Order Confirmation Logic:
â”œâ”€â”€ ðŸ“‹ Comprehensive order validation
â”œâ”€â”€ ðŸ‘¤ Customer credit verification
â”œâ”€â”€ ðŸ’° Financial calculations and verification
â”œâ”€â”€ ðŸ“… Delivery date validation and commitment
â””â”€â”€ ðŸ“¡ Cross-domain event orchestration

Financial Data Preparation:
â”œâ”€â”€ ðŸ’° Order total calculation and verification
â”œâ”€â”€ ðŸ“Š Tax calculation and application
â”œâ”€â”€ ðŸ’¸ Discount application and validation
â”œâ”€â”€ ðŸ“… Payment terms determination
â””â”€â”€ ðŸ¦ Revenue recognition rule application

Customer Relationship Management:
â”œâ”€â”€ ðŸ‘¤ Customer status verification
â”œâ”€â”€ ðŸ’³ Credit limit and utilization checking
â”œâ”€â”€ ðŸ“Š Payment history analysis
â”œâ”€â”€ ðŸŽ¯ Customer risk assessment
â””â”€â”€ ðŸ“ˆ Customer lifetime value updates
```

### **ðŸ’³ Financial Service**
```
AccountReceivable Creation Logic:
â”œâ”€â”€ ðŸ“Š Financial validation and compliance
â”œâ”€â”€ ðŸ’° Payment terms processing and application
â”œâ”€â”€ ðŸ“… Installment schedule generation
â”œâ”€â”€ ðŸ’¸ Revenue recognition processing
â””â”€â”€ ðŸ“ˆ Financial metrics and projections

Payment Processing Strategy:
â”œâ”€â”€ ðŸ’µ Cash payments: Immediate processing
â”œâ”€â”€ ðŸ“Š Installments: Complex schedule creation
â”œâ”€â”€ ðŸ“… Credit terms: Due date calculations
â”œâ”€â”€ ðŸŽ¯ Custom terms: Flexible rule application
â””â”€â”€ ðŸ¦ Payment method validation

Financial Impact Management:
â”œâ”€â”€ ðŸ’¸ Cash flow projection updates
â”œâ”€â”€ ðŸ“Š Customer credit utilization tracking
â”œâ”€â”€ ðŸ“ˆ Revenue recognition compliance
â”œâ”€â”€ ðŸŽ¯ Financial ratio impact assessment
â””â”€â”€ âš ï¸ Risk threshold monitoring
```

### **ðŸ‘¤ Customer Service**
```
Credit Management:
â”œâ”€â”€ ðŸ’³ Credit limit verification and management
â”œâ”€â”€ ðŸ“Š Payment history analysis and scoring
â”œâ”€â”€ ðŸŽ¯ Risk assessment and categorization
â”œâ”€â”€ ðŸ’° Current debt calculation and tracking
â””â”€â”€ ðŸ“ˆ Credit utilization monitoring

Customer Validation:
â”œâ”€â”€ âœ… Account status verification (active/inactive)
â”œâ”€â”€ ðŸš¨ Fraud detection and prevention
â”œâ”€â”€ ðŸ“‹ KYC (Know Your Customer) compliance
â”œâ”€â”€ ðŸ¦ Banking and payment information validation
â””â”€â”€ ðŸ“Š Customer relationship health assessment
```

## ðŸ’° Payment Terms and Revenue Recognition

### **ðŸ“… Payment Terms Processing**
```
Ã€ Vista (Cash Payment):
â”œâ”€â”€ ðŸ“… Due Date: Order date + 0-7 days
â”œâ”€â”€ ðŸ’° Discount: 2-5% early payment discount typically
â”œâ”€â”€ ðŸ¦ Method: Cash, debit, immediate bank transfer
â”œâ”€â”€ ðŸ’¸ Cash Flow: Immediate or near-immediate inflow
â””â”€â”€ ðŸ“Š Risk: Lowest credit risk, immediate recognition

Parcelado (Installment Payment):
â”œâ”€â”€ ðŸ“… Due Dates: Monthly payments over 2-24 months
â”œâ”€â”€ ðŸ’° Interest: 1-3% monthly compound interest
â”œâ”€â”€ ðŸ“Š Installments: Equal payments with interest
â”œâ”€â”€ ðŸ’¸ Cash Flow: Spread over installment period
â””â”€â”€ ðŸ“‹ Risk: Medium risk, payment tracking required

Prazo (Credit Terms):
â”œâ”€â”€ ðŸ“… Due Date: 15/30/45/60 days from order date
â”œâ”€â”€ ðŸ’° Standard: Most common B2B payment terms
â”œâ”€â”€ ðŸ¦ Method: Bank transfer, check, credit
â”œâ”€â”€ ðŸ’¸ Cash Flow: Single future inflow
â””â”€â”€ ðŸ“Š Risk: Standard business credit risk

Customizado (Custom Terms):
â”œâ”€â”€ ðŸ“… Due Date: Negotiated based on customer relationship
â”œâ”€â”€ ðŸ’° Complex: May include milestones, conditions
â”œâ”€â”€ ðŸ“Š Special Cases: Large customers, strategic accounts
â”œâ”€â”€ ðŸ’¸ Cash Flow: Varies by agreement terms
â””â”€â”€ ðŸ“‹ Approval: Requires manager/director approval
```

### **ðŸ“Š Revenue Recognition Rules**
```
Immediate Recognition (FOB Origin):
â”œâ”€â”€ ðŸšš Recognition: When goods shipped from warehouse
â”œâ”€â”€ ðŸ“… Timing: Order confirmation + shipping
â”œâ”€â”€ ðŸ’° Amount: Full order amount recognized
â”œâ”€â”€ ðŸ“Š Risk: Standard revenue recognition
â””â”€â”€ ðŸ“‹ Compliance: GAAP/IFRS standard approach

Deferred Recognition (FOB Destination):
â”œâ”€â”€ ðŸšš Recognition: When goods delivered to customer
â”œâ”€â”€ ðŸ“… Timing: Delivery confirmation required
â”œâ”€â”€ ðŸ’° Amount: Revenue held in deferred account
â”œâ”€â”€ ðŸ“Š Risk: Delivery completion required
â””â”€â”€ ðŸ“‹ Compliance: Conservative approach

Milestone Recognition (Custom):
â”œâ”€â”€ ðŸŽ¯ Recognition: Based on completion milestones
â”œâ”€â”€ ðŸ“… Timing: Percentage completion method
â”œâ”€â”€ ðŸ’° Amount: Proportional to milestone completion
â”œâ”€â”€ ðŸ“Š Risk: Complex tracking required
â””â”€â”€ ðŸ“‹ Compliance: Project accounting standards

Service Recognition (Ongoing):
â”œâ”€â”€ â° Recognition: Over service delivery period
â”œâ”€â”€ ðŸ“… Timing: Monthly/periodic recognition
â”œâ”€â”€ ðŸ’° Amount: Straight-line over service period
â”œâ”€â”€ ðŸ“Š Risk: Service delivery performance risk
â””â”€â”€ ðŸ“‹ Compliance: Subscription revenue standards
```

## ðŸ”’ Credit Management and Risk Assessment

### **ðŸ’³ Customer Credit Framework**
```
Credit Limit Determination:
â”œâ”€â”€ ðŸ“Š Credit Score: External credit bureau data
â”œâ”€â”€ ðŸ’° Financial Statements: Customer financial health
â”œâ”€â”€ ðŸ“ˆ Payment History: Past payment performance
â”œâ”€â”€ ðŸ¢ Business Relationship: Length and depth
â””â”€â”€ ðŸŽ¯ Industry Risk: Sector-specific risk factors

Credit Utilization Monitoring:
â”œâ”€â”€ ðŸ’° Current Outstanding: All unpaid invoices
â”œâ”€â”€ ðŸ“Š Available Credit: Limit - outstanding
â”œâ”€â”€ ðŸ“ˆ Utilization Ratio: Outstanding / limit
â”œâ”€â”€ âš ï¸ Alert Thresholds: 75%, 90%, 100% utilization
â””â”€â”€ ðŸš¨ Actions: Hold orders, require payment

Risk Assessment Categories:
â”œâ”€â”€ ðŸŸ¢ Low Risk: Excellent credit, long relationship
â”œâ”€â”€ ðŸŸ¡ Medium Risk: Good credit, standard terms
â”œâ”€â”€ ðŸŸ  High Risk: Fair credit, restricted terms
â”œâ”€â”€ ðŸ”´ Very High Risk: Poor credit, cash only
â””â”€â”€ âš« Blocked: No new orders, collection required
```

### **ðŸ“Š Credit Decision Matrix**
| Credit Score | Payment History | Order Amount | Decision | Terms |
|--------------|----------------|--------------|----------|-------|
| **Excellent (750+)** | Perfect | Any | Auto-Approve | Standard |
| **Good (650-749)** | Good | < $50K | Auto-Approve | Standard |
| **Fair (550-649)** | Mixed | < $25K | Manager Review | Restricted |
| **Poor (< 550)** | Poor | Any | Director Review | Cash Only |

### **ðŸš¨ Alert and Action Framework**
```
Credit Alerts:
â”œâ”€â”€ ðŸŸ¡ Warning: 75% credit utilization reached
â”œâ”€â”€ ðŸŸ  Caution: 90% credit utilization reached
â”œâ”€â”€ ðŸ”´ Critical: 100% credit utilization reached
â”œâ”€â”€ âš« Block: Payment overdue > 30 days
â””â”€â”€ ðŸš¨ Escalate: Manager review required

Automated Actions:
â”œâ”€â”€ ðŸ“§ Email: Customer payment reminder
â”œâ”€â”€ ðŸ“ž Call: Sales team follow-up required
â”œâ”€â”€ ðŸš« Hold: New orders temporarily suspended
â”œâ”€â”€ ðŸ”’ Block: No new orders until payment
â””â”€â”€ ðŸ“‹ Review: Manual credit review triggered
```

## ðŸ“Š Financial Calculations and Metrics

### **ðŸ’° Order Financial Calculations**
```
Base Calculations:
â”œâ”€â”€ ðŸ§® Subtotal = Sum of (quantity Ã— unit_price) for all items
â”œâ”€â”€ ðŸ’¸ Discounts = Customer discounts + promotional discounts
â”œâ”€â”€ ðŸ“Š Taxes = Subtotal Ã— applicable tax rates
â”œâ”€â”€ ðŸšš Shipping = Based on delivery terms and distance
â””â”€â”€ ðŸ’° Total = Subtotal - Discounts + Taxes + Shipping

Installment Calculations:
â”œâ”€â”€ ðŸ“Š Principal = Total amount / number of installments
â”œâ”€â”€ ðŸ’° Interest = Principal Ã— monthly interest rate
â”œâ”€â”€ ðŸ“… Payment = Principal + accrued interest
â”œâ”€â”€ ðŸ§® Total Interest = Sum of all interest payments
â””â”€â”€ âœ… Validation = Sum of payments = total + total interest

Tax Calculations:
â”œâ”€â”€ ðŸ“Š Sales Tax = Subtotal Ã— local sales tax rate
â”œâ”€â”€ ðŸ’° VAT = (Subtotal + shipping) Ã— VAT rate
â”œâ”€â”€ ðŸŽ¯ Service Tax = Service items Ã— service tax rate
â”œâ”€â”€ ðŸ“‹ Compliance = Tax jurisdiction determination
â””â”€â”€ âœ… Validation = Tax calculations per regulations
```

### **ðŸ“ˆ Cash Flow Impact**
```
Immediate Impact:
â”œâ”€â”€ ðŸ’° Expected Inflow = Order total amount
â”œâ”€â”€ ðŸ“… Expected Date = Based on payment terms
â”œâ”€â”€ ðŸŽ¯ Probability = Based on customer credit score
â”œâ”€â”€ ðŸ’¸ Present Value = Discounted for time value
â””â”€â”€ ðŸ“Š Confidence = Risk-adjusted expected value

Projected Impact:
â”œâ”€â”€ ðŸ“ˆ 7-day projection: Payment due this week
â”œâ”€â”€ ðŸ“Š 30-day projection: Payment due this month
â”œâ”€â”€ ðŸ’° 90-day projection: Quarterly cash impact
â”œâ”€â”€ ðŸ“… Annual projection: Yearly revenue impact
â””â”€â”€ ðŸŽ¯ Scenario Analysis: Best/worst/likely cases
```

## ðŸ”„ Error Handling and Business Rules

### **âŒ Common Error Scenarios**
```
Customer Credit Issues:
â”œâ”€â”€ ðŸ’³ Credit limit exceeded by order amount
â”œâ”€â”€ ðŸ“Š Customer payment history poor
â”œâ”€â”€ ðŸš¨ Customer account flagged for collection
â”œâ”€â”€ ðŸ’° Outstanding invoices past due
â””â”€â”€ ðŸ”’ Customer account temporarily suspended

Payment Terms Issues:
â”œâ”€â”€ ðŸ“… Invalid payment terms for customer type
â”œâ”€â”€ ðŸ’° Installment terms exceed maximum allowed
â”œâ”€â”€ ðŸ¦ Payment method not supported for customer
â”œâ”€â”€ ðŸ“Š Custom terms require additional approval
â””â”€â”€ ðŸ’¸ Early payment discount calculation error

Financial Validation Issues:
â”œâ”€â”€ ðŸ§® Order total calculation mismatch
â”œâ”€â”€ ðŸ“Š Tax calculation errors or missing rates
â”œâ”€â”€ ðŸ’° Revenue recognition rule violations
â”œâ”€â”€ ðŸ“… Due date calculation outside business rules
â””â”€â”€ ðŸ¦ Chart of accounts mapping errors
```

### **ðŸ”§ Recovery and Resolution**
```
Credit Resolution Process:
â”œâ”€â”€ ðŸ”„ Automatic retry after payment received
â”œâ”€â”€ ðŸ‘¤ Sales team customer contact for resolution
â”œâ”€â”€ ðŸ’³ Temporary credit increase approval process
â”œâ”€â”€ ðŸ’° Payment plan negotiation and setup
â””â”€â”€ ðŸ“‹ Escalation to finance manager for decisions

Financial Error Resolution:
â”œâ”€â”€ ðŸ§® Automatic recalculation triggers
â”œâ”€â”€ ðŸ“Š Manual review queue for complex cases
â”œâ”€â”€ ðŸ’° Finance team notification and intervention
â”œâ”€â”€ ðŸ”„ Transaction rollback and retry mechanisms
â””â”€â”€ ðŸ“‹ Audit trail maintenance for all corrections

Business Rule Updates:
â”œâ”€â”€ ðŸ“‹ Dynamic rule engine updates
â”œâ”€â”€ ðŸŽ¯ A/B testing for new rule implementations
â”œâ”€â”€ ðŸ“Š Impact analysis before rule changes
â”œâ”€â”€ ðŸ”„ Rollback capabilities for failed changes
â””â”€â”€ ðŸ“ˆ Performance monitoring post-changes
```

## ðŸ“ˆ Performance and Monitoring

### **âš¡ Performance Targets**
```
Response Time SLAs:
â”œâ”€â”€ ðŸŽ¯ Order confirmation: < 3 seconds
â”œâ”€â”€ ðŸ“Š Credit validation: < 2 seconds
â”œâ”€â”€ ðŸ’° AR creation: < 5 seconds
â”œâ”€â”€ ðŸ“§ Customer notification: < 10 seconds
â””â”€â”€ ðŸ”„ Cross-domain sync: < 15 seconds

Business Process SLAs:
â”œâ”€â”€ ðŸ’³ Credit decision: < 30 seconds automated
â”œâ”€â”€ ðŸ“Š Invoice generation: < 2 minutes
â”œâ”€â”€ ðŸ“§ Customer communication: < 5 minutes
â”œâ”€â”€ ðŸ’° Payment processing: < 1 hour
â””â”€â”€ ðŸ“‹ Financial reporting: < 4 hours
```

### **ðŸ“Š Key Metrics and KPIs**
```
Business Metrics:
â”œâ”€â”€ ðŸ’° Order-to-cash cycle time
â”œâ”€â”€ ðŸ“Š Customer payment compliance rate
â”œâ”€â”€ ðŸ’³ Credit utilization efficiency
â”œâ”€â”€ ðŸ“ˆ Revenue recognition accuracy
â””â”€â”€ ðŸŽ¯ Customer satisfaction scores

Technical Metrics:
â”œâ”€â”€ ðŸ”Œ API response times and reliability
â”œâ”€â”€ ðŸ’¾ Database transaction performance
â”œâ”€â”€ ðŸ“¡ Event processing latency
â”œâ”€â”€ ðŸš¨ Error rates by component
â””â”€â”€ ðŸ“ˆ System throughput under load

Financial Metrics:
â”œâ”€â”€ ðŸ’° Days Sales Outstanding (DSO)
â”œâ”€â”€ ðŸ“Š Bad debt write-off percentage
â”œâ”€â”€ ðŸ’³ Credit limit utilization trends
â”œâ”€â”€ ðŸ“ˆ Revenue per order trends
â””â”€â”€ ðŸŽ¯ Cash flow forecast accuracy
```

---

**Arquivo**: `03-order-to-receivable-flow.md`  
**Fluxo**: OrderEntry â†’ AccountReceivable (AutomÃ¡tico)  
**DomÃ­nios**: Sales â†’ Financial  
**Complexidade**: âš ï¸ MÃ©dia (7+ participantes, 15+ interaÃ§Ãµes)  
**AtualizaÃ§Ã£o**: 16/06/2025
