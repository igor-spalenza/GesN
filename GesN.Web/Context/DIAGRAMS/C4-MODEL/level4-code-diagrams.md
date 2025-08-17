# 💻 C4 LEVEL 4 - CODE DIAGRAMS

## 🎯 Visão Geral
Diagramas de código do sistema GesN mostrando classes, interfaces e implementações específicas. Este nível mostra os detalhes de implementação mais importantes para desenvolvedores, incluindo herança, padrões de design e contratos de interface.

## 🧬 Product Domain - TPH Inheritance Implementation

```mermaid
classDiagram
    %% ==========================================
    %% CORE PRODUCT HIERARCHY (TABLE PER HIERARCHY)
    %% ==========================================
    
    class Product {
        <<abstract>>
        +string Id
        +ProductType ProductType*
        +string Name
        +string Description
        +decimal Price
        +int QuantityPrice
        +decimal UnitPrice
        +decimal Cost
        +string CategoryId
        +string SKU
        +string ImageUrl
        +string Note
        +int AssemblyTime
        +string AssemblyInstructions
        +string StateCode
        +DateTime CreatedDate
        +DateTime ModifiedDate
        
        +CalculateUnitPrice() decimal
        +IsActive() bool
        +ValidateBusinessRules() bool*
        +GetAssemblyInstructions() string
    }
    
    class ProductType {
        <<enumeration>>
        Simple
        Composite
        Group
    }
    
    class SimpleProduct {
        +ValidateBusinessRules() bool
        +CalculateCost() decimal
        +GetRequiredIngredients() List~ProductIngredient~
    }
    
    class CompositeProduct {
        +ICollection~CompositeProductXHierarchy~ Hierarchies
        +ValidateBusinessRules() bool
        +CalculateCost() decimal
        +ValidateComponentConfiguration(config) bool
        +GetAvailableComponents() List~ProductComponent~
        +CalculateCompositePrice(selections) decimal
    }
    
    class ProductGroup {
        +ICollection~ProductGroupItem~ GroupItems
        +ICollection~ProductGroupExchangeRule~ ExchangeRules
        +ValidateBusinessRules() bool
        +CalculateCost() decimal
        +ValidateGroupConfiguration(config) bool
        +ApplyExchangeRules(exchanges) bool
        +CalculateGroupPrice(configuration) decimal
    }
    
    %% ==========================================
    %% PRODUCT DOMAIN INTERFACES
    %% ==========================================
    
    class IProductService {
        <<interface>>
        +GetByIdAsync(id) Task~Product~
        +GetByTypeAsync(type) Task~List~Product~~
        +CreateAsync(product) Task~string~
        +UpdateAsync(product) Task~bool~
        +DeleteAsync(id) Task~bool~
        +ValidateProductAsync(product) Task~bool~
        +CalculateProductCostAsync(id) Task~decimal~
        +GetActiveProductsAsync() Task~List~Product~~
        +SearchProductsAsync(criteria) Task~List~Product~~
        +ConfigureCompositeProductAsync(config) Task~ConfigResult~
        +ValidateGroupExchangeAsync(exchange) Task~bool~
    }
    
    class IProductRepository {
        <<interface>>
        +GetByIdAsync(id) Task~Product~
        +GetByTypeAsync(type) Task~IEnumerable~Product~~
        +CreateAsync(product) Task~string~
        +UpdateAsync(product) Task~bool~
        +DeleteAsync(id) Task~bool~
        +GetWithCategoryAsync(id) Task~Product~
        +GetActiveAsync() Task~IEnumerable~Product~~
        +GetWithComponentsAsync(id) Task~CompositeProduct~
        +GetGroupWithItemsAsync(id) Task~ProductGroup~
    }
    
    class ProductService {
        -IProductRepository _productRepository
        -IProductCategoryService _categoryService
        -IProductComponentService _componentService
        -ILogger~ProductService~ _logger
        -IMapper _mapper
        
        +GetByIdAsync(id) Task~Product~
        +CreateAsync(product) Task~string~
        +UpdateAsync(product) Task~bool~
        +ValidateProductAsync(product) Task~bool~
        +CalculateProductCostAsync(id) Task~decimal~
        +ConfigureCompositeProductAsync(config) Task~ConfigResult~
        -ConfigureProductType(product) void
        -ValidateSimpleProduct(product) bool
        -ValidateCompositeProduct(product) bool
        -ValidateProductGroup(product) bool
    }
    
    class ProductRepository {
        -IDbConnection _connection
        -ILogger~ProductRepository~ _logger
        -string _connectionString
        
        +GetByIdAsync(id) Task~Product~
        +CreateAsync(product) Task~string~
        +UpdateAsync(product) Task~bool~
        +DeleteAsync(id) Task~bool~
        +GetWithCategoryAsync(id) Task~Product~
        +GetActiveAsync() Task~IEnumerable~Product~~
        -MapToProduct(reader) Product
        -GetProductType(reader) ProductType
        -ExecuteWithRetryAsync(operation) Task~T~
    }
    
    %% ==========================================
    %% RELATIONSHIPS
    %% ==========================================
    
    Product <|-- SimpleProduct : inherits
    Product <|-- CompositeProduct : inherits
    Product <|-- ProductGroup : inherits
    Product --> ProductType : uses
    
    IProductService <|.. ProductService : implements
    IProductRepository <|.. ProductRepository : implements
    ProductService --> IProductRepository : depends on
    ProductService --> Product : manages
    
    %% ==========================================
    %% STYLING
    %% ==========================================
    
    classDef abstractClass fill:#00a86b,stroke:#00a86b,stroke-width:3px,color:white
    classDef concreteClass fill:#2dd4aa,stroke:#00a86b,stroke-width:2px,color:black
    classDef interface fill:#a7f3d0,stroke:#00a86b,stroke-width:2px,color:black
    classDef implementation fill:#6ee7b7,stroke:#00a86b,stroke-width:2px,color:black
    classDef enum fill:#e5e7eb,stroke:#6b7280,stroke-width:2px,color:black
    
    class Product abstractClass
    class SimpleProduct,CompositeProduct,ProductGroup concreteClass
    class IProductService,IProductRepository interface
    class ProductService,ProductRepository implementation
    class ProductType enum
```

