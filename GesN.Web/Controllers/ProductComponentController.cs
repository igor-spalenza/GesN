using Microsoft.AspNetCore.Mvc;
using GesN.Web.Interfaces.Services;
using GesN.Web.Models.Entities.Production;
using Microsoft.AspNetCore.Authorization;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Controllers
{
    [Authorize]
    public class ProductComponentController : Controller
    {
        private readonly IProductComponentService _productComponentService;
        private readonly IProductService _productService;

        public ProductComponentController(
            IProductComponentService productComponentService,
            IProductService productService)
        {
            _productComponentService = productComponentService;
            _productService = productService;
        }

        // GET: ProductComponent
        public async Task<IActionResult> Index()
        {
            var components = await _productComponentService.GetAllAsync();
            return View(components);
        }

        // GET: ProductComponent/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var component = await _productComponentService.GetByIdAsync(id);
            if (component == null)
                return NotFound();

            return View(component);
        }

        // GET: ProductComponent/Create
        public async Task<IActionResult> Create(string? compositeProductId = null)
        {
            await PopulateDropdownsAsync();
            
            var component = new ProductComponent();
            if (!string.IsNullOrEmpty(compositeProductId))
            {
                component.CompositeProductId = compositeProductId;
            }
            
            return View(component);
        }

        // POST: ProductComponent/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductComponent component)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _productComponentService.CreateAsync(component);
                    TempData["SuccessMessage"] = "Componente criado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Erro interno do servidor.");
                }
            }

            await PopulateDropdownsAsync();
            return View(component);
        }

        // GET: ProductComponent/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var component = await _productComponentService.GetByIdAsync(id);
            if (component == null)
                return NotFound();

            await PopulateDropdownsAsync();
            return View(component);
        }

        // POST: ProductComponent/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ProductComponent component)
        {
            if (id != component.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var success = await _productComponentService.UpdateAsync(component);
                    if (success)
                    {
                        TempData["SuccessMessage"] = "Componente atualizado com sucesso!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Componente não encontrado.");
                    }
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Erro interno do servidor.");
                }
            }

            await PopulateDropdownsAsync();
            return View(component);
        }

        // GET: ProductComponent/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var component = await _productComponentService.GetByIdAsync(id);
            if (component == null)
                return NotFound();

            return View(component);
        }

        // POST: ProductComponent/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var success = await _productComponentService.DeleteAsync(id);
                if (success)
                {
                    TempData["SuccessMessage"] = "Componente excluído com sucesso!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Componente não encontrado.";
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Erro ao excluir componente.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: ProductComponent/ByComposite/5
        public async Task<IActionResult> ByComposite(string compositeProductId)
        {
            if (string.IsNullOrEmpty(compositeProductId))
                return BadRequest();

            var components = await _productComponentService.GetByCompositeProductIdAsync(compositeProductId);
            ViewBag.CompositeProductId = compositeProductId;
            
            return View("Index", components);
        }

        // AJAX: Check if component can be created
        [HttpPost]
        public async Task<IActionResult> CanCreateComponent(string compositeProductId, string componentProductId)
        {
            try
            {
                var canCreate = await _productComponentService.CanCreateComponentAsync(compositeProductId, componentProductId);
                return Json(new { success = true, canCreate });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // AJAX: Calculate component cost
        [HttpGet]
        public async Task<IActionResult> CalculateComponentCost(string compositeProductId)
        {
            try
            {
                var cost = await _productComponentService.CalculateComponentCostAsync(compositeProductId);
                return Json(new { success = true, cost });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // AJAX: Calculate assembly time
        [HttpGet]
        public async Task<IActionResult> CalculateAssemblyTime(string compositeProductId)
        {
            try
            {
                var time = await _productComponentService.CalculateAssemblyTimeAsync(compositeProductId);
                return Json(new { success = true, time });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // AJAX: Search components
        [HttpGet]
        public async Task<IActionResult> Search(string term)
        {
            try
            {
                var components = await _productComponentService.SearchAsync(term);
                return PartialView("_ComponentList", components);
            }
            catch (Exception)
            {
                return PartialView("_ComponentList", new List<ProductComponent>());
            }
        }

        private async Task PopulateDropdownsAsync()
        {
            // Buscar produtos compostos (tipo Composite)
            var compositeProducts = await _productService.GetByTypeAsync(ProductType.Composite);
            ViewBag.CompositeProducts = compositeProducts.Select(p => new { p.Id, p.Name });

            // Buscar todos os produtos para componentes
            var allProducts = await _productService.GetAllAsync();
            ViewBag.ComponentProducts = allProducts.Select(p => new { p.Id, p.Name });
        }
    }
} 