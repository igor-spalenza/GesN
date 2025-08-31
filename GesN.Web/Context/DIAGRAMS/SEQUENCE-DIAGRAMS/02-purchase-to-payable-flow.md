# ðŸ”„ SEQUENCE DIAGRAM - PurchaseOrder â†’ AccountPayable Flow

## ðŸŽ¯ VisÃ£o Geral
Diagrama de sequÃªncia detalhado mostrando o fluxo automÃ¡tico de criaÃ§Ã£o de contas a pagar a partir da aprovaÃ§Ã£o de pagamento de ordens de compra. Este fluxo crÃ­tico conecta os domÃ­nios de Compras e Financeiro, garantindo que todas as obrigaÃ§Ãµes financeiras sejam devidamente registradas e programadas para pagamento.

## ðŸ“Š Complexidade do Fluxo
- **âš ï¸ MÃ©dia Complexidade**: Cross-domain integration, payment scheduling, financial calculations
- **ðŸ‘¥ Participantes**: 6+ system components
- **ðŸ”„ InteraÃ§Ãµes**: 15+ interactions per purchase order
- **ðŸŒ Cross-Domain**: Purchasing â†’ Financial integration
- **ðŸ“‹ ValidaÃ§Ãµes**: Payment approval, budget validation, supplier verification

## ðŸŽ¯ Trigger Event
**PaymentApproved** (Purchasing Domain) â†’ Automatic AccountPayable creation

## ðŸ“ Sequence Diagram

