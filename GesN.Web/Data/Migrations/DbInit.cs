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

                // ========== DOMÍNIO DE VENDAS ==========
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

                // ========== DOMÍNIO DE PRODUÇÃO ==========
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
                    ProductId TEXT NOT NULL,
                    ProductGroupId TEXT NOT NULL,
                    Quantity INTEGER NOT NULL DEFAULT 1,
                    MinQuantity INTEGER NOT NULL DEFAULT 1,
                    MaxQuantity INTEGER,
                    DefaultQuantity INTEGER NOT NULL DEFAULT 1,
                    IsOptional INTEGER NOT NULL DEFAULT 0,
                    ExtraPrice REAL DEFAULT 0,
                    PRIMARY KEY(Id),
                    FOREIGN KEY(ProductId) REFERENCES Product(Id),
                    FOREIGN KEY(ProductGroupId) REFERENCES Product(Id)
                );";

                var createProductComponentTable = @"
                CREATE TABLE IF NOT EXISTS ProductComponent (
                    Id TEXT NOT NULL UNIQUE,
                    CreatedAt TEXT NOT NULL,
                    CreatedBy TEXT NOT NULL,
                    LastModifiedAt TEXT,
                    LastModifiedBy TEXT,
                    StateCode INTEGER NOT NULL DEFAULT 1,
                    CompositeProductId TEXT NOT NULL,
                    ComponentProductId TEXT NOT NULL,
                    Quantity REAL NOT NULL DEFAULT 1,
                    Unit TEXT DEFAULT 'UN',
                    IsOptional INTEGER NOT NULL DEFAULT 0,
                    AssemblyOrder INTEGER DEFAULT 1,
                    Notes TEXT,
                    PRIMARY KEY(Id),
                    FOREIGN KEY(CompositeProductId) REFERENCES Product(Id),
                    FOREIGN KEY(ComponentProductId) REFERENCES Product(Id)
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
                    OriginalProductId TEXT NOT NULL,
                    ExchangeProductId TEXT NOT NULL,
                    ExchangeRatio REAL NOT NULL DEFAULT 1,
                    AdditionalCost REAL DEFAULT 0,
                    IsActive INTEGER NOT NULL DEFAULT 1,
                    PRIMARY KEY(Id),
                    FOREIGN KEY(ProductGroupId) REFERENCES Product(Id),
                    FOREIGN KEY(OriginalProductId) REFERENCES Product(Id),
                    FOREIGN KEY(ExchangeProductId) REFERENCES Product(Id)
                );";

                var createProductGroupOptionTable = @"
                CREATE TABLE IF NOT EXISTS ProductGroupOption (
                    Id TEXT NOT NULL UNIQUE,
                    CreatedAt TEXT NOT NULL,
                    CreatedBy TEXT NOT NULL,
                    LastModifiedAt TEXT,
                    LastModifiedBy TEXT,
                    StateCode INTEGER NOT NULL DEFAULT 1,
                    ProductGroupId TEXT NOT NULL,
                    Name TEXT NOT NULL,
                    Description TEXT,
                    OptionType TEXT NOT NULL,
                    IsRequired INTEGER NOT NULL DEFAULT 0,
                    DisplayOrder INTEGER DEFAULT 1,
                    PRIMARY KEY(Id),
                    FOREIGN KEY(ProductGroupId) REFERENCES Product(Id)
                );";

                var createProductionOrderTable = @"
                CREATE TABLE IF NOT EXISTS ProductionOrder (
                    Id TEXT NOT NULL UNIQUE,
                    CreatedAt TEXT NOT NULL,
                    CreatedBy TEXT NOT NULL,
                    LastModifiedAt TEXT,
                    LastModifiedBy TEXT,
                    StateCode INTEGER NOT NULL DEFAULT 1,
                    OrderId TEXT NOT NULL,
                    OrderItemId TEXT NOT NULL,
                    ProductId TEXT NOT NULL,
                    Quantity INTEGER NOT NULL,
                    Status TEXT NOT NULL DEFAULT 'Pending',
                    Priority TEXT DEFAULT 'Normal',
                    ScheduledStartDate TEXT,
                    ScheduledEndDate TEXT,
                    ActualStartDate TEXT,
                    ActualEndDate TEXT,
                    AssignedTo TEXT,
                    Notes TEXT,
                    EstimatedTime INTEGER,
                    ActualTime INTEGER,
                    PRIMARY KEY(Id),
                    FOREIGN KEY(OrderId) REFERENCES OrderEntry(Id),
                    FOREIGN KEY(OrderItemId) REFERENCES OrderItem(Id),
                    FOREIGN KEY(ProductId) REFERENCES Product(Id)
                );";

                // ========== DOMÍNIO FINANCEIRO ==========
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

                // ========== EXECUÇÃO DAS MIGRATIONS ==========
                // Tabelas legadas
                connection.Execute(createClienteTable);
                connection.Execute(createPedidoTable);

                // Value Objects (executados primeiro devido às dependências)
                connection.Execute(createAddressTable);
                connection.Execute(createFiscalDataTable);

                // Domínio de Vendas
                connection.Execute(createCustomerTable);
                connection.Execute(createOrderTable);
                connection.Execute(createOrderItemTable);
                connection.Execute(createContractTable);

                // Domínio de Produção
                connection.Execute(createProductCategoryTable);
                connection.Execute(createSupplierTable);
                connection.Execute(createProductTable);
                connection.Execute(createProductGroupItemTable);
                connection.Execute(createProductComponentTable);
                connection.Execute(createIngredientTable);
                connection.Execute(createProductIngredientTable);
                connection.Execute(createProductGroupExchangeRuleTable);
                connection.Execute(createProductGroupOptionTable);
                connection.Execute(createProductionOrderTable);

                // Domínio Financeiro
                connection.Execute(createTransactionCategoryTable);
                connection.Execute(createPaymentMethodTable);
                connection.Execute(createFinancialTransactionTable);
            }
        }
    }
}
