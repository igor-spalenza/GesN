# ðŸ§© C4 LEVEL 3 - COMPONENT DIAGRAMS

## ðŸŽ¯ VisÃ£o Geral
Diagramas de componentes do sistema GesN mostrando como cada container Ã© decomposto em componentes. Este nÃ­vel mostra a organizaÃ§Ã£o interna dos bounded contexts e suas responsabilidades especÃ­ficas.

## ðŸ“Š Web Application - Component Overview

```mermaid
C4Component
    title Component Diagram - GesN Web Application
    
    Container_Boundary(web_app, "GesN Web Application") {
        
        %% Presentation Layer
        Component(product_controller, "Product Controller", "ASP.NET Core MVC", "Handle requests: catalog, configuration, management")
        Component(sales_controller, "Sales Controller", "ASP.NET Core MVC", "Handle requests: customers, orders, status tracking")
        Component(production_controller, "Production Controller", "ASP.NET Core MVC", "Handle requests: demands, execution, monitoring")
        Component(purchasing_controller, "Purchasing Controller", "ASP.NET Core MVC", "Handle requests: suppliers, purchase orders, IA processing")
        Component(financial_controller, "Financial Controller", "ASP.NET Core MVC", "Handle requests: accounts, transactions, reports")
        
        %% Service Layer (Domain Business Logic)
        Component(product_service, "Product Service", "C# Service", "Product management: types, configuration, validation")
        Component(sales_service, "Sales Service", "C# Service", "Order processing: creation, confirmation, lifecycle")
        Component(production_service, "Production Service", "C# Service", "Demand management: creation, execution, tracking")
        Component(purchasing_service, "Purchasing Service", "C# Service", "Purchase management: suppliers, orders, stock control")
        Component(financial_service, "Financial Service", "C# Service", "Financial management: accounts, transactions, cash flow")
        
        %% Repository Layer (Data Access)
        Component(product_repository, "Product Repository", "Dapper Repository", "CRUD operations: Product, ProductCategory, Components")
        Component(sales_repository, "Sales Repository", "Dapper Repository", "CRUD operations: Customer, OrderEntry, OrderItem")
        Component(production_repository, "Production Repository", "Dapper Repository", "CRUD operations: Demand, ProductComposition, ProductionOrder")
        Component(purchasing_repository, "Purchasing Repository", "Dapper Repository", "CRUD operations: Supplier, PurchaseOrder, Ingredient")
        Component(financial_repository, "Financial Repository", "Dapper Repository", "CRUD operations: Accounts, Transactions")
        
        %% Infrastructure Components
        Component(identity_service, "Identity Service", "ASP.NET Core Identity", "Authentication, authorization, user management")
        Component(integration_service, "Integration Service", "C# Service", "Google Workspace APIs: People, Calendar, Maps")
        Component(ai_service, "AI Service", "C# Service", "Document processing: OCR, ML, data extraction")
        Component(notification_service, "Notification Service", "C# Service", "Email, SMS, in-app notifications")
        Component(audit_service, "Audit Service", "C# Service", "Logging, tracking, compliance")
        Component(cache_service, "Cache Service", "In-Memory Cache", "Performance optimization, data caching")
        
        %% Cross-Cutting Components
        Component(event_bus, "Event Bus", "In-Memory Events", "Domain events coordination between bounded contexts")
        Component(validation_engine, "Validation Engine", "FluentValidation", "Business rules validation across domains")
        Component(mapping_service, "Mapping Service", "AutoMapper", "DTO mapping, data transformation")
    }
    
    ContainerDb(database, "SQLite Database", "SQLite", "Tenant-specific data storage")
    Container_Ext(google_apis, "Google Workspace APIs", "External APIs", "People, Calendar, Maps integration")
    Container_Ext(file_system, "File Storage", "File System", "Documents, images, reports")
    
    %% Controller â†’ Service relationships
    Rel(product_controller, product_service, "Uses", "Business logic")
    Rel(sales_controller, sales_service, "Uses", "Business logic")
    Rel(production_controller, production_service, "Uses", "Business logic")  
    Rel(purchasing_controller, purchasing_service, "Uses", "Business logic")
    Rel(financial_controller, financial_service, "Uses", "Business logic")
    
    %% Service â†’ Repository relationships
    Rel(product_service, product_repository, "Uses", "Data access")
    Rel(sales_service, sales_repository, "Uses", "Data access")
    Rel(production_service, production_repository, "Uses", "Data access")
    Rel(purchasing_service, purchasing_repository, "Uses", "Data access")
    Rel(financial_service, financial_repository, "Uses", "Data access")
    
    %% Repository â†’ Database relationships
    Rel(product_repository, database, "Reads/Writes", "SQL queries")
    Rel(sales_repository, database, "Reads/Writes", "SQL queries")
    Rel(production_repository, database, "Reads/Writes", "SQL queries")
    Rel(purchasing_repository, database, "Reads/Writes", "SQL queries")
    Rel(financial_repository, database, "Reads/Writes", "SQL queries")
    
    %% Cross-domain integrations (Customer-Supplier pattern)
    Rel(sales_service, product_service, "Uses", "Product catalog")
    Rel(sales_service, production_service, "Triggers", "Demand creation")
    Rel(sales_service, financial_service, "Triggers", "Account receivable")
    Rel(production_service, purchasing_service, "Triggers", "Stock consumption")
    Rel(purchasing_service, financial_service, "Triggers", "Account payable")
    
    %% Infrastructure service usage
    Rel(sales_controller, identity_service, "Uses", "Authorization")
    Rel(production_controller, identity_service, "Uses", "Authorization")
    Rel(purchasing_controller, identity_service, "Uses", "Authorization")
    Rel(financial_controller, identity_service, "Uses", "Authorization")
    Rel(product_controller, identity_service, "Uses", "Authorization")
    
    Rel(sales_service, event_bus, "Publishes", "Domain events")
    Rel(production_service, event_bus, "Publishes/Subscribes", "Domain events")
    Rel(purchasing_service, event_bus, "Publishes/Subscribes", "Domain events")
    Rel(financial_service, event_bus, "Subscribes", "Domain events")
    
    Rel(purchasing_service, ai_service, "Uses", "Document processing")
    Rel(production_service, integration_service, "Uses", "Google Calendar")
    Rel(sales_service, integration_service, "Uses", "Google Maps")
    Rel(purchasing_service, integration_service, "Uses", "Google People")
    
    %% External integrations
    Rel(integration_service, google_apis, "Calls", "HTTPS/REST")
    Rel(ai_service, file_system, "Reads/Writes", "Document files")
    
    %% Styling by domain
    UpdateElementStyle(product_controller, $bgColor="#00a86b", $fontColor="white")
    UpdateElementStyle(product_service, $bgColor="#00a86b", $fontColor="white")
    UpdateElementStyle(product_repository, $bgColor="#00a86b", $fontColor="white")
    
    UpdateElementStyle(sales_controller, $bgColor="#f36b21", $fontColor="white")
    UpdateElementStyle(sales_service, $bgColor="#f36b21", $fontColor="white")
    UpdateElementStyle(sales_repository, $bgColor="#f36b21", $fontColor="white")
    
    UpdateElementStyle(production_controller, $bgColor="#fba81d", $fontColor="black")
    UpdateElementStyle(production_service, $bgColor="#fba81d", $fontColor="black")
    UpdateElementStyle(production_repository, $bgColor="#fba81d", $fontColor="black")
    
    UpdateElementStyle(purchasing_controller, $bgColor="#0562aa", $fontColor="white")
    UpdateElementStyle(purchasing_service, $bgColor="#0562aa", $fontColor="white")
    UpdateElementStyle(purchasing_repository, $bgColor="#0562aa", $fontColor="white")
    
    UpdateElementStyle(financial_controller, $bgColor="#083e61", $fontColor="white")
    UpdateElementStyle(financial_service, $bgColor="#083e61", $fontColor="white")
    UpdateElementStyle(financial_repository, $bgColor="#083e61", $fontColor="white")
    
    UpdateElementStyle(identity_service, $bgColor="#8b5cf6", $fontColor="white")
    UpdateElementStyle(integration_service, $bgColor="#ef4444", $fontColor="white")
    UpdateElementStyle(ai_service, $bgColor="#ec4899", $fontColor="white")
    UpdateElementStyle(event_bus, $bgColor="#6b7280", $fontColor="white")
```

