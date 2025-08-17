using Microsoft.AspNetCore.Mvc;
using GesN.Web.Interfaces.Services;
using GesN.Web.Models.Entities.Production;
using Microsoft.AspNetCore.Authorization;
using GesN.Web.Models.Enumerators;
using GesN.Web.Models.ViewModels.Production;

namespace GesN.Web.Controllers
{
    [Authorize]
    public class ProductComponentController : Controller
    {
        private readonly IProductComponentService _productComponentService;
        private readonly IProductComponentHierarchyService _productComponentHierarchyService;
        private readonly IProductService _productService;
        private readonly ILogger<ProductComponentController> _logger;

        public ProductComponentController(
            IProductComponentService productComponentService,
            IProductComponentHierarchyService productComponentHierarchyService,
            IProductService productService,
            ILogger<ProductComponentController> logger)
        {
            _productComponentService = productComponentService;
            _productComponentHierarchyService = productComponentHierarchyService;
            _productService = productService;
            _logger = logger;
        }

        // GET: ProductComponent
        public async Task<IActionResult> Index()
        {
            var components = await _productComponentService.GetAllAsync();
            
            var viewModel = new ProductComponentIndexViewModel
            {
                Components = components.Select(c => c.ToViewModel()).ToList(),
                Statistics = new ProductComponentStatisticsViewModel
                {
                    TotalComponents = components.Count(),
                    ActiveComponents = components.Count(c => c.StateCode == ObjectState.Active),
                    InactiveComponents = components.Count(c => c.StateCode == ObjectState.Inactive),
                    TotalAdditionalCosts = components.Sum(c => c.AdditionalCost),
                    AverageAdditionalCost = components.Any() ? components.Average(c => c.AdditionalCost) : 0
                }
            };
            
            return View(viewModel);
        }

        // GET: ProductComponent/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var component = await _productComponentService.GetByIdAsync(id);
            if (component == null)
                return NotFound();

            var viewModel = component.ToDetailsViewModel();
            return PartialView("_Details", viewModel);
        }

        // GET: ProductComponent/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new CreateProductComponentViewModel();
            return PartialView("_CreateComponent", viewModel);
        }

        // POST: ProductComponent/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductComponentViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var component = viewModel.ToEntity();
                    await _productComponentService.CreateAsync(component);
                    
                    return Json(new { success = true, message = "Componente criado com sucesso!" });
                }
                catch (InvalidOperationException ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao criar componente");
                    return Json(new { success = false, message = "Erro interno do servidor." });
                }
            }

            // Se chegou aqui, há erros de validação
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            return Json(new { success = false, errors = errors });
        }

        // GET: ProductComponent/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var component = await _productComponentService.GetByIdAsync(id);
            if (component == null)
                return NotFound();

            var viewModel = component.ToEditViewModel();
            return PartialView("_EditComponent", viewModel);
        }

        // POST: ProductComponent/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditProductComponentViewModel viewModel)
        {
            if (id != viewModel.Id)
                return Json(new { success = false, message = "ID inconsistente." });

            if (ModelState.IsValid)
            {
                try
                {
                    var existingComponent = await _productComponentService.GetByIdAsync(id);
                    if (existingComponent == null)
                        return Json(new { success = false, message = "Componente não encontrado." });

                    var updatedComponent = viewModel.UpdateEntity(existingComponent);
                    var success = await _productComponentService.UpdateAsync(updatedComponent);
                    
                    if (success)
                    {
                        return Json(new { 
                            success = true, 
                            message = "Componente atualizado com sucesso!",
                            componentName = updatedComponent.Name
                        });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Falha ao atualizar componente." });
                    }
                }
                catch (InvalidOperationException ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao atualizar componente {ComponentId}", id);
                    return Json(new { success = false, message = "Erro interno do servidor." });
                }
            }

            // Se chegou aqui, há erros de validação
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            return Json(new { success = false, errors = errors });
        }

        // POST: ProductComponent/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var success = await _productComponentService.DeleteAsync(id);
                if (success)
                {
                    return Json(new { success = true, message = "Componente excluído com sucesso!" });
                }
                else
                {
                    return Json(new { success = false, message = "Componente não encontrado." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir componente {ComponentId}", id);
                return Json(new { success = false, message = "Erro ao excluir componente." });
            }
        }

        // GET: ProductComponent/Grid
        public async Task<IActionResult> Grid()
        {
            var components = await _productComponentService.GetAllAsync();
            var viewModels = components.Select(c => c.ToViewModel()).ToList();
            
            return PartialView("_Grid", viewModels);
        }

        // AJAX: Search components
        [HttpGet]
        public async Task<IActionResult> Search(string term)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term))
                {
                    return Json(new List<object>());
                }

                var components = await _productComponentService.SearchAsync(term);
                var result = components.Select(c => new {
                    id = c.Id,
                    name = c.Name,
                    description = c.Description,
                    hierarchyName = c.ProductComponentHierarchy?.Name
                }).Take(10);

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar componentes com termo: {Term}", term);
                return Json(new List<object>());
            }
        }









        /// <summary>
        /// Endpoint para autocomplete de ProductComponentHierarchy
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> BuscarHierarchyAutocomplete(string termo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(termo) || termo.Length < 2)
                    return Json(new List<object>());

                // Buscar hierarquias que correspondem ao termo
                var hierarchies = await _productComponentHierarchyService.SearchAsync(termo);
                
                var result = hierarchies
                    .Where(h => h.StateCode == ObjectState.Active)
                    .Take(10) // Limitar a 10 resultados
                    .Select(h => new ProductComponentHierarchyAutocompleteViewModel
                    {
                        Id = h.Id,
                        Name = h.Name,
                        Description = h.Description
                    })
                    .ToList();

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar hierarquias para autocomplete com termo: {Termo}", termo);
                return Json(new List<object>());
            }
        }


    }
} 