```mermaid
sequenceDiagram
    participant UI as ðŸ‘¤ User Interface
    participant PC as ðŸ›’ Purchasing Controller
    participant PS as âš™ï¸ Purchasing Service
    participant PR as ðŸ—„ï¸ Purchasing Repository
    participant EB as ðŸ“¡ Event Bus
    participant FS as ðŸ’³ Financial Service
    participant FR as ðŸ—„ï¸ Financial Repository
    participant VS as âœ… Validation Service
    participant NS as ðŸ”” Notification Service
    participant DB as ðŸ’¾ Database
    
    Note over UI, DB: PurchaseOrder â†’ AccountPayable Flow (Triggered by Payment Approval)
    
    %% ==========================================
    %% PURCHASING DOMAIN - PAYMENT APPROVAL
    %% ==========================================
    
    UI->>PC: POST /PurchaseOrder/{id}/ApprovePayment
    Note over PC: Validate user authorization for payment approval
    
    PC->>PS: ApprovePurchasePaymentAsync(purchaseOrderId, approvalData)
    activate PS
    
    PS->>PR: GetPurchaseOrderByIdAsync(purchaseOrderId)
    activate PR
    PR->>DB: SELECT PurchaseOrder + Items + Supplier
    DB-->>PR: PurchaseOrder with full details
    PR-->>PS: PurchaseOrder entity
    deactivate PR
    
    Note over PS: Validate purchase order can be approved
    PS->>VS: ValidatePurchaseForPaymentAsync(purchaseOrder)
    activate VS
    
    VS->>VS: CheckPurchaseOrderStatus() // Must be "Received"
    VS->>VS: ValidateSupplierStatus() // Supplier must be active
    VS->>VS: ValidateBudgetAvailability() // Check budget limits
    VS->>VS: VerifyReceiptConfirmation() // All items received
    VS->>VS: CheckDuplicatePaymentPrevention() // Not already paid
    
    VS-->>PS: ValidationResult (Success)
    deactivate VS
    
    Note over PS: Calculate payment details
    PS->>PS: CalculatePaymentAmount(purchaseOrder)
    PS->>PS: DeterminePaymentTerms(supplier.PaymentTerms)
    PS->>PS: CalculateDueDate(paymentTerms, approvalDate)
    
    Note over PS: Update purchase order status
    PS->>PR: UpdatePurchaseOrderStatusAsync(purchaseOrderId, "PaymentApproved")
    activate PR
    PR->>DB: UPDATE PurchaseOrder SET Status = 'PaymentApproved', ApprovedDate = NOW()
    DB-->>PR: Success
    PR-->>PS: Purchase order updated
    deactivate PR
    
    Note over PS: Prepare AccountPayable data
    PS->>PS: PrepareAccountPayableData(purchaseOrder, paymentTerms)
    
    %% ==========================================
    %% EVENT PUBLISHING - CROSS DOMAIN
    %% ==========================================
    
    Note over PS: Publish PaymentApproved event
    PS->>EB: PublishAsync(PaymentApproved event)
    activate EB
    
    Note over EB: Event contains all necessary data for Financial domain
    EB->>EB: RouteEventToFinancialDomain(paymentApprovedEvent)
    deactivate EB
    
    PS-->>PC: Payment approved successfully
    deactivate PS
    PC-->>UI: 200 OK - Payment approved
    
    %% ==========================================
    %% FINANCIAL DOMAIN - ACCOUNT PAYABLE CREATION
    %% ==========================================
    
    Note over EB, DB: Financial Domain Processing (Background)
    
    EB->>FS: Handle PaymentApproved event
    activate FS
    
    Note over FS: Extract purchase order data from event
    FS->>FS: ExtractPurchaseDataFromEvent(paymentApprovedEvent)
    
    Note over FS: Validate financial business rules
    FS->>VS: ValidateFinancialRulesAsync(purchaseData)
    activate VS
    
    VS->>VS: ValidateSupplierFinancialStatus()
    VS->>VS: CheckCashFlowImpact()
    VS->>VS: ValidatePaymentMethod()
    VS->>VS: VerifyChartOfAccounts()
    VS->>VS: CheckApprovalAuthority()
    
    VS-->>FS: ValidationResult (Success)
    deactivate VS
    
    %% ==========================================
    %% PAYMENT TERMS PROCESSING
    %% ==========================================
    
    Note over FS: Process payment terms and calculate schedule
    alt Payment Terms = "Ã€ Vista" (Immediate)
        FS->>FS: CreateImmediatePayment(purchaseData)
        FS->>FS: CalculateSingleDueDate(approvalDate, terms)
        
    else Payment Terms = "Parcelado" (Installments)
        FS->>FS: CreateInstallmentPayment(purchaseData)
        FS->>FS: CalculateInstallmentSchedule(totalAmount, installmentCount)
        
        Note over FS: Create multiple payment records for installments
        loop For each Installment
            FS->>FS: CreateInstallmentData(amount, dueDate, installmentNumber)
        end
        
    else Payment Terms = "Prazo Fixo" (Fixed Term)
        FS->>FS: CreateFixedTermPayment(purchaseData)
        FS->>FS: CalculateFixedDueDate(approvalDate, fixedDays)
    end
    
    %% ==========================================
    %% ACCOUNT PAYABLE CREATION
    %% ==========================================
    
    Note over FS: Create main AccountPayable record
    FS->>FR: CreateAccountPayableAsync(accountPayableData)
    activate FR
    
    FR->>DB: BEGIN TRANSACTION
    
    FR->>DB: INSERT INTO AccountPayable (main record)
    DB-->>FR: AccountPayableId
    
    Note over FR: Create payment schedule records
    opt Has Payment Schedule (Installments)
        loop For each Scheduled Payment
            FR->>DB: INSERT INTO PaymentSchedule
            DB-->>FR: PaymentScheduleId
        end
    end
    
    FR->>DB: COMMIT TRANSACTION
    DB-->>FR: Transaction committed
    
    FR-->>FS: AccountPayable created with PaymentSchedule
    deactivate FR
    
    %% ==========================================
    %% CASH FLOW AND FINANCIAL UPDATES
    %% ==========================================
    
    Note over FS: Update cash flow projections
    FS->>FS: UpdateCashFlowProjections(accountPayable)
    
    FS->>FR: UpdateCashFlowAsync(outflowProjections)
    activate FR
    FR->>DB: UPDATE CashFlowProjection SET projected_outflows
    DB-->>FR: Cash flow updated
    FR-->>FS: Cash flow projections updated
    deactivate FR
    
    %% ==========================================
    %% NOTIFICATIONS AND ALERTS
    %% ==========================================
    
    Note over FS: Generate notifications and alerts
    FS->>NS: NotifyFinanceTeamAsync(accountPayableCreated)
    activate NS
    NS->>NS: SendEmailNotification(financeTeam, paymentSchedule)
    NS-->>FS: Notification sent
    deactivate NS
    
    opt Payment Due Soon (< 7 days)
        FS->>NS: CreatePaymentReminderAsync(accountPayable)
        activate NS
        NS->>NS: SchedulePaymentReminder(dueDate - 2 days)
        NS-->>FS: Reminder scheduled
        deactivate NS
    end
    
    opt Large Amount (> Threshold)
        FS->>NS: NotifyManagersAsync(largePaymentAlert)
        activate NS
        NS->>NS: SendHighValuePaymentAlert(managers, amount)
        NS-->>FS: Alert sent
        deactivate NS
    end
    
    %% ==========================================
    %% CROSS-DOMAIN STATUS UPDATE
    %% ==========================================
    
    Note over FS: Notify Purchasing domain of successful AP creation
    FS->>EB: PublishAsync(AccountPayableCreated event)
    activate EB
    EB->>PS: Handle AccountPayableCreated event
    activate PS
    
    PS->>PR: UpdatePurchaseOrderFinancialStatusAsync(purchaseOrderId, "AP_Created")
    activate PR
    PR->>DB: UPDATE PurchaseOrder SET FinancialStatus = 'AP_Created'
    DB-->>PR: Status updated
    PR-->>PS: Purchase order updated
    deactivate PR
    
    PS-->>EB: Event handled successfully
    deactivate PS
    deactivate EB
    
    FS-->>EB: AccountPayable creation completed
    deactivate FS
    
    %% ==========================================
    %% ERROR HANDLING SCENARIOS
    %% ==========================================
    
    Note over UI, DB: Error Handling Scenarios
    
    alt Insufficient Budget Available
        VS-->>FS: ValidationResult (Budget exceeded)
        FS->>FS: LogBudgetConstraintError(purchaseOrderId)
        FS->>EB: PublishAsync(PaymentApprovalRejected event)
        FS->>NS: NotifyAsync(purchasingTeam, "Budget constraint")
    end
    
    alt Supplier Financial Hold
        VS-->>FS: ValidationResult (Supplier blocked)
        FS->>FS: LogSupplierHoldError(supplierId)
        FS->>EB: PublishAsync(SupplierPaymentBlocked event)
        FS->>NS: NotifyAsync(purchasingTeam, "Supplier hold")
    end
    
    alt Database Transaction Failure
        FR-->>FS: DatabaseError (Transaction failed)
        FS->>FS: LogDatabaseError(purchaseOrderId, error)
        FS->>FS: InitiateRetryMechanism(paymentApprovedEvent)
        FS->>NS: NotifyAsync(techTeam, "AP creation failed")
    end
    
    alt Cash Flow Alert Triggered
        FS->>FS: CashFlowThresholdExceeded(projectedOutflow)
        FS->>NS: NotifyAsync(financeManagers, "Cash flow alert")
        FS->>FS: SuggestPaymentDeferral(accountPayable)
    end
```