## 🏗️ Service Layer Interfaces by Domain

### **💰 Sales Domain Contracts**

```mermaid
classDiagram
    %% ==========================================
    %% SALES DOMAIN INTERFACES
    %% ==========================================
    
    class ISalesService {
        <<interface>>
        +CreateOrderAsync(orderDto) Task~string~
        +ConfirmOrderAsync(orderId) Task~bool~
        +AddOrderItemAsync(orderId, itemDto) Task~string~
        +ConfigureOrderItemAsync(itemId, config) Task~bool~
        +CalculateOrderTotalAsync(orderId) Task~decimal~
        +UpdateOrderStatusAsync(orderId, status) Task~bool~
        +CancelOrderAsync(orderId, reason) Task~bool~
        +GetOrderByIdAsync(orderId) Task~OrderEntry~
        +GetCustomerOrdersAsync(customerId) Task~List~OrderEntry~~
        +SearchOrdersAsync(criteria) Task~PagedResult~OrderEntry~~
    }
    
    class IOrderItemService {
        <<interface>>
        +AddItemToOrderAsync(orderId, productId, qty) Task~string~
        +ConfigureCompositeItemAsync(itemId, config) Task~bool~
        +ExplodeGroupItemAsync(itemId, groupConfig) Task~List~string~~
        +RemoveItemFromOrderAsync(itemId) Task~bool~
        +ValidateItemConfigurationAsync(itemId) Task~ValidationResult~
        +CalculateItemPriceAsync(itemId) Task~decimal~
    }
    
    class ICustomerService {
        <<interface>>
        +CreateCustomerAsync(customerDto) Task~string~
        +UpdateCustomerAsync(customerId, customerDto) Task~bool~
        +GetCustomerByIdAsync(customerId) Task~Customer~
        +SearchCustomersAsync(criteria) Task~PagedResult~Customer~~
        +SyncWithGoogleContactsAsync(customerId) Task~bool~
        +ValidateCustomerCreditAsync(customerId, amount) Task~bool~
    }
    
    %% ==========================================
    %% SALES DOMAIN IMPLEMENTATIONS
    %% ==========================================
    
    class SalesService {
        -ISalesRepository _salesRepository
        -IProductService _productService
        -IProductionService _productionService
        -IFinancialService _financialService
        -IEventBus _eventBus
        -ILogger~SalesService~ _logger
        
        +CreateOrderAsync(orderDto) Task~string~
        +ConfirmOrderAsync(orderId) Task~bool~
        +AddOrderItemAsync(orderId, itemDto) Task~string~
        +ConfigureOrderItemAsync(itemId, config) Task~bool~
        -ValidateOrderBusinessRules(order) ValidationResult
        -PublishOrderEvents(orderId, eventType) Task
        -CalculateDynamicPricing(orderItem) decimal
    }
    
    class OrderItemService {
        -IOrderItemRepository _orderItemRepository
        -IProductService _productService
        -IDemandService _demandService
        -IEventBus _eventBus
        
        +AddItemToOrderAsync(orderId, productId, qty) Task~string~
        +ConfigureCompositeItemAsync(itemId, config) Task~bool~
        +ExplodeGroupItemAsync(itemId, groupConfig) Task~List~string~~
        -CreateDemandsForItemAsync(orderItem) Task
        -ValidateProductConfiguration(config) bool
        -CalculateConfigurationPrice(config) decimal
    }
    
    %% ==========================================
    %% RELATIONSHIPS
    %% ==========================================
    
    ISalesService <|.. SalesService : implements
    IOrderItemService <|.. OrderItemService : implements
    ICustomerService <|.. CustomerService : implements
    
    SalesService --> IProductService : uses
    SalesService --> IEventBus : publishes events
    OrderItemService --> IProductService : validates products
    OrderItemService --> IDemandService : creates demands
    
    %% ==========================================
    %% STYLING
    %% ==========================================
    
    classDef salesInterface fill:#fed7aa,stroke:#f36b21,stroke-width:2px,color:black
    classDef salesImpl fill:#f36b21,stroke:#f36b21,stroke-width:2px,color:white
    
    class ISalesService,IOrderItemService,ICustomerService salesInterface
    class SalesService,OrderItemService,CustomerService salesImpl
```

