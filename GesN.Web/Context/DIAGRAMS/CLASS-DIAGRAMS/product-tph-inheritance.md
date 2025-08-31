# ðŸ—ï¸ DIAGRAMA DE CLASSES - HERANÃ‡A TPH DO DOMÃNIO DE PRODUTO

## ðŸŽ¯ VisÃ£o Geral
Diagrama de classes mostrando a implementaÃ§Ã£o de heranÃ§a Table Per Hierarchy (TPH) do DomÃ­nio de Produto, incluindo a classe abstrata base, classes derivadas, interfaces de serviÃ§os e padrÃµes arquiteturais implementados.

## ðŸ§¬ Diagrama de HeranÃ§a e Interfaces

```mermaid
classDiagram
    %% ==========================================
    %% CLASSE ABSTRATA BASE
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
        +datetime CreatedDate
        +datetime ModifiedDate
        +ProductCategory CategoryNavigation
        
        +CalculateUnitPrice() decimal
        +IsActive() bool
        +ValidateBusinessRules() bool*
        +GetAssemblyInstructions() string
    }

    %% ==========================================
    %% ENUMERADOR DE TIPOS
    %% ==========================================
    class ProductType {
        <<enumeration>>
        Simple
        Composite
        Group
    }

    %% ==========================================
    %% CLASSES DERIVADAS (TPH)
    %% ==========================================
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
    %% ENTIDADES RELACIONADAS
    %% ==========================================
    class ProductCategory {
        +string Id
        +string Name
        +string Description
        +string StateCode
        +datetime CreatedDate
        +datetime ModifiedDate
    }

    class ProductComponentHierarchy {
        +string Id
        +string Name
        +string Description
        +string Notes
        +string StateCode
        +ICollection~ProductComponent~ Components
        +ICollection~CompositeProductXHierarchy~ CompositeProductRelations
    }

    class ProductComponent {
        +string Id
        +string Name
        +string Description
        +string ProductComponentHierarchyId
        +decimal AdditionalCost
        +string StateCode
        +ProductComponentHierarchy ProductComponentHierarchy
    }

    class CompositeProductXHierarchy {
        +int Id
        +string ProductComponentHierarchyId
        +string ProductId
        +int MinQuantity
        +int MaxQuantity
        +bool IsOptional
        +int AssemblyOrder
        +string Notes
    }

    class ProductGroupItem {
        +string Id
        +string ProductGroupId
        +string ProductId
        +string ProductCategoryId
        +int Quantity
        +int MinQuantity
        +int MaxQuantity
        +int DefaultQuantity
        +bool IsOptional
        +decimal ExtraPrice
        +string StateCode
    }

    class ProductGroupExchangeRule {
        +string Id
        +string ProductGroupId
        +string SourceGroupItemId
        +int SourceGroupItemWeight
        +string TargetGroupItemId
        +int TargetGroupItemWeight
        +decimal ExchangeRatio
        +bool IsActive
        +string StateCode
    }

    %% ==========================================
    %% INTERFACES DE SERVIÃ‡OS
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
    }

    %% ==========================================
    %% IMPLEMENTAÃ‡Ã•ES DE SERVIÃ‡OS
    %% ==========================================
    class ProductService {
        -IProductRepository _productRepository
        -IProductCategoryService _categoryService
        -IProductComponentService _componentService
        
        +GetByIdAsync(id) Task~Product~
        +CreateAsync(product) Task~string~
        +UpdateAsync(product) Task~bool~
        +ValidateProductAsync(product) Task~bool~
        +CalculateProductCostAsync(id) Task~decimal~
        -ConfigureProductType(product) void
        -ValidateSimpleProduct(product) bool
        -ValidateCompositeProduct(product) bool
        -ValidateProductGroup(product) bool
    }

    class ProductRepository {
        -IDbConnection _connection
        
        +GetByIdAsync(id) Task~Product~
        +CreateAsync(product) Task~string~
        +UpdateAsync(product) Task~bool~
        +DeleteAsync(id) Task~bool~
        -MapToProduct(reader) Product
        -GetProductType(reader) ProductType
    }

    %% ==========================================
    %% RELACIONAMENTOS DE HERANÃ‡A
    %% ==========================================
    Product <|-- SimpleProduct : inherits
    Product <|-- CompositeProduct : inherits
    Product <|-- ProductGroup : inherits
    
    Product --> ProductType : uses
    Product --> ProductCategory : belongs to

    %% ==========================================
    %% RELACIONAMENTOS DE COMPOSIÃ‡ÃƒO
    %% ==========================================
    CompositeProduct "1" --> "*" CompositeProductXHierarchy : configures
    CompositeProductXHierarchy "*" --> "1" ProductComponentHierarchy : references
    ProductComponentHierarchy "1" --> "*" ProductComponent : contains

    ProductGroup "1" --> "*" ProductGroupItem : contains
    ProductGroup "1" --> "*" ProductGroupExchangeRule : defines

    ProductGroupItem "*" --> "1" Product : can reference
    ProductGroupItem "*" --> "1" ProductCategory : can reference

    %% ==========================================
    %% RELACIONAMENTOS DE DEPENDÃŠNCIA
    %% ==========================================
    IProductService <|.. ProductService : implements
    IProductRepository <|.. ProductRepository : implements
    ProductService --> IProductRepository : depends on
    ProductService --> Product : manages

    %% ==========================================
    %% STYLING POR DOMÃNIO
    %% ==========================================
    
    %% CLASSES PRINCIPAIS = Verde escuro
    class Product {
        background-color: #00a86b
        color: white
    }
    class SimpleProduct {
        background-color: #00a86b
        color: white
    }
    class CompositeProduct {
        background-color: #00a86b
        color: white
    }
    class ProductGroup {
        background-color: #00a86b
        color: white
    }
    
    %% ENTIDADES RELACIONADAS = Verde mÃ©dio
    class ProductCategory {
        background-color: #2dd4aa
        color: black
    }
    class ProductComponentHierarchy {
        background-color: #2dd4aa
        color: black
    }
    class ProductComponent {
        background-color: #2dd4aa
        color: black
    }
    class CompositeProductXHierarchy {
        background-color: #2dd4aa
        color: black
    }
    class ProductGroupItem {
        background-color: #2dd4aa
        color: black
    }
    class ProductGroupExchangeRule {
        background-color: #2dd4aa
        color: black
    }
    
    %% INTERFACES = Verde claro
    class IProductService {
        background-color: #a7f3d0
        color: black
    }
    class IProductRepository {
        background-color: #a7f3d0
        color: black
    }
    
    %% IMPLEMENTAÃ‡Ã•ES = Verde mÃ©dio
    class ProductService {
        background-color: #6ee7b7
        color: black
    }
    class ProductRepository {
        background-color: #6ee7b7
        color: black
    }
    
    %% ENUMERAÃ‡ÃƒO = Cinza
    class ProductType {
        background-color: #e5e7eb
        color: black
    }
```