## ðŸŽ¯ Detailed Component Responsibilities

### **ðŸ›’ Purchasing Controller**
```
Responsibilities:
â”œâ”€â”€ ðŸ” Validate user authorization for payment approval
â”œâ”€â”€ ðŸ“‹ HTTP request validation and routing
â”œâ”€â”€ ðŸ’° Payment approval workflow initiation
â”œâ”€â”€ ðŸ“Š Return appropriate response codes
â””â”€â”€ ðŸ” Log approval-related activities

Authorization Levels:
â”œâ”€â”€ ðŸ‘¤ Standard User: < $1,000
â”œâ”€â”€ ðŸ‘‘ Manager: < $10,000
â”œâ”€â”€ ðŸ’¼ Director: < $50,000
â””â”€â”€ ðŸ¢ Executive: Any amount
```

### **âš™ï¸ Purchasing Service**
```
Payment Approval Logic:
â”œâ”€â”€ ðŸ“‹ Purchase order validation and verification
â”œâ”€â”€ ðŸ’° Payment amount calculation and verification
â”œâ”€â”€ ðŸ“… Payment terms interpretation and application
â”œâ”€â”€ ðŸ“Š Supplier status and relationship validation
â””â”€â”€ ðŸ“¡ Cross-domain event coordination

Calculation Responsibilities:
â”œâ”€â”€ ðŸ’° Net payment amount (total - discounts)
â”œâ”€â”€ ðŸ“… Due date calculation based on terms
â”œâ”€â”€ ðŸ¦ Payment method determination
â”œâ”€â”€ ðŸ’¸ Early payment discount evaluation
â””â”€â”€ ðŸ“Š Budget impact assessment

Data Preparation:
â”œâ”€â”€ ðŸ“¦ AccountPayable entity data mapping
â”œâ”€â”€ ðŸ“… Payment schedule data preparation
â”œâ”€â”€ ðŸ¢ Supplier financial information compilation
â”œâ”€â”€ ðŸ“Š Purchase order reference data
â””â”€â”€ ðŸ’¾ Audit trail data preparation
```