## ðŸ“¦ Detalhamento por Bounded Context

### **ðŸ“¦ PRODUCT DOMAIN COMPONENTS**

#### **ðŸŽ® Product Controller**
```
Responsabilidades:
â”œâ”€â”€ ðŸ“‹ ProductController
â”‚   â”œâ”€â”€ GET /Product - Listar produtos
â”‚   â”œâ”€â”€ GET /Product/{id} - Detalhes produto
â”‚   â”œâ”€â”€ POST /Product - Criar produto
â”‚   â”œâ”€â”€ PUT /Product/{id} - Atualizar produto
â”‚   â””â”€â”€ DELETE /Product/{id} - Remover produto
â”œâ”€â”€ ðŸ“‚ ProductCategoryController
â”‚   â”œâ”€â”€ GET /ProductCategory - Listar categorias
â”‚   â””â”€â”€ POST /ProductCategory - Criar categoria
â””â”€â”€ ðŸ§© ProductComponentController
    â”œâ”€â”€ GET /ProductComponent - Listar componentes
    â””â”€â”€ POST /ProductComponent - Criar componente

ValidaÃ§Ãµes:
â”œâ”€â”€ âœ… Authorize attributes por role
â”œâ”€â”€ ðŸ“Š Model validation (FluentValidation)
â”œâ”€â”€ ðŸ”’ Tenant isolation verification
â””â”€â”€ ðŸ“ Input sanitization
```

