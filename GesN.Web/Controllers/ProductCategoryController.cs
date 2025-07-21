using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GesN.Web.Interfaces.Services;
using GesN.Web.Models.ViewModels.Production;
using System.Security.Claims;

namespace GesN.Web.Controllers
{
    [Authorize]
    public class ProductCategoryController : Controller
    {
        private readonly IProductCategoryService _productCategoryService;
        private readonly ILogger<ProductCategoryController> _logger;

        public ProductCategoryController(
            IProductCategoryService productCategoryService,
            ILogger<ProductCategoryController> logger)
        {
            _productCategoryService = productCategoryService;
            _logger = logger;
        }

        // GET: ProductCategory
        public async Task<IActionResult> Index()
        {
            try
            {
                var categories = await _productCategoryService.GetAllCategoriesAsync();
                var activeCategories = await _productCategoryService.GetActiveCategoriesAsync();

                var viewModel = new ProductCategoryIndexViewModel
                {
                    Categories = categories,
                    Statistics = new ProductCategoryStatisticsViewModel
                    {
                        TotalCategories = categories.Count(),
                        ActiveCategories = activeCategories.Count(),
                        InactiveCategories = categories.Count() - activeCategories.Count(),
                        LastUpdate = DateTime.Now
                    }
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar página de categorias");
                TempData["ErrorMessage"] = "Erro ao carregar as categorias. Tente novamente.";
                return View(new ProductCategoryIndexViewModel());
            }
        }

        // GET: ProductCategory/CreatePartial
        [HttpGet]
        public IActionResult CreatePartial()
        {
            return PartialView("_Create", new CreateProductCategoryViewModel());
        }

        // GET: ProductCategory/EditPartial/5
        [HttpGet]
        public async Task<IActionResult> EditPartial(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest("ID da categoria é obrigatório");
                }

                var category = await _productCategoryService.GetCategoryByIdAsync(id);
                if (category == null)
                {
                    return NotFound("Categoria não encontrada");
                }

                var viewModel = category.ToEditViewModel();
                return PartialView("_Edit", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de edição da categoria: {CategoryId}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        // GET: ProductCategory/DetailsPartial/5
        [HttpGet]
        public async Task<IActionResult> DetailsPartial(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest("ID da categoria é obrigatório");
                }

                var category = await _productCategoryService.GetCategoryByIdAsync(id);
                if (category == null)
                {
                    return NotFound("Categoria não encontrada");
                }

                var viewModel = category.ToDetailsViewModel();
                return PartialView("_Details", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar detalhes da categoria: {CategoryId}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        // POST: ProductCategory/SalvarNovo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarNovo(CreateProductCategoryViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Dados inválidos", errors = ModelState });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
                var category = viewModel.ToEntity();
                var categoryId = await _productCategoryService.CreateCategoryAsync(category, userId);

                return Json(new { 
                    success = true, 
                    message = "Categoria criada com sucesso!", 
                    id = categoryId,
                    numberSequence = category.Name
                });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar categoria");
                return Json(new { success = false, message = "Erro interno do servidor" });
            }
        }

        // POST: ProductCategory/SalvarEdicaoCategoria/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarEdicaoCategoria(string id, EditProductCategoryViewModel viewModel)
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
                var category = viewModel.ToEntity();
                var success = await _productCategoryService.UpdateCategoryAsync(category, userId);

                if (success)
                {
                    return Json(new { success = true, message = "Categoria atualizada com sucesso!" });
                }

                return Json(new { success = false, message = "Erro ao atualizar categoria" });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar categoria: {CategoryId}", id);
                return Json(new { success = false, message = "Erro interno do servidor" });
            }
        }

