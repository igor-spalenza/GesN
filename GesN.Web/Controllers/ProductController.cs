using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GesN.Web.Interfaces.Services;
using GesN.Web.Models.ViewModels.Production;
using GesN.Web.Models.Entities.Production;
using System.Security.Claims;

namespace GesN.Web.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductCategoryService _productCategoryService;
        private readonly ISupplierService _supplierService;
        private readonly IIngredientService _ingredientService;
        private readonly IProductIngredientService _productIngredientService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(
            IProductCategoryService productCategoryService,
            ISupplierService supplierService,
            IIngredientService ingredientService,
            IProductIngredientService productIngredientService,
            ILogger<ProductController> logger)
        {
            _productCategoryService = productCategoryService;
            _supplierService = supplierService;
            _ingredientService = ingredientService;
            _productIngredientService = productIngredientService;
            _logger = logger;
        }

        // GET: Product
        public async Task<IActionResult> Index()
        {
            try
            {
                // TODO: Implementar repositório para Product quando estiver pronto
                var viewModel = new ProductIndexViewModel
                {
                    Products = new List<Product>(),
                    Statistics = new ProductStatisticsViewModel
                    {
                        TotalProducts = 0,
                        ActiveProducts = 0,
                        InactiveProducts = 0,
                        SimpleProducts = 0,
                        CompositeProducts = 0,
                        ProductGroups = 0,
                        LowStockProducts = 0,
                        LastUpdate = DateTime.Now
                    }
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar página de produtos");
                TempData["ErrorMessage"] = "Erro ao carregar os produtos. Tente novamente.";
                return View(new ProductIndexViewModel());
            }
        }

        // GET: Product/SelecionarTipo
        [HttpGet]
        public IActionResult SelecionarTipo()
        {
            var viewModel = new ProductTypeSelectionViewModel();
            return PartialView("_SelectType", viewModel);
        }

        // GET: Product/FormularioCriacao
        [HttpGet]
        public IActionResult FormularioCriacao(ProductType productType = ProductType.SimpleProduct)
        {
            // Por enquanto, só SimpleProduct está implementado
            if (productType != ProductType.SimpleProduct)
            {
                return BadRequest("Tipo de produto não implementado ainda");
            }

            var viewModel = new CreateSimpleProductViewModel();
            return PartialView("_Create", viewModel);
        }

        // GET: Product/FormularioEdicao/5
        [HttpGet]
        public async Task<IActionResult> FormularioEdicao(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest("ID do produto é obrigatório");
                }

                // TODO: Implementar busca do produto quando repositório estiver pronto
                // Por enquanto, retornar erro
                return NotFound("Funcionalidade em desenvolvimento");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de edição do produto: {ProductId}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        // GET: Product/DetalhesProduct/5
        [HttpGet]
        public async Task<IActionResult> DetalhesProduct(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest("ID do produto é obrigatório");
                }

                // TODO: Implementar busca do produto quando repositório estiver pronto
                return NotFound("Funcionalidade em desenvolvimento");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar detalhes do produto: {ProductId}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        // POST: Product/SalvarNovo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarNovo(CreateSimpleProductViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Dados inválidos", errors = ModelState });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
                
                // TODO: Implementar criação do produto quando repositório estiver pronto
                // Por enquanto, simular sucesso
                var productId = Guid.NewGuid().ToString();

                return Json(new { 
                    success = true, 
                    message = "Produto criado com sucesso!", 
                    productId = productId,
                    redirectUrl = Url.Action("FormularioEdicao", new { id = productId })
                });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar produto");
                return Json(new { success = false, message = "Erro interno do servidor" });
            }
        }

        // POST: Product/SalvarEdicaoProduct/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarEdicaoProduct(string id, EditSimpleProductViewModel viewModel)
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
                
                // TODO: Implementar atualização do produto quando repositório estiver pronto
                return Json(new { success = true, message = "Produto atualizado com sucesso!" });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar produto: {ProductId}", id);
                return Json(new { success = false, message = "Erro interno do servidor" });
            }
        }

        // POST: Product/ExcluirProduct/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirProduct(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return Json(new { success = false, message = "ID do produto é obrigatório" });
                }

                // TODO: Implementar exclusão do produto quando repositório estiver pronto
                return Json(new { success = true, message = "Produto excluído com sucesso!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir produto: {ProductId}", id);
                return Json(new { success = false, message = "Erro interno do servidor" });
            }
        }

        // GET: Product/ListaProduct
        [HttpGet]
        public async Task<IActionResult> ListaProduct(string? searchTerm = null, bool showInactive = false, ProductType? productType = null, string? categoryId = null, string? supplierId = null, bool showLowStock = false)
        {
            try
            {
                // TODO: Implementar listagem quando repositório estiver pronto
                var products = new List<Product>();
                return PartialView("_ListaProduct", products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar produtos");
                return PartialView("_ListaProduct", new List<Product>());
            }
        }

        // GET: Product/BuscarProduct
        [HttpGet]
        public async Task<IActionResult> BuscarProduct(string termo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(termo))
                {
                    return Json(new List<object>());
                }

                // TODO: Implementar busca quando repositório estiver pronto
                return Json(new List<object>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar produtos: {Termo}", termo);
                return Json(new List<object>());
            }
        }

        // GET: Product/BuscaProductAutocomplete
        [HttpGet]
        public async Task<IActionResult> BuscaProductAutocomplete(string termo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(termo))
                {
                    return Json(new List<object>());
                }

                // TODO: Implementar autocomplete quando repositório estiver pronto
                return Json(new List<object>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar produtos para autocomplete: {Termo}", termo);
                return Json(new List<object>());
            }
        }

        // GET: Product/_Grid
        [HttpGet]
        public async Task<IActionResult> _Grid(string? searchTerm = null, bool showInactive = false, ProductType? productType = null, string? categoryId = null, string? supplierId = null, bool showLowStock = false, int page = 1, int pageSize = 10)
        {
            try
            {
                // TODO: Implementar grid quando repositório estiver pronto
                var products = new List<Product>();
                return PartialView("_Grid", products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar grid de produtos");
                return PartialView("_Grid", new List<Product>());
            }
        }

        #region ProductIngredients Management

        // GET: Product/Ingredientes/5
        [HttpGet]
        public async Task<IActionResult> Ingredientes(string productId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productId))
                {
                    return BadRequest("ID do produto é obrigatório");
                }

                var productIngredients = await _productIngredientService.GetProductIngredientsByProductIdAsync(productId);
                var viewModels = productIngredients.ToViewModels();

                return PartialView("_ProductIngredients", viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar ingredientes do produto: {ProductId}", productId);
                return PartialView("_ProductIngredients", new List<ProductIngredientViewModel>());
            }
        }

        // GET: Product/FormularioIngrediente
        [HttpGet]
        public IActionResult FormularioIngrediente(string productId)
        {
            var viewModel = new ProductIngredientViewModel
            {
                ProductId = productId
            };
            return PartialView("_CreateIngredient", viewModel);
        }

        // GET: Product/FormularioEdicaoIngrediente/5
        [HttpGet]
        public async Task<IActionResult> FormularioEdicaoIngrediente(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest("ID do relacionamento é obrigatório");
                }

                var productIngredient = await _productIngredientService.GetProductIngredientByIdAsync(id);
                if (productIngredient == null)
                {
                    return NotFound("Relacionamento não encontrado");
                }

                var viewModel = productIngredient.ToViewModel();
                return PartialView("_EditIngredient", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de edição do ingrediente: {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        // POST: Product/SalvarIngrediente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarIngrediente(ProductIngredientViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Dados inválidos", errors = ModelState });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
                var productIngredient = viewModel.ToEntity();
                var relationId = await _productIngredientService.CreateProductIngredientAsync(productIngredient, userId);

                return Json(new { 
                    success = true, 
                    message = "Ingrediente adicionado com sucesso!",
                    relationId = relationId
                });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar ingrediente ao produto");
                return Json(new { success = false, message = "Erro interno do servidor" });
            }
        }

        // POST: Product/SalvarEdicaoIngrediente/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarEdicaoIngrediente(string id, ProductIngredientViewModel viewModel)
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
                var productIngredient = viewModel.ToEntity();
                var success = await _productIngredientService.UpdateProductIngredientAsync(productIngredient, userId);

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
                _logger.LogError(ex, "Erro ao atualizar ingrediente: {Id}", id);
                return Json(new { success = false, message = "Erro interno do servidor" });
            }
        }

        // POST: Product/ExcluirIngrediente/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirIngrediente(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return Json(new { success = false, message = "ID do relacionamento é obrigatório" });
                }

                var success = await _productIngredientService.DeleteProductIngredientAsync(id);

                if (success)
                {
                    return Json(new { success = true, message = "Ingrediente removido com sucesso!" });
                }

                return Json(new { success = false, message = "Relacionamento não encontrado" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover ingrediente: {Id}", id);
                return Json(new { success = false, message = "Erro interno do servidor" });
            }
        }

        // GET: Product/CalcularCustoIngredientes/5
        [HttpGet]
        public async Task<IActionResult> CalcularCustoIngredientes(string productId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productId))
                {
                    return Json(new { success = false, message = "ID do produto é obrigatório" });
                }

                var totalCost = await _productIngredientService.CalculateProductIngredientCostAsync(productId);
                
                return Json(new { 
                    success = true, 
                    totalCost = totalCost,
                    formattedCost = totalCost.ToString("C2")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao calcular custo dos ingredientes: {ProductId}", productId);
                return Json(new { success = false, message = "Erro interno do servidor" });
            }
        }

        // GET: Product/VerificarEstoque/5
        [HttpGet]
        public async Task<IActionResult> VerificarEstoque(string productId, int quantity = 1)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productId))
                {
                    return Json(new { success = false, message = "ID do produto é obrigatório" });
                }

                var hasSufficientStock = await _productIngredientService.HasSufficientStockForProductAsync(productId, quantity);
                
                return Json(new { 
                    success = true, 
                    hasSufficientStock = hasSufficientStock,
                    message = hasSufficientStock ? "Estoque suficiente" : "Estoque insuficiente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar estoque do produto: {ProductId}", productId);
                return Json(new { success = false, message = "Erro interno do servidor" });
            }
        }

        #endregion
    }
} 