#### **âš™ï¸ Product Service**
```
MÃ©todos Principais:
â”œâ”€â”€ ðŸ“¦ CreateProductAsync(ProductCreateDto)
â”œâ”€â”€ ðŸ” GetProductByIdAsync(string id)
â”œâ”€â”€ ðŸ“‹ GetProductsByTypeAsync(ProductType type)
â”œâ”€â”€ âœ… ValidateProductConfigurationAsync(ProductConfigDto)
â”œâ”€â”€ ðŸ’° CalculateProductPriceAsync(string id, ConfigDto config)
â””â”€â”€ ðŸ”„ UpdateProductAsync(string id, ProductUpdateDto)

Regras de NegÃ³cio:
â”œâ”€â”€ ðŸŽ¯ Product type-specific validation (TPH)
â”œâ”€â”€ ðŸ§© Component hierarchy validation (Composite)
â”œâ”€â”€ ðŸ“Š Group item validation (Group)
â”œâ”€â”€ ðŸ’° Price calculation with components
â””â”€â”€ ðŸ” SKU uniqueness verification

IntegraÃ§Ãµes:
â”œâ”€â”€ ðŸŽ¯ Product Repository (data access)
â”œâ”€â”€ ðŸ”” Event Bus (ProductCreated, ProductUpdated)
â”œâ”€â”€ ðŸ’¾ Cache Service (product catalog caching)
â””â”€â”€ ðŸ“Š Validation Engine (business rules)
```

#### **ðŸ—„ï¸ Product Repository**
```
OperaÃ§Ãµes CRUD:
â”œâ”€â”€ ðŸ“¦ Products (base table with TPH discriminator)
â”œâ”€â”€ ðŸ“‚ ProductCategories
â”œâ”€â”€ ðŸ§© ProductComponents
â”œâ”€â”€ ðŸ—ï¸ ProductComponentHierarchy
â”œâ”€â”€ ðŸ”— CompositeProductXHierarchy
â”œâ”€â”€ ðŸ“¦ ProductGroupItems
â””â”€â”€ âš–ï¸ ProductGroupExchangeRules

Queries Especializadas:
â”œâ”€â”€ ðŸ” GetProductsWithConfigurationAsync()
â”œâ”€â”€ ðŸ“Š GetProductsByTypeAsync(ProductType)
â”œâ”€â”€ ðŸ§© GetComponentsByHierarchyAsync(string hierarchyId)
â”œâ”€â”€ ðŸ’° GetProductPricingDataAsync(string productId)
â””â”€â”€ ðŸ“ˆ GetProductMetricsAsync()

Performance:
â”œâ”€â”€ ðŸŽ¯ Eager loading para relacionamentos
â”œâ”€â”€ ðŸ’¾ Query result caching
â”œâ”€â”€ ðŸ“Š Optimized JOIN queries
â””â”€â”€ ðŸ” Indexed searches por SKU/Name
```

