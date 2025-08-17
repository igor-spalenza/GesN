using Microsoft.AspNetCore.Mvc;
using GesN.Web.Interfaces.Services;
using GesN.Web.Interfaces.Repositories;
using GesN.Web.Models.Entities.Production;
using Microsoft.AspNetCore.Authorization;
using GesN.Web.Models.Enumerators;
using GesN.Web.Models.ViewModels.Production;
using System.Security.Claims;

namespace GesN.Web.Controllers
{
    [Authorize]
    public class ProductGroupController : Controller
    {
        private readonly IProductGroupService _productGroupService;
        private readonly IProductService _productService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductGroupController> _logger;

        public ProductGroupController(
            IProductGroupService productGroupService,
            IProductService productService,
            IProductCategoryService productCategoryService,
            IProductRepository productRepository,
            ILogger<ProductGroupController> logger)
        {
            _productGroupService = productGroupService;
            _productService = productService;
            _productCategoryService = productCategoryService;
            _productRepository = productRepository;
            _logger = logger;
        }

        // GET: ProductGroup
        public async Task<IActionResult> Index()
        {
            var groups = await _productService.GetByTypeAsync(ProductType.Group);
            return View(groups.Cast<ProductGroup>());
        }

        // GET: ProductGroup/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var group = await _productService.GetByIdAsync(id) as ProductGroup;
            if (group == null)
                return NotFound();

            // Carregar itens e regras de troca
            var groupItems = await _productGroupService.GetGroupItemsAsync(id);
            var exchangeRules = await _productGroupService.GetExchangeRulesAsync(id);

            ViewBag.GroupItems = groupItems;
            ViewBag.ExchangeRules = exchangeRules;

            return View(group);
        }

        // GET: ProductGroup/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsAsync();
            
            var group = new ProductGroup();
            