## ðŸ“‹ Detalhes da ImplementaÃ§Ã£o

### **ðŸ—ï¸ PadrÃ£o Table Per Hierarchy (TPH)**
- **EstratÃ©gia**: Uma Ãºnica tabela `Product` para todos os tipos
- **Discriminador**: Coluna `ProductType` (Simple|Composite|Group)
- **Vantagens**: Performance, simplicidade de queries, integridade referencial
- **Constraint**: `CHECK (ProductType IN ('Simple', 'Composite', 'Group'))`

### **ðŸ§¬ Hierarquia de Classes**

#### **Product (Classe Abstrata Base)**
- ContÃ©m todas as propriedades comuns
- MÃ©todo abstrato `ValidateBusinessRules()`
- MÃ©todos virtuais para override nas classes derivadas

#### **SimpleProduct**
- ImplementaÃ§Ã£o mais simples
- ValidaÃ§Ã£o bÃ¡sica de nome e preÃ§o
- CÃ¡lculo de custo baseado em ingredientes

#### **CompositeProduct**
- Relacionamento com hierarquias de componentes
- ValidaÃ§Ã£o complexa de configuraÃ§Ãµes
- CÃ¡lculo dinÃ¢mico de preÃ§o baseado em seleÃ§Ãµes

#### **ProductGroup**
- Relacionamento com itens do grupo
- Regras de troca entre itens
- CÃ¡lculo de preÃ§o baseado em configuraÃ§Ã£o

### **ðŸ”§ PadrÃµes Arquiteturais Implementados**

#### **Repository Pattern**
- `IProductRepository`: Interface de acesso a dados
- `ProductRepository`: ImplementaÃ§Ã£o usando Dapper
- AbstraÃ§Ã£o do acesso a dados

#### **Service Layer Pattern**
- `IProductService`: Interface de regras de negÃ³cio
- `ProductService`: ImplementaÃ§Ã£o das regras
- OrquestraÃ§Ã£o entre repository e validaÃ§Ãµes

#### **Strategy Pattern**
- ValidaÃ§Ã£o especÃ­fica por tipo de produto
- CÃ¡lculo de custo especÃ­fico por tipo
- ConfiguraÃ§Ã£o especÃ­fica por tipo

#### **Factory Pattern** (ImplÃ­cito)
- CriaÃ§Ã£o de instÃ¢ncias corretas baseada em ProductType
- Mapping automÃ¡tico no Repository

### **âš–ï¸ Regras de NegÃ³cio por Tipo**

#### **Simple Product**
```csharp
public override bool ValidateBusinessRules()
{
    return !string.IsNullOrEmpty(Name) && 
           Price > 0 && 
           Cost >= 0;
}
```

#### **Composite Product**
```csharp
public override bool ValidateBusinessRules()
{
    return base.ValidateBusinessRules() && 
           Hierarchies.Any() &&
           Hierarchies.All(h => h.MinQuantity >= 1);
}
```

#### **Product Group**
```csharp
public override bool ValidateBusinessRules()
{
    return base.ValidateBusinessRules() && 
           GroupItems.Any() &&
           GroupItems.All(i => i.IsValidConfiguration());
}
```

## ðŸ”„ Fluxo de CriaÃ§Ã£o de Produtos

1. **Cliente chama** `ProductService.CreateAsync(product)`
2. **Service valida** tipo e regras de negÃ³cio
3. **Service configura** propriedades especÃ­ficas do tipo
4. **Repository persiste** na tabela Product com discriminador
5. **Sistema retorna** ID do produto criado

---

**Arquivo**: `product-tph-inheritance.md`  
**DomÃ­nio**: Produto (#00a86b)  
**Tipo**: Class Diagram  
**PadrÃ£o**: Table Per Hierarchy (TPH)