### **ðŸ’° SALES DOMAIN COMPONENTS**

#### **ðŸŽ® Sales Controllers**
```
CustomerController:
â”œâ”€â”€ GET /Customer - Lista paginada clientes
â”œâ”€â”€ POST /Customer - Criar cliente
â”œâ”€â”€ PUT /Customer/{id} - Atualizar cliente
â””â”€â”€ GET /Customer/{id}/Orders - HistÃ³rico pedidos

OrderController:
â”œâ”€â”€ GET /Order - Lista pedidos (filtros)
â”œâ”€â”€ GET /Order/{id} - Detalhes pedido
â”œâ”€â”€ POST /Order - Criar pedido (modal rÃ¡pido)
â”œâ”€â”€ PUT /Order/{id} - Atualizar pedido
â”œâ”€â”€ POST /Order/{id}/Confirm - Confirmar pedido
â”œâ”€â”€ POST /Order/{id}/Cancel - Cancelar pedido
â””â”€â”€ GET /Order/{id}/Status - Tracking em tempo real

OrderItemController:
â”œâ”€â”€ POST /Order/{orderId}/Items - Adicionar item
â”œâ”€â”€ PUT /OrderItem/{id} - Configurar item
â”œâ”€â”€ DELETE /OrderItem/{id} - Remover item
â””â”€â”€ POST /OrderItem/{id}/Configure - Config produtos compostos
```

#### **âš™ï¸ Sales Service**
```
Principais OperaÃ§Ãµes:
â”œâ”€â”€ ðŸ†• CreateOrderAsync(OrderCreateDto)
â”œâ”€â”€ âœ… ConfirmOrderAsync(string orderId)
â”œâ”€â”€ ðŸ“¦ AddOrderItemAsync(string orderId, OrderItemDto)
â”œâ”€â”€ ðŸ§© ConfigureCompositeProductAsync(string itemId, ConfigDto)
â”œâ”€â”€ ðŸ“Š CalculateOrderTotalAsync(string orderId)
â””â”€â”€ ðŸ”„ UpdateOrderStatusAsync(string orderId, OrderStatus status)

Business Logic Complexa:
â”œâ”€â”€ ðŸŽ¯ Product type detection (Simple/Composite/Group)
â”œâ”€â”€ ðŸ§© Composite product configuration validation
â”œâ”€â”€ ðŸ“¦ Group product explosion logic
â”œâ”€â”€ ðŸ’° Dynamic pricing calculation
â”œâ”€â”€ ðŸ“… Delivery date validation
â””â”€â”€ ðŸ‘¤ Customer credit limit verification

Event Publishing:
â”œâ”€â”€ ðŸ“‹ OrderCreated
â”œâ”€â”€ âœ… OrderConfirmed (triggers production + financial)
â”œâ”€â”€ ðŸ“¦ OrderItemAdded
â”œâ”€â”€ ðŸ§© ProductConfigured
â””â”€â”€ âŒ OrderCancelled
```

### **ðŸ­ PRODUCTION DOMAIN COMPONENTS**

