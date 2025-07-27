using Dapper;
using Microsoft.Data.Sqlite;

namespace GesN.Web.Data.Migrations
{
    public class DbInit
    {
        private readonly string _connectionString;

        public DbInit(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Initialize()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                // ========== TABELAS LEGADAS (MANTIDAS PARA COMPATIBILIDADE) ==========
                var createClienteTable = @"
                CREATE TABLE IF NOT EXISTS Cliente (
                    ClienteId INTEGER NOT NULL UNIQUE,
                    ClienteIdGoogleContacts BLOB,
                    DataCadastro TEXT NOT NULL,
                    Nome TEXT NOT NULL,
                    Sobrenome TEXT NOT NULL,
                    Cpf TEXT,
                    TelefonePrincipal INTEGER NOT NULL,
                    DataModificacao TEXT NOT NULL,
                    PRIMARY KEY(ClienteId AUTOINCREMENT)
                );";

                var createPedidoTable = @"
                CREATE TABLE IF NOT EXISTS Pedido (
                    PedidoId INTEGER NOT NULL UNIQUE,
                    ClienteId INTEGER NOT NULL,
	                DataCadastro TEXT NOT NULL,
	                DataPedido TEXT NOT NULL,
	                DataModificacao	TEXT NOT NULL,
	                PRIMARY KEY(PedidoId AUTOINCREMENT)
                );";

                // ========== VALUE OBJECTS ==========
                var createAddressTable = @"
                CREATE TABLE IF NOT EXISTS Address (
                    Id TEXT NOT NULL UNIQUE,
                    Street TEXT NOT NULL,
                    Number TEXT,
                    Complement TEXT,
                    Neighborhood TEXT,
                    City TEXT NOT NULL,
                    State TEXT NOT NULL,
                    ZipCode TEXT,
                    Country TEXT DEFAULT 'Brasil',
                    PRIMARY KEY(Id)
                );";

                var createFiscalDataTable = @"
                CREATE TABLE IF NOT EXISTS FiscalData (
                    Id TEXT NOT NULL UNIQUE,
                    TaxNumber TEXT,
                    StateRegistration TEXT,
                    MunicipalRegistration TEXT,
                    CompanyName TEXT,
                    TradeName TEXT,
                    PRIMARY KEY(Id)
                );";

                // ========== DOM�NIO DE VENDAS ==========
                var createCustomerTable = @"
                CREATE TABLE IF NOT EXISTS Customer (
                    Id TEXT NOT NULL UNIQUE,
                    CreatedAt TEXT NOT NULL,
                    CreatedBy TEXT NOT NULL,
                    LastModifiedAt TEXT,
                    LastModifiedBy TEXT,
                    StateCode INTEGER NOT NULL DEFAULT 1,
                    FirstName TEXT NOT NULL,
                    LastName TEXT,
                    Email TEXT,
                    Phone TEXT,
                    DocumentNumber TEXT,
                    DocumentType TEXT,
                    GoogleContactId TEXT,
                    AddressId TEXT,
                    PRIMARY KEY(Id),
                    FOREIGN KEY(AddressId) REFERENCES Address(Id)
                );";

                var createOrderTable = @"
                CREATE TABLE IF NOT EXISTS OrderEntry (
                    Id TEXT NOT NULL UNIQUE,
                    CreatedAt TEXT NOT NULL,
                    CreatedBy TEXT NOT NULL,
                    LastModifiedAt TEXT,
                    LastModifiedBy TEXT,
                    StateCode INTEGER NOT NULL DEFAULT 1,
                    NumberSequence TEXT NOT NULL,
                    OrderDate TEXT,
                    DeliveryDate TEXT,
                    CustomerId TEXT NOT NULL,
                    Status TEXT NOT NULL DEFAULT 'Draft',
                    Type TEXT NOT NULL,
                    TotalAmount REAL NOT NULL DEFAULT 0,
                    Subtotal REAL NOT NULL DEFAULT 0,
                    TaxAmount REAL NOT NULL DEFAULT 0,
                    DiscountAmount REAL NOT NULL DEFAULT 0,
                    Notes TEXT,
                    DeliveryAddressId TEXT,
                    RequiresFiscalReceipt INTEGER NOT NULL DEFAULT 0,
                    FiscalDataId TEXT,
                    PrintStatus TEXT DEFAULT 'NotPrinted',
                    PrintBatchNumber INTEGER,
                    PRIMARY KEY(Id),
                    FOREIGN KEY(CustomerId) REFERENCES Customer(Id),
                    FOREIGN KEY(DeliveryAddressId) REFERENCES Address(Id),
                    FOREIGN KEY(FiscalDataId) REFERENCES FiscalData(Id)
                );";

                var createOrderItemTable = @"
                CREATE TABLE IF NOT EXISTS OrderItem (
                    Id TEXT NOT NULL UNIQUE,
                    CreatedAt TEXT NOT NULL,
                    CreatedBy TEXT NOT NULL,
                    LastModifiedAt TEXT,
                    LastModifiedBy TEXT,
                    StateCode INTEGER NOT NULL DEFAULT 1,
                    OrderId TEXT NOT NULL,
                    ProductId TEXT NOT NULL,
                    Quantity INTEGER NOT NULL,
                    UnitPrice REAL NOT NULL,
                    Discount REAL NOT NULL DEFAULT 0,
                    Notes TEXT,
                    PRIMARY KEY(Id),
                    FOREIGN KEY(OrderId) REFERENCES OrderEntry(Id),
                    FOREIGN KEY(ProductId) REFERENCES Product(Id)
                );";

                var createContractTable = @"
                CREATE TABLE IF NOT EXISTS Contract (
                    Id TEXT NOT NULL UNIQUE,
                    CreatedAt TEXT NOT NULL,
                    CreatedBy TEXT NOT NULL,
                    LastModifiedAt TEXT,
                    LastModifiedBy TEXT,
                    StateCode INTEGER NOT NULL DEFAULT 1,
                    Number TEXT NOT NULL,
                    OrderId TEXT NOT NULL,
                    CreationDate TEXT NOT NULL,
                    ExpirationDate TEXT,
                    Status TEXT NOT NULL DEFAULT 'Draft',
                    FileUrl TEXT,
                    SignatureUrl TEXT,
                    CustomerSignedAt TEXT,
                    CompanySignedAt TEXT,
                    PRIMARY KEY(Id),
                    FOREIGN KEY(OrderId) REFERENCES OrderEntry(Id)
                );";

                // ========== DOM�NIO DE PRODU��O ==========
                var createProductCategoryTable = @"
                CREATE TABLE IF NOT EXISTS ProductCategory (
                    Id TEXT NOT NULL UNIQUE,
                    CreatedAt TEXT NOT NULL,
                    CreatedBy TEXT NOT NULL,
                    LastModifiedAt TEXT,
                    LastModifiedBy TEXT,
                    StateCode INTEGER NOT NULL DEFAULT 1,
                    Name TEXT NOT NULL,
                    Description TEXT,
                    IsActive INTEGER NOT NULL DEFAULT 1,
                    PRIMARY KEY(Id)
                );";

                var createSupplierTable = @"
                CREATE TABLE IF NOT EXISTS Supplier (
                    Id TEXT NOT NULL UNIQUE,
                    CreatedAt TEXT NOT NULL,
                    CreatedBy TEXT NOT NULL,
                    LastModifiedAt TEXT,
                    LastModifiedBy TEXT,
                    StateCode INTEGER NOT NULL DEFAULT 1,
                    Name TEXT NOT NULL,
                    CompanyName TEXT,
                    DocumentNumber TEXT,
                    DocumentType TEXT,
                    Email TEXT,
                    Phone TEXT,
                    AddressId TEXT,
                    IsActive INTEGER NOT NULL DEFAULT 1,
                    PRIMARY KEY(Id),
                    FOREIGN KEY(AddressId) REFERENCES Address(Id)
                );";

                var createProductTable = @"
                CREATE TABLE IF NOT EXISTS Product (
                    Id TEXT NOT NULL UNIQUE,
                    CreatedAt TEXT NOT NULL,
                    CreatedBy TEXT NOT NULL,
                    LastModifiedAt TEXT,
                    LastModifiedBy TEXT,
                    StateCode INTEGER NOT NULL DEFAULT 1,
                    Name TEXT NOT NULL,
                    Description TEXT,
                    Price REAL NOT NULL DEFAULT 0,
                    QuantityPrice INTEGER NOT NULL DEFAULT 0,
                    UnitPrice REAL NOT NULL DEFAULT 0,
                    CategoryId TEXT,
                    Category TEXT,
                    SKU TEXT,
                    ImageUrl TEXT,
                    Note TEXT,
                    Cost REAL NOT NULL DEFAULT 0,
                    AssemblyTime INTEGER DEFAULT 0,
                    AssemblyInstructions TEXT,
                    IsProductOfCatalog TEXT,
                    ProductType TEXT NOT NULL CHECK (ProductType IN ('Simple', 'Composite', 'Group')),
                    PRIMARY KEY(Id),
                    FOREIGN KEY(CategoryId) REFERENCES ProductCategory(Id)
                );";
                    /*MinStock INTEGER DEFAULT 0,
                    CurrentStock INTEGER DEFAULT 0,
                    SupplierId TEXT,
                    AllowCustomization INTEGER DEFAULT 0,
                    MinItemsRequired INTEGER DEFAULT 1,
                    MaxItemsAllowed INTEGER,
                    */

                var createProductGroupItemTable = @"
                CREATE TABLE IF NOT EXISTS ProductGroupItem (
                    Id TEXT NOT NULL UNIQUE,
                    CreatedAt TEXT NOT NULL,
                    CreatedBy TEXT NOT NULL,
                    LastModifiedAt TEXT,
                    LastModifiedBy TEXT,
                    StateCode INTEGER NOT NULL DEFAULT 1,
                    ProductId TEXT,
                    ProductGroupId TEXT NOT NULL,
                    ProductCategoryId TEXT,
                    Quantity INTEGER NOT NULL DEFAULT 1,
                    MinQuantity INTEGER NOT NULL DEFAULT 1,
                    MaxQuantity INTEGER,
                    DefaultQuantity INTEGER NOT NULL DEFAULT 1,
                    IsOptional INTEGER NOT NULL DEFAULT 0,
                    ExtraPrice REAL DEFAULT 0,
                    PRIMARY KEY(Id),
                    FOREIGN KEY(ProductId) REFERENCES Product(Id),
                    FOREIGN KEY(ProductGroupId) REFERENCES Product(Id),
                    FOREIGN KEY(ProductCategoryId) REFERENCES ProductCategory(Id)

                );";

                var createCompositeProductXHierarchyTable = @"
                CREATE TABLE IF NOT EXISTS CompositeProductXHierarchy (
                    Id INTEGER NOT NULL UNIQUE,
                    ProductComponentHierarchyId TEXT NOT NULL,
                    ProductId TEXT NOT NULL,
                    MinQuantity INTEGER NOT NULL,
                    MaxQuantity INTEGER,
                    IsOptional INTEGER NOT NULL DEFAULT 0,
                    AssemblyOrder INTEGER NOT NULL,
                    Notes TEXT,
                    PRIMARY KEY(Id AUTOINCREMENT),
                    FOREIGN KEY(ProductComponentHierarchyId) REFERENCES ProductComponentHierarchy(Id),
                    FOREIGN KEY(ProductId) REFERENCES Product(Id)
                );";

                var createProductComponentHierarchyTable = @"
                CREATE TABLE IF NOT EXISTS ProductComponentHierarchy (
                    Id TEXT NOT NULL UNIQUE,
                    CreatedAt TEXT NOT NULL,
                    CreatedBy TEXT NOT NULL,
                    LastModifiedAt TEXT,
                    LastModifiedBy TEXT,
                    StateCode INTEGER NOT NULL DEFAULT 1,
                    Name TEXT NOT NULL,
                    Description TEXT,
                    Notes TEXT,
                    PRIMARY KEY(Id)
                );";

                var createProductComponentTable = @"
                CREATE TABLE IF NOT EXISTS ProductComponent (
                    Id TEXT NOT NULL UNIQUE,
                    CreatedAt TEXT NOT NULL,
                    CreatedBy TEXT NOT NULL,
                    LastModifiedAt TEXT,
                    LastModifiedBy TEXT,
                    StateCode INTEGER NOT NULL DEFAULT 1,

                    Name TEXT NOT NULL,
                    Description TEXT,
                    ProductComponentHierarchyId TEXT NOT NULL,
                    AdditionalCost REAL DEFAULT 0,
                    PRIMARY KEY(Id),
                    FOREIGN KEY(ProductComponentHierarchyId) REFERENCES ProductComponentHierarchy(Id)
                );";

                var createProductCompositionTable = @"
                CREATE TABLE IF NOT EXISTS ProductComposition (
                    Id INTEGER NOT NULL UNIQUE,
                    DemandId TEXT NOT NULL,
                    ProductComponentId TEXT NOT NULL,
                    HierarchyName TEXT NOT NULL,
                    PRIMARY KEY(Id AUTOINCREMENT),
                    FOREIGN KEY(DemandId) REFERENCES Demand(Id),
                    FOREIGN KEY(ProductComponentId) REFERENCES ProductComponent(Id)
                );";

                var createDemandTable = @"
                CREATE TABLE IF NOT EXISTS Demand (
                    Id TEXT NOT NULL UNIQUE,
                    CreatedAt TEXT NOT NULL,
                    CreatedBy TEXT NOT NULL,
                    LastModifiedAt TEXT,
                    LastModifiedBy TEXT,
                    StateCode INTEGER NOT NULL DEFAULT 1,
                    OrderItemId TEXT NOT NULL,
                    ProductionOrderId TEXT,
                    ProductId TEXT NOT NULL,
                    Quantity TEXT NOT NULL,
                    DemandStatus TEXT NOT NULL DEFAULT 'Pending',
                    Notes TEXT,
                    PRIMARY KEY(Id),
                    FOREIGN KEY(OrderItemId) REFERENCES OrderItem(Id),
                    FOREIGN KEY(ProductionOrderId) REFERENCES ProductionOrder(Id),
                    FOREIGN KEY(ProductId) REFERENCES Product(Id)
                );";

                var createProductionOrderTable = @"
                CREATE TABLE IF NOT EXISTS ProductionOrder (
                    Id TEXT NOT NULL UNIQUE,
                    CreatedAt TEXT NOT NULL,
                    CreatedBy TEXT NOT NULL,
                    LastModifiedAt TEXT,
                    LastModifiedBy TEXT,
                    StateCode INTEGER NOT NULL DEFAULT 1,
                    OrderEntryId TEXT NOT NULL,
                    OrderNumber TEXT NOT NULL,
                    Status TEXT NOT NULL,
                    StartDate TEXT NOT NULL,
                    EndDate TEXT NOT NULL,
                    Notes TEXT,
                    ConsumptionTime TEXT,
                    PRIMARY KEY(Id),
                    FOREIGN KEY(OrderEntryId) REFERENCES OrderEntry(Id)
                );";

                var createIngredientTable = @"
                CREATE TABLE IF NOT EXISTS Ingredient (
                    Id TEXT NOT NULL UNIQUE,
                    CreatedAt TEXT NOT NULL,
                    CreatedBy TEXT NOT NULL,
                    LastModifiedAt TEXT,
                    LastModifiedBy TEXT,
                    StateCode INTEGER NOT NULL DEFAULT 1,
                    Name TEXT NOT NULL,
                    Description TEXT,
                    Unit TEXT NOT NULL DEFAULT 'UN',
                    CostPerUnit REAL NOT NULL DEFAULT 0,
                    SupplierId TEXT,
                    MinStock REAL DEFAULT 0,
                    CurrentStock REAL DEFAULT 0,
                    ExpirationDays INTEGER,
                    IsActive INTEGER NOT NULL DEFAULT 1,
                    PRIMARY KEY(Id),
                    FOREIGN KEY(SupplierId) REFERENCES Supplier(Id)
                );";

                var createProductIngredientTable = @"
                CREATE TABLE IF NOT EXISTS ProductIngredient (
                    Id TEXT NOT NULL UNIQUE,
                    CreatedAt TEXT NOT NULL,
                    CreatedBy TEXT NOT NULL,
                    LastModifiedAt TEXT,
                    LastModifiedBy TEXT,
                    StateCode INTEGER NOT NULL DEFAULT 1,
                    ProductId TEXT NOT NULL,
                    IngredientId TEXT NOT NULL,
                    Quantity REAL NOT NULL,
                    Unit TEXT NOT NULL,
                    IsOptional INTEGER NOT NULL DEFAULT 0,
                    Notes TEXT,
                    PRIMARY KEY(Id),
                    FOREIGN KEY(ProductId) REFERENCES Product(Id),
                    FOREIGN KEY(IngredientId) REFERENCES Ingredient(Id)
                );";

                var createProductGroupExchangeRuleTable = @"
                CREATE TABLE IF NOT EXISTS ProductGroupExchangeRule (
                    Id TEXT NOT NULL UNIQUE,
                    CreatedAt TEXT NOT NULL,
                    CreatedBy TEXT NOT NULL,
                    LastModifiedAt TEXT,
                    LastModifiedBy TEXT,
                    StateCode INTEGER NOT NULL DEFAULT 1,
                    ProductGroupId TEXT NOT NULL,
                    SourceGroupItemId TEXT NOT NULL,
                    SourceGroupItemWeight INTEGER NOT NULL DEFAULT 1,
                    TargetGroupItemId TEXT NOT NULL,
                    TargetGroupItemWeight INTEGER NOT NULL DEFAULT 1,
                    ExchangeRatio REAL NOT NULL DEFAULT 1,
                    IsActive INTEGER NOT NULL DEFAULT 1,
                    PRIMARY KEY(Id),
                    FOREIGN KEY(ProductGroupId) REFERENCES Product(Id),
                    FOREIGN KEY(SourceGroupItemId) REFERENCES ProductGroupItem(Id),
                    FOREIGN KEY(TargetGroupItemId) REFERENCES ProductGroupItem(Id)
                );";

                // ========== DOM�NIO FINANCEIRO ==========
                var createTransactionCategoryTable = @"
                CREATE TABLE IF NOT EXISTS TransactionCategory (
                    Id TEXT NOT NULL UNIQUE,
                    CreatedAt TEXT NOT NULL,
                    CreatedBy TEXT NOT NULL,
                    LastModifiedAt TEXT,
                    LastModifiedBy TEXT,
                    StateCode INTEGER NOT NULL DEFAULT 1,
                    Name TEXT NOT NULL,
                    Description TEXT,
                    Type TEXT NOT NULL,
                    IsActive INTEGER NOT NULL DEFAULT 1,
                    PRIMARY KEY(Id)
                );";

                var createPaymentMethodTable = @"
                CREATE TABLE IF NOT EXISTS PaymentMethod (
                    Id TEXT NOT NULL UNIQUE,
                    CreatedAt TEXT NOT NULL,
                    CreatedBy TEXT NOT NULL,
                    LastModifiedAt TEXT,
                    LastModifiedBy TEXT,
                    StateCode INTEGER NOT NULL DEFAULT 1,
                    Name TEXT NOT NULL,
                    Description TEXT,
                    IsActive INTEGER NOT NULL DEFAULT 1,
                    PRIMARY KEY(Id)
                );";

                var createFinancialTransactionTable = @"
                CREATE TABLE IF NOT EXISTS FinancialTransaction (
                    Id TEXT NOT NULL UNIQUE,
                    CreatedAt TEXT NOT NULL,
                    CreatedBy TEXT NOT NULL,
                    LastModifiedAt TEXT,
                    LastModifiedBy TEXT,
                    StateCode INTEGER NOT NULL DEFAULT 1,
                    TransactionDate TEXT NOT NULL,
                    Description TEXT NOT NULL,
                    Amount REAL NOT NULL,
                    Type TEXT NOT NULL,
                    Category TEXT NOT NULL,
                    PaymentMethodId TEXT,
                    OrderId TEXT,
                    PRIMARY KEY(Id),
                    FOREIGN KEY(PaymentMethodId) REFERENCES PaymentMethod(Id),
                    FOREIGN KEY(OrderId) REFERENCES OrderEntry(Id)
                );";

                // ========== EXECU��O DAS MIGRATIONS ==========
                // Tabelas legadas
                connection.Execute(createClienteTable);
                connection.Execute(createPedidoTable);

                // Value Objects (executados primeiro devido �s depend�ncias)
                connection.Execute(createAddressTable);
                connection.Execute(createFiscalDataTable);

                // Dom�nio de Vendas
                connection.Execute(createCustomerTable);
                connection.Execute(createOrderTable);
                connection.Execute(createOrderItemTable);
                connection.Execute(createContractTable);

                // Dom�nio de Produ��o
                connection.Execute(createSupplierTable);

                connection.Execute(createProductCategoryTable);
                connection.Execute(createProductTable);

                connection.Execute(createProductGroupItemTable);
                connection.Execute(createProductGroupExchangeRuleTable);

                connection.Execute(createProductComponentHierarchyTable);
                connection.Execute(createCompositeProductXHierarchyTable);
                connection.Execute(createProductComponentTable);

                connection.Execute(createProductionOrderTable);
                connection.Execute(createDemandTable);
                connection.Execute(createProductCompositionTable);

                connection.Execute(createIngredientTable);
                connection.Execute(createProductIngredientTable);

                // Dom�nio Financeiro
                connection.Execute(createTransactionCategoryTable);
                connection.Execute(createPaymentMethodTable);
                connection.Execute(createFinancialTransactionTable);
            }
        }
    }
}