### **🏭 Production Domain Contracts**

```mermaid
classDiagram
    %% ==========================================
    %% PRODUCTION DOMAIN INTERFACES
    %% ==========================================
    
    class IProductionService {
        <<interface>>
        +CreateDemandAsync(orderItemId, productId, qty) Task~string~
        +CreateProductCompositionAsync(demandId, config) Task~List~string~~
        +StartProductionAsync(demandId) Task~bool~
        +CompleteProductionAsync(demandId) Task~bool~
        +CreateProductionOrderAsync(demandIds) Task~string~
        +ScheduleProductionOrderAsync(poId, date) Task~bool~
        +GetDemandStatusAsync(demandId) Task~DemandStatus~
        +GetProductionProgressAsync(poId) Task~ProgressInfo~
    }
    
    class IDemandService {
        <<interface>>
        +CreateFromOrderItemAsync(orderItem) Task~string~
        +UpdateStatusAsync(demandId, status) Task~bool~
        +GetByOrderItemAsync(orderItemId) Task~List~Demand~~
        +GetPendingDemandsAsync() Task~List~Demand~~
        +CalculateEstimatesAsync(demandId) Task~EstimateInfo~
        +ConsumeIngredientsAsync(demandId) Task~bool~
    }
    
    class IProductCompositionService {
        <<interface>>
        +CreateForDemandAsync(demandId, components) Task~List~string~~
        +StartCompositionAsync(compositionId) Task~bool~
        +CompleteCompositionAsync(compositionId) Task~bool~
        +GetByDemandAsync(demandId) Task~List~ProductComposition~~
        +TrackProgressAsync(demandId) Task~ProgressInfo~
    }
    
    %% ==========================================
    %% PRODUCTION DOMAIN IMPLEMENTATIONS
    %% ==========================================
    
    class ProductionService {
        -IProductionRepository _productionRepository
        -IPurchasingService _purchasingService
        -IIntegrationService _integrationService
        -IEventBus _eventBus
        -ILogger _logger
        
        +CreateDemandAsync(orderItemId, productId, qty) Task~string~
        +CreateProductCompositionAsync(demandId, config) Task~List~string~~
        +StartProductionAsync(demandId) Task~bool~
        +CreateProductionOrderAsync(demandIds) Task~string~
        +ScheduleProductionOrderAsync(poId, date) Task~bool~
        -ValidateProductionCapacity(date, demands) bool
        -CreateGoogleCalendarEvent(productionOrder) Task
        -NotifyStakeholders(event, data) Task
    }
    
    class DemandService {
        -IDemandRepository _demandRepository
        -IProductService _productService
        -IPurchasingService _purchasingService
        -IEventBus _eventBus
        
        +CreateFromOrderItemAsync(orderItem) Task~string~
        +UpdateStatusAsync(demandId, status) Task~bool~
        +ConsumeIngredientsAsync(demandId) Task~bool~
        -DetermineProductionStrategy(orderItem) ProductionStrategy
        -CreateCompositionTasks(demand, config) Task
        -ReserveIngredients(demand) Task~bool~
    }
    
    %% ==========================================
    %% RELATIONSHIPS
    %% ==========================================
    
    IProductionService <|.. ProductionService : implements
    IDemandService <|.. DemandService : implements
    IProductCompositionService <|.. ProductCompositionService : implements
    
    ProductionService --> IDemandService : orchestrates
    ProductionService --> IPurchasingService : coordinates stock
    DemandService --> IProductService : validates products
    
    %% ==========================================
    %% STYLING
    %% ==========================================
    
    classDef productionInterface fill:#fed7aa,stroke:#fba81d,stroke-width:2px,color:black
    classDef productionImpl fill:#fba81d,stroke:#fba81d,stroke-width:2px,color:black
    
    class IProductionService,IDemandService,IProductCompositionService productionInterface
    class ProductionService,DemandService,ProductCompositionService productionImpl
```

