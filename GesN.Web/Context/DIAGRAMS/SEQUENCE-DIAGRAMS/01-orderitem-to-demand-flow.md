# ðŸ”„ SEQUENCE DIAGRAM - OrderItem â†’ Demand Generation Flow

## ðŸŽ¯ VisÃ£o Geral
Diagrama de sequÃªncia detalhado mostrando o fluxo automÃ¡tico de geraÃ§Ã£o de demands a partir de OrderItems quando um pedido Ã© confirmado. Este Ã© um dos fluxos mais crÃ­ticos do sistema, envolvendo integraÃ§Ã£o entre domÃ­nios de Vendas e ProduÃ§Ã£o, com lÃ³gica complexa dependendo do tipo de produto (Simple, Composite, Group).

## ðŸ“Š Complexidade do Fluxo
- **ðŸš¨ Alta Complexidade**: Multiple product types, cross-domain integration, complex business rules
- **ðŸ‘¥ Participantes**: 8+ system components
- **ðŸ”„ InteraÃ§Ãµes**: 20+ interactions per order item
- **ðŸŒ Cross-Domain**: Sales â†’ Production integration
- **ðŸ“‹ ValidaÃ§Ãµes**: Product availability, configuration validation, business rules

## ðŸŽ¯ Trigger Event
**OrderConfirmed** (Sales Domain) â†’ Automatic demand generation for all OrderItems

## ðŸ“ Sequence Diagram

