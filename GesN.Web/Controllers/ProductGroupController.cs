using Microsoft.AspNetCore.Mvc;
using GesN.Web.Interfaces.Services;
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
        private readonly ILogger<ProductGroupController> _logger;

        public ProductGroupController(
            IProductGroupService productGroupService,
            IProductService productService,
            IProductCategoryService productCategoryService,
            ILogger<ProductGroupController> logger)
        {
            _productGroupService = productGroupService;
            _productService = productService;
            _productCategoryService = productCategoryService;
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

            // Carregar itens, opções e regras de troca
            var groupItems = await _productGroupService.GetGroupItemsAsync(id);
            var groupOptions = await _productGroupService.GetGroupOptionsAsync(id);
            var exchangeRules = await _productGroupService.GetExchangeRulesAsync(id);

            ViewBag.GroupItems = groupItems;
            ViewBag.GroupOptions = groupOptions;
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

        // GET: ProductGroup/ManageOptions/5
        public async Task<IActionResult> ManageOptions(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var group = await _productService.GetByIdAsync(id) as ProductGroup;
            if (group == null)
                return NotFound();

            var groupOptions = await _productGroupService.GetGroupOptionsAsync(id);
            ViewBag.Group = group;

            return View(groupOptions);
        }

        // POST: ProductGroup/AddOption
        [HttpPost]
        public async Task<IActionResult> AddOption(ProductGroupOption option)
        {
            try
            {
                await _productGroupService.AddGroupOptionAsync(option);
                return Json(new { success = true, message = "Opção adicionada ao grupo com sucesso!" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Erro interno do servidor." });
            }
        }

        // POST: ProductGroup/UpdateOption
        [HttpPost]
        public async Task<IActionResult> UpdateOption(ProductGroupOption option)
        {
            try
            {
                var success = await _productGroupService.UpdateGroupOptionAsync(option);
                if (success)
                {
                    return Json(new { success = true, message = "Opção atualizada com sucesso!" });
                }
                else
                {
                    return Json(new { success = false, message = "Opção não encontrada." });
                }
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Erro interno do servidor." });
            }
        }

        // POST: ProductGroup/RemoveOption
        [HttpPost]
        public async Task<IActionResult> RemoveOption(string id)
        {
            try
            {
                var success = await _productGroupService.RemoveGroupOptionAsync(id);
                if (success)
                {
                    return Json(new { success = true, message = "Opção removida do grupo com sucesso!" });
                }
                else
                {
                    return Json(new { success = false, message = "Opção não encontrada." });
                }
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Erro interno do servidor." });
            }
        }

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
                    var product = await _productService.GetByIdAsync(item.ProductId);
                    viewModels.Add(new ProductGroupItemViewModel
                    {
                        Id = item.Id,
                        ProductGroupId = item.ProductGroupId,
                        ProductId = item.ProductId,
                        ProductName = product?.Name ?? "Produto não encontrado",
                        Quantity = item.Quantity,
                        MinQuantity = item.MinQuantity,
                        MaxQuantity = item.MaxQuantity,
                        DefaultQuantity = item.DefaultQuantity,
                        IsOptional = item.IsOptional,
                        ExtraPrice = item.ExtraPrice,
                        CreatedAt = item.CreatedAt,
                        ModifiedAt = item.LastModifiedAt
                    });
                }

                ViewBag.ProductId = productId;
                return PartialView("_ProductGroupItems", viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar itens do grupo: {ProductId}", productId);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        // GET: ProductGroup/FormularioGroupItem/5
        [HttpGet]
        public IActionResult FormularioGroupItem(string productId)
        {
            var viewModel = new CreateProductGroupItemViewModel
            {
                ProductGroupId = productId
            };
            return PartialView("_CreateGroupItem", viewModel);
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

                var product = await _productService.GetByIdAsync(item.ProductId);
                var productGroup = await _productService.GetByIdAsync(item.ProductGroupId);

                var viewModel = new EditProductGroupItemViewModel
                {
                    Id = item.Id,
                    ProductGroupId = item.ProductGroupId,
                    ProductGroupName = productGroup?.Name ?? "Grupo não encontrado",
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    MinQuantity = item.MinQuantity,
                    MaxQuantity = item.MaxQuantity,
                    DefaultQuantity = item.DefaultQuantity,
                    IsOptional = item.IsOptional,
                    ExtraPrice = item.ExtraPrice
                };

                return PartialView("_EditGroupItem", viewModel);
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
                    return Json(new { success = false, message = "Dados inválidos", errors = ModelState });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
                
                var item = new ProductGroupItem
                {
                    ProductGroupId = viewModel.ProductGroupId,
                    ProductId = viewModel.ProductId,
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
                    return Json(new { success = false, message = "Dados inválidos", errors = ModelState });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
                
                var item = new ProductGroupItem
                {
                    Id = viewModel.Id,
                    ProductGroupId = viewModel.ProductGroupId,
                    ProductId = viewModel.ProductId,
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

        #region ProductGroup Options Management

        // GET: ProductGroup/ProductGroupOptions/5
        [HttpGet]
        public async Task<IActionResult> ProductGroupOptions(string productId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productId))
                {
                    return BadRequest("ID do produto é obrigatório");
                }

                var groupOptions = await _productGroupService.GetGroupOptionsAsync(productId);
                var viewModels = groupOptions.Select(option => new ProductGroupOptionViewModel
                {
                    Id = option.Id,
                    ProductGroupId = option.ProductGroupId,
                    Name = option.Name,
                    Description = option.Description,
                    OptionType = option.OptionType.ToString(),
                    IsRequired = option.IsRequired,
                    DisplayOrder = option.DisplayOrder,
                    CreatedAt = option.CreatedAt,
                    ModifiedAt = option.LastModifiedAt
                }).ToList();

                ViewBag.ProductId = productId;
                return PartialView("_ProductGroupOptions", viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar opções do grupo: {ProductId}", productId);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        // GET: ProductGroup/FormularioGroupOption/5
        [HttpGet]
        public IActionResult FormularioGroupOption(string productId)
        {
            var viewModel = new CreateProductGroupOptionViewModel
            {
                ProductGroupId = productId
            };
            return PartialView("_CreateGroupOption", viewModel);
        }

        // GET: ProductGroup/FormularioEdicaoGroupOption/5
        [HttpGet]
        public async Task<IActionResult> FormularioEdicaoGroupOption(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest("ID da opção é obrigatório");
                }

                var option = await _productGroupService.GetGroupOptionByIdAsync(id);

                if (option == null)
                {
                    return NotFound("Opção não encontrada");
                }

                var viewModel = new EditProductGroupOptionViewModel
                {
                    Id = option.Id,
                    ProductGroupId = option.ProductGroupId,
                    Name = option.Name,
                    Description = option.Description,
                    OptionType = option.OptionType.ToString(),
                    IsRequired = option.IsRequired,
                    DisplayOrder = option.DisplayOrder
                };

                return PartialView("_EditGroupOption", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de edição da opção: {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        // POST: ProductGroup/SalvarGroupOption
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarGroupOption(CreateProductGroupOptionViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Dados inválidos", errors = ModelState });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
                
                var option = new ProductGroupOption
                {
                    ProductGroupId = viewModel.ProductGroupId,
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    OptionType = Enum.Parse<ProductGroupOptionType>(viewModel.OptionType ?? "Single"),
                    IsRequired = viewModel.IsRequired,
                    DisplayOrder = viewModel.DisplayOrder ?? 1,
                    CreatedBy = userId,
                    CreatedAt = DateTime.UtcNow
                };

                var success = await _productGroupService.AddGroupOptionAsync(option);

                if (success)
                {
                    return Json(new { success = true, message = "Opção adicionada com sucesso!" });
                }

                return Json(new { success = false, message = "Erro ao adicionar opção" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar opção do grupo");
                return Json(new { success = false, message = "Erro interno do servidor" });
            }
        }

        // POST: ProductGroup/SalvarEdicaoGroupOption/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarEdicaoGroupOption(string id, EditProductGroupOptionViewModel viewModel)
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
                
                var option = new ProductGroupOption
                {
                    Id = viewModel.Id,
                    ProductGroupId = viewModel.ProductGroupId,
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    OptionType = Enum.Parse<ProductGroupOptionType>(viewModel.OptionType ?? "Single"),
                    IsRequired = viewModel.IsRequired,
                    DisplayOrder = viewModel.DisplayOrder ?? 1,
                    LastModifiedBy = userId,
                    LastModifiedAt = DateTime.UtcNow
                };

                var success = await _productGroupService.UpdateGroupOptionAsync(option);

                if (success)
                {
                    return Json(new { success = true, message = "Opção atualizada com sucesso!" });
                }

                return Json(new { success = false, message = "Erro ao atualizar opção" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar opção do grupo: {Id}", id);
                return Json(new { success = false, message = "Erro interno do servidor" });
            }
        }

        // POST: ProductGroup/ExcluirGroupOption/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirGroupOption(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return Json(new { success = false, message = "ID da opção é obrigatório" });
                }

                var success = await _productGroupService.RemoveGroupOptionAsync(id);

                if (success)
                {
                    return Json(new { success = true, message = "Opção removida com sucesso!" });
                }

                return Json(new { success = false, message = "Opção não encontrada" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir opção do grupo: {Id}", id);
                return Json(new { success = false, message = "Erro interno do servidor" });
            }
        }

        #endregion

        #region ProductGroup Exchange Rules Management

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
                    var originalProduct = await _productService.GetByIdAsync(rule.OriginalProductId);
                    var exchangeProduct = await _productService.GetByIdAsync(rule.ExchangeProductId);
                    
                    viewModels.Add(new ProductGroupExchangeRuleViewModel
                    {
                        Id = rule.Id,
                        ProductGroupId = rule.ProductGroupId,
                        OriginalProductId = rule.OriginalProductId,
                        OriginalProductName = originalProduct?.Name ?? "Produto não encontrado",
                        ExchangeProductId = rule.ExchangeProductId,
                        ExchangeProductName = exchangeProduct?.Name ?? "Produto não encontrado",
                        ExchangeRatio = rule.ExchangeRatio,
                        AdditionalCost = rule.AdditionalCost,
                        IsActive = rule.IsActive,
                        CreatedAt = rule.CreatedAt,
                        ModifiedAt = rule.LastModifiedAt
                    });
                }

                ViewBag.ProductId = productId;
                return PartialView("_ProductGroupExchangeRules", viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar regras de troca do grupo: {ProductId}", productId);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        // GET: ProductGroup/FormularioGroupExchangeRule/5
        [HttpGet]
        public IActionResult FormularioGroupExchangeRule(string productId)
        {
            var viewModel = new CreateProductGroupExchangeRuleViewModel
            {
                ProductGroupId = productId
            };
            return PartialView("_CreateGroupExchangeRule", viewModel);
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
                    OriginalProductId = rule.OriginalProductId,
                    ExchangeProductId = rule.ExchangeProductId,
                    ExchangeRatio = rule.ExchangeRatio,
                    AdditionalCost = rule.AdditionalCost,
                    IsActive = rule.IsActive
                };

                return PartialView("_EditGroupExchangeRule", viewModel);
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
                    OriginalProductId = viewModel.OriginalProductId,
                    ExchangeProductId = viewModel.ExchangeProductId,
                    ExchangeRatio = viewModel.ExchangeRatio,
                    AdditionalCost = viewModel.AdditionalCost ?? 0m,
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
                    OriginalProductId = viewModel.OriginalProductId,
                    ExchangeProductId = viewModel.ExchangeProductId,
                    ExchangeRatio = viewModel.ExchangeRatio,
                    AdditionalCost = viewModel.AdditionalCost ?? 0m,
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