## 🔧 Repository Pattern Implementation

### **🗄️ Generic Repository Base**

```mermaid
classDiagram
    %% ==========================================
    %% REPOSITORY PATTERN BASE
    %% ==========================================
    
    class IRepository~T~ {
        <<interface>>
        +GetByIdAsync(id) Task~T~
        +GetAllAsync() Task~IEnumerable~T~~
        +CreateAsync(entity) Task~string~
        +UpdateAsync(entity) Task~bool~
        +DeleteAsync(id) Task~bool~
        +ExistsAsync(id) Task~bool~
        +GetPagedAsync(page, size) Task~PagedResult~T~~
    }
    
    class BaseRepository~T~ {
        <<abstract>>
        #IDbConnection Connection
        #ILogger Logger
        #string TableName
        #string TenantId
        
        +GetByIdAsync(id) Task~T~
        +GetAllAsync() Task~IEnumerable~T~~
        +CreateAsync(entity) Task~string~
        +UpdateAsync(entity) Task~bool~
        +DeleteAsync(id) Task~bool~
        +ExistsAsync(id) Task~bool~
        +GetPagedAsync(page, size) Task~PagedResult~T~~
        #ExecuteQueryAsync~TResult~(sql, parameters) Task~TResult~
        #ExecuteAsync(sql, parameters) Task~int~
        #MapEntity(reader) T*
        #GetInsertSql() string*
        #GetUpdateSql() string*
        #GetSelectSql() string*
        #ApplyTenantFilter(sql) string
    }
    
    %% ==========================================
    %% DOMAIN-SPECIFIC REPOSITORIES
    %% ==========================================
    
    class IProductRepository {
        <<interface>>
        +GetByTypeAsync(type) Task~IEnumerable~Product~~
        +GetWithCategoryAsync(id) Task~Product~
        +GetActiveAsync() Task~IEnumerable~Product~~
        +GetWithComponentsAsync(id) Task~CompositeProduct~
        +GetGroupWithItemsAsync(id) Task~ProductGroup~
        +SearchBySKUAsync(sku) Task~Product~
        +GetProductMetricsAsync() Task~ProductMetrics~
    }
    
    class ProductRepository {
        +GetByTypeAsync(type) Task~IEnumerable~Product~~
        +GetWithCategoryAsync(id) Task~Product~
        +GetActiveAsync() Task~IEnumerable~Product~~
        +GetWithComponentsAsync(id) Task~CompositeProduct~
        +GetGroupWithItemsAsync(id) Task~ProductGroup~
        +SearchBySKUAsync(sku) Task~Product~
        #MapEntity(reader) Product
        #GetInsertSql() string
        #GetUpdateSql() string
        #GetSelectSql() string
        -MapToProductType(reader) ProductType
        -MapProductHierarchy(reader) CompositeProduct
        -MapProductGroup(reader) ProductGroup
    }
    
    class ISalesRepository {
        <<interface>>
        +GetOrderWithItemsAsync(orderId) Task~OrderEntry~
        +GetCustomerOrdersAsync(customerId) Task~List~OrderEntry~~
        +GetOrdersByStatusAsync(status) Task~List~OrderEntry~~
        +GetOrdersByDateRangeAsync(start, end) Task~List~OrderEntry~~
        +SearchOrdersAsync(criteria) Task~PagedResult~OrderEntry~~
        +GetSalesMetricsAsync(period) Task~SalesMetrics~
    }
    
    class SalesRepository {
        +GetOrderWithItemsAsync(orderId) Task~OrderEntry~
        +GetCustomerOrdersAsync(customerId) Task~List~OrderEntry~~
        +GetOrdersByStatusAsync(status) Task~List~OrderEntry~~
        +SearchOrdersAsync(criteria) Task~PagedResult~OrderEntry~~
        #MapEntity(reader) OrderEntry
        -MapOrderItems(orderId) Task~List~OrderItem~~
        -MapCustomerData(reader) Customer
        -BuildSearchQuery(criteria) string
    }
    
    %% ==========================================
    %% RELATIONSHIPS
    %% ==========================================
    
    IRepository~T~ <|.. BaseRepository~T~ : implements
    BaseRepository~Product~ <|-- ProductRepository : extends
    BaseRepository~OrderEntry~ <|-- SalesRepository : extends
    IProductRepository <|.. ProductRepository : implements
    ISalesRepository <|.. SalesRepository : implements
    
    %% ==========================================
    %% STYLING
    %% ==========================================
    
    classDef baseInterface fill:#e5e7eb,stroke:#6b7280,stroke-width:2px,color:black
    classDef baseClass fill:#9ca3af,stroke:#6b7280,stroke-width:2px,color:white
    classDef specificInterface fill:#dbeafe,stroke:#3b82f6,stroke-width:2px,color:black
    classDef specificClass fill:#3b82f6,stroke:#3b82f6,stroke-width:2px,color:white
    
    class IRepository~T~ baseInterface
    class BaseRepository~T~ baseClass
    class IProductRepository,ISalesRepository specificInterface
    class ProductRepository,SalesRepository specificClass
```