```mermaid
sequenceDiagram
    participant UI as ðŸ‘¤ User Interface
    participant SC as ðŸŽ® Sales Controller
    participant SS as âš™ï¸ Sales Service  
    participant SR as ðŸ—„ï¸ Sales Repository
    participant EB as ðŸ“¡ Event Bus
    participant PS as ðŸ­ Production Service
    participant PR as ðŸ—„ï¸ Production Repository
    participant ProdS as ðŸ“¦ Product Service
    participant VS as âœ… Validation Service
    participant NS as ðŸ”” Notification Service
    participant DB as ðŸ’¾ Database
    
    Note over UI, DB: OrderItem â†’ Demand Generation Flow (Triggered by OrderConfirmed)
    
    %% ==========================================
    %% SALES DOMAIN - ORDER CONFIRMATION
    %% ==========================================
    
    UI->>SC: POST /Order/{orderId}/Confirm
    Note over SC: Validate order can be confirmed
    
    SC->>SS: ConfirmOrderAsync(orderId)
    activate SS
    
    SS->>SR: GetOrderWithItemsAsync(orderId)
    activate SR
    SR->>DB: SELECT order + items + configurations
    DB-->>SR: Order + OrderItems[]
    SR-->>SS: OrderEntry with OrderItems[]
    deactivate SR
    
    Note over SS: Validate order business rules
    SS->>VS: ValidateOrderForConfirmationAsync(order)
    activate VS
    VS->>VS: Check customer credit limit
    VS->>VS: Validate delivery date feasibility
    VS->>VS: Ensure all items properly configured
    VS-->>SS: ValidationResult (Success)
    deactivate VS
    
    SS->>SR: UpdateOrderStatusAsync(orderId, "Confirmed")
    activate SR
    SR->>DB: UPDATE OrderEntry SET Status = 'Confirmed'
    DB-->>SR: Success
    SR-->>SS: Success
    deactivate SR
    
    Note over SS: Publish OrderConfirmed event
    SS->>EB: PublishAsync(OrderConfirmed event)
    activate EB
    EB->>EB: Route event to Production domain
    deactivate EB
    
    SS-->>SC: Order confirmed successfully
    deactivate SS
    SC-->>UI: 200 OK - Order confirmed
    
    %% ==========================================
    %% PRODUCTION DOMAIN - DEMAND GENERATION
    %% ==========================================
    
    Note over EB, DB: Production Domain Processing (Background)
    
    EB->>PS: Handle OrderConfirmed event
    activate PS
    
    Note over PS: Extract OrderItems from event
    PS->>PS: ParseOrderItemsFromEvent(orderConfirmedEvent)
    
    Note over PS: Process each OrderItem individually
    loop For each OrderItem in Order
        PS->>ProdS: GetProductByIdAsync(orderItem.ProductId)
        activate ProdS
        ProdS-->>PS: Product with ProductType
        deactivate ProdS
        
        Note over PS: Strategy pattern based on ProductType
        alt ProductType = Simple
            Note over PS: Simple Product Strategy
            PS->>PS: CreateSimpleProductDemand(orderItem)
            PS->>PR: CreateDemandAsync(demandData)
            activate PR
            PR->>DB: INSERT INTO Demand (Simple product)
            DB-->>PR: DemandId
            PR-->>PS: Demand created with DemandId
            deactivate PR
            
            PS->>EB: PublishAsync(DemandCreated event)
            
        else ProductType = Composite
            Note over PS: Composite Product Strategy
            PS->>PS: CreateCompositeProductDemand(orderItem)
            
            %% Create main Demand
            PS->>PR: CreateDemandAsync(demandData)
            activate PR
            PR->>DB: INSERT INTO Demand (Composite product)
            DB-->>PR: DemandId
            PR-->>PS: Demand created with DemandId
            deactivate PR
            
            %% Extract configuration and create ProductCompositions
            PS->>PS: ExtractConfigurationFromOrderItem(orderItem)
            PS->>ProdS: ValidateProductConfigurationAsync(productId, configuration)
            activate ProdS
            
            ProdS->>ProdS: GetProductHierarchiesAsync(productId)
            ProdS->>ProdS: ValidateComponentSelections(configuration)
            ProdS-->>PS: ConfigurationValidationResult (Success)
            deactivate ProdS
            
            Note over PS: Create ProductComposition for each component
            loop For each Component in Configuration
                PS->>PR: CreateProductCompositionAsync(demandId, componentData)
                activate PR
                PR->>DB: INSERT INTO ProductComposition
                DB-->>PR: ProductCompositionId
                PR-->>PS: ProductComposition created
                deactivate PR
            end
            
            PS->>EB: PublishAsync(DemandCreated event)
            PS->>EB: PublishAsync(ProductCompositionRequired event)
            
        else ProductType = Group
            Note over PS: Group Product Strategy
            PS->>PS: CreateGroupProductDemands(orderItem)
            
            %% Get group configuration
            PS->>ProdS: GetProductGroupConfigurationAsync(productId)
            activate ProdS
            ProdS-->>PS: ProductGroup with items and exchange rules
            deactivate ProdS
            
            PS->>PS: ExplodeGroupIntoConcreteProducts(orderItem, productGroup)
            
            Note over PS: Create separate Demand for each concrete product
            loop For each Concrete Product in Group
                PS->>PR: CreateDemandAsync(concreteProductDemandData)
                activate PR
                PR->>DB: INSERT INTO Demand (from group explosion)
                DB-->>PR: DemandId
                PR-->>PS: Demand created with DemandId
                deactivate PR
                
                PS->>EB: PublishAsync(DemandCreated event)
            end
            
            %% Apply exchange rules if any
            opt Has Exchange Rules
                PS->>PS: ApplyGroupExchangeRules(demands, exchangeRules)
                PS->>PR: UpdateDemandsWithExchanges(demands)
                activate PR
                PR->>DB: UPDATE Demands with exchange adjustments
                DB-->>PR: Success
                PR-->>PS: Demands updated
                deactivate PR
            end
        end
        
        %% ==========================================
        %% CROSS-DOMAIN VALIDATIONS
        %% ==========================================
        
        Note over PS: Cross-domain validations for each created demand
        PS->>PS: ValidateDemandBusinessRules(demand)
        
        %% Check ingredient availability (for Production planning)
        PS->>PS: CheckIngredientAvailabilityAsync(demand)
        
        %% Estimate production time and cost
        PS->>PS: CalculateProductionEstimatesAsync(demand)
        
        %% Update demand with estimates
        PS->>PR: UpdateDemandEstimatesAsync(demandId, estimates)
        activate PR
        PR->>DB: UPDATE Demand SET estimates
        DB-->>PR: Success
        PR-->>PS: Demand updated with estimates
        deactivate PR
    end
    
    %% ==========================================
    %% NOTIFICATIONS AND STATUS UPDATES
    %% ==========================================
    
    Note over PS: All demands created, notify relevant parties
    PS->>NS: NotifyProductionTeamAsync(demandsCreated)
    activate NS
    NS->>NS: Send notification to production managers
    NS-->>PS: Notification sent
    deactivate NS
    
    %% Update order status in Sales domain
    PS->>EB: PublishAsync(OrderSentToProduction event)
    activate EB
    EB->>SS: Handle OrderSentToProduction event
    activate SS
    SS->>SR: UpdateOrderStatusAsync(orderId, "SentToProduction")
    activate SR
    SR->>DB: UPDATE OrderEntry SET Status = 'SentToProduction'
    DB-->>SR: Success
    SR-->>SS: Status updated
    deactivate SR
    SS-->>EB: Event handled
    deactivate SS
    deactivate EB
    
    PS-->>EB: Demand generation completed
    deactivate PS
    
    %% ==========================================
    %% ERROR HANDLING SCENARIOS
    %% ==========================================
    
    Note over UI, DB: Error Handling Scenarios
    
    alt Product Configuration Invalid
        ProdS-->>PS: ConfigurationValidationResult (Failed)
        PS->>PS: LogValidationError(orderItemId, errors)
        PS->>EB: PublishAsync(DemandCreationFailed event)
        PS->>NS: NotifyAsync(salesTeam, "Configuration Error")
    end
    
    alt Insufficient Ingredient Stock
        PS->>PS: InsufficientStockDetected(demandId, ingredients)
        PS->>EB: PublishAsync(LowStockAlert event)
        PS->>NS: NotifyAsync(purchasingTeam, "Stock Alert")
        PS->>PR: UpdateDemandStatusAsync(demandId, "Pending-Ingredients")
    end
    
    alt Database Transaction Failure
        PR-->>PS: DatabaseError (Transaction failed)
        PS->>PS: LogError(orderItemId, "Demand creation failed")
        PS->>PS: InitiateRetryMechanism(orderItemId)
        PS->>NS: NotifyAsync(techTeam, "System Error")
    end
```

