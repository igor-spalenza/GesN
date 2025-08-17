# ANÁLISE DE REFATORAÇÃO DO MÓDULO DE VENDAS

## Executive Summary

A análise revelou que o sistema já possui uma estrutura robusta de produtos com suporte a três tipos (Simple, Composite, Group) e o módulo de vendas já tem relacionamento com produtos através de OrderItem. As principais refatorações necessárias são:

1. **Atualização da lógica de negócio** para suportar produtos compostos e grupos
2. **Aprimoramento das validações** para diferentes tipos de produtos
3. **Extensão dos ViewModels** para suportar seleção de componentes/grupos
4. **Atualização das queries** para carregar hierarquias de produtos
5. **Implementação de cálculos de preço** considerando componentes e grupos

## 1. Current State Analysis

### 1.1 Domínio de Produto (Implementado e Funcional)

#### Estrutura de Entidades
```
Product (abstract base)
├── SimpleProduct - Produto simples individual
├── CompositeProduct - Produto formado por componentes
└── ProductGroup - Grupo de produtos com opções

Entidades de Suporte:
- ProductComponent - Componentes de produtos compostos
- ProductComponentHierarchy - Hierarquia de componentes
- ProductGroupItem - Itens de grupos de produtos
- ProductGroupExchangeRule - Regras de troca em grupos
- ProductCategory - Categorias de produtos
```

#### Características Principais
- **Herança polimórfica** com discriminador `ProductType`
- **Suporte a composição** através de ProductComponent
- **Grupos flexíveis** com ProductGroupItem
- **Preços dinâmicos** baseados em composição/seleção
- **Tempo de montagem** calculado hierarquicamente

### 1.2 Módulo de Vendas Atual

#### Estrutura de Entidades
```
OrderEntry (Pedido principal)
├── OrderItem (N itens) 
│   └── Product (relacionamento existente)
├── Customer
├── Address (entrega)
├── FiscalData
└── Contract
```

#### Características Atuais
- **OrderItem já tem ProductId** - relacionamento existe
- **Cálculos simples** - Quantity * UnitPrice
- **Sem suporte a composição** - não gerencia componentes
- **Sem suporte a grupos** - não permite seleção de opções

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

#### OrderItem não suporta:
1. **Seleção de componentes** para CompositeProduct
2. **Seleção de itens** para ProductGroup
3. **Cálculo de preço dinâmico** baseado em seleções
4. **Tempo de montagem agregado**
5. **Validação de componentes obrigatórios**

#### Repository/Service não implementam:
1. **Carregamento de hierarquias** de produtos
2. **Validação de disponibilidade** de componentes
3. **Cálculo de preços** com componentes extras
4. **Persistência de seleções** do usuário

### 2.2 Gaps de Negócio

1. **Validações ausentes**:
   - Componentes obrigatórios vs opcionais
   - Quantidade mínima/máxima em grupos
   - Disponibilidade de produtos em grupos

2. **Cálculos incompletos**:
   - Preço total com componentes extras
   - Tempo de montagem agregado
   - Descontos por grupo/volume

3. **Interface limitada**:
   - Sem UI para seleção de componentes
   - Sem visualização de composição
   - Sem preço dinâmico em tempo real

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
    
    // Navegação
    public OrderItem OrderItem { get; set; }
    public ProductComponent? ProductComponent { get; set; }
    public ProductGroupItem? GroupItem { get; set; }
}
```

#### B. Modificar OrderItem
```csharp
// Adicionar à OrderItem:
public ICollection<OrderItemComponent> SelectedComponents { get; set; }

// Novos métodos:
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
    // Implementar cálculo baseado em Product e SelectedComponents
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
// Adicionar métodos:
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
// Adicionar validações:
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
    // Validar componentes obrigatórios
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
1. ✅ Criar entidade OrderItemComponent
2. ✅ Criar tabela no banco de dados
3. ✅ Implementar Repository básico
4. ✅ Criar ViewModels

### Fase 2: Lógica de Negócio (2-3 sprints)
1. ⚡ Implementar services de componentes
2. ⚡ Adicionar validações de produtos
3. ⚡ Implementar cálculos de preço dinâmico
4. ⚡ Criar testes unitários

