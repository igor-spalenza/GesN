using Microsoft.AspNetCore.Mvc;
using GesN.Web.Interfaces.Services;
using GesN.Web.Models.Entities.Production;
using Microsoft.AspNetCore.Authorization;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Controllers
{
    [Authorize]
    public class ProductGroupController : Controller
    {
        private readonly IProductGroupService _productGroupService;
        private readonly IProductService _productService;

        public ProductGroupController(
            IProductGroupService productGroupService,
            IProductService productService)
        {
            _productGroupService = productGroupService;
            _productService = productService;
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
            var groupItemProductIds = groupItems.Select(gi => gi.ProductId).ToHashSet();
            
            return allProducts.Where(p => p.Id != groupId && !groupItemProductIds.Contains(p.Id));
        }
    }
} 