## ðŸŽ¯ Detailed Component Responsibilities

### **ðŸŽ® Sales Controller**
```
Responsibilities:
â”œâ”€â”€ ðŸ” Authentication and authorization validation
â”œâ”€â”€ ðŸ“‹ HTTP request validation and sanitization
â”œâ”€â”€ ðŸŽ¯ Route confirmation request to Sales Service
â”œâ”€â”€ ðŸ“Š Return appropriate HTTP response codes
â””â”€â”€ ðŸ” Log controller-level events and errors

Validation Points:
â”œâ”€â”€ âœ… User has permission to confirm orders
â”œâ”€â”€ âœ… Order ID format is valid
â”œâ”€â”€ âœ… Request payload is properly formatted
â””â”€â”€ âœ… Rate limiting and security checks
```

### **âš™ï¸ Sales Service**
```
Core Business Logic:
â”œâ”€â”€ ðŸ“‹ Order confirmation workflow orchestration
â”œâ”€â”€ âœ… Complex business rule validation
â”œâ”€â”€ ðŸ“Š Order status management
â”œâ”€â”€ ðŸ“¡ Event publishing coordination
â””â”€â”€ ðŸ”„ Cross-domain integration management

Validation Rules:
â”œâ”€â”€ ðŸ¦ Customer credit limit verification
â”œâ”€â”€ ðŸ“… Delivery date feasibility check
â”œâ”€â”€ ðŸ§© Product configuration completeness
â”œâ”€â”€ ðŸ“¦ Order item consistency validation
â””â”€â”€ ðŸ’° Pricing and total amount verification

Event Management:
â”œâ”€â”€ ðŸ“¤ Publish OrderConfirmed event
â”œâ”€â”€ ðŸ“¥ Handle OrderSentToProduction event
â”œâ”€â”€ ðŸ”„ Coordinate event sequencing
â””â”€â”€ ðŸ“Š Track event processing status
```