## ⚡ Event-Driven Architecture

### **📡 Event Bus Implementation**

```mermaid
classDiagram
    %% ==========================================
    %% EVENT SYSTEM CONTRACTS
    %% ==========================================
    
    class IDomainEvent {
        <<interface>>
        +string EventId
        +DateTime OccurredOn
        +string AggregateId
        +string EventType
        +string TenantId
        +Dictionary~string,object~ EventData
    }
    
    class IEventBus {
        <<interface>>
        +PublishAsync~T~(T domainEvent) Task where T : IDomainEvent
        +SubscribeAsync~T~(Func~T,Task~ handler) void where T : IDomainEvent
        +UnsubscribeAsync~T~() void where T : IDomainEvent
    }
    
    class IEventHandler~T~ {
        <<interface>>
        +HandleAsync(T domainEvent) Task
    }
    
    %% ==========================================
    %% DOMAIN EVENTS
    %% ==========================================
    
    class OrderConfirmed {
        +string OrderId
        +string CustomerId
        +decimal TotalValue
        +DateTime DeliveryDate
        +List~OrderItemData~ Items
        +PaymentTerms PaymentTerms
    }
    
    class DemandCreated {
        +string DemandId
        +string OrderItemId
        +string ProductId
        +int Quantity
        +DateTime RequiredDate
        +ProductType ProductType
        +string Configuration
    }
    
    class ProductionCompleted {
        +string DemandId
        +string ProductionOrderId
        +DateTime CompletedDate
        +decimal ActualCost
        +int ActualTimeMinutes
        +List~string~ CompletedCompositions
    }
    
    class PaymentReceived {
        +string AccountReceivableId
        +string CustomerId
        +decimal Amount
        +DateTime PaymentDate
        +PaymentMethod PaymentMethod
        +string TransactionId
    }
    
    %% ==========================================
    %% EVENT HANDLERS
    %% ==========================================
    
    class OrderConfirmedHandler {
        -IProductionService _productionService
        -IFinancialService _financialService
        -ILogger _logger
        
        +HandleAsync(OrderConfirmed event) Task
        -CreateDemandsAsync(orderItems) Task
        -CreateAccountReceivableAsync(order) Task
        -NotifyProductionAsync(order) Task
    }
    
    class DemandCreatedHandler {
        -IPurchasingService _purchasingService
        -INotificationService _notificationService
        
        +HandleAsync(DemandCreated event) Task
        -CheckIngredientAvailabilityAsync(demand) Task
        -ReserveIngredientsAsync(demand) Task
        -TriggerPurchaseSuggestionAsync(lowStockItems) Task
    }
    
    class ProductionCompletedHandler {
        -ISalesService _salesService
        -IIntegrationService _integrationService
        
        +HandleAsync(ProductionCompleted event) Task
        -UpdateOrderStatusAsync(demandId) Task
        -NotifyCustomerAsync(order) Task
        -UpdateGoogleCalendarAsync(event) Task
    }
    
    %% ==========================================
    %% EVENT BUS IMPLEMENTATION
    %% ==========================================
    
    class InMemoryEventBus {
        -Dictionary~Type, List~Func~IDomainEvent,Task~~~ _handlers
        -IServiceProvider _serviceProvider
        -ILogger _logger
        
        +PublishAsync~T~(T domainEvent) Task
        +SubscribeAsync~T~(Func~T,Task~ handler) void
        +UnsubscribeAsync~T~() void
        -GetHandlersForEvent~T~() List~Func~T,Task~~
        -LogEventAsync(IDomainEvent event) Task
    }
    
    %% ==========================================
    %% RELATIONSHIPS
    %% ==========================================
    
    IDomainEvent <|.. OrderConfirmed : implements
    IDomainEvent <|.. DemandCreated : implements
    IDomainEvent <|.. ProductionCompleted : implements
    IDomainEvent <|.. PaymentReceived : implements
    
    IEventHandler~OrderConfirmed~ <|.. OrderConfirmedHandler : implements
    IEventHandler~DemandCreated~ <|.. DemandCreatedHandler : implements
    IEventHandler~ProductionCompleted~ <|.. ProductionCompletedHandler : implements
    
    IEventBus <|.. InMemoryEventBus : implements
    
    OrderConfirmedHandler --> IProductionService : uses
    OrderConfirmedHandler --> IFinancialService : uses
    DemandCreatedHandler --> IPurchasingService : uses
    
    %% ==========================================
    %% STYLING
    %% ==========================================
    
    classDef eventInterface fill:#f3e8ff,stroke:#8b5cf6,stroke-width:2px,color:black
    classDef eventClass fill:#8b5cf6,stroke:#8b5cf6,stroke-width:2px,color:white
    classDef handlerClass fill:#c084fc,stroke:#8b5cf6,stroke-width:2px,color:black
    classDef eventBusClass fill:#a855f7,stroke:#8b5cf6,stroke-width:2px,color:white
    
    class IDomainEvent,IEventBus,IEventHandler~T~ eventInterface
    class OrderConfirmed,DemandCreated,ProductionCompleted,PaymentReceived eventClass
    class OrderConfirmedHandler,DemandCreatedHandler,ProductionCompletedHandler handlerClass
    class InMemoryEventBus eventBusClass
```