        // POST: ProductCategory/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return Json(new { success = false, message = "ID da categoria é obrigatório" });
                }

                var success = await _productCategoryService.DeleteCategoryAsync(id);

                if (success)
                {
                    return Json(new { success = true, message = "Categoria excluída com sucesso!" });
                }

                return Json(new { success = false, message = "Categoria não encontrada" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir categoria: {CategoryId}", id);
                return Json(new { success = false, message = "Erro interno do servidor" });
            }
        }

        // GET: ProductCategory/Grid
        [HttpGet]
        public async Task<IActionResult> Grid(string? status = null, string? searchTerm = null)
        {
            try
            {
                IEnumerable<GesN.Web.Models.Entities.Production.ProductCategory> categories;

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    categories = await _productCategoryService.SearchCategoriesAsync(searchTerm);
                }
                else if (status == "ativo")
                {
                    categories = await _productCategoryService.GetActiveCategoriesAsync();
                }
                else if (status == "inativo")
                {
                    categories = (await _productCategoryService.GetAllCategoriesAsync()).Where(c => !c.IsActive);
                }
                else
                {
                    categories = await _productCategoryService.GetAllCategoriesAsync();
                }

                var viewModels = categories.Select(c => c.ToDetailsViewModel());
                return PartialView("_Grid", viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar categorias");
                return PartialView("_Grid", new List<ProductCategoryDetailsViewModel>());
            }
        }

        // GET: ProductCategory/ListaProductCategory
        [HttpGet]
        public async Task<IActionResult> ListaProductCategory(string? searchTerm = null, bool showInactive = false)
        {
            try
            {
                IEnumerable<GesN.Web.Models.Entities.Production.ProductCategory> categories;

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    categories = await _productCategoryService.SearchCategoriesAsync(searchTerm);
                }
                else
                {
                    categories = showInactive 
                        ? await _productCategoryService.GetAllCategoriesAsync()
                        : await _productCategoryService.GetActiveCategoriesAsync();
                }

                return PartialView("_ListaProductCategory", categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar categorias");
                return PartialView("_ListaProductCategory", new List<GesN.Web.Models.Entities.Production.ProductCategory>());
            }
        }

        // GET: ProductCategory/BuscarProductCategory
        [HttpGet]
        public async Task<IActionResult> BuscarProductCategory(string termo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(termo))
                {
                    return Json(new List<object>());
                }

                var categories = await _productCategoryService.SearchCategoriesAsync(termo);
                var result = categories.Select(c => new
                {
                    id = c.Id,
                    name = c.Name,
                    description = c.Description,
                    isActive = c.IsActive
                });

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar categorias: {Termo}", termo);
                return Json(new List<object>());
            }
        }

        // GET: ProductCategory/BuscaProductCategoryAutocomplete
        [HttpGet]
        public async Task<IActionResult> BuscaProductCategoryAutocomplete(string termo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(termo))
                {
                    // Retornar algumas categorias ativas quando não há termo de busca
                    var allCategories = await _productCategoryService.GetActiveCategoriesAsync();
                    var allResult = allCategories.Take(10).Select(c => new
                    {
                        id = c.Id,
                        name = c.Name,
                        label = c.Name,
                        value = c.Name,
                        description = c.Description
                    });
                    return Json(allResult);
                }

                var categories = await _productCategoryService.SearchCategoriesForAutocompleteAsync(termo);
                var result = categories.Take(10).Select(c => new
                {
                    id = c.Id,
                    name = c.Name,
                    label = c.Name,
                    value = c.Name,
                    description = c.Description
                });

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar categorias para autocomplete: {Termo}", termo);
                
                // Fallback: retornar dados de teste em caso de erro
                var testData = new[]
                {
                    new { id = "1", name = "Bebidas", label = "Bebidas", value = "Bebidas", description = "Bebidas em geral" },
                    new { id = "2", name = "Alimentos", label = "Alimentos", value = "Alimentos", description = "Produtos alimentícios" },
                    new { id = "3", name = "Sobremesas", label = "Sobremesas", value = "Sobremesas", description = "Doces e sobremesas" }
                };
                
                return Json(testData.Where(t => t.name.Contains(termo ?? "", StringComparison.OrdinalIgnoreCase)));
            }
        }

        // GET: ProductCategory/_Grid
        [HttpGet]
        public async Task<IActionResult> _Grid(string? searchTerm = null, bool showInactive = false, int page = 1, int pageSize = 10)
        {
            try
            {
                IEnumerable<GesN.Web.Models.Entities.Production.ProductCategory> categories;

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    categories = await _productCategoryService.SearchCategoriesAsync(searchTerm);
                }
                else
                {
                    categories = showInactive 
                        ? await _productCategoryService.GetAllCategoriesAsync()
                        : await _productCategoryService.GetActiveCategoriesAsync();
                }

                // Aplicar paginação se necessário
                var pagedCategories = categories.Skip((page - 1) * pageSize).Take(pageSize);

                return PartialView("_Grid", pagedCategories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar grid de categorias");
                return PartialView("_Grid", new List<GesN.Web.Models.Entities.Production.ProductCategory>());
            }
        }
    }
} 