### **ðŸ­ Production Service**
```
Demand Generation Strategy:
â”œâ”€â”€ ðŸŽ¯ ProductType-based strategy selection
â”œâ”€â”€ ðŸ“Š Demand data model construction
â”œâ”€â”€ ðŸ§© Complex configuration processing
â”œâ”€â”€ ðŸ”„ Cross-domain data validation
â””â”€â”€ ðŸ“ˆ Production estimates calculation

Product Type Strategies:
â”œâ”€â”€ ðŸ”¹ Simple: 1:1 OrderItem to Demand mapping
â”œâ”€â”€ ðŸ”¶ Composite: 1:N with ProductComposition creation
â”œâ”€â”€ ðŸ”¸ Group: 1:N with product explosion and exchange rules
â””â”€â”€ âš™ï¸ Strategy pattern for extensibility

Cross-Domain Validations:
â”œâ”€â”€ ðŸ“¦ Product availability verification
â”œâ”€â”€ ðŸ§© Configuration validation with Product domain
â”œâ”€â”€ ðŸ¥˜ Ingredient availability checking
â”œâ”€â”€ â° Production capacity assessment
â””â”€â”€ ðŸ’° Cost estimation and validation
```

### **ðŸ“¦ Product Service Integration**
```
Product Data Retrieval:
â”œâ”€â”€ ðŸ” Product lookup by ID
â”œâ”€â”€ ðŸ“Š ProductType determination
â”œâ”€â”€ ðŸ§© Configuration rules retrieval
â”œâ”€â”€ ðŸ“‹ Component hierarchy access
â””â”€â”€ ðŸ”¸ Group explosion logic

Validation Services:
â”œâ”€â”€ âœ… Product configuration validation
â”œâ”€â”€ ðŸ—ï¸ Component compatibility checking
â”œâ”€â”€ ðŸ“Š Quantity and constraint validation
â”œâ”€â”€ ðŸ’° Pricing rule application
â””â”€â”€ ðŸ”„ Business rule enforcement
```

## ðŸ’¡ Business Rules and Constraints

### **ðŸ“‹ Order Confirmation Rules**
```
Pre-Confirmation Validations:
â”œâ”€â”€ ðŸ¦ Customer credit limit must not be exceeded
â”œâ”€â”€ ðŸ“… Delivery date must be achievable
â”œâ”€â”€ ðŸ§© All composite products must be fully configured
â”œâ”€â”€ ðŸ“¦ All products must be active and available
â”œâ”€â”€ ðŸ’° Order total must match sum of item totals
â””â”€â”€ ðŸ“‹ Minimum order requirements must be met

Post-Confirmation Rules:
â”œâ”€â”€ ðŸ”’ Confirmed orders cannot be modified (only cancelled)
â”œâ”€â”€ ðŸ“Š Order status must progress through defined states
â”œâ”€â”€ ðŸŽ¯ All order items must generate production demands
â”œâ”€â”€ ðŸ“¡ Financial accounts must be created automatically
â””â”€â”€ ðŸ“… Delivery commitments become binding
```