### **ðŸ’³ Financial Service**
```
AccountPayable Creation Logic:
â”œâ”€â”€ ðŸ“Š Financial validation and business rules
â”œâ”€â”€ ðŸ’° Payment terms processing and interpretation
â”œâ”€â”€ ðŸ“… Payment schedule generation
â”œâ”€â”€ ðŸ’¸ Cash flow impact calculation
â””â”€â”€ ðŸ”” Alert and notification management

Payment Terms Handling:
â”œâ”€â”€ ðŸ’µ Ã€ Vista: Immediate payment processing
â”œâ”€â”€ ðŸ“Š Parcelado: Installment schedule creation
â”œâ”€â”€ ðŸ“… Prazo Fixo: Fixed term due date calculation
â”œâ”€â”€ ðŸ’° Early Payment: Discount calculation
â””â”€â”€ ðŸ¦ Custom Terms: Flexible payment arrangements

Financial Impact Analysis:
â”œâ”€â”€ ðŸ’¸ Cash flow projection updates
â”œâ”€â”€ ðŸ“Š Budget utilization tracking
â”œâ”€â”€ ðŸ¦ Working capital impact assessment
â”œâ”€â”€ ðŸ“ˆ Financial ratio impact evaluation
â””â”€â”€ âš ï¸ Alert threshold monitoring
```

## ðŸ’° Payment Terms Processing