#### **ðŸŽ® Production Controller**
```
DemandController:
â”œâ”€â”€ GET /Demand - Lista demands (filtros por status)
â”œâ”€â”€ GET /Demand/{id} - Detalhes demand + composition
â”œâ”€â”€ PUT /Demand/{id}/Status - Atualizar status
â””â”€â”€ POST /Demand/{id}/Start - Iniciar produÃ§Ã£o

ProductionOrderController:
â”œâ”€â”€ GET /ProductionOrder - Lista ordens produÃ§Ã£o
â”œâ”€â”€ POST /ProductionOrder - Criar ordem (agrupa demands)
â”œâ”€â”€ PUT /ProductionOrder/{id}/Schedule - Agendar produÃ§Ã£o
â”œâ”€â”€ POST /ProductionOrder/{id}/Start - Iniciar lote
â””â”€â”€ GET /ProductionOrder/{id}/Progress - Progresso tempo real

ProductCompositionController:
â”œâ”€â”€ GET /Demand/{id}/Composition - Lista tarefas
â”œâ”€â”€ PUT /ProductComposition/{id}/Start - Iniciar tarefa
â”œâ”€â”€ PUT /ProductComposition/{id}/Complete - Finalizar tarefa
â””â”€â”€ POST /ProductComposition/{id}/ConsumeIngredient - Registrar consumo
```

#### **âš™ï¸ Production Service**
```
Demand Management:
â”œâ”€â”€ ðŸ†• CreateDemandFromOrderItemAsync(OrderItem)
â”œâ”€â”€ ðŸ§© CreateProductCompositionAsync(Demand, ProductConfig)
â”œâ”€â”€ ðŸ“Š CalculateDemandEstimatesAsync(string demandId)
â”œâ”€â”€ âš¡ ProcessDemandStatusChangeAsync(string demandId, DemandStatus)
â””â”€â”€ ðŸ”„ SyncDemandWithOrderAsync(string demandId)

Production Execution:
â”œâ”€â”€ ðŸ­ CreateProductionOrderAsync(List<string> demandIds)
â”œâ”€â”€ â° ScheduleProductionAsync(string productionOrderId, DateTime date)
â”œâ”€â”€ â–¶ï¸ StartProductionAsync(string productionOrderId)
â”œâ”€â”€ ðŸ“Š TrackProductionProgressAsync(string productionOrderId)
â””â”€â”€ âœ… CompleteProductionAsync(string productionOrderId)

Integrations:
â”œâ”€â”€ ðŸ›’ Purchasing Service (ingredient consumption)
â”œâ”€â”€ ðŸ’° Sales Service (demand status updates)
â”œâ”€â”€ ðŸ“… Google Calendar (production scheduling)
â””â”€â”€ ðŸ”” Notification Service (alerts)
```

### **ðŸ›’ PURCHASING DOMAIN COMPONENTS**

#### **ðŸŽ® Purchasing Controllers**
```
SupplierController:
â”œâ”€â”€ GET /Supplier - Lista fornecedores
â”œâ”€â”€ POST /Supplier - Criar fornecedor
â”œâ”€â”€ PUT /Supplier/{id} - Atualizar fornecedor
â””â”€â”€ GET /Supplier/{id}/Performance - MÃ©tricas fornecedor

PurchaseOrderController:
â”œâ”€â”€ GET /PurchaseOrder - Lista ordens compra
â”œâ”€â”€ POST /PurchaseOrder - Criar ordem (manual/IA)
â”œâ”€â”€ POST /PurchaseOrder/UploadFiscalDocument - Upload nota fiscal
â”œâ”€â”€ PUT /PurchaseOrder/{id}/Send - Enviar ao fornecedor
â”œâ”€â”€ POST /PurchaseOrder/{id}/Receive - Registrar recebimento
â””â”€â”€ GET /PurchaseOrder/{id}/Status - Status tracking

IngredientController:
â”œâ”€â”€ GET /Ingredient - Lista ingredientes
â”œâ”€â”€ POST /Ingredient - Criar ingrediente
â”œâ”€â”€ GET /Ingredient/{id}/Stock - NÃ­veis estoque
â””â”€â”€ GET /Ingredient/LowStock - Alertas estoque mÃ­nimo
```

