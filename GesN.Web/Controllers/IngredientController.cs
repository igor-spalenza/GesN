using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GesN.Web.Interfaces.Services;
using GesN.Web.Models.ViewModels.Production;
using System.Security.Claims;

namespace GesN.Web.Controllers
{
    [Authorize]
    public class IngredientController : Controller
    {
        private readonly IIngredientService _ingredientService;
        private readonly ISupplierService _supplierService;
        private readonly ILogger<IngredientController> _logger;

        public IngredientController(
            IIngredientService ingredientService,
            ISupplierService supplierService,
            ILogger<IngredientController> logger)
        {
            _ingredientService = ingredientService;
            _supplierService = supplierService;
            _logger = logger;
        }

        // GET: Ingredient
        public async Task<IActionResult> Index()
        {
            try
            {
                var ingredients = await _ingredientService.GetAllIngredientsAsync();
                var activeIngredients = await _ingredientService.GetActiveIngredientsAsync();
                var lowStockIngredients = await _ingredientService.GetLowStockIngredientsAsync();
                var perishableIngredients = await _ingredientService.GetPerishableIngredientsAsync();

                var viewModel = new IngredientIndexViewModel
                {
                    Ingredients = ingredients,
                    Statistics = new IngredientStatisticsViewModel
                    {
                        TotalIngredients = ingredients.Count(),
                        ActiveIngredients = activeIngredients.Count(),
                        InactiveIngredients = ingredients.Count() - activeIngredients.Count(),
                        LowStockIngredients = lowStockIngredients.Count(),
                        PerishableIngredients = perishableIngredients.Count(),
                        LastUpdate = DateTime.Now
                    }
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar página de ingredientes");
                TempData["ErrorMessage"] = "Erro ao carregar os ingredientes. Tente novamente.";
                return View(new IngredientIndexViewModel());
            }
        }

        // GET: Ingredient/CreatePartial
        [HttpGet]
        public IActionResult CreatePartial()
        {
            var viewModel = new CreateIngredientViewModel();
            return PartialView("_Create", viewModel);
        }

        // GET: Ingredient/EditPartial/5
        [HttpGet]
        public async Task<IActionResult> EditPartial(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest("ID do ingrediente é obrigatório");
                }

                var ingredient = await _ingredientService.GetIngredientByIdAsync(id);
                if (ingredient == null)
                {
                    return NotFound("Ingrediente não encontrado");
                }

                var viewModel = ingredient.ToEditViewModel();
                return PartialView("_Edit", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de edição do ingrediente: {IngredientId}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        // GET: Ingredient/DetailsPartial/5
        [HttpGet]
        public async Task<IActionResult> DetailsPartial(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest("ID do ingrediente é obrigatório");
                }

                var ingredient = await _ingredientService.GetIngredientByIdAsync(id);
                if (ingredient == null)
                {
                    return NotFound("Ingrediente não encontrado");
                }

                var viewModel = ingredient.ToDetailsViewModel();
                return PartialView("_Details", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar detalhes do ingrediente: {IngredientId}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        // GET: Ingredient/UpdateStockPartial/5
        [HttpGet]
        public async Task<IActionResult> UpdateStockPartial(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest("ID do ingrediente é obrigatório");
                }

                var ingredient = await _ingredientService.GetIngredientByIdAsync(id);
                if (ingredient == null)
                {
                    return NotFound("Ingrediente não encontrado");
                }

                var viewModel = ingredient.ToUpdateStockViewModel();
                return PartialView("_UpdateStock", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de estoque: {IngredientId}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        // POST: Ingredient/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateIngredientViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Dados inválidos", errors = ModelState });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
                var ingredient = viewModel.ToEntity();
                var ingredientId = await _ingredientService.CreateIngredientAsync(ingredient, userId);

                return Json(new { 
                    success = true, 
                    message = "Ingrediente criado com sucesso!", 
                    id = ingredientId,
                    numberSequence = ingredient.Name
                });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar ingrediente");
                return Json(new { success = false, message = "Erro interno do servidor" });
            }
        }

        // POST: Ingredient/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditIngredientViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Dados inválidos", errors = ModelState });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
                var ingredient = viewModel.ToEntity();
                var success = await _ingredientService.UpdateIngredientAsync(ingredient, userId);

                if (success)
                {
                    return Json(new { success = true, message = "Ingrediente atualizado com sucesso!" });
                }

                return Json(new { success = false, message = "Erro ao atualizar ingrediente" });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar ingrediente: {Id}", viewModel.Id);
                return Json(new { success = false, message = "Erro interno do servidor" });
            }
        }

        // POST: Ingredient/UpdateStock
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStock(UpdateIngredientStockViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Dados inválidos", errors = ModelState });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
                var success = await _ingredientService.UpdateIngredientStockAsync(viewModel.Id, viewModel.NewStock, userId);

                if (success)
                {
                    return Json(new { success = true, message = "Estoque atualizado com sucesso!" });
                }

                return Json(new { success = false, message = "Erro ao atualizar estoque" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar estoque do ingrediente: {Id}", viewModel.Id);
                return Json(new { success = false, message = "Erro interno do servidor" });
            }
        }

        // POST: Ingredient/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return Json(new { success = false, message = "ID do ingrediente é obrigatório" });
                }

                var success = await _ingredientService.DeleteIngredientAsync(id);