### **ðŸ“… Payment Terms Types**
```
Ã€ Vista (Immediate Payment):
â”œâ”€â”€ ðŸ“… Due Date: Approval date + 0-3 days
â”œâ”€â”€ ðŸ’° Discount: Often includes early payment discount
â”œâ”€â”€ ðŸ¦ Method: Bank transfer, cash, check
â”œâ”€â”€ ðŸ’¸ Cash Flow: Immediate outflow
â””â”€â”€ ðŸ“Š Frequency: Common for small suppliers

Parcelado (Installment Payment):
â”œâ”€â”€ ðŸ“… Due Dates: Monthly installments
â”œâ”€â”€ ðŸ’° Interest: May include interest charges
â”œâ”€â”€ ðŸ“Š Installments: 2-12 payments typically
â”œâ”€â”€ ðŸ’¸ Cash Flow: Spread over time
â””â”€â”€ ðŸ“‹ Use Case: Large purchases, equipment

Prazo Fixo (Fixed Term):
â”œâ”€â”€ ðŸ“… Due Date: Approval date + fixed days (30/60/90)
â”œâ”€â”€ ðŸ’° Standard: Most common business terms
â”œâ”€â”€ ðŸ¦ Method: Bank transfer typically
â”œâ”€â”€ ðŸ’¸ Cash Flow: Single future outflow
â””â”€â”€ ðŸ“Š Supplier Relationship: Standard terms

Custom Terms:
â”œâ”€â”€ ðŸ“… Due Date: Negotiated terms
â”œâ”€â”€ ðŸ’° Complex: May include milestones
â”œâ”€â”€ ðŸ“Š Special Cases: Large contracts
â”œâ”€â”€ ðŸ’¸ Cash Flow: Varies by agreement
â””â”€â”€ ðŸ“‹ Approval: Requires special authorization
```

### **ðŸ’° Calculation Logic**
```
Payment Amount Calculation:
â”œâ”€â”€ ðŸ§® Base Amount = Sum of all received items
â”œâ”€â”€ ðŸ’¸ Discounts Applied = Early payment, volume, etc.
â”œâ”€â”€ ðŸ“Š Taxes Included = As per local regulations
â”œâ”€â”€ ðŸ’° Final Amount = Base - Discounts + Taxes
â””â”€â”€ âœ… Validation = Amount matches purchase order

Due Date Calculation:
â”œâ”€â”€ ðŸ“… Start Date = Payment approval date
â”œâ”€â”€ â° Business Days = Exclude weekends/holidays
â”œâ”€â”€ ðŸ“Š Supplier Terms = Apply negotiated terms
â”œâ”€â”€ ðŸ“… Final Due Date = Start + Terms (business days)
â””â”€â”€ âœ… Validation = Date is in future and reasonable

Installment Calculation:
â”œâ”€â”€ ðŸ’° Principal = Total amount / installment count
â”œâ”€â”€ ðŸ“Š Interest = Applied to remaining balance
â”œâ”€â”€ ðŸ“… Schedule = Monthly intervals from approval
â”œâ”€â”€ ðŸ’¸ Final Amount = Principal + accrued interest
â””â”€â”€ âœ… Validation = Sum equals total amount
```

## ðŸ”’ Validation and Security

### **âœ… Purchase Order Validations**
```
Status Validations:
â”œâ”€â”€ âœ… Purchase order exists and is accessible
â”œâ”€â”€ âœ… Status is "Received" (items confirmed received)
â”œâ”€â”€ âœ… Not already approved for payment
â”œâ”€â”€ âœ… Not cancelled or voided
â””â”€â”€ âœ… All required fields are populated

Financial Validations:
â”œâ”€â”€ ðŸ’° Total amount is positive and reasonable
â”œâ”€â”€ ðŸ¦ Supplier bank details are valid
â”œâ”€â”€ ðŸ“Š Budget allocation is sufficient
â”œâ”€â”€ ðŸ’¸ Payment method is supported
â””â”€â”€ ðŸ“… Payment terms are valid

Supplier Validations:
â”œâ”€â”€ ðŸ¢ Supplier is active and not blocked
â”œâ”€â”€ ðŸ’° No outstanding issues or disputes
â”œâ”€â”€ ðŸ¦ Banking information is current
â”œâ”€â”€ ðŸ“Š Credit status is acceptable
â””â”€â”€ ðŸ“‹ Contract terms are valid
```