#### **âš™ï¸ Purchasing Service**
```
Purchase Order Management:
â”œâ”€â”€ ðŸ†• CreatePurchaseOrderAsync(PurchaseOrderDto)
â”œâ”€â”€ ðŸ¤– ProcessFiscalDocumentWithAIAsync(Stream document)
â”œâ”€â”€ âœ… ValidateAIExtractedDataAsync(AIExtractedData)
â”œâ”€â”€ ðŸ“¤ SendPurchaseOrderAsync(string orderId)
â”œâ”€â”€ ðŸ“¦ ReceivePurchaseOrderAsync(string orderId, ReceiveDto)
â””â”€â”€ ðŸ“Š EvaluateSupplierPerformanceAsync(string supplierId)

AI Processing:
â”œâ”€â”€ ðŸ“„ ExtractDataFromDocumentAsync(Stream document)
â”œâ”€â”€ ðŸ” MapSuppliersFromExtractedDataAsync(SupplierData)
â”œâ”€â”€ ðŸ§© MapIngredientsFromExtractedDataAsync(List<ItemData>)
â”œâ”€â”€ âœ… ValidateExtractedDataAsync(ExtractedData)
â””â”€â”€ ðŸ“‹ GeneratePrefilledFormAsync(ExtractedData)

Stock Management:
â”œâ”€â”€ ðŸ“Š CheckLowStockIngredientsAsync()
â”œâ”€â”€ ðŸ’¡ GeneratePurchaseSuggestionsAsync()
â”œâ”€â”€ ðŸ“¦ UpdateStockOnReceiptAsync(string orderId)
â”œâ”€â”€ ðŸ¥˜ ReserveIngredientsAsync(List<IngredientReservation>)
â””â”€â”€ ðŸ“ˆ CalculateStockMetricsAsync()
```

### **ðŸ’³ FINANCIAL DOMAIN COMPONENTS**

#### **ðŸŽ® Financial Controllers**
```
AccountReceivableController:
â”œâ”€â”€ GET /AccountReceivable - Lista contas a receber
â”œâ”€â”€ POST /AccountReceivable/{id}/Payment - Registrar recebimento
â”œâ”€â”€ GET /AccountReceivable/Overdue - Contas vencidas
â””â”€â”€ GET /AccountReceivable/{id}/Installments - Parcelas

AccountPayableController:
â”œâ”€â”€ GET /AccountPayable - Lista contas a pagar
â”œâ”€â”€ POST /AccountPayable/{id}/Payment - Registrar pagamento
â”œâ”€â”€ GET /AccountPayable/Overdue - Contas vencidas
â””â”€â”€ GET /AccountPayable/DueToday - Vencimentos hoje

TransactionController:
â”œâ”€â”€ GET /Transaction - Extrato transaÃ§Ãµes
â”œâ”€â”€ POST /Transaction - Registrar transaÃ§Ã£o manual
â”œâ”€â”€ POST /Transaction/Reconcile - ConciliaÃ§Ã£o bancÃ¡ria
â””â”€â”€ GET /Transaction/CashFlow - Fluxo de caixa

ReportController:
â”œâ”€â”€ GET /Report/Profitability - RelatÃ³rio lucratividade
â”œâ”€â”€ GET /Report/CashFlow - ProjeÃ§Ã£o fluxo caixa
â”œâ”€â”€ GET /Report/AgingReport - RelatÃ³rio aging
â””â”€â”€ GET /Report/Dashboard - Dashboard executivo
```

