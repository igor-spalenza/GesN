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
            
            var viewModel = new ProductComponentIndexViewModel
            {
                Components = components.Select(c => new ProductComponentViewModel
                {
                    Id = c.Id,
                    CompositeProductId = c.CompositeProductId,
                    CompositeProductName = c.CompositeProduct?.Name,
                    ComponentProductId = c.ComponentProductId,
                    ComponentProductName = c.ComponentProduct?.Name,
                    ComponentProductSKU = c.ComponentProduct?.SKU,
                    Quantity = c.Quantity,
                    Unit = c.Unit.ToString(),
                    IsOptional = c.IsOptional,
                    AssemblyOrder = c.AssemblyOrder,
                    Notes = c.Notes,
                    CreatedAt = c.CreatedAt,
                    ModifiedAt = c.LastModifiedAt,
                    TotalCost = c.CalculateTotalCost()
                }).ToList(),
                Statistics = new ProductComponentStatisticsViewModel
                {
                    TotalComponents = components.Count(),
                    OptionalComponents = components.Count(c => c.IsOptional),
                    RequiredComponents = components.Count(c => !c.IsOptional),
                    EstimatedTotalCost = components.Sum(c => c.CalculateTotalCost())
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

            var viewModel = new ProductComponentDetailsViewModel
            {
                Id = component.Id,
                CompositeProductId = component.CompositeProductId,
                CompositeProductName = component.CompositeProduct?.Name,
                ComponentProductId = component.ComponentProductId,
                ComponentProductName = component.ComponentProduct?.Name,
                Quantity = component.Quantity,
                Unit = component.Unit.ToString(),
                IsOptional = component.IsOptional,
                AssemblyOrder = component.AssemblyOrder,
                Notes = component.Notes,
                CreatedAt = component.CreatedAt,
                ModifiedAt = component.LastModifiedAt
            };

            return View(viewModel);
        }

        // GET: ProductComponent/Create
        public async Task<IActionResult> Create(string? compositeProductId = null)
        {
            await PopulateDropdownsAsync();
            
            var viewModel = new CreateProductComponentViewModel();
            if (!string.IsNullOrEmpty(compositeProductId))
            {
                viewModel.CompositeProductId = compositeProductId;
            }
            
            return View(viewModel);
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
                    var component = new ProductComponent
                    {
                        CompositeProductId = viewModel.CompositeProductId,
                        ComponentProductId = viewModel.ComponentProductId,
                        Quantity = viewModel.Quantity,
                        Unit = Enum.Parse<ProductionUnit>(viewModel.Unit),
                        IsOptional = viewModel.IsOptional,
                        AssemblyOrder = viewModel.AssemblyOrder,
                        Notes = viewModel.Notes
                    };

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
            return View(viewModel);
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
            
            var viewModel = new EditProductComponentViewModel
            {
                Id = component.Id,
                CompositeProductId = component.CompositeProductId,
                ComponentProductId = component.ComponentProductId,
                Quantity = component.Quantity,
                Unit = component.Unit.ToString(),
                IsOptional = component.IsOptional,
                AssemblyOrder = component.AssemblyOrder,
                Notes = component.Notes,
                CreatedAt = component.CreatedAt,
                ModifiedAt = component.LastModifiedAt
            };
            
            return View(viewModel);
        }

        // POST: ProductComponent/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditProductComponentViewModel viewModel)
        {
            if (id != viewModel.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var component = new ProductComponent
                    {
                        Id = viewModel.Id,
                        CompositeProductId = viewModel.CompositeProductId,
                        ComponentProductId = viewModel.ComponentProductId,
                        Quantity = viewModel.Quantity,
                        Unit = Enum.Parse<ProductionUnit>(viewModel.Unit),
                        IsOptional = viewModel.IsOptional,
                        AssemblyOrder = viewModel.AssemblyOrder,
                        Notes = viewModel.Notes
                    };

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
            return View(viewModel);
        }

        // GET: ProductComponent/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var component = await _productComponentService.GetByIdAsync(id);
            if (component == null)
                return NotFound();

            var viewModel = new ProductComponentDetailsViewModel
            {
                Id = component.Id,
                CompositeProductId = component.CompositeProductId,
                CompositeProductName = component.CompositeProduct?.Name,
                ComponentProductId = component.ComponentProductId,
                ComponentProductName = component.ComponentProduct?.Name,
                Quantity = component.Quantity,
                Unit = component.Unit.ToString(),
                IsOptional = component.IsOptional,
                AssemblyOrder = component.AssemblyOrder,
                Notes = component.Notes,
                CreatedAt = component.CreatedAt,
                ModifiedAt = component.LastModifiedAt
            };

            return View(viewModel);
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
            
            var viewModel = new ProductComponentIndexViewModel
            {
                Components = components.Select(c => new ProductComponentViewModel
                {
                    Id = c.Id,
                    CompositeProductId = c.CompositeProductId,
                    CompositeProductName = c.CompositeProduct?.Name,
                    ComponentProductId = c.ComponentProductId,
                    ComponentProductName = c.ComponentProduct?.Name,
                    ComponentProductSKU = c.ComponentProduct?.SKU,
                    Quantity = c.Quantity,
                    Unit = c.Unit.ToString(),
                    IsOptional = c.IsOptional,
                    AssemblyOrder = c.AssemblyOrder,
                    Notes = c.Notes,
                    CreatedAt = c.CreatedAt,
                    ModifiedAt = c.LastModifiedAt,
                    TotalCost = c.CalculateTotalCost()
                }).ToList(),
                CompositeProductId = compositeProductId,
                Statistics = new ProductComponentStatisticsViewModel
                {
                    TotalComponents = components.Count(),
                    OptionalComponents = components.Count(c => c.IsOptional),
                    RequiredComponents = components.Count(c => !c.IsOptional),
                    EstimatedTotalCost = components.Sum(c => c.CalculateTotalCost())
                }
            };
            
            return View("Index", viewModel);
        }

        // AJAX: Check if component can be created
        [HttpPost]
        [AllowAnonymous]
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
        [AllowAnonymous]
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
        [AllowAnonymous]
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

        // AJAX: List components for composite product
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> List(string productId)
        {
            try
            {
                if (string.IsNullOrEmpty(productId))
                {
                    var emptyViewModel = new ProductComponentIndexViewModel
                    {
                        Components = new List<ProductComponentViewModel>(),
                        CompositeProductId = productId,
                        Statistics = new ProductComponentStatisticsViewModel()
                    };
                    return PartialView("_List", emptyViewModel);
                }

                var components = await _productComponentService.GetByCompositeProductIdAsync(productId);
                
                var viewModel = new ProductComponentIndexViewModel
                {
                    Components = components.Select(c => new ProductComponentViewModel
                    {
                        Id = c.Id,
                        CompositeProductId = c.CompositeProductId,
                        ComponentProductId = c.ComponentProductId,
                        ComponentProductName = c.ComponentProduct?.Name,
                        ComponentProductSKU = c.ComponentProduct?.SKU,
                        Quantity = c.Quantity,
                        Unit = c.Unit.ToString(),
                        IsOptional = c.IsOptional,
                        AssemblyOrder = c.AssemblyOrder,
                        Notes = c.Notes,
                        CreatedAt = c.CreatedAt,
                        ModifiedAt = c.LastModifiedAt,
                        TotalCost = c.CalculateTotalCost()
                    }).ToList(),
                    CompositeProductId = productId,
                    Statistics = new ProductComponentStatisticsViewModel
                    {
                        TotalComponents = components.Count(),
                        OptionalComponents = components.Count(c => c.IsOptional),
                        RequiredComponents = components.Count(c => !c.IsOptional),
                        EstimatedTotalCost = components.Sum(c => c.CalculateTotalCost())
                    }
                };
                
                return PartialView("_List", viewModel);
            }
            catch (Exception ex)
            {
                // Log error if needed
                var errorViewModel = new ProductComponentIndexViewModel
                {
                    Components = new List<ProductComponentViewModel>(),
                    CompositeProductId = productId,
                    Statistics = new ProductComponentStatisticsViewModel()
                };
                return PartialView("_List", errorViewModel);
            }
        }

        // AJAX: Component form for creation
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> FormularioComponente(string compositeProductId)
        {
            try
            {
                await PopulateDropdownsAsync();
                var viewModel = new CreateProductComponentViewModel
                {
                    CompositeProductId = compositeProductId
                };
                
                return PartialView("_Create", viewModel);
            }
            catch (Exception)
            {
                return BadRequest("Erro ao carregar formulário");
            }
        }

        // AJAX: Component form for editing
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> FormularioEdicaoComponente(string componentId)
        {
            try
            {
                var component = await _productComponentService.GetByIdAsync(componentId);
                if (component == null)
                {
                    return NotFound();
                }

                await PopulateDropdownsAsync();
                
                var viewModel = new EditProductComponentViewModel
                {
                    Id = component.Id,
                    CompositeProductId = component.CompositeProductId,
                    ComponentProductId = component.ComponentProductId,
                    Quantity = component.Quantity,
                    Unit = component.Unit.ToString(),
                    IsOptional = component.IsOptional,
                    AssemblyOrder = component.AssemblyOrder,
                    Notes = component.Notes,
                    CreatedAt = component.CreatedAt,
                    ModifiedAt = component.LastModifiedAt
                };
                
                return PartialView("_Edit", viewModel);
            }
            catch (Exception)
            {
                return BadRequest("Erro ao carregar formulário de edição");
            }
        }

        // AJAX: Save component
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarComponente(CreateProductComponentViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                                           .ToDictionary(
                                               kvp => kvp.Key,
                                               kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                                           );
                    return Json(new { success = false, errors });
                }

                var component = new ProductComponent
                {
                    CompositeProductId = viewModel.CompositeProductId,
                    ComponentProductId = viewModel.ComponentProductId,
                    Quantity = viewModel.Quantity,
                    Unit = Enum.Parse<ProductionUnit>(viewModel.Unit),
                    IsOptional = viewModel.IsOptional,
                    AssemblyOrder = viewModel.AssemblyOrder,
                    Notes = viewModel.Notes
                };

                await _productComponentService.CreateAsync(component);
                return Json(new { success = true, message = "Componente adicionado com sucesso!" });
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

        // AJAX: Save component edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarEdicaoComponente(string componentId, EditProductComponentViewModel viewModel)
        {
            try
            {
                if (componentId != viewModel.Id)
                {
                    return Json(new { success = false, message = "ID inconsistente." });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                                           .ToDictionary(
                                               kvp => kvp.Key,
                                               kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                                           );
                    return Json(new { success = false, errors });
                }

                var component = new ProductComponent
                {
                    Id = viewModel.Id,
                    CompositeProductId = viewModel.CompositeProductId,
                    ComponentProductId = viewModel.ComponentProductId,
                    Quantity = viewModel.Quantity,
                    Unit = Enum.Parse<ProductionUnit>(viewModel.Unit),
                    IsOptional = viewModel.IsOptional,
                    AssemblyOrder = viewModel.AssemblyOrder,
                    Notes = viewModel.Notes
                };

                var success = await _productComponentService.UpdateAsync(component);
                if (success)
                {
                    return Json(new { success = true, message = "Componente atualizado com sucesso!" });
                }
                else
                {
                    return Json(new { success = false, message = "Componente não encontrado." });
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

        // AJAX: Delete component
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirComponente(string componentId)
        {
            try
            {
                var component = await _productComponentService.GetByIdAsync(componentId);
                if (component == null)
                {
                    return Json(new { success = false, message = "Componente não encontrado." });
                }

                var success = await _productComponentService.DeleteAsync(componentId);
                if (success)
                {
                    return Json(new { 
                        success = true, 
                        message = "Componente removido com sucesso!",
                        productId = component.CompositeProductId
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Erro ao remover componente." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro interno: " + ex.Message });
            }
        }

        // AJAX: Search components
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Search(string term)
        {
            try
            {
                var components = await _productComponentService.SearchAsync(term);
                var viewModels = components.Select(c => new ProductComponentViewModel
                {
                    Id = c.Id,
                    CompositeProductId = c.CompositeProductId,
                    CompositeProductName = c.CompositeProduct?.Name,
                    ComponentProductId = c.ComponentProductId,
                    ComponentProductName = c.ComponentProduct?.Name,
                    ComponentProductSKU = c.ComponentProduct?.SKU,
                    Quantity = c.Quantity,
                    Unit = c.Unit.ToString(),
                    IsOptional = c.IsOptional,
                    AssemblyOrder = c.AssemblyOrder,
                    Notes = c.Notes,
                    CreatedAt = c.CreatedAt,
                    ModifiedAt = c.LastModifiedAt,
                    TotalCost = c.CalculateTotalCost()
                }).ToList();
                
                return PartialView("_ComponentList", viewModels);
            }
            catch (Exception)
            {
                return PartialView("_ComponentList", new List<ProductComponentViewModel>());
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