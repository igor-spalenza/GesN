# ANÃLISE DE REFATORAÃ‡ÃƒO DO MÃ“DULO DE VENDAS

## Executive Summary

A anÃ¡lise revelou que o sistema jÃ¡ possui uma estrutura robusta de produtos com suporte a trÃªs tipos (Simple, Composite, Group) e o mÃ³dulo de vendas jÃ¡ tem relacionamento com produtos atravÃ©s de OrderItem. As principais refatoraÃ§Ãµes necessÃ¡rias sÃ£o:

1. **AtualizaÃ§Ã£o da lÃ³gica de negÃ³cio** para suportar produtos compostos e grupos
2. **Aprimoramento das validaÃ§Ãµes** para diferentes tipos de produtos
3. **ExtensÃ£o dos ViewModels** para suportar seleÃ§Ã£o de componentes/grupos
4. **AtualizaÃ§Ã£o das queries** para carregar hierarquias de produtos
5. **ImplementaÃ§Ã£o de cÃ¡lculos de preÃ§o** considerando componentes e grupos

## 1. Current State Analysis

### 1.1 DomÃ­nio de Produto (Implementado e Funcional)

#### Estrutura de Entidades
```
Product (abstract base)
â”œâ”€â”€ SimpleProduct - Produto simples individual
â”œâ”€â”€ CompositeProduct - Produto formado por componentes
â””â”€â”€ ProductGroup - Grupo de produtos com opÃ§Ãµes

Entidades de Suporte:
- ProductComponent - Componentes de produtos compostos
- ProductComponentHierarchy - Hierarquia de componentes
- ProductGroupItem - Itens de grupos de produtos
- ProductGroupExchangeRule - Regras de troca em grupos
- ProductCategory - Categorias de produtos
```

#### CaracterÃ­sticas Principais
- **HeranÃ§a polimÃ³rfica** com discriminador `ProductType`
- **Suporte a composiÃ§Ã£o** atravÃ©s de ProductComponent
- **Grupos flexÃ­veis** com ProductGroupItem
- **PreÃ§os dinÃ¢micos** baseados em composiÃ§Ã£o/seleÃ§Ã£o
- **Tempo de montagem** calculado hierarquicamente

### 1.2 MÃ³dulo de Vendas Atual

#### Estrutura de Entidades
```
OrderEntry (Pedido principal)
â”œâ”€â”€ OrderItem (N itens) 
â”‚   â””â”€â”€ Product (relacionamento existente)
â”œâ”€â”€ Customer
â”œâ”€â”€ Address (entrega)
â”œâ”€â”€ FiscalData
â””â”€â”€ Contract
```

#### CaracterÃ­sticas Atuais
- **OrderItem jÃ¡ tem ProductId** - relacionamento existe
- **CÃ¡lculos simples** - Quantity * UnitPrice
- **Sem suporte a composiÃ§Ã£o** - nÃ£o gerencia componentes
- **Sem suporte a grupos** - nÃ£o permite seleÃ§Ã£o de opÃ§Ãµes

### 1.3 Schema de Banco de Dados

#### Tabela OrderItem (Atual)
```sql
OrderItem:
- Id (PK)
- OrderId (FK -> OrderEntry)
- ProductId (FK -> Product)
- Quantity
- UnitPrice
- DiscountAmount
- TaxAmount
- Notes
```

#### Tabela Product (Atual)
```sql
Product:
- Id (PK)
- ProductType (Simple/Composite/Group)
- Name, Description, Price, UnitPrice
- CategoryId (FK -> ProductCategory)
- AssemblyTime, AssemblyInstructions
```

## 2. Gap Analysis

### 2.1 Gaps Estruturais

#### OrderItem nÃ£o suporta:
1. **SeleÃ§Ã£o de componentes** para CompositeProduct
2. **SeleÃ§Ã£o de itens** para ProductGroup
3. **CÃ¡lculo de preÃ§o dinÃ¢mico** baseado em seleÃ§Ãµes
4. **Tempo de montagem agregado**
5. **ValidaÃ§Ã£o de componentes obrigatÃ³rios**