#### **âš™ï¸ Financial Service**
```
Account Management:
â”œâ”€â”€ ðŸ†• CreateAccountReceivableAsync(OrderEntry)
â”œâ”€â”€ ðŸ†• CreateAccountPayableAsync(PurchaseOrder)
â”œâ”€â”€ ðŸ’° ProcessPaymentAsync(string accountId, PaymentDto)
â”œâ”€â”€ ðŸ“Š CalculateInstallmentsAsync(decimal amount, PaymentTerms)
â””â”€â”€ âš ï¸ ProcessOverdueAccountsAsync()

Transaction Processing:
â”œâ”€â”€ ðŸ’³ CreateTransactionAsync(TransactionDto)
â”œâ”€â”€ ðŸ¦ ReconcileTransactionAsync(string transactionId, BankData)
â”œâ”€â”€ ðŸ“Š CalculateCashFlowAsync(DateTime start, DateTime end)
â”œâ”€â”€ ðŸ“ˆ GenerateProfitabilityReportAsync(string orderId)
â””â”€â”€ ðŸ’¹ UpdateFinancialMetricsAsync()

Integration Events:
â”œâ”€â”€ ðŸ“¥ Handle OrderConfirmed (create AR)
â”œâ”€â”€ ðŸ“¥ Handle PurchaseCompleted (create AP)
â”œâ”€â”€ ðŸ“¥ Handle PaymentReceived (update AR)
â”œâ”€â”€ ðŸ“¥ Handle PaymentMade (update AP)
â””â”€â”€ ðŸ“¤ Publish CashFlowUpdated
```

## ðŸ”§ Infrastructure Components

### **ðŸ” Identity Service**
```
ASP.NET Core Identity Features:
â”œâ”€â”€ ðŸ‘¤ User management per tenant
â”œâ”€â”€ ðŸŽ­ Role-based authorization
â”œâ”€â”€ ðŸŽ¯ Claims-based permissions
â”œâ”€â”€ ðŸ”’ Password policies
â”œâ”€â”€ ðŸ”‘ Two-factor authentication
â””â”€â”€ ðŸ“Š Audit trail

Tenant Isolation:
â”œâ”€â”€ ðŸ  Separate Identity tables per tenant
â”œâ”€â”€ ðŸ”’ Cross-tenant access prevention
â”œâ”€â”€ ðŸŽ¯ Role scoping within tenant
â””â”€â”€ ðŸ“‹ Permission inheritance
```

### **ðŸŒ Integration Service**
```
Google Workspace APIs:
â”œâ”€â”€ ðŸ‘¥ People API Client
â”‚   â”œâ”€â”€ SyncContactsAsync()
â”‚   â”œâ”€â”€ CreateContactAsync(Customer)
â”‚   â””â”€â”€ UpdateContactAsync(string id, ContactData)
â”œâ”€â”€ ðŸ“… Calendar API Client
â”‚   â”œâ”€â”€ CreateEventAsync(ProductionOrder)
â”‚   â”œâ”€â”€ UpdateEventAsync(string eventId, EventData)
â”‚   â””â”€â”€ DeleteEventAsync(string eventId)
â””â”€â”€ ðŸ—ºï¸ Maps API Client
    â”œâ”€â”€ CalculateRouteAsync(string origin, string destination)
    â”œâ”€â”€ GetDistanceMatrixAsync(List<Address>)
    â””â”€â”€ GeocodingAsync(string address)

Circuit Breaker Pattern:
â”œâ”€â”€ âš¡ Auto-retry on transient failures
â”œâ”€â”€ ðŸ”´ Circuit open on consecutive failures
â”œâ”€â”€ ðŸ’¾ Fallback to cached data
â””â”€â”€ ðŸ“Š Health check monitoring
```

### **ðŸ¤– AI Service**
```
Document Processing Pipeline:
â”œâ”€â”€ ðŸ“„ OCR Engine (Extract text from images/PDF)
â”œâ”€â”€ ðŸ§  ML Models (Classification + Named Entity Recognition)
â”œâ”€â”€ ðŸ” Data Mapping (Match suppliers/ingredients)
â”œâ”€â”€ âœ… Confidence Scoring (Accuracy assessment)
â””â”€â”€ ðŸ“‹ Result Formatting (Structured JSON output)

Processing Steps:
â”œâ”€â”€ 1ï¸âƒ£ File upload validation
â”œâ”€â”€ 2ï¸âƒ£ OCR text extraction
â”œâ”€â”€ 3ï¸âƒ£ Document type classification
â”œâ”€â”€ 4ï¸âƒ£ Entity extraction (supplier, items, values)
â”œâ”€â”€ 5ï¸âƒ£ Data mapping to system entities
â”œâ”€â”€ 6ï¸âƒ£ Confidence calculation
â””â”€â”€ 7ï¸âƒ£ Human validation queue
```