## 🔐 Cross-Cutting Concerns

### **🛡️ Security and Validation**

```mermaid
classDiagram
    %% ==========================================
    %% VALIDATION FRAMEWORK
    %% ==========================================
    
    class IValidator~T~ {
        <<interface>>
        +Validate(T instance) ValidationResult
        +ValidateAsync(T instance) Task~ValidationResult~
    }
    
    class ValidationResult {
        +bool IsValid
        +List~ValidationError~ Errors
        +AddError(property, message) void
        +AddErrors(errors) void
    }
    
    class ValidationError {
        +string PropertyName
        +string ErrorMessage
        +string ErrorCode
        +object AttemptedValue
    }
    
    %% ==========================================
    %% DOMAIN VALIDATORS
    %% ==========================================
    
    class ProductValidator {
        +Validate(Product product) ValidationResult
        -ValidateProductType(Product product) bool
        -ValidatePricing(Product product) bool
        -ValidateSKUUniqueness(string sku) Task~bool~
        -ValidateAssemblyTime(int time) bool
    }
    
    class OrderValidator {
        +Validate(OrderEntry order) ValidationResult
        -ValidateCustomer(string customerId) Task~bool~
        -ValidateDeliveryDate(DateTime date) bool
        -ValidateOrderItems(List~OrderItem~ items) ValidationResult
        -ValidateBusinessRules(OrderEntry order) ValidationResult
    }
    
    class CompositeProductValidator {
        +Validate(CompositeProduct product) ValidationResult
        +ValidateConfiguration(ProductConfiguration config) ValidationResult
        -ValidateHierarchies(List~Hierarchy~ hierarchies) bool
        -ValidateComponents(List~Component~ components) bool
        -ValidateMinMaxQuantities(Configuration config) bool
    }
    
    %% ==========================================
    %% AUTHORIZATION FRAMEWORK
    %% ==========================================
    
    class IAuthorizationService {
        <<interface>>
        +CanAccessAsync(string userId, string resource) Task~bool~
        +CanPerformAsync(string userId, string action, string resource) Task~bool~
        +GetUserRolesAsync(string userId) Task~List~string~~
        +GetUserClaimsAsync(string userId) Task~List~Claim~~
    }
    
    class TenantAwareAuthorizationService {
        -IUserManager _userManager
        -IRoleManager _roleManager
        -string _tenantId
        
        +CanAccessAsync(string userId, string resource) Task~bool~
        +CanPerformAsync(string userId, string action, string resource) Task~bool~
        -ValidateTenantAccess(string userId) Task~bool~
        -EvaluateResourcePermissions(userId, resource) Task~bool~
        -CheckRoleBasedAccess(userId, action) Task~bool~
    }
    
    %% ==========================================
    %% AUDIT AND LOGGING
    %% ==========================================
    
    class IAuditService {
        <<interface>>
        +LogActionAsync(AuditLog log) Task
        +GetAuditTrailAsync(string entityId) Task~List~AuditLog~~
        +GetUserActionsAsync(string userId, period) Task~List~AuditLog~~
    }
    
    class AuditLog {
        +string Id
        +string TenantId
        +string UserId
        +string EntityType
        +string EntityId
        +string Action
        +DateTime Timestamp
        +Dictionary~string,object~ OldValues
        +Dictionary~string,object~ NewValues
        +string IPAddress
        +string UserAgent
    }
    
    class AuditService {
        -IAuditRepository _auditRepository
        -IHttpContextAccessor _httpContext
        -ILogger _logger
        
        +LogActionAsync(AuditLog log) Task
        +GetAuditTrailAsync(string entityId) Task~List~AuditLog~~
        -CaptureContextData() AuditContext
        -SerializeEntityChanges(entity, action) Dictionary~string,object~
    }
    
    %% ==========================================
    %% RELATIONSHIPS
    %% ==========================================
    
    IValidator~T~ <|.. ProductValidator : implements
    IValidator~T~ <|.. OrderValidator : implements
    IValidator~T~ <|.. CompositeProductValidator : implements
    
    IAuthorizationService <|.. TenantAwareAuthorizationService : implements
    IAuditService <|.. AuditService : implements
    
    ProductValidator --> ValidationResult : returns
    OrderValidator --> ValidationResult : returns
    AuditService --> AuditLog : creates
    
    %% ==========================================
    %% STYLING
    %% ==========================================
    
    classDef validationInterface fill:#fef3c7,stroke:#f59e0b,stroke-width:2px,color:black
    classDef validationClass fill:#fbbf24,stroke:#f59e0b,stroke-width:2px,color:black
    classDef authInterface fill:#dbeafe,stroke:#3b82f6,stroke-width:2px,color:black
    classDef authClass fill:#60a5fa,stroke:#3b82f6,stroke-width:2px,color:black
    classDef auditInterface fill:#f3e8ff,stroke:#8b5cf6,stroke-width:2px,color:black
    classDef auditClass fill:#c084fc,stroke:#8b5cf6,stroke-width:2px,color:black
    
    class IValidator~T~,ValidationResult,ValidationError validationInterface
    class ProductValidator,OrderValidator,CompositeProductValidator validationClass
    class IAuthorizationService authInterface
    class TenantAwareAuthorizationService authClass
    class IAuditService,AuditLog auditInterface
    class AuditService auditClass
```