### **ðŸ­ Demand Generation Rules**
```
Universal Demand Rules:
â”œâ”€â”€ ðŸ“Š One OrderItem may generate 1:N Demands
â”œâ”€â”€ ðŸŽ¯ Each Demand represents one concrete product
â”œâ”€â”€ ðŸ“… Demand due date = Order delivery date - production time
â”œâ”€â”€ ðŸ“¦ Demand quantity respects OrderItem quantity
â””â”€â”€ ðŸ”„ Demand status starts as "Pending"

Product Type Specific Rules:
â”œâ”€â”€ ðŸ”¹ Simple Products:
â”‚   â”œâ”€â”€ 1 OrderItem â†’ 1 Demand (exact mapping)
â”‚   â”œâ”€â”€ No composition tasks required
â”‚   â””â”€â”€ Straightforward production workflow
â”œâ”€â”€ ðŸ”¶ Composite Products:
â”‚   â”œâ”€â”€ 1 OrderItem â†’ 1 Demand + N ProductComposition
â”‚   â”œâ”€â”€ Configuration must be validated
â”‚   â”œâ”€â”€ Component availability must be checked
â”‚   â””â”€â”€ Production tasks created per component
â””â”€â”€ ðŸ”¸ Product Groups:
    â”œâ”€â”€ 1 OrderItem â†’ N Demands (one per concrete product)
    â”œâ”€â”€ Group configuration exploded into concrete products
    â”œâ”€â”€ Exchange rules applied if configured
    â””â”€â”€ Separate production workflows per concrete product
```

### **ðŸ”„ Integration Rules**
```
Cross-Domain Consistency:
â”œâ”€â”€ ðŸ“Š Order status updates must be synchronized
â”œâ”€â”€ ðŸŽ¯ Demand creation must be atomic per OrderItem
â”œâ”€â”€ ðŸ“¡ Event publishing must follow correct sequence
â”œâ”€â”€ ðŸ”„ Failure in Production must notify Sales
â””â”€â”€ ðŸ“‹ All state changes must be auditable

Data Integrity Rules:
â”œâ”€â”€ ðŸŽ¯ Demand must always reference valid OrderItem
â”œâ”€â”€ ðŸ“¦ Product references must be consistent across domains
â”œâ”€â”€ ðŸ§© Configuration data must be preserved exactly
â”œâ”€â”€ ðŸ’° Quantity and pricing must remain consistent
â””â”€â”€ ðŸ“… Dates and timelines must be logically consistent
```

## âš¡ Performance Considerations

### **ðŸš€ Optimization Strategies**
```
Batch Processing:
â”œâ”€â”€ ðŸ“Š Process multiple OrderItems in single transaction
â”œâ”€â”€ ðŸŽ¯ Bulk database operations where possible
â”œâ”€â”€ ðŸ“¡ Batch event publishing to reduce overhead
â””â”€â”€ ðŸ”„ Group similar operations together

Caching Strategies:
â”œâ”€â”€ ðŸ“¦ Cache Product data and configurations
â”œâ”€â”€ ðŸ§© Cache component hierarchies and rules
â”œâ”€â”€ ðŸ’° Cache pricing calculations
â””â”€â”€ âœ… Cache validation results for repeated patterns

Async Processing:
â”œâ”€â”€ ðŸ“¡ Event-driven asynchronous processing
â”œâ”€â”€ ðŸ”„ Non-blocking cross-domain calls
â”œâ”€â”€ ðŸ“Š Background demand generation processing
â””â”€â”€ ðŸŽ¯ Parallel processing of independent OrderItems
```

### **ðŸ“Š Performance Metrics**
```
Target SLAs:
â”œâ”€â”€ ðŸŽ¯ OrderItem â†’ Demand creation: < 2 seconds per item
â”œâ”€â”€ ðŸ“Š Order confirmation response: < 5 seconds total
â”œâ”€â”€ ðŸ”„ Cross-domain event propagation: < 10 seconds
â””â”€â”€ ðŸ’¾ Database transaction completion: < 1 second

Scalability Targets:
â”œâ”€â”€ ðŸ“ˆ Support 1000+ OrderItems per order
â”œâ”€â”€ ðŸŽ¯ Handle 100+ concurrent order confirmations
â”œâ”€â”€ ðŸ“Š Process 10,000+ demands per hour
â””â”€â”€ ðŸ”„ Maintain < 1% error rate under load
```

## ðŸš¨ Error Handling and Recovery

