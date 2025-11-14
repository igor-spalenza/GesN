using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GesN.Web.Interfaces.Services;
using GesN.Web.Models.Entities.Sales;
using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;
using GesN.Web.Models.ViewModels.Sales;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace GesN.Web.Controllers
{
    /// <summary>
    /// Controller responsável pela gestão de itens de pedidos (OrderItems)
    /// Centraliza toda lógica de carrinho/itens de pedido
    /// </summary>
    [Authorize]
    public class OrderItemController : Controller
    {
        private readonly IOrderItemService _orderItemService;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly ILogger<OrderItemController> _logger;

        public OrderItemController(
            IOrderItemService orderItemService,
            IOrderService orderService,
            IProductService productService,
            ILogger<OrderItemController> logger)
        {
            _orderItemService = orderItemService;
            _orderService = orderService;
            _productService = productService;
            _logger = logger;
        }

        #region Add To Cart - Simple Products

        /// <summary>
        /// Adiciona um produto simples ao carrinho do pedido
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddSimpleItem([FromBody] AddSimpleItemRequest request)
        {
            try
            {
                // Validar dados básicos
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => x.Value.Errors.First().ErrorMessage)
                        .ToList();
                    
                    return Json(new { success = false, message = string.Join("; ", errors) });
                }

                // Verificar se o pedido existe
                var order = await _orderService.GetOrderByIdAsync(request.OrderId);
                if (order == null)
                {
                    return Json(new { success = false, message = "Pedido não encontrado" });
                }

                // Verificar se o produto existe e é do tipo Simple
                var product = await _productService.GetByIdAsync(request.ProductId);
                if (product == null)
                {
                    return Json(new { success = false, message = "Produto não encontrado" });
                }

                if (product.ProductType != ProductType.Simple)
                {
                    return Json(new { success = false, message = "Este endpoint é específico para produtos simples" });
                }

                // Verificar se o produto está ativo
                if (product.StateCode != ObjectState.Active)
                {
                    return Json(new { success = false, message = "Produto não está disponível" });
                }

                // Criar OrderItem
                var userId = User.Identity?.Name ?? "Sistema";
                
                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid().ToString(),
                    OrderId = request.OrderId,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    UnitPrice = product.UnitPrice,
                    DiscountAmount = request.DiscountAmount,
                    TaxAmount = request.TaxAmount,
                    Notes = request.Notes ?? string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId,
                    LastModifiedAt = DateTime.UtcNow,
                    LastModifiedBy = userId,
                    StateCode = ObjectState.Active
                };

                // Salvar OrderItem
                await _orderItemService.CreateAsync(orderItem);

                // Recalcular totais do pedido
                await _orderService.RecalculateOrderTotalsAsync(request.OrderId);

                // Retornar sucesso com dados do item criado
                return Json(new 
                { 
                    success = true, 
                    message = $"Produto '{product.Name}' adicionado ao carrinho com sucesso!",
                    item = new 
                    {
                        id = orderItem.Id,
                        productId = orderItem.ProductId,
                        productName = product.Name,
                        quantity = orderItem.Quantity,
                        unitPrice = orderItem.UnitPrice,
                        discountAmount = orderItem.DiscountAmount,
                        taxAmount = orderItem.TaxAmount,
                        subtotal = orderItem.Quantity * orderItem.UnitPrice,
                        total = (orderItem.Quantity * orderItem.UnitPrice) + orderItem.TaxAmount - orderItem.DiscountAmount
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar produto simples ao carrinho. OrderId: {OrderId}, ProductId: {ProductId}", 
                    request.OrderId, request.ProductId);
                return Json(new { success = false, message = "Erro interno do servidor ao adicionar produto" });
            }
        }

        #endregion

        #region Add To Cart - Composite Products (FASE 2)

        /// <summary>
        /// Adiciona um produto composto ao carrinho (FASE 2)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddCompositeItem([FromBody] AddCompositeItemRequest request)
        {
            try
            {
                // Validar dados básicos
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => x.Value.Errors.First().ErrorMessage)
                        .ToList();
                    
                    return Json(new { success = false, message = string.Join("; ", errors) });
                }

                // Verificar se o pedido existe
                var order = await _orderService.GetOrderByIdAsync(request.OrderId);
                if (order == null)
                {
                    return Json(new { success = false, message = "Pedido não encontrado" });
                }

                // Verificar se o produto existe e é do tipo Composite
                var product = await _productService.GetByIdAsync(request.ProductId);
                if (product == null)
                {
                    return Json(new { success = false, message = "Produto não encontrado" });
                }

                if (product.ProductType != ProductType.Composite)
                {
                    return Json(new { success = false, message = "Produto não é do tipo composto" });
                }

                // Verificar se o produto está ativo
                if (product.StateCode != ObjectState.Active)
                {
                    return Json(new { success = false, message = "Produto não está disponível" });
                }

                // Validar configuração de componentes se fornecida
                decimal calculatedUnitPrice = product.UnitPrice; // Preço base
                
                if (request.ComponentConfigurations != null && request.ComponentConfigurations.Any())
                {
                    // Calcular preço baseado nos componentes selecionados
                    decimal additionalCost = 0;
                    
                    foreach (var config in request.ComponentConfigurations)
                    {
                        var componentService = HttpContext.RequestServices.GetRequiredService<IProductComponentService>();
                        var component = await componentService.GetByIdAsync(config.ComponentId);
                        
                        if (component == null)
                        {
                            return Json(new { success = false, message = $"Componente {config.ComponentId} não encontrado" });
                        }

                        if (component.StateCode != ObjectState.Active)
                        {
                            return Json(new { success = false, message = $"Componente '{component.Name}' não está disponível" });
                        }

                        additionalCost += component.AdditionalCost * config.Quantity;
                    }
                    
                    calculatedUnitPrice += additionalCost;
                }

                // Criar OrderItem principal
                var userId = User.Identity?.Name ?? "Sistema";
                
                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid().ToString(),
                    OrderId = request.OrderId,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    UnitPrice = calculatedUnitPrice,
                    DiscountAmount = request.DiscountAmount,
                    TaxAmount = request.TaxAmount,
                    Notes = request.Notes ?? string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId,
                    LastModifiedAt = DateTime.UtcNow,
                    LastModifiedBy = userId,
                    StateCode = ObjectState.Active
                };

                // Salvar OrderItem
                await _orderItemService.CreateAsync(orderItem);

                // **INTEGRAÇÃO COM PRODUÇÃO** - Criar Demand para produto composto
                var demandService = HttpContext.RequestServices.GetRequiredService<IDemandService>();
                var demandCreated = await demandService.CreateDemandFromOrderItemAsync(orderItem.Id, userId);
                
                if (!demandCreated)
                {
                    _logger.LogWarning("Falha ao criar demanda de produção para OrderItem {OrderItemId}", orderItem.Id);
                }

                // **TODO: IMPLEMENTAR ProductComposition**
                // Para produtos compostos, após criar a Demand, precisamos criar registros de ProductComposition
                // baseados nas configurações de componentes (request.ComponentConfigurations)
                // Isso será implementado quando ProductCompositionRepository estiver disponível
                _logger.LogInformation("OrderItem {OrderItemId} criado para produto composto {ProductName} com {ComponentsCount} componentes configurados. ProductComposition será implementado posteriormente.", 
                    orderItem.Id, product.Name, request.ComponentConfigurations?.Count ?? 0);

                // Recalcular totais do pedido
                await _orderService.RecalculateOrderTotalsAsync(request.OrderId);

                // Retornar sucesso com dados detalhados
                return Json(new 
                { 
                    success = true, 
                    message = $"Produto composto '{product.Name}' adicionado ao carrinho com sucesso!",
                    item = new 
                    {
                        id = orderItem.Id,
                        productId = orderItem.ProductId,
                        productName = product.Name,
                        productType = product.ProductType.ToString(),
                        quantity = orderItem.Quantity,
                        unitPrice = orderItem.UnitPrice,
                        discountAmount = orderItem.DiscountAmount,
                        taxAmount = orderItem.TaxAmount,
                        subtotal = orderItem.Quantity * orderItem.UnitPrice,
                        total = (orderItem.Quantity * orderItem.UnitPrice) + orderItem.TaxAmount - orderItem.DiscountAmount,
                        demandCreated = demandCreated,
                        componentCount = request.ComponentConfigurations?.Count ?? 0
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar produto composto ao carrinho. OrderId: {OrderId}, ProductId: {ProductId}", 
                    request.OrderId, request.ProductId);
                return Json(new { success = false, message = "Erro interno do servidor ao adicionar produto composto" });
            }
        }

        #endregion

        #region Add To Cart - Group Products (FASE 3)

        /// <summary>
        /// Adiciona um grupo de produtos ao carrinho (FASE 3)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddGroupItem([FromBody] AddGroupItemRequest request)
        {
            try
            {
                // Validar dados básicos
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => x.Value.Errors.First().ErrorMessage)
                        .ToList();
                    
                    return Json(new { success = false, message = string.Join("; ", errors) });
                }

                // Verificar se o pedido existe
                var order = await _orderService.GetOrderByIdAsync(request.OrderId);
                if (order == null)
                {
                    return Json(new { success = false, message = "Pedido não encontrado" });
                }

                // Verificar se o produto existe e é do tipo Group
                var product = await _productService.GetByIdAsync(request.ProductId);
                if (product == null)
                {
                    return Json(new { success = false, message = "Produto não encontrado" });
                }

                if (product.ProductType != ProductType.Group)
                {
                    return Json(new { success = false, message = "Produto não é do tipo grupo" });
                }

                // Verificar se o produto está ativo
                if (product.StateCode != ObjectState.Active)
                {
                    return Json(new { success = false, message = "Produto não está disponível" });
                }

                // Validar configurações do grupo se fornecidas
                decimal calculatedUnitPrice = 0; // Grupos virtuais começam com preço 0
                var addedProductsInfo = new List<object>();
                
                if (request.GroupConfigurations != null && request.GroupConfigurations.Any())
                {
                    var productGroupService = HttpContext.RequestServices.GetRequiredService<IProductGroupService>();
                    
                    // **LÓGICA CRUCIAL: Produtos de grupo são VIRTUAIS**
                    // Cada seleção do grupo vira um OrderItem separado para o produto concreto selecionado
                    foreach (var config in request.GroupConfigurations)
                    {
                        // Validar item do grupo
                        var groupItem = await productGroupService.GetGroupItemByIdAsync(config.GroupItemId);
                        if (groupItem == null)
                        {
                            return Json(new { success = false, message = $"Item do grupo {config.GroupItemId} não encontrado" });
                        }

                        // Validar produto selecionado
                        var selectedProduct = await _productService.GetByIdAsync(config.SelectedProductId);
                        if (selectedProduct == null)
                        {
                            return Json(new { success = false, message = $"Produto selecionado {config.SelectedProductId} não encontrado" });
                        }

                        if (selectedProduct.StateCode != ObjectState.Active)
                        {
                            return Json(new { success = false, message = $"Produto '{selectedProduct.Name}' não está disponível" });
                        }

                        // Validar quantidades
                        if (config.Quantity < groupItem.MinQuantity)
                        {
                            return Json(new { success = false, message = $"Quantidade mínima para '{groupItem.GetDisplayName()}' é {groupItem.MinQuantity}" });
                        }

                        if (groupItem.MaxQuantity.HasValue && config.Quantity > groupItem.MaxQuantity.Value)
                        {
                            return Json(new { success = false, message = $"Quantidade máxima para '{groupItem.GetDisplayName()}' é {groupItem.MaxQuantity}" });
                        }

                        // Calcular preço efetivo com extras
                        decimal effectivePrice = selectedProduct.UnitPrice + groupItem.ExtraPrice;
                        
                        addedProductsInfo.Add(new
                        {
                            groupItemId = config.GroupItemId,
                            selectedProductId = config.SelectedProductId,
                            productName = selectedProduct.Name,
                            quantity = config.Quantity,
                            unitPrice = selectedProduct.UnitPrice,
                            extraPrice = groupItem.ExtraPrice,
                            effectivePrice = effectivePrice,
                            totalPrice = effectivePrice * config.Quantity * request.Quantity
                        });

                        calculatedUnitPrice += effectivePrice * config.Quantity;
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Configurações do grupo são obrigatórias" });
                }

                var userId = User.Identity?.Name ?? "Sistema";
                var createdOrderItems = new List<object>();

                // **ESTRATÉGIA GRUPOS VIRTUAIS**: Criar OrderItems individuais para cada produto selecionado
                foreach (var config in request.GroupConfigurations)
                {
                    var selectedProduct = await _productService.GetByIdAsync(config.SelectedProductId);
                    var groupItem = await HttpContext.RequestServices.GetRequiredService<IProductGroupService>()
                        .GetGroupItemByIdAsync(config.GroupItemId);
                    
                    var effectivePrice = selectedProduct.UnitPrice + groupItem.ExtraPrice;
                    var totalQuantity = config.Quantity * request.Quantity; // Quantidade do item × quantidade do grupo

                    var orderItem = new OrderItem
                    {
                        Id = Guid.NewGuid().ToString(),
                        OrderId = request.OrderId,
                        ProductId = config.SelectedProductId, // ID do produto concreto selecionado
                        Quantity = totalQuantity,
                        UnitPrice = effectivePrice,
                        DiscountAmount = request.DiscountAmount / request.GroupConfigurations.Count, // Dividir desconto proporcionalmente
                        TaxAmount = request.TaxAmount / request.GroupConfigurations.Count, // Dividir impostos proporcionalmente
                        Notes = $"[GRUPO: {product.Name}] {request.Notes ?? ""}".Trim(),
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = userId,
                        LastModifiedAt = DateTime.UtcNow,
                        LastModifiedBy = userId,
                        StateCode = ObjectState.Active
                    };

                    // Salvar OrderItem individual
                    await _orderItemService.CreateAsync(orderItem);

                    // **INTEGRAÇÃO COM PRODUÇÃO** - Criar Demand para cada produto selecionado do grupo
                    var demandService = HttpContext.RequestServices.GetRequiredService<IDemandService>();
                    var demandCreated = await demandService.CreateDemandFromOrderItemAsync(orderItem.Id, userId);
                    
                    if (!demandCreated)
                    {
                        _logger.LogWarning("Falha ao criar demanda de produção para OrderItem {OrderItemId} do grupo {GroupName}", 
                            orderItem.Id, product.Name);
                    }

                    createdOrderItems.Add(new
                    {
                        id = orderItem.Id,
                        productId = orderItem.ProductId,
                        productName = selectedProduct.Name,
                        groupReference = product.Name,
                        quantity = orderItem.Quantity,
                        unitPrice = orderItem.UnitPrice,
                        total = orderItem.Quantity * orderItem.UnitPrice,
                        demandCreated = demandCreated
                    });

                    _logger.LogInformation("OrderItem {OrderItemId} criado para produto {ProductName} do grupo {GroupName}. Demanda criada: {DemandCreated}", 
                        orderItem.Id, selectedProduct.Name, product.Name, demandCreated);
                }

                // Recalcular totais do pedido
                await _orderService.RecalculateOrderTotalsAsync(request.OrderId);

                // Retornar sucesso com dados detalhados
                return Json(new 
                { 
                    success = true, 
                    message = $"Grupo '{product.Name}' adicionado ao carrinho com {createdOrderItems.Count} produto(s)!",
                    groupInfo = new 
                    {
                        groupId = product.Id,
                        groupName = product.Name,
                        groupType = product.ProductType.ToString(),
                        totalGroupQuantity = request.Quantity,
                        totalItems = createdOrderItems.Count,
                        totalValue = addedProductsInfo.Sum(p => (decimal)((dynamic)p).totalPrice)
                    },
                    createdItems = createdOrderItems
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar grupo de produtos ao carrinho. OrderId: {OrderId}, ProductId: {ProductId}", 
                    request.OrderId, request.ProductId);
                return Json(new { success = false, message = "Erro interno do servidor ao adicionar grupo de produtos" });
            }
        }

        #endregion

        #region Item Management

        /// <summary>
        /// Atualiza um item do pedido
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UpdateItem([FromBody] UpdateOrderItemRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => x.Value.Errors.First().ErrorMessage)
                        .ToList();
                    
                    return Json(new { success = false, message = string.Join("; ", errors) });
                }

                // Buscar o OrderItem existente
                var existingItem = await _orderItemService.GetByIdAsync(request.ItemId);
                if (existingItem == null)
                {
                    return Json(new { success = false, message = "Item do pedido não encontrado" });
                }

                // Atualizar dados
                var userId = User.Identity?.Name ?? "Sistema";
                existingItem.Quantity = request.Quantity;
                existingItem.UnitPrice = request.UnitPrice;
                existingItem.DiscountAmount = request.DiscountAmount;
                existingItem.TaxAmount = request.TaxAmount;
                existingItem.Notes = request.Notes ?? string.Empty;
                existingItem.LastModifiedAt = DateTime.UtcNow;
                existingItem.LastModifiedBy = userId;

                // Salvar alterações
                var result = await _orderItemService.UpdateAsync(existingItem);
                if (!result)
                {
                    return Json(new { success = false, message = "Erro ao atualizar item do pedido" });
                }

                // Recalcular totais do pedido
                await _orderService.RecalculateOrderTotalsAsync(existingItem.OrderId);

                return Json(new { success = true, message = "Item atualizado com sucesso!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar item do pedido. ItemId: {ItemId}", request.ItemId);
                return Json(new { success = false, message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Remove um item do pedido
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> RemoveItem([FromBody] RemoveOrderItemRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.ItemId))
                {
                    return Json(new { success = false, message = "ID do item é obrigatório" });
                }

                // Buscar o OrderItem
                var existingItem = await _orderItemService.GetByIdAsync(request.ItemId);
                if (existingItem == null)
                {
                    return Json(new { success = false, message = "Item do pedido não encontrado" });
                }

                var orderId = existingItem.OrderId;

                // Remover item
                var result = await _orderItemService.DeleteAsync(request.ItemId);
                if (!result)
                {
                    return Json(new { success = false, message = "Erro ao remover item do pedido" });
                }

                // Recalcular totais do pedido
                await _orderService.RecalculateOrderTotalsAsync(orderId);

                return Json(new { success = true, message = "Item removido com sucesso!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover item do pedido. ItemId: {ItemId}", request.ItemId);
                return Json(new { success = false, message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Recarrega a lista de itens de um pedido (movido de OrderController)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ReloadItems(string orderId)
        {
            try
            {
                if (string.IsNullOrEmpty(orderId))
                {
                    return PartialView("_OrderItems", new List<OrderEntryItemViewModel>());
                }

                var order = await _orderService.GetOrderByIdAsync(orderId);
                var itemsViewModel = order?.Items?.Select(i => new OrderEntryItemViewModel
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    DiscountAmount = i.DiscountAmount,
                    TaxAmount = i.TaxAmount,
                    Notes = i.Notes
                }).ToList() ?? new List<OrderEntryItemViewModel>();

                return PartialView("_OrderItems", itemsViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recarregar itens do pedido. OrderId: {OrderId}", orderId);
                return PartialView("_OrderItems", new List<OrderEntryItemViewModel>());
            }
        }

        #endregion

        #region Item Details

        /// <summary>
        /// Obtém detalhes de um item específico
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetItemDetails(string itemId)
        {
            try
            {
                if (string.IsNullOrEmpty(itemId))
                {
                    return Json(new { success = false, message = "ID do item é obrigatório" });
                }

                var item = await _orderItemService.GetByIdAsync(itemId);
                if (item == null)
                {
                    return Json(new { success = false, message = "Item não encontrado" });
                }

                // Carregar dados do produto se necessário
                if (!string.IsNullOrEmpty(item.ProductId) && item.Product == null)
                {
                    item.Product = await _productService.GetByIdAsync(item.ProductId);
                }

                var itemDetails = new
                {
                    id = item.Id,
                    orderId = item.OrderId,
                    productId = item.ProductId,
                    productName = item.Product?.Name,
                    productType = item.Product?.ProductType.ToString(),
                    quantity = item.Quantity,
                    unitPrice = item.UnitPrice,
                    discountAmount = item.DiscountAmount,
                    taxAmount = item.TaxAmount,
                    notes = item.Notes,
                    subtotal = item.Quantity * item.UnitPrice,
                    total = (item.Quantity * item.UnitPrice) + item.TaxAmount - item.DiscountAmount
                };

                return Json(new { success = true, item = itemDetails });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter detalhes do item. ItemId: {ItemId}", itemId);
                return Json(new { success = false, message = "Erro interno do servidor" });
            }
        }

        #endregion
    }

    #region Request/Response Models

    /// <summary>
    /// Request para adicionar produto simples ao carrinho
    /// </summary>
    public class AddSimpleItemRequest
    {
        [Required(ErrorMessage = "O ID do pedido é obrigatório")]
        public string OrderId { get; set; } = string.Empty;

        [Required(ErrorMessage = "O ID do produto é obrigatório")]
        public string ProductId { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
        public int Quantity { get; set; } = 1;

        [Range(0, double.MaxValue, ErrorMessage = "O desconto não pode ser negativo")]
        public decimal DiscountAmount { get; set; } = 0;

        [Range(0, double.MaxValue, ErrorMessage = "Os impostos não podem ser negativos")]
        public decimal TaxAmount { get; set; } = 0;

        [StringLength(500, ErrorMessage = "As observações devem ter no máximo {1} caracteres")]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Configuração de componente para produto composto
    /// </summary>
    public class CompositeItemConfiguration
    {
        [Required(ErrorMessage = "O ID do componente é obrigatório")]
        public string ComponentId { get; set; } = string.Empty;

        [Required(ErrorMessage = "O ID da hierarquia é obrigatório")]
        public string HierarchyId { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
        public int Quantity { get; set; } = 1;

        public string? ComponentName { get; set; }
        public string? HierarchyName { get; set; }
        public decimal AdditionalCost { get; set; } = 0;
    }

    /// <summary>
    /// Request para adicionar produto composto ao carrinho (FASE 2)
    /// </summary>
    public class AddCompositeItemRequest
    {
        [Required(ErrorMessage = "O ID do pedido é obrigatório")]
        public string OrderId { get; set; } = string.Empty;

        [Required(ErrorMessage = "O ID do produto é obrigatório")]
        public string ProductId { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
        public int Quantity { get; set; } = 1;

        /// <summary>
        /// Configurações dos componentes selecionados para o produto composto
        /// </summary>
        public List<CompositeItemConfiguration> ComponentConfigurations { get; set; } = new();

        [Range(0, double.MaxValue, ErrorMessage = "O desconto não pode ser negativo")]
        public decimal DiscountAmount { get; set; } = 0;

        [Range(0, double.MaxValue, ErrorMessage = "Os impostos não podem ser negativos")]
        public decimal TaxAmount { get; set; } = 0;

        [StringLength(500, ErrorMessage = "As observações devem ter no máximo {1} caracteres")]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Configuração de item do grupo
    /// </summary>
    public class GroupItemConfiguration
    {
        [Required(ErrorMessage = "O ID do item do grupo é obrigatório")]
        public string GroupItemId { get; set; } = string.Empty;

        [Required(ErrorMessage = "O ID do produto selecionado é obrigatório")]
        public string SelectedProductId { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
        public int Quantity { get; set; } = 1;

        public string? GroupItemName { get; set; }
        public string? ProductName { get; set; }
        public decimal ExtraPrice { get; set; } = 0;
        public string? ItemType { get; set; } // "Produto" ou "Categoria"
    }

    /// <summary>
    /// Request para adicionar grupo de produtos ao carrinho (FASE 3)
    /// </summary>
    public class AddGroupItemRequest
    {
        [Required(ErrorMessage = "O ID do pedido é obrigatório")]
        public string OrderId { get; set; } = string.Empty;

        [Required(ErrorMessage = "O ID do produto é obrigatório")]
        public string ProductId { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
        public int Quantity { get; set; } = 1;

        /// <summary>
        /// Configurações dos itens selecionados do grupo
        /// </summary>
        [Required(ErrorMessage = "Configurações do grupo são obrigatórias")]
        public List<GroupItemConfiguration> GroupConfigurations { get; set; } = new();

        [Range(0, double.MaxValue, ErrorMessage = "O desconto não pode ser negativo")]
        public decimal DiscountAmount { get; set; } = 0;

        [Range(0, double.MaxValue, ErrorMessage = "Os impostos não podem ser negativos")]
        public decimal TaxAmount { get; set; } = 0;

        [StringLength(500, ErrorMessage = "As observações devem ter no máximo {1} caracteres")]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Request para atualizar um item do pedido
    /// </summary>
    public class UpdateOrderItemRequest
    {
        [Required(ErrorMessage = "O ID do item é obrigatório")]
        public string ItemId { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
        public int Quantity { get; set; } = 1;

        [Range(0, double.MaxValue, ErrorMessage = "O preço unitário não pode ser negativo")]
        public decimal UnitPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "O desconto não pode ser negativo")]
        public decimal DiscountAmount { get; set; } = 0;

        [Range(0, double.MaxValue, ErrorMessage = "Os impostos não podem ser negativos")]
        public decimal TaxAmount { get; set; } = 0;

        [StringLength(500, ErrorMessage = "As observações devem ter no máximo {1} caracteres")]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Request para remover um item do pedido
    /// </summary>
    public class RemoveOrderItemRequest
    {
        [Required(ErrorMessage = "O ID do item é obrigatório")]
        public string ItemId { get; set; } = string.Empty;
    }

    #endregion
}