### **ðŸ” Authorization Matrix**
```
Approval Limits by Role:
â”œâ”€â”€ ðŸ‘¤ Purchasing Agent: $0 - $1,000
â”œâ”€â”€ ðŸ‘‘ Purchasing Manager: $1,001 - $10,000
â”œâ”€â”€ ðŸ’¼ Department Director: $10,001 - $50,000
â”œâ”€â”€ ðŸ¢ Finance Director: $50,001 - $250,000
â””â”€â”€ ðŸ‘” Executive: $250,001+

Additional Requirements:
â”œâ”€â”€ ðŸ” Dual approval for amounts > $25,000
â”œâ”€â”€ ðŸ“Š Budget owner approval for budget impact
â”œâ”€â”€ ðŸ‘‘ Department head approval for new suppliers
â”œâ”€â”€ ðŸ’¼ Finance approval for payment term changes
â””â”€â”€ ðŸ¢ Executive approval for policy exceptions
```

## ðŸ“Š Financial Impact Analysis

### **ðŸ’¸ Cash Flow Calculations**
```
Immediate Impact:
â”œâ”€â”€ ðŸ’° Current Cash Position Assessment
â”œâ”€â”€ ðŸ“Š Available Credit Line Evaluation
â”œâ”€â”€ ðŸ’¸ Immediate Liquidity Requirements
â”œâ”€â”€ ðŸ“… Other Payments Due Same Period
â””â”€â”€ âš ï¸ Cash Flow Alert Thresholds

Projected Impact:
â”œâ”€â”€ ðŸ“ˆ 7-day cash flow projection update
â”œâ”€â”€ ðŸ“Š 30-day cash flow projection update
â”œâ”€â”€ ðŸ’° 90-day cash flow trend analysis
â”œâ”€â”€ ðŸ“… Seasonal payment pattern consideration
â””â”€â”€ ðŸŽ¯ Working capital impact assessment

Risk Assessment:
â”œâ”€â”€ âš ï¸ Liquidity risk evaluation
â”œâ”€â”€ ðŸ“Š Concentration risk (single supplier)
â”œâ”€â”€ ðŸ’° Credit risk (supplier default)
â”œâ”€â”€ ðŸ“… Timing risk (payment clustering)
â””â”€â”€ ðŸ¦ Banking relationship impact
```

### **ðŸ“‹ Alert Thresholds**
```
Cash Flow Alerts:
â”œâ”€â”€ ðŸš¨ Critical: Available cash < 7 days operating expense
â”œâ”€â”€ âš ï¸ Warning: Available cash < 15 days operating expense
â”œâ”€â”€ ðŸ“Š Notice: Single payment > 5% of monthly budget
â”œâ”€â”€ ðŸ’° Large: Single payment > $50,000
â””â”€â”€ ðŸ“… Timing: Multiple large payments same week

Operational Alerts:
â”œâ”€â”€ ðŸ¢ Supplier Concentration: > 20% of monthly spend
â”œâ”€â”€ ðŸ“Š Budget Variance: Payment exceeds budget by > 10%
â”œâ”€â”€ ðŸ’¸ Payment Terms: Terms longer than 60 days
â”œâ”€â”€ ðŸ¦ Banking: Payment method change from standard
â””â”€â”€ ðŸ“… Timing: Payment due during cash flow shortage
```

## ðŸ”„ Error Handling and Recovery

### **âŒ Error Scenarios**
```
Financial Validation Failures:
â”œâ”€â”€ ðŸ’° Insufficient budget allocation
â”œâ”€â”€ ðŸ¦ Supplier account blocked/frozen
â”œâ”€â”€ ðŸ“Š Payment amount exceeds limits
â”œâ”€â”€ ðŸ“… Invalid payment terms specified
â””â”€â”€ ðŸ’¸ Cash flow constraint violation

Technical Failures:
â”œâ”€â”€ ðŸ—„ï¸ Database transaction timeout
â”œâ”€â”€ ðŸ“¡ Event publishing failure
â”œâ”€â”€ ðŸ’¾ Data consistency violation
â”œâ”€â”€ ðŸ”Œ External service unavailable
â””â”€â”€ ðŸš¨ System resource exhaustion

Business Logic Errors:
â”œâ”€â”€ ðŸ“‹ Purchase order status inconsistency
â”œâ”€â”€ ðŸ¢ Supplier status change during processing
â”œâ”€â”€ ðŸ’° Concurrent payment approval conflict
â”œâ”€â”€ ðŸ“Š Budget allocation race condition
â””â”€â”€ ðŸ“… Due date calculation error
```