### **ðŸ”§ Error Scenarios**
```
Product Configuration Errors:
â”œâ”€â”€ âŒ Invalid component selections
â”œâ”€â”€ âŒ Incompatible component combinations
â”œâ”€â”€ âŒ Missing required components
â””â”€â”€ ðŸ”„ Recovery: Reject confirmation, notify user

Resource Availability Errors:
â”œâ”€â”€ âŒ Insufficient ingredient stock
â”œâ”€â”€ âŒ Production capacity exceeded
â”œâ”€â”€ âŒ Component temporarily unavailable
â””â”€â”€ ðŸ”„ Recovery: Create demand with "Pending" status

System Integration Errors:
â”œâ”€â”€ âŒ Database transaction failures
â”œâ”€â”€ âŒ Event publishing failures
â”œâ”€â”€ âŒ Cross-domain communication timeouts
â””â”€â”€ ðŸ”„ Recovery: Retry mechanism with exponential backoff

Business Rule Violations:
â”œâ”€â”€ âŒ Credit limit exceeded
â”œâ”€â”€ âŒ Delivery date impossible
â”œâ”€â”€ âŒ Product restrictions violated
â””â”€â”€ ðŸ”„ Recovery: Block confirmation, provide clear error message
```

### **ðŸ”„ Recovery Mechanisms**
```
Retry Strategies:
â”œâ”€â”€ ðŸ” Exponential backoff for transient failures
â”œâ”€â”€ ðŸŽ¯ Circuit breaker for external service failures
â”œâ”€â”€ ðŸ“Š Dead letter queue for failed events
â””â”€â”€ ðŸš¨ Manual intervention queue for complex errors

Compensation Actions:
â”œâ”€â”€ ðŸ”„ Reverse demand creation on failure
â”œâ”€â”€ ðŸ“Š Restore order status on rollback
â”œâ”€â”€ ðŸ“¡ Publish compensation events
â””â”€â”€ ðŸ”” Notify relevant parties of failures

Data Consistency Recovery:
â”œâ”€â”€ ðŸ“Š Eventual consistency through event replay
â”œâ”€â”€ ðŸŽ¯ Reconciliation processes for data drift
â”œâ”€â”€ ðŸ”„ Audit trail for manual correction
â””â”€â”€ ðŸ“‹ Health check monitoring for early detection
```

## ðŸ“‹ Validation Matrix

### **ðŸŽ¯ Validation Layers**
| Validation Type | Layer | Scope | Error Handling |
|----------------|-------|-------|----------------|
| **Input Validation** | Controller | HTTP request format | 400 Bad Request |
| **Business Rules** | Service | Order confirmation rules | Business exception |
| **Product Config** | Cross-Domain | Component compatibility | Configuration error |
| **Resource Check** | Production | Ingredient availability | Resource warning |
| **Data Integrity** | Repository | Database constraints | Transaction rollback |

### **âœ… Validation Checklist**
```
Order Level:
â”œâ”€â”€ âœ… Order exists and is in correct status
â”œâ”€â”€ âœ… Customer is active and has sufficient credit
â”œâ”€â”€ âœ… Delivery date is feasible
â”œâ”€â”€ âœ… All required fields are populated
â””â”€â”€ âœ… Order total matches calculated total

OrderItem Level:
â”œâ”€â”€ âœ… Product is active and available
â”œâ”€â”€ âœ… Quantity is positive and within limits
â”œâ”€â”€ âœ… Configuration is complete and valid
â”œâ”€â”€ âœ… Pricing is accurate and current
â””â”€â”€ âœ… Special requirements are achievable

Demand Level:
â”œâ”€â”€ âœ… Production capacity is available
â”œâ”€â”€ âœ… Required ingredients are in stock
â”œâ”€â”€ âœ… Production lead time allows delivery date
â”œâ”€â”€ âœ… All components are available
â””â”€â”€ âœ… Cost estimates are within budgets
```

---

**Arquivo**: `01-orderitem-to-demand-flow.md`  
**Fluxo**: OrderItem â†’ Demand (AutomÃ¡tico)  
**DomÃ­nios**: Sales â†’ Production  
**Complexidade**: ðŸš¨ Alta (8+ participantes, 20+ interaÃ§Ãµes)  
**AtualizaÃ§Ã£o**: 16/06/2025