            return View(group);
        }

        // POST: ProductGroup/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductGroup group)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _productService.CreateAsync(group);
                    TempData["SuccessMessage"] = "Grupo de produtos criado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Erro interno do servidor.");
                }
            }

            await PopulateDropdownsAsync();
            return View(group);
        }

        // GET: ProductGroup/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var group = await _productService.GetByIdAsync(id) as ProductGroup;
            if (group == null)
                return NotFound();

            await PopulateDropdownsAsync();
            return View(group);
        }

        // POST: ProductGroup/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ProductGroup group)
        {
            if (id != group.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var success = await _productService.UpdateAsync(group);
                    if (success)
                    {
                        TempData["SuccessMessage"] = "Grupo de produtos atualizado com sucesso!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Grupo de produtos não encontrado.");
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Erro interno do servidor.");
                }
            }

            await PopulateDropdownsAsync();
            return View(group);
        }

        // GET: ProductGroup/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var group = await _productService.GetByIdAsync(id) as ProductGroup;
            if (group == null)
                return NotFound();

            return View(group);
        }

        // POST: ProductGroup/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var success = await _productService.DeleteAsync(id);
                if (success)
                {
                    TempData["SuccessMessage"] = "Grupo de produtos excluído com sucesso!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Grupo de produtos não encontrado.";
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Erro ao excluir grupo de produtos.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: ProductGroup/ManageItems/5
        public async Task<IActionResult> ManageItems(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var group = await _productService.GetByIdAsync(id) as ProductGroup;
            if (group == null)
                return NotFound();

            var groupItems = await _productGroupService.GetGroupItemsAsync(id);
            ViewBag.Group = group;
            ViewBag.AvailableProducts = await GetAvailableProductsAsync(id);

            return View(groupItems);
        }

        // POST: ProductGroup/AddItem
        [HttpPost]
        public async Task<IActionResult> AddItem(ProductGroupItem item)
        {
            try
            {
                await _productGroupService.AddGroupItemAsync(item);
                return Json(new { success = true, message = "Item adicionado ao grupo com sucesso!" });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Erro interno do servidor." });
            }
        }

        // POST: ProductGroup/UpdateItem
        [HttpPost]
        public async Task<IActionResult> UpdateItem(ProductGroupItem item)
        {
            try
            {
                var success = await _productGroupService.UpdateGroupItemAsync(item);
                if (success)
                {
                    return Json(new { success = true, message = "Item atualizado com sucesso!" });
                }
                else
                {
                    return Json(new { success = false, message = "Item não encontrado." });
                }
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Erro interno do servidor." });
            }
        }

        // POST: ProductGroup/RemoveItem
        [HttpPost]
        public async Task<IActionResult> RemoveItem(string itemId)
        {
            try
            {
                var success = await _productGroupService.RemoveGroupItemAsync(itemId);
                if (success)
                {
                    return Json(new { success = true, message = "Item removido do grupo com sucesso!" });
                }
                else
                {
                    return Json(new { success = false, message = "Item não encontrado." });
                }
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Erro interno do servidor." });
            }
        }

        // ManageOptions method removed - ProductGroupOptions were removed from the system





        // GET: ProductGroup/ManageExchangeRules/5
        public async Task<IActionResult> ManageExchangeRules(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var group = await _productService.GetByIdAsync(id) as ProductGroup;
            if (group == null)
                return NotFound();

            var exchangeRules = await _productGroupService.GetExchangeRulesAsync(id);
            ViewBag.Group = group;
            ViewBag.AvailableProducts = await GetAvailableProductsAsync(id);

            return View(exchangeRules);
        }

        // POST: ProductGroup/AddExchangeRule
        [HttpPost]
        public async Task<IActionResult> AddExchangeRule(ProductGroupExchangeRule rule)
        {
            try
            {
                await _productGroupService.AddExchangeRuleAsync(rule);
                return Json(new { success = true, message = "Regra de troca adicionada com sucesso!" });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Erro interno do servidor." });
            }
        }

        // POST: ProductGroup/UpdateExchangeRule
        [HttpPost]
        public async Task<IActionResult> UpdateExchangeRule(ProductGroupExchangeRule rule)
        {
            try
            {
                var success = await _productGroupService.UpdateExchangeRuleAsync(rule);
                if (success)
                {
                    return Json(new { success = true, message = "Regra de troca atualizada com sucesso!" });
                }
                else
                {
                    return Json(new { success = false, message = "Regra de troca não encontrada." });
                }
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Erro interno do servidor." });
            }
        }

        // POST: ProductGroup/RemoveExchangeRule
        [HttpPost]
        public async Task<IActionResult> RemoveExchangeRule(string id)
        {
            try
            {
                var success = await _productGroupService.RemoveExchangeRuleAsync(id);
                if (success)
                {
                    return Json(new { success = true, message = "Regra de troca removida com sucesso!" });
                }
                else
                {
                    return Json(new { success = false, message = "Regra de troca não encontrada." });
                }
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Erro interno do servidor." });
            }
        }

        // AJAX: Calculate group price
        [HttpGet]
        public async Task<IActionResult> CalculateGroupPrice(string groupId)
        {
            try
            {
                var price = await _productGroupService.CalculateGroupPriceAsync(groupId);
                return Json(new { success = true, price });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // AJAX: Validate group configuration
        [HttpPost]
        public async Task<IActionResult> ValidateGroupConfiguration(string groupId)
        {
            try
            {
                var group = await _productService.GetByIdAsync(groupId) as ProductGroup;
                if (group == null)
                {
                    return Json(new { success = false, message = "Grupo não encontrado." });
                }

                var isValid = await _productGroupService.ValidateGroupConfigurationAsync(group);
                return Json(new { success = true, isValid });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // AJAX: Search groups
        [HttpGet]
        public async Task<IActionResult> Search(string term)
        {
            try
            {
                var groups = await _productService.SearchAsync(term);
                var productGroups = groups.Where(p => p.ProductType == ProductType.Group).Cast<ProductGroup>();
                return PartialView("_GroupList", productGroups);
            }
            catch (Exception)
            {
                return PartialView("_GroupList", new List<ProductGroup>());
            }
        }

        private async Task PopulateDropdownsAsync()
        {
            var categories = await _productService.GetCategoriesAsync();
            ViewBag.Categories = categories.Select(c => new { c.Id, c.Name });
        }

        private async Task<IEnumerable<Product>> GetAvailableProductsAsync(string groupId)
        {
            var allProducts = await _productService.GetAllAsync();
            var groupItems = await _productGroupService.GetGroupItemsAsync(groupId);
            var usedProductIds = groupItems.Select(gi => gi.ProductId).ToHashSet();
            
            return allProducts.Where(p => !usedProductIds.Contains(p.Id) && p.ProductType == ProductType.Simple);
        }

        #region ProductGroup Items Management

        // GET: ProductGroup/ProductGroupItems/5
        [HttpGet]
        public async Task<IActionResult> ProductGroupItems(string productId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productId))
                {
                    return BadRequest("ID do produto é obrigatório");
                }

                var groupItems = await _productGroupService.GetGroupItemsAsync(productId);
                var viewModels = new List<ProductGroupItemViewModel>();
                
                foreach (var item in groupItems)
                {
                    var viewModel = new ProductGroupItemViewModel
                    {
                        Id = item.Id,
                        ProductGroupId = item.ProductGroupId,
                        ProductId = item.ProductId,
                        ProductCategoryId = item.ProductCategoryId,
                        Quantity = item.Quantity,
                        MinQuantity = item.MinQuantity,
                        MaxQuantity = item.MaxQuantity,
                        DefaultQuantity = item.DefaultQuantity,
                        IsOptional = item.IsOptional,
                        ExtraPrice = item.ExtraPrice,
                        CreatedAt = item.CreatedAt,
                        ModifiedAt = item.LastModifiedAt
                    };

                    // Populate product or category data
                    if (!string.IsNullOrWhiteSpace(item.ProductId))
                    {
                        var product = await _productService.GetByIdAsync(item.ProductId);
                        viewModel.ProductName = product?.Name ?? "Produto não encontrado";
                    }
                    else if (!string.IsNullOrWhiteSpace(item.ProductCategoryId))
                    {
                        var category = await _productCategoryService.GetCategoryByIdAsync(item.ProductCategoryId);
                        viewModel.ProductCategoryName = category?.Name ?? "Categoria não encontrada";
                    }

                    viewModels.Add(viewModel);
                }

                ViewBag.ProductId = productId;
                return PartialView("~/Views/ProductGroup/_ProductGroupItems.cshtml", viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar itens do grupo: {ProductId}", productId);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        // GET: ProductGroup/FormularioGroupItem/5
        [HttpGet]
        public IActionResult FormularioGroupItem(string id)
        {
            _logger.LogInformation("FormularioGroupItem called with id: {Id}", id);
            
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Id is null or empty in FormularioGroupItem");
                return BadRequest("ProductId é obrigatório");
            }
            
            var viewModel = new CreateProductGroupItemViewModel
            {
                ProductGroupId = id
            };
            
            _logger.LogInformation("ViewModel created with ProductGroupId: {ProductGroupId}", viewModel.ProductGroupId);
            
            return PartialView("~/Views/ProductGroup/_CreateGroupItem.cshtml", viewModel);
        }

        // GET: ProductGroup/FormularioEdicaoGroupItem/5
        [HttpGet]
        public async Task<IActionResult> FormularioEdicaoGroupItem(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest("ID do item é obrigatório");
                }

                var item = await _productGroupService.GetGroupItemByIdAsync(id);
                if (item == null)
                {
                    return NotFound("Item não encontrado");
                }

                var productGroup = await _productService.GetByIdAsync(item.ProductGroupId);

                var viewModel = new EditProductGroupItemViewModel
                {
                    Id = item.Id,
                    ProductGroupId = item.ProductGroupId,
                    ProductGroupName = productGroup?.Name ?? "Grupo não encontrado",
                    ProductId = item.ProductId,
                    ProductCategoryId = item.ProductCategoryId,
                    Quantity = item.Quantity,
                    MinQuantity = item.MinQuantity,
                    MaxQuantity = item.MaxQuantity,
                    DefaultQuantity = item.DefaultQuantity,
                    IsOptional = item.IsOptional,
                    ExtraPrice = item.ExtraPrice
                };

                // Populate product or category name based on what's set
                if (!string.IsNullOrWhiteSpace(item.ProductId))
                {
                    var product = await _productService.GetByIdAsync(item.ProductId);
                    viewModel.ProductName = product?.Name + (product?.SKU != null ? " - " + product.SKU : "");
                    viewModel.ItemType = "Produto";
                }
                else if (!string.IsNullOrWhiteSpace(item.ProductCategoryId))
                {
                    var category = await _productCategoryService.GetCategoryByIdAsync(item.ProductCategoryId);
                    viewModel.ProductCategoryName = category?.Name;
                    viewModel.ItemType = "Categoria";
                }

                return PartialView("~/Views/ProductGroup/_EditGroupItem.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de edição do item: {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        // POST: ProductGroup/SalvarGroupItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarGroupItem(CreateProductGroupItemViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    return Json(new { success = false, message = "Dados inválidos", errors = errors });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
                
                var item = new ProductGroupItem
                {
                    ProductGroupId = viewModel.ProductGroupId,
                    ProductId = viewModel.ProductId,
                    ProductCategoryId = viewModel.ProductCategoryId,
                    Quantity = viewModel.Quantity,
                    MinQuantity = viewModel.MinQuantity ?? 1,
                    MaxQuantity = viewModel.MaxQuantity ?? 0,
                    DefaultQuantity = viewModel.DefaultQuantity ?? 1,
                    IsOptional = viewModel.IsOptional,
                    ExtraPrice = viewModel.ExtraPrice ?? 0,
                    CreatedBy = userId,
                    CreatedAt = DateTime.UtcNow
                };

                var success = await _productGroupService.AddGroupItemAsync(item);

                if (success)
                {
                    return Json(new { success = true, message = "Item adicionado com sucesso!" });
                }

                return Json(new { success = false, message = "Erro ao adicionar item" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar item do grupo");
                return Json(new { success = false, message = "Erro interno do servidor" });
            }
        }

        // POST: ProductGroup/SalvarEdicaoGroupItem/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarEdicaoGroupItem(string id, EditProductGroupItemViewModel viewModel)
        {
            try
            {
                if (id != viewModel.Id)
                {
                    return Json(new { success = false, message = "ID inconsistente" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    return Json(new { success = false, message = "Dados inválidos", errors = errors });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
                
                var item = new ProductGroupItem
                {
                    Id = viewModel.Id,
                    ProductGroupId = viewModel.ProductGroupId,
                    ProductId = viewModel.ProductId,
                    ProductCategoryId = viewModel.ProductCategoryId,
                    Quantity = viewModel.Quantity,
                    MinQuantity = viewModel.MinQuantity ?? 1,
                    MaxQuantity = viewModel.MaxQuantity ?? 0,
                    DefaultQuantity = viewModel.DefaultQuantity ?? 1,
                    IsOptional = viewModel.IsOptional,
                    ExtraPrice = viewModel.ExtraPrice ?? 0,
                    LastModifiedBy = userId,
                    LastModifiedAt = DateTime.UtcNow
                };

                var success = await _productGroupService.UpdateGroupItemAsync(item);

                if (success)
                {
                    return Json(new { success = true, message = "Item atualizado com sucesso!" });
                }

                return Json(new { success = false, message = "Erro ao atualizar item" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar item do grupo: {Id}", id);
                return Json(new { success = false, message = "Erro interno do servidor" });
            }
        }

        // POST: ProductGroup/ExcluirGroupItem/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirGroupItem(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return Json(new { success = false, message = "ID do item é obrigatório" });
                }

                var success = await _productGroupService.RemoveGroupItemAsync(id);

                if (success)
                {
                    return Json(new { success = true, message = "Item removido com sucesso!" });
                }

                return Json(new { success = false, message = "Item não encontrado" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir item do grupo: {Id}", id);
                return Json(new { success = false, message = "Erro interno do servidor" });
            }
        }

        #endregion

        #region ProductGroup Items Autocomplete

        // GET: ProductGroup/BuscaGroupItemAutocomplete
        [HttpGet]
        public async Task<IActionResult> BuscaGroupItemAutocomplete(string termo, string productGroupId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(termo) || string.IsNullOrWhiteSpace(productGroupId))
                {
                    return Json(new List<object>());
                }

                // ✅ CORREÇÃO: Usar método que carrega Product OU ProductCategory
                var groupItems = await _productGroupService.GetGroupItemsWithProductDataAsync(productGroupId);
                
                var filteredItems = groupItems
                    .Where(item => 
                    {
                        // ✅ CORREÇÃO: Filtrar tanto Product quanto ProductCategory
                        if (item.Product != null)
                        {
                            return item.Product.Name.Contains(termo, StringComparison.OrdinalIgnoreCase) ||
                                   (item.Product.SKU != null && item.Product.SKU.Contains(termo, StringComparison.OrdinalIgnoreCase));
                        }
                        else if (item.ProductCategory != null)
                        {
                            return item.ProductCategory.Name.Contains(termo, StringComparison.OrdinalIgnoreCase);
                        }
                        return false;
                    })
                    .Select(item => 
                    {
                        // ✅ CORREÇÃO: Usar método unificado para display name
                        var displayName = _productGroupService.GetGroupItemDisplayName(item);
                        var itemName = "";
                        var itemSKU = "";
                        
                        if (item.Product != null)
                        {
                            itemName = item.Product.Name;
                            itemSKU = item.Product.SKU ?? "";
                        }
                        else if (item.ProductCategory != null)
                        {
                            itemName = item.ProductCategory.Name + " (Categoria)";
                            itemSKU = "";
                        }
                        
                        return new
                        {
                            id = item.Id,
                            label = displayName,
                            value = displayName,
                            productName = itemName,
                            productSKU = itemSKU,
                            weight = item.Quantity,
                            unitPrice = item.GetEffectivePrice()
                        };
                    })
                    .Take(10)
                    .ToList();

                return Json(filteredItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar itens do grupo para autocomplete: {Termo}", termo);
                return Json(new List<object>());
            }
        }

        #endregion

        #region ProductGroup Exchange Rules Management

        // GET: ProductGroup/ItemExchangeRules/{productGroupId}/{itemId}
        [HttpGet]
        [Route("ProductGroup/ItemExchangeRules/{productGroupId}/{itemId}")]
        public async Task<IActionResult> ItemExchangeRules(string productGroupId, string itemId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productGroupId) || string.IsNullOrWhiteSpace(itemId))
                {
                    return BadRequest("ProductGroupId e ItemId são obrigatórios");
                }

                // ✅ CORREÇÃO: Obter informações do item com dados carregados (Product ou ProductCategory)
                var item = await _productGroupService.GetGroupItemWithDataByIdAsync(itemId);
                if (item == null)
                {
                    return NotFound("Item não encontrado");
                }

                // ✅ CORREÇÃO: Usar método unificado para obter display name
                var itemName = _productGroupService.GetGroupItemDisplayName(item);

                // Obter regras onde este item é origem
                var sourceRules = await _productGroupService.GetExchangeRulesAsync(productGroupId);
                var sourceRulesList = sourceRules.Where(r => r.SourceGroupItemId == itemId).ToList();

                // Obter regras onde este item é destino
                var targetRulesList = sourceRules.Where(r => r.TargetGroupItemId == itemId).ToList();

                // Converter para ViewModels
                var sourceRulesViewModels = new List<ProductGroupExchangeRuleViewModel>();
                var targetRulesViewModels = new List<ProductGroupExchangeRuleViewModel>();

                foreach (var rule in sourceRulesList)
                {
                    // ✅ CORREÇÃO: Buscar target item com dados carregados
                    var targetItem = await _productGroupService.GetGroupItemWithDataByIdAsync(rule.TargetGroupItemId);
                    var targetItemName = _productGroupService.GetGroupItemDisplayName(targetItem);
                    
                    sourceRulesViewModels.Add(new ProductGroupExchangeRuleViewModel
                    {
                        Id = rule.Id,
                        ProductGroupId = rule.ProductGroupId,
                        SourceGroupItemId = rule.SourceGroupItemId,
                        SourceGroupItemName = itemName,
                        SourceGroupItemWeight = rule.SourceGroupItemWeight,
                        TargetGroupItemId = rule.TargetGroupItemId,
                        TargetGroupItemName = targetItemName,
                        TargetGroupItemWeight = rule.TargetGroupItemWeight,
                        ExchangeRatio = rule.ExchangeRatio,
                        IsActive = rule.IsActive,
                        CreatedAt = rule.CreatedAt,
                        ModifiedAt = rule.LastModifiedAt
                    });
                }

                foreach (var rule in targetRulesList)
                {
                    // ✅ CORREÇÃO: Buscar source item com dados carregados
                    var sourceItem = await _productGroupService.GetGroupItemWithDataByIdAsync(rule.SourceGroupItemId);
                    var sourceItemName = _productGroupService.GetGroupItemDisplayName(sourceItem);
                    
                    targetRulesViewModels.Add(new ProductGroupExchangeRuleViewModel
                    {
                        Id = rule.Id,
                        ProductGroupId = rule.ProductGroupId,
                        SourceGroupItemId = rule.SourceGroupItemId,
                        SourceGroupItemName = sourceItemName,
                        SourceGroupItemWeight = rule.SourceGroupItemWeight,
                        TargetGroupItemId = rule.TargetGroupItemId,
                        TargetGroupItemName = itemName,
                        TargetGroupItemWeight = rule.TargetGroupItemWeight,
                        ExchangeRatio = rule.ExchangeRatio,
                        IsActive = rule.IsActive,
                        CreatedAt = rule.CreatedAt,
                        ModifiedAt = rule.LastModifiedAt
                    });
                }

                // Combinando as regras para usar a view existente
                var allRules = new List<ProductGroupExchangeRuleViewModel>();
                allRules.AddRange(sourceRulesViewModels);
                allRules.AddRange(targetRulesViewModels);
                
                ViewBag.ProductId = productGroupId;
                ViewBag.ItemId = itemId;
                ViewBag.ItemName = itemName;
                ViewBag.IsItemSpecific = true;
                
                return PartialView("~/Views/ProductGroup/_ProductGroupExchangeRules.cshtml", allRules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar regras de troca do item: {ProductGroupId}/{ItemId}", productGroupId, itemId);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        // GET: ProductGroup/ExchangeRulesInfo/{itemId}
        [HttpGet]
        [Route("ProductGroup/ExchangeRulesInfo/{itemId}")]
        public async Task<IActionResult> ExchangeRulesInfo(string itemId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(itemId))
                {
                    return BadRequest("ItemId é obrigatório");
                }

                var item = await _productGroupService.GetGroupItemByIdAsync(itemId);
                if (item == null)
                {
                    return Json(new { success = false, message = "Item não encontrado" });
                }

                var allRules = await _productGroupService.GetExchangeRulesAsync(item.ProductGroupId);
                var sourceRulesCount = allRules.Count(r => r.SourceGroupItemId == itemId && r.IsActive);
                var targetRulesCount = allRules.Count(r => r.TargetGroupItemId == itemId && r.IsActive);
                var totalRules = sourceRulesCount + targetRulesCount;

                var info = new
                {
                    success = true,
                    totalRules = totalRules,
                    sourceRulesCount = sourceRulesCount,
                    targetRulesCount = targetRulesCount,
                    html = totalRules > 0 
                        ? $"<span class='badge bg-success'>{totalRules} ativa{(totalRules > 1 ? "s" : "")}</span>"
                        : "<span class='text-muted'>Sem regras</span>"
                };

                return Json(info);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter informações de regras de troca: {ItemId}", itemId);
                return Json(new { success = false, message = "Erro interno do servidor" });
            }
        }

        // GET: ProductGroup/ProductGroupExchangeRules/5
        [HttpGet]
        public async Task<IActionResult> ProductGroupExchangeRules(string productId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productId))
                {
                    return BadRequest("ID do produto é obrigatório");
                }

                var exchangeRules = await _productGroupService.GetExchangeRulesAsync(productId);
                var viewModels = new List<ProductGroupExchangeRuleViewModel>();
                
                foreach (var rule in exchangeRules)
                {
                    // ✅ CORREÇÃO: Buscar itens com dados carregados (Product ou ProductCategory)
                    var sourceItem = await _productGroupService.GetGroupItemWithDataByIdAsync(rule.SourceGroupItemId);
                    var targetItem = await _productGroupService.GetGroupItemWithDataByIdAsync(rule.TargetGroupItemId);
                    
                    var sourceItemName = _productGroupService.GetGroupItemDisplayName(sourceItem);
                    var targetItemName = _productGroupService.GetGroupItemDisplayName(targetItem);
                    
                    viewModels.Add(new ProductGroupExchangeRuleViewModel
                    {
                        Id = rule.Id,
                        ProductGroupId = rule.ProductGroupId,
                        SourceGroupItemId = rule.SourceGroupItemId,
                        SourceGroupItemName = sourceItemName,
                        SourceGroupItemWeight = rule.SourceGroupItemWeight,
                        TargetGroupItemId = rule.TargetGroupItemId,
                        TargetGroupItemName = targetItemName,
                        TargetGroupItemWeight = rule.TargetGroupItemWeight,
                        ExchangeRatio = rule.ExchangeRatio,
                        IsActive = rule.IsActive,
                        CreatedAt = rule.CreatedAt,
                        ModifiedAt = rule.LastModifiedAt
                    });
                }

                ViewBag.ProductId = productId;
                return PartialView("~/Views/ProductGroup/_ProductGroupExchangeRules.cshtml", viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar regras de troca do grupo: {ProductId}", productId);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        // GET: ProductGroup/FormularioGroupExchangeRule/{productGroupId}/{sourceItemId?}
        [HttpGet]
        public async Task<IActionResult> FormularioGroupExchangeRule(string id, string sourceItemId = null)
        {
            try
            {
                var viewModel = new CreateProductGroupExchangeRuleViewModel
                {
                    ProductGroupId = id
                };

                // Se foi especificado um item de origem, carregar seus dados
                if (!string.IsNullOrWhiteSpace(sourceItemId))
                {
                    // ✅ CORREÇÃO: Usar método que carrega Product OU ProductCategory
                    var sourceItem = await _productGroupService.GetGroupItemWithDataByIdAsync(sourceItemId);
                    if (sourceItem != null)
                    {
                        viewModel.SourceGroupItemId = sourceItem.Id;
                        viewModel.SourceGroupItemWeight = sourceItem.Quantity;
                        
                        // ✅ CORREÇÃO: Usar método unificado para obter display name
                        ViewBag.SourceGroupItemName = _productGroupService.GetGroupItemDisplayName(sourceItem);
                        ViewBag.IsSourceItemPredefined = true;
                    }
                }

                return PartialView("~/Views/ProductGroup/_CreateGroupExchangeRule.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de criação de regra: {ProductGroupId}/{SourceItemId}", id, sourceItemId);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        // GET: ProductGroup/FormularioEdicaoGroupExchangeRule/5
        [HttpGet]
        public async Task<IActionResult> FormularioEdicaoGroupExchangeRule(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest("ID da regra é obrigatório");
                }

                var rule = await _productGroupService.GetExchangeRuleByIdAsync(id);

                if (rule == null)
                {
                    return NotFound("Regra não encontrada");
                }

                var viewModel = new EditProductGroupExchangeRuleViewModel
                {
                    Id = rule.Id,
                    ProductGroupId = rule.ProductGroupId,
                    SourceGroupItemId = rule.SourceGroupItemId,
                    SourceGroupItemWeight = rule.SourceGroupItemWeight,
                    TargetGroupItemId = rule.TargetGroupItemId,
                    TargetGroupItemWeight = rule.TargetGroupItemWeight,
                    ExchangeRatio = rule.ExchangeRatio,
                    IsActive = rule.IsActive
                };

                return PartialView("~/Views/ProductGroup/_EditGroupExchangeRule.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de edição da regra: {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        // POST: ProductGroup/SalvarGroupExchangeRule
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarGroupExchangeRule(CreateProductGroupExchangeRuleViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Dados inválidos", errors = ModelState });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
                
                var rule = new ProductGroupExchangeRule
                {
                    ProductGroupId = viewModel.ProductGroupId,
                    SourceGroupItemId = viewModel.SourceGroupItemId,
                    SourceGroupItemWeight = viewModel.SourceGroupItemWeight,
                    TargetGroupItemId = viewModel.TargetGroupItemId,
                    TargetGroupItemWeight = viewModel.TargetGroupItemWeight,
                    ExchangeRatio = viewModel.ExchangeRatio,
                    IsActive = viewModel.IsActive,
                    CreatedBy = userId,
                    CreatedAt = DateTime.UtcNow
                };

                var success = await _productGroupService.AddExchangeRuleAsync(rule);

                if (success)
                {
                    return Json(new { success = true, message = "Regra de troca adicionada com sucesso!" });
                }

                return Json(new { success = false, message = "Erro ao adicionar regra" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar regra de troca do grupo");
                return Json(new { success = false, message = "Erro interno do servidor" });
            }
        }

        // POST: ProductGroup/SalvarEdicaoGroupExchangeRule/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarEdicaoGroupExchangeRule(string id, EditProductGroupExchangeRuleViewModel viewModel)
        {
            try
            {
                if (id != viewModel.Id)
                {
                    return Json(new { success = false, message = "ID inconsistente" });
                }

                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Dados inválidos", errors = ModelState });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
                
                var rule = new ProductGroupExchangeRule
                {
                    Id = viewModel.Id,
                    ProductGroupId = viewModel.ProductGroupId,
                    SourceGroupItemId = viewModel.SourceGroupItemId,
                    SourceGroupItemWeight = viewModel.SourceGroupItemWeight,
                    TargetGroupItemId = viewModel.TargetGroupItemId,
                    TargetGroupItemWeight = viewModel.TargetGroupItemWeight,
                    ExchangeRatio = viewModel.ExchangeRatio,
                    IsActive = viewModel.IsActive,
                    LastModifiedBy = userId,
                    LastModifiedAt = DateTime.UtcNow
                };

                var success = await _productGroupService.UpdateExchangeRuleAsync(rule);

                if (success)
                {
                    return Json(new { success = true, message = "Regra de troca atualizada com sucesso!" });
                }

                return Json(new { success = false, message = "Erro ao atualizar regra" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar regra de troca do grupo: {Id}", id);
                return Json(new { success = false, message = "Erro interno do servidor" });
            }
        }

        // POST: ProductGroup/ExcluirGroupExchangeRule/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirGroupExchangeRule(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return Json(new { success = false, message = "ID da regra é obrigatório" });
                }

                var success = await _productGroupService.RemoveExchangeRuleAsync(id);

                if (success)
                {
                    return Json(new { success = true, message = "Regra de troca removida com sucesso!" });
                }

                return Json(new { success = false, message = "Regra não encontrada" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir regra de troca do grupo: {Id}", id);
                return Json(new { success = false, message = "Erro interno do servidor" });
            }
        }

        #endregion
    }
} 