### **âš¡ Event Bus**
```
In-Memory Event Coordination:
â”œâ”€â”€ ðŸ“¤ Event Publishing (IEventPublisher)
â”œâ”€â”€ ðŸ“¥ Event Subscription (IEventHandler<T>)
â”œâ”€â”€ ðŸ”„ Event Routing (by event type)
â”œâ”€â”€ âš¡ Async Processing (BackgroundService)
â””â”€â”€ ðŸ“Š Event Tracking (audit + metrics)

Key Events:
â”œâ”€â”€ ðŸ’° OrderConfirmed â†’ Create Demands + AccountReceivable
â”œâ”€â”€ ðŸ­ DemandCreated â†’ Check ingredient availability
â”œâ”€â”€ ðŸ“¦ ProductionCompleted â†’ Update order status
â”œâ”€â”€ ðŸ›’ PurchaseReceived â†’ Update stock + AccountPayable
â””â”€â”€ ðŸ’³ PaymentReceived â†’ Update account status
```

## ðŸ”„ Component Interaction Patterns

### **ðŸ“Š Cross-Domain Integration Pattern**
```mermaid
sequenceDiagram
    participant SC as Sales Controller
    participant SS as Sales Service
    participant EB as Event Bus
    participant PS as Production Service
    participant FS as Financial Service
    
    SC->>SS: ConfirmOrder(orderId)
    SS->>SS: Validate business rules
    SS->>EB: Publish OrderConfirmed event
    
    par Production Domain
        EB->>PS: Handle OrderConfirmed
        PS->>PS: Create Demands for OrderItems
    and Financial Domain
        EB->>FS: Handle OrderConfirmed
        FS->>FS: Create AccountReceivable
    end
    
    SS-->>SC: Order confirmed successfully
```

### **ðŸ¤– AI Processing Pattern**
```mermaid
sequenceDiagram
    participant PC as Purchasing Controller
    participant PS as Purchasing Service
    participant AI as AI Service
    participant FS as File Storage
    participant BGS as Background Service
    
    PC->>PS: UploadFiscalDocument(file)
    PS->>FS: Store document
    PS->>BGS: Queue AI processing
    PS-->>PC: Document uploaded (processing started)
    
    BGS->>AI: ProcessDocument(filePath)
    AI->>FS: Read document
    AI->>AI: OCR + ML processing
    AI->>PS: Return extracted data
    PS->>PS: Validate and map data
    PS->>PC: Notify completion (SignalR)
```

## ðŸ“Š Performance and Monitoring

### **ðŸŽ¯ Component SLA Targets**
| Component | Response Time | Throughput | Error Rate |
|-----------|---------------|------------|------------|
| **Controllers** | < 200ms | 1000 req/min | < 0.1% |
| **Services** | < 100ms | 5000 ops/min | < 0.01% |
| **Repositories** | < 50ms | 10000 queries/min | < 0.001% |
| **AI Service** | < 30s | 100 docs/hour | < 5% |
| **Integration Service** | < 2s | 1000 API calls/hour | < 1% |

### **ðŸ“ˆ Monitoring Strategy**
```
Metrics Collection:
â”œâ”€â”€ ðŸŽ® Controller: Request count, response time, error rate
â”œâ”€â”€ âš™ï¸ Service: Method execution time, business rule violations
â”œâ”€â”€ ðŸ—„ï¸ Repository: Query performance, connection pooling
â”œâ”€â”€ ðŸ¤– AI Service: Processing time, accuracy rate, queue length
â”œâ”€â”€ ðŸŒ Integration: API latency, quota usage, circuit breaker status
â””â”€â”€ ðŸ“Š Event Bus: Event throughput, processing lag, dead letters
```

---

**Arquivo**: `level3-component-diagrams.md`  
**NÃ­vel C4**: 3 - Component  
**AudiÃªncia**: Arquitetos de software e desenvolvedores  
**Foco**: OrganizaÃ§Ã£o interna dos containers em componentes  
**AtualizaÃ§Ã£o**: 16/06/2025