### **ðŸ”§ Recovery Mechanisms**
```
Retry Strategies:
â”œâ”€â”€ ðŸ” Exponential backoff for transient failures
â”œâ”€â”€ ðŸŽ¯ Circuit breaker for external services
â”œâ”€â”€ ðŸ“Š Dead letter queue for failed events
â”œâ”€â”€ ðŸš¨ Manual intervention for business errors
â””â”€â”€ ðŸ”„ Automatic reconciliation processes

Compensation Actions:
â”œâ”€â”€ ðŸ”„ Reverse AccountPayable creation on failure
â”œâ”€â”€ ðŸ“Š Restore purchase order status
â”œâ”€â”€ ðŸ’° Release budget allocation
â”œâ”€â”€ ðŸ“¡ Publish compensation events
â””â”€â”€ ðŸ”” Notify relevant stakeholders

Data Integrity Recovery:
â”œâ”€â”€ ðŸ“Š Cross-domain consistency checks
â”œâ”€â”€ ðŸŽ¯ Reconciliation reporting
â”œâ”€â”€ ðŸ”„ Manual correction workflows
â”œâ”€â”€ ðŸ“‹ Audit trail maintenance
â””â”€â”€ ðŸš¨ Health check monitoring
```

## ðŸ“ˆ Performance and Monitoring

### **âš¡ Performance Targets**
```
Response Time SLAs:
â”œâ”€â”€ ðŸŽ¯ Payment approval response: < 3 seconds
â”œâ”€â”€ ðŸ“Š AccountPayable creation: < 5 seconds
â”œâ”€â”€ ðŸ’¸ Cash flow update: < 2 seconds
â”œâ”€â”€ ðŸ”” Notification delivery: < 10 seconds
â””â”€â”€ ðŸ“‹ Cross-domain sync: < 15 seconds

Throughput Targets:
â”œâ”€â”€ ðŸ“Š 100+ payment approvals per hour
â”œâ”€â”€ ðŸ’° 1000+ AccountPayable records per day
â”œâ”€â”€ ðŸ“ˆ Support 50+ concurrent approvals
â””â”€â”€ ðŸŽ¯ Maintain > 99.5% success rate
```

### **ðŸ“Š Monitoring Metrics**
```
Business Metrics:
â”œâ”€â”€ ðŸ’° Average payment approval time
â”œâ”€â”€ ðŸ“Š Payment approval success rate
â”œâ”€â”€ ðŸ’¸ Cash flow projection accuracy
â”œâ”€â”€ ðŸ¦ Supplier payment compliance
â””â”€â”€ ðŸ“… Due date accuracy

Technical Metrics:
â”œâ”€â”€ ðŸ”Œ Cross-domain event latency
â”œâ”€â”€ ðŸ“Š Database transaction performance
â”œâ”€â”€ ðŸ’¾ System resource utilization
â”œâ”€â”€ ðŸš¨ Error rate by component
â””â”€â”€ ðŸ“ˆ Alert response effectiveness
```

---

**Arquivo**: `02-purchase-to-payable-flow.md`  
**Fluxo**: PurchaseOrder â†’ AccountPayable (AutomÃ¡tico)  
**DomÃ­nios**: Purchasing â†’ Financial  
**Complexidade**: âš ï¸ MÃ©dia (6+ participantes, 15+ interaÃ§Ãµes)  
**AtualizaÃ§Ã£o**: 16/06/2025