## 📊 Performance and Monitoring

### **📈 Metrics and Health Checks**

```mermaid
classDiagram
    %% ==========================================
    %% MONITORING INTERFACES
    %% ==========================================
    
    class IHealthCheck {
        <<interface>>
        +CheckHealthAsync(HealthCheckContext context) Task~HealthCheckResult~
    }
    
    class IMetricsCollector {
        <<interface>>
        +IncrementCounter(string name, tags) void
        +RecordHistogram(string name, double value, tags) void
        +SetGauge(string name, double value, tags) void
        +StartTimer(string name) IDisposable
    }
    
    class IPerformanceProfiler {
        <<interface>>
        +StartProfileAsync(string operationName) Task~IDisposable~
        +GetMetricsAsync(period) Task~PerformanceMetrics~
        +GetSlowQueriesAsync(threshold) Task~List~SlowQuery~~
    }
    
    %% ==========================================
    %% HEALTH CHECK IMPLEMENTATIONS
    %% ==========================================
    
    class DatabaseHealthCheck {
        -IDbConnection _connection
        -ILogger _logger
        
        +CheckHealthAsync(HealthCheckContext context) Task~HealthCheckResult~
        -TestDatabaseConnection() Task~bool~
        -CheckTableIntegrity() Task~bool~
        -ValidateSchema() Task~bool~
    }
    
    class GoogleApisHealthCheck {
        -IIntegrationService _integrationService
        -IConfiguration _configuration
        
        +CheckHealthAsync(HealthCheckContext context) Task~HealthCheckResult~
        -CheckPeopleApiAsync() Task~bool~
        -CheckCalendarApiAsync() Task~bool~
        -CheckMapsApiAsync() Task~bool~
        -CheckApiQuotas() Task~QuotaStatus~
    }
    
    class AIServiceHealthCheck {
        -IAIService _aiService
        -IFileStorage _fileStorage
        
        +CheckHealthAsync(HealthCheckContext context) Task~HealthCheckResult~
        -CheckOCREngineAsync() Task~bool~
        -CheckMLModelsAsync() Task~bool~
        -CheckProcessingQueueAsync() Task~QueueStatus~
    }
    
    %% ==========================================
    %% METRICS COLLECTION
    %% ==========================================
    
    class PrometheusMetricsCollector {
        -MetricServer _metricServer
        -Dictionary~string,Counter~ _counters
        -Dictionary~string,Histogram~ _histograms
        -Dictionary~string,Gauge~ _gauges
        
        +IncrementCounter(string name, tags) void
        +RecordHistogram(string name, double value, tags) void
        +SetGauge(string name, double value, tags) void
        +StartTimer(string name) IDisposable
        -GetOrCreateCounter(string name) Counter
        -GetOrCreateHistogram(string name) Histogram
        -GetOrCreateGauge(string name) Gauge
    }
    
    class PerformanceProfiler {
        -IMetricsCollector _metricsCollector
        -ILogger _logger
        -ConcurrentDictionary~string,OperationMetrics~ _operations
        
        +StartProfileAsync(string operationName) Task~IDisposable~
        +GetMetricsAsync(period) Task~PerformanceMetrics~
        +GetSlowQueriesAsync(threshold) Task~List~SlowQuery~~
        -TrackOperation(string name, TimeSpan duration) void
        -AnalyzePerformancePatterns() PerformanceAnalysis
    }
    
    %% ==========================================
    %% MONITORING DATA MODELS
    %% ==========================================
    
    class HealthCheckResult {
        +HealthStatus Status
        +string Description
        +Exception Exception
        +Dictionary~string,object~ Data
        +TimeSpan Duration
    }
    
    class PerformanceMetrics {
        +string OperationName
        +int RequestCount
        +double AverageResponseTime
        +double P95ResponseTime
        +double P99ResponseTime
        +double ErrorRate
        +DateTime PeriodStart
        +DateTime PeriodEnd
    }
    
    class SlowQuery {
        +string Query
        +TimeSpan ExecutionTime
        +DateTime ExecutedAt
        +Dictionary~string,object~ Parameters
        +string StackTrace
    }
    
    %% ==========================================
    %% RELATIONSHIPS
    %% ==========================================
    
    IHealthCheck <|.. DatabaseHealthCheck : implements
    IHealthCheck <|.. GoogleApisHealthCheck : implements
    IHealthCheck <|.. AIServiceHealthCheck : implements
    
    IMetricsCollector <|.. PrometheusMetricsCollector : implements
    IPerformanceProfiler <|.. PerformanceProfiler : implements
    
    DatabaseHealthCheck --> HealthCheckResult : returns
    PerformanceProfiler --> PerformanceMetrics : produces
    PerformanceProfiler --> SlowQuery : tracks
    
    %% ==========================================
    %% STYLING
    %% ==========================================
    
    classDef monitoringInterface fill:#ecfdf5,stroke:#10b981,stroke-width:2px,color:black
    classDef healthCheckClass fill:#34d399,stroke:#10b981,stroke-width:2px,color:black
    classDef metricsClass fill:#6ee7b7,stroke:#10b981,stroke-width:2px,color:black
    classDef dataClass fill:#a7f3d0,stroke:#10b981,stroke-width:2px,color:black
    
    class IHealthCheck,IMetricsCollector,IPerformanceProfiler monitoringInterface
    class DatabaseHealthCheck,GoogleApisHealthCheck,AIServiceHealthCheck healthCheckClass
    class PrometheusMetricsCollector,PerformanceProfiler metricsClass
    class HealthCheckResult,PerformanceMetrics,SlowQuery dataClass
```

---

**Arquivo**: `level4-code-diagrams.md`  
**Nível C4**: 4 - Code  
**Audiência**: Desenvolvedores  
**Foco**: Classes, interfaces e implementações específicas  
**Atualização**: 16/06/2025