### Fase 3: Integração (1-2 sprints)
1. 🔄 Atualizar OrderService
2. 🔄 Modificar OrderItemRepository
3. 🔄 Integrar com ProductService
4. 🔄 Testes de integração

### Fase 4: Interface do Usuário (2-3 sprints)
1. 🎨 Criar componentes UI para seleção
2. 🎨 Implementar cálculo de preço em tempo real
3. 🎨 Adicionar validações client-side
4. 🎨 Testes E2E

### Fase 5: Migração de Dados (1 sprint)
1. 📊 Criar scripts de migração
2. 📊 Validar dados existentes
3. 📊 Executar migração em staging
4. 📊 Deploy em produção

## 5. Risk Assessment

### Riscos Altos
1. **Breaking changes em APIs existentes**
   - Mitigação: Versionamento de API, manter compatibilidade
   
2. **Performance com múltiplas JOINs**
   - Mitigação: Índices otimizados, cache, lazy loading

3. **Complexidade da UI para seleção**
   - Mitigação: UX iterativo, protótipos, feedback usuários

### Riscos Médios
1. **Migração de dados existentes**
   - Mitigação: Scripts testados, rollback plan
   
2. **Validações complexas de negócio**
   - Mitigação: Documentação clara, testes abrangentes

### Riscos Baixos
1. **Compatibilidade com multi-tenancy**
   - Mitigação: Já estruturado, testar isolamento

## 6. Code Examples

### Exemplo de Criação de OrderItem com Componentes
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
            throw new NotFoundException("Produto não encontrado");
        
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
        
        // 4. Calcular preço final
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

### Exemplo de Validação de Produto Composto
```csharp
private async Task<bool> ValidateCompositeProductAsync(
    CompositeProduct product, 
    List<SelectedComponentDto> selectedComponents)
{
    // 1. Carregar hierarquia de componentes
    var hierarchies = await _hierarchyRepository
        .GetByProductIdAsync(product.Id);
    
    // 2. Validar componentes obrigatórios
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
            $"Componentes obrigatórios faltando: {string.Join(", ", missingRequired)}");
    }
    
    // 3. Validar quantidades
    foreach (var selected in selectedComponents)
    {
        var hierarchy = hierarchies
            .FirstOrDefault(h => h.ProductComponentHierarchyId == selected.ComponentId);
            
        if (hierarchy == null)
        {
            throw new ValidationException(
                $"Componente {selected.ComponentId} não pertence ao produto");
        }
        
        if (selected.Quantity < hierarchy.MinQuantity)
        {
            throw new ValidationException(
                $"Quantidade mínima para {selected.ComponentId} é {hierarchy.MinQuantity}");
        }
        
        if (hierarchy.MaxQuantity.HasValue && 
            selected.Quantity > hierarchy.MaxQuantity)
        {
            throw new ValidationException(
                $"Quantidade máxima para {selected.ComponentId} é {hierarchy.MaxQuantity}");
        }
    }
    
    return true;
}
```

## 7. Next Steps

### Ações Imediatas (Sprint Atual)
1. **Revisar e aprovar** este documento com stakeholders
2. **Criar branch** feature/sales-product-integration
3. **Implementar OrderItemComponent** entity e repository
4. **Criar migration** para nova tabela

### Próximo Sprint
1. **Implementar services** de validação e cálculo
2. **Criar testes unitários** para nova lógica
3. **Prototipar UI** para seleção de componentes
4. **Documentar APIs** novas/modificadas

### Considerações para Implementação
1. **Manter backward compatibility** - OrderItems existentes continuam funcionando
2. **Feature flags** para ativar gradualmente funcionalidades
3. **Logging detalhado** para monitorar adoção e problemas
4. **Métricas de performance** para queries complexas

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

## Conclusão

A refatoração proposta mantém a estrutura existente funcional enquanto adiciona suporte completo para produtos compostos e grupos. A abordagem incremental permite validação contínua e minimiza riscos. O sistema resultante será mais flexível, mantendo performance e compatibilidade com dados existentes.