                if (success)
                {
                    return Json(new { success = true, message = "Ingrediente excluído com sucesso!" });
                }

                return Json(new { success = false, message = "Ingrediente não encontrado" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir ingrediente: {IngredientId}", id);
                return Json(new { success = false, message = "Erro interno do servidor" });
            }
        }

        // GET: Ingredient/Grid
        [HttpGet]
        public async Task<IActionResult> Grid()
        {
            try
            {
                var ingredients = await _ingredientService.GetAllIngredientsAsync();
                var viewModels = ingredients.ToDetailsViewModels();
                return PartialView("_Grid", viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar grid de ingredientes");
                return PartialView("_Grid", new List<IngredientDetailsViewModel>());
            }
        }

        // GET: Ingredient/LowStock
        [HttpGet]
        public async Task<IActionResult> LowStock()
        {
            try
            {
                var lowStockIngredients = await _ingredientService.GetLowStockIngredientsAsync();
                var result = lowStockIngredients.Select(i => new {
                    name = i.Name,
                    currentStock = i.CurrentStock,
                    minStock = i.MinStock
                });
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar ingredientes com estoque baixo");
                return Json(new List<object>());
            }
        }

        // GET: Ingredient/ListaIngredient
        [HttpGet]
        public async Task<IActionResult> ListaIngredient(string? searchTerm = null, bool showInactive = false, bool showLowStock = false, bool showPerishable = false, string? supplierId = null)
        {
            try
            {
                IEnumerable<GesN.Web.Models.Entities.Production.Ingredient> ingredients;

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    ingredients = await _ingredientService.SearchIngredientsAsync(searchTerm);
                }
                else if (showLowStock)
                {
                    ingredients = await _ingredientService.GetLowStockIngredientsAsync();
                }
                else if (showPerishable)
                {
                    ingredients = await _ingredientService.GetPerishableIngredientsAsync();
                }
                else if (!string.IsNullOrWhiteSpace(supplierId))
                {
                    ingredients = await _ingredientService.GetIngredientsBySupplierId(supplierId);
                }
                else
                {
                    ingredients = showInactive 
                        ? await _ingredientService.GetAllIngredientsAsync()
                        : await _ingredientService.GetActiveIngredientsAsync();
                }

                return PartialView("_ListaIngredient", ingredients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar ingredientes");
                return PartialView("_ListaIngredient", new List<GesN.Web.Models.Entities.Production.Ingredient>());
            }
        }

        // GET: Ingredient/BuscarIngredient
        [HttpGet]
        public async Task<IActionResult> BuscarIngredient(string termo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(termo))
                {
                    return Json(new List<object>());
                }

                var ingredients = await _ingredientService.SearchIngredientsAsync(termo);
                var result = ingredients.Select(i => new
                {
                    id = i.Id,
                    name = i.Name,
                    description = i.Description,
                    unit = i.Unit.ToString(),
                    costPerUnit = i.CostPerUnit,
                    currentStock = i.CurrentStock,
                    minStock = i.MinStock,
                    isLowStock = i.CurrentStock <= i.MinStock,
                    supplierName = i.Supplier?.Name,
                    isActive = i.IsActive
                });

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar ingredientes: {Termo}", termo);
                return Json(new List<object>());
            }
        }

        // GET: Ingredient/BuscaIngredientAutocomplete
        [HttpGet]
        public async Task<IActionResult> BuscaIngredientAutocomplete(string termo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(termo))
                {
                    return Json(new List<object>());
                }

                var ingredients = await _ingredientService.SearchIngredientsForAutocompleteAsync(termo);
                var result = ingredients.ToAutocompleteViewModels().Select(i => new
                {
                    id = i.Id,
                    text = i.Name,
                    unit = i.Unit.ToString(),
                    costPerUnit = i.CostPerUnit,
                    currentStock = i.CurrentStock,
                    supplierName = i.SupplierName
                });

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar ingredientes para autocomplete: {Termo}", termo);
                return Json(new List<object>());
            }
        }

        // GET: Ingredient/_Grid
        [HttpGet]
        public async Task<IActionResult> _Grid(string? searchTerm = null, bool showInactive = false, bool showLowStock = false, bool showPerishable = false, string? supplierId = null, int page = 1, int pageSize = 10)
        {
            try
            {
                IEnumerable<GesN.Web.Models.Entities.Production.Ingredient> ingredients;

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    ingredients = await _ingredientService.SearchIngredientsAsync(searchTerm);
                }
                else if (showLowStock)
                {
                    ingredients = await _ingredientService.GetLowStockIngredientsAsync();
                }
                else if (showPerishable)
                {
                    ingredients = await _ingredientService.GetPerishableIngredientsAsync();
                }
                else if (!string.IsNullOrWhiteSpace(supplierId))
                {
                    ingredients = await _ingredientService.GetIngredientsBySupplierId(supplierId);
                }
                else
                {
                    ingredients = showInactive 
                        ? await _ingredientService.GetAllIngredientsAsync()
                        : await _ingredientService.GetActiveIngredientsAsync();
                }

                // Aplicar paginação se necessário
                var pagedIngredients = ingredients.Skip((page - 1) * pageSize).Take(pageSize);

                return PartialView("_Grid", pagedIngredients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar grid de ingredientes");
                return PartialView("_Grid", new List<GesN.Web.Models.Entities.Production.Ingredient>());
            }
        }
    }
} 