#### Repository/Service nÃ£o implementam:
1. **Carregamento de hierarquias** de produtos
2. **ValidaÃ§Ã£o de disponibilidade** de componentes
3. **CÃ¡lculo de preÃ§os** com componentes extras
4. **PersistÃªncia de seleÃ§Ãµes** do usuÃ¡rio

### 2.2 Gaps de NegÃ³cio

1. **ValidaÃ§Ãµes ausentes**:
   - Componentes obrigatÃ³rios vs opcionais
   - Quantidade mÃ­nima/mÃ¡xima em grupos
   - Disponibilidade de produtos em grupos

2. **CÃ¡lculos incompletos**:
   - PreÃ§o total com componentes extras
   - Tempo de montagem agregado
   - Descontos por grupo/volume

3. **Interface limitada**:
   - Sem UI para seleÃ§Ã£o de componentes
   - Sem visualizaÃ§Ã£o de composiÃ§Ã£o
   - Sem preÃ§o dinÃ¢mico em tempo real

## 3. Refactoring Roadmap

### 3.1 DOMAIN ENTITIES

#### A. Criar OrderItemComponent (Nova Entidade)
```csharp
public class OrderItemComponent : Entity
{
    public string OrderItemId { get; set; }
    public string ComponentId { get; set; }  // ProductComponent ou ProductGroupItem
    public string ComponentType { get; set; } // "Component" ou "GroupItem"
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal ExtraPrice { get; set; }
    
    // NavegaÃ§Ã£o
    public OrderItem OrderItem { get; set; }
    public ProductComponent? ProductComponent { get; set; }
    public ProductGroupItem? GroupItem { get; set; }
}
```

#### B. Modificar OrderItem
```csharp
// Adicionar Ã  OrderItem:
public ICollection<OrderItemComponent> SelectedComponents { get; set; }

// Novos mÃ©todos:
public decimal CalculateDynamicPrice()
{
    if (Product == null) return UnitPrice * Quantity;
    
    switch (Product.ProductType)
    {
        case ProductType.Simple:
            return UnitPrice * Quantity;
            
        case ProductType.Composite:
            var basePrice = UnitPrice * Quantity;
            var componentsPrice = SelectedComponents
                .Sum(c => c.UnitPrice * c.Quantity + c.ExtraPrice);
            return basePrice + componentsPrice;
            
        case ProductType.Group:
            return SelectedComponents
                .Sum(c => (c.UnitPrice + c.ExtraPrice) * c.Quantity);
    }
}

public int CalculateTotalAssemblyTime()
{
    // Implementar cÃ¡lculo baseado em Product e SelectedComponents
}
```

### 3.2 REPOSITORY LAYER

#### A. Criar IOrderItemComponentRepository
```csharp
public interface IOrderItemComponentRepository
{
    Task<IEnumerable<OrderItemComponent>> GetByOrderItemIdAsync(string orderItemId);
    Task<string> CreateAsync(OrderItemComponent component);
    Task<bool> UpdateAsync(OrderItemComponent component);
    Task<bool> DeleteAsync(string id);
    Task<bool> DeleteByOrderItemIdAsync(string orderItemId);
}
```

#### B. Modificar OrderItemRepository
```csharp
// Adicionar mÃ©todos:
Task<OrderItem?> GetWithComponentsAsync(string id);
Task<IEnumerable<OrderItem>> GetByOrderIdWithComponentsAsync(string orderId);

// Modificar queries para incluir JOINs com Product completo:
const string sql = @"
    SELECT oi.*, p.*, pc.*, pgi.*
    FROM OrderItem oi
    LEFT JOIN Product p ON oi.ProductId = p.Id
    LEFT JOIN OrderItemComponent oic ON oi.Id = oic.OrderItemId
    LEFT JOIN ProductComponent pc ON oic.ComponentId = pc.Id AND oic.ComponentType = 'Component'
    LEFT JOIN ProductGroupItem pgi ON oic.ComponentId = pgi.Id AND oic.ComponentType = 'GroupItem'
    WHERE oi.OrderId = @OrderId";
```

### 3.3 SERVICE LAYER

#### A. Criar OrderItemComponentService
```csharp
public interface IOrderItemComponentService
{
    Task<bool> ValidateComponentsAsync(string productId, List<OrderItemComponent> components);
    Task<decimal> CalculatePriceWithComponentsAsync(string productId, List<OrderItemComponent> components);
    Task<List<AvailableComponent>> GetAvailableComponentsAsync(string productId);
}
```

#### B. Modificar OrderService
```csharp
// Adicionar validaÃ§Ãµes:
public async Task<bool> ValidateOrderItemAsync(OrderItem item)
{
    var product = await _productRepository.GetByIdAsync(item.ProductId);
    
    switch (product.ProductType)
    {
        case ProductType.Composite:
            return await ValidateCompositeProductAsync(product as CompositeProduct, item);
            
        case ProductType.Group:
            return await ValidateProductGroupAsync(product as ProductGroup, item);
            
        default:
            return true;
    }
}

private async Task<bool> ValidateCompositeProductAsync(CompositeProduct product, OrderItem item)
{
    // Validar componentes obrigatÃ³rios
    // Validar disponibilidade
    // Validar quantidades
}
```

### 3.4 DATABASE SCHEMA

#### A. Criar tabela OrderItemComponent
```sql
CREATE TABLE OrderItemComponent (
    Id TEXT NOT NULL UNIQUE,
    CreatedAt TEXT NOT NULL,
    CreatedBy TEXT NOT NULL,
    LastModifiedAt TEXT,
    LastModifiedBy TEXT,
    StateCode INTEGER NOT NULL DEFAULT 1,
    OrderItemId TEXT NOT NULL,
    ComponentId TEXT NOT NULL,
    ComponentType TEXT NOT NULL CHECK (ComponentType IN ('Component', 'GroupItem')),
    Quantity INTEGER NOT NULL DEFAULT 1,
    UnitPrice REAL NOT NULL,
    ExtraPrice REAL DEFAULT 0,
    Notes TEXT,
    PRIMARY KEY(Id),
    FOREIGN KEY(OrderItemId) REFERENCES OrderItem(Id) ON DELETE CASCADE
);

CREATE INDEX idx_orderitemcomponent_orderitem ON OrderItemComponent(OrderItemId);
CREATE INDEX idx_orderitemcomponent_component ON OrderItemComponent(ComponentId, ComponentType);
```

#### B. Adicionar campos em OrderItem
```sql
ALTER TABLE OrderItem ADD COLUMN AssemblyTime INTEGER DEFAULT 0;
ALTER TABLE OrderItem ADD COLUMN HasCustomization INTEGER DEFAULT 0;
ALTER TABLE OrderItem ADD COLUMN CustomizationNotes TEXT;
```

### 3.5 VIEW MODELS

#### A. Criar OrderItemComponentViewModel
```csharp
public class OrderItemComponentViewModel
{
    public string Id { get; set; }
    public string ComponentId { get; set; }
    public string ComponentName { get; set; }
    public string ComponentType { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal ExtraPrice { get; set; }
    public bool IsOptional { get; set; }
    public int MinQuantity { get; set; }
    public int? MaxQuantity { get; set; }
}
```

#### B. Modificar OrderItemViewModel
```csharp
public class OrderItemViewModel
{
    // Campos existentes...
    
    // Adicionar:
    public string ProductType { get; set; }
    public List<OrderItemComponentViewModel> SelectedComponents { get; set; }
    public List<AvailableComponentViewModel> AvailableComponents { get; set; }
    public decimal DynamicPrice { get; set; }
    public int TotalAssemblyTime { get; set; }
}
```

## 4. Implementation Priority

### Fase 1: Infraestrutura Base (1-2 sprints)
1. âœ… Criar entidade OrderItemComponent
2. âœ… Criar tabela no banco de dados
3. âœ… Implementar Repository bÃ¡sico
4. âœ… Criar ViewModels

### Fase 2: LÃ³gica de NegÃ³cio (2-3 sprints)
1. âš¡ Implementar services de componentes
2. âš¡ Adicionar validaÃ§Ãµes de produtos
3. âš¡ Implementar cÃ¡lculos de preÃ§o dinÃ¢mico
4. âš¡ Criar testes unitÃ¡rios

### Fase 3: IntegraÃ§Ã£o (1-2 sprints)
1. ðŸ”„ Atualizar OrderService
2. ðŸ”„ Modificar OrderItemRepository
3. ðŸ”„ Integrar com ProductService
4. ðŸ”„ Testes de integraÃ§Ã£o

### Fase 4: Interface do UsuÃ¡rio (2-3 sprints)
1. ðŸŽ¨ Criar componentes UI para seleÃ§Ã£o
2. ðŸŽ¨ Implementar cÃ¡lculo de preÃ§o em tempo real
3. ðŸŽ¨ Adicionar validaÃ§Ãµes client-side
4. ðŸŽ¨ Testes E2E

### Fase 5: MigraÃ§Ã£o de Dados (1 sprint)
1. ðŸ“Š Criar scripts de migraÃ§Ã£o
2. ðŸ“Š Validar dados existentes
3. ðŸ“Š Executar migraÃ§Ã£o em staging
4. ðŸ“Š Deploy em produÃ§Ã£o

## 5. Risk Assessment

### Riscos Altos
1. **Breaking changes em APIs existentes**
   - MitigaÃ§Ã£o: Versionamento de API, manter compatibilidade
   
2. **Performance com mÃºltiplas JOINs**
   - MitigaÃ§Ã£o: Ãndices otimizados, cache, lazy loading

3. **Complexidade da UI para seleÃ§Ã£o**
   - MitigaÃ§Ã£o: UX iterativo, protÃ³tipos, feedback usuÃ¡rios

### Riscos MÃ©dios
1. **MigraÃ§Ã£o de dados existentes**
   - MitigaÃ§Ã£o: Scripts testados, rollback plan
   
2. **ValidaÃ§Ãµes complexas de negÃ³cio**
   - MitigaÃ§Ã£o: DocumentaÃ§Ã£o clara, testes abrangentes

### Riscos Baixos
1. **Compatibilidade com multi-tenancy**
   - MitigaÃ§Ã£o: JÃ¡ estruturado, testar isolamento

## 6. Code Examples

### Exemplo de CriaÃ§Ã£o de OrderItem com Componentes
```csharp
public async Task<string> CreateOrderItemWithComponentsAsync(
    CreateOrderItemDto dto, string userId)
{
    using var transaction = await _connectionFactory.BeginTransactionAsync();
    
    try
    {
        // 1. Validar produto
        var product = await _productRepository.GetByIdAsync(dto.ProductId);
        if (product == null)
            throw new NotFoundException("Produto nÃ£o encontrado");
        
        // 2. Criar OrderItem
        var orderItem = new OrderItem
        {
            Id = Guid.NewGuid().ToString(),
            OrderId = dto.OrderId,
            ProductId = dto.ProductId,
            Quantity = dto.Quantity,
            UnitPrice = product.UnitPrice,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        };
        
        // 3. Processar componentes baseado no tipo
        if (product.ProductType == ProductType.Composite)
        {
            await ProcessCompositeComponentsAsync(orderItem, dto.SelectedComponents);
        }
        else if (product.ProductType == ProductType.Group)
        {
            await ProcessGroupItemsAsync(orderItem, dto.SelectedComponents);
        }
        
        // 4. Calcular preÃ§o final
        orderItem.UnitPrice = await CalculateFinalPriceAsync(orderItem);
        
        // 5. Salvar OrderItem
        await _orderItemRepository.CreateAsync(orderItem);
        
        // 6. Salvar componentes
        foreach (var component in orderItem.SelectedComponents)
        {
            await _componentRepository.CreateAsync(component);
        }
        
        await transaction.CommitAsync();
        return orderItem.Id;
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}
```

### Exemplo de ValidaÃ§Ã£o de Produto Composto
```csharp
private async Task<bool> ValidateCompositeProductAsync(
    CompositeProduct product, 
    List<SelectedComponentDto> selectedComponents)
{
    // 1. Carregar hierarquia de componentes
    var hierarchies = await _hierarchyRepository
        .GetByProductIdAsync(product.Id);
    
    // 2. Validar componentes obrigatÃ³rios
    var requiredComponents = hierarchies
        .Where(h => !h.IsOptional)
        .Select(h => h.ProductComponentHierarchyId);
    
    var selectedIds = selectedComponents
        .Select(c => c.ComponentId);
    
    var missingRequired = requiredComponents
        .Except(selectedIds);
    
    if (missingRequired.Any())
    {
        throw new ValidationException(
            $"Componentes obrigatÃ³rios faltando: {string.Join(", ", missingRequired)}");
    }
    
    // 3. Validar quantidades
    foreach (var selected in selectedComponents)
    {
        var hierarchy = hierarchies
            .FirstOrDefault(h => h.ProductComponentHierarchyId == selected.ComponentId);
            
        if (hierarchy == null)
        {
            throw new ValidationException(
                $"Componente {selected.ComponentId} nÃ£o pertence ao produto");
        }
        
        if (selected.Quantity < hierarchy.MinQuantity)
        {
            throw new ValidationException(
                $"Quantidade mÃ­nima para {selected.ComponentId} Ã© {hierarchy.MinQuantity}");
        }
        
        if (hierarchy.MaxQuantity.HasValue && 
            selected.Quantity > hierarchy.MaxQuantity)
        {
            throw new ValidationException(
                $"Quantidade mÃ¡xima para {selected.ComponentId} Ã© {hierarchy.MaxQuantity}");
        }
    }
    
    return true;
}
```

## 7. Next Steps

### AÃ§Ãµes Imediatas (Sprint Atual)
1. **Revisar e aprovar** este documento com stakeholders
2. **Criar branch** feature/sales-product-integration
3. **Implementar OrderItemComponent** entity e repository
4. **Criar migration** para nova tabela

### PrÃ³ximo Sprint
1. **Implementar services** de validaÃ§Ã£o e cÃ¡lculo
2. **Criar testes unitÃ¡rios** para nova lÃ³gica
3. **Prototipar UI** para seleÃ§Ã£o de componentes
4. **Documentar APIs** novas/modificadas

### ConsideraÃ§Ãµes para ImplementaÃ§Ã£o
1. **Manter backward compatibility** - OrderItems existentes continuam funcionando
2. **Feature flags** para ativar gradualmente funcionalidades
3. **Logging detalhado** para monitorar adoÃ§Ã£o e problemas
4. **MÃ©tricas de performance** para queries complexas

## 8. Arquivos Impactados

### Novos Arquivos a Criar
```
/Models/Entities/Sales/OrderItemComponent.cs
/Interfaces/Repositories/IOrderItemComponentRepository.cs
/Data/Repositories/OrderItemComponentRepository.cs
/Interfaces/Services/IOrderItemComponentService.cs
/Services/OrderItemComponentService.cs
/Models/ViewModels/Sales/OrderItemComponentViewModels.cs
/Data/Migrations/AddOrderItemComponentTable.cs
```

### Arquivos a Modificar
```
/Models/Entities/Sales/OrderItem.cs
/Models/ViewModels/Sales/OrderItemViewModels.cs
/Interfaces/Repositories/IOrderItemRepository.cs
/Data/Repositories/OrderItemRepository.cs
/Interfaces/Services/IOrderService.cs
/Services/OrderService.cs
/Controllers/OrderController.cs
/Views/Order/Create.cshtml
/Views/Order/Edit.cshtml
/wwwroot/js/order.js
```

## ConclusÃ£o

A refatoraÃ§Ã£o proposta mantÃ©m a estrutura existente funcional enquanto adiciona suporte completo para produtos compostos e grupos. A abordagem incremental permite validaÃ§Ã£o contÃ­nua e minimiza riscos. O sistema resultante serÃ¡ mais flexÃ­vel, mantendo performance e compatibilidade com dados existentes.
