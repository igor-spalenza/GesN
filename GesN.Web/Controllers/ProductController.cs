using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GesN.Web.Interfaces.Services;
using GesN.Web.Models.ViewModels.Production;
using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;
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
        private readonly IProductService _productService;
        private readonly IProductComponentService _productComponentService;
        private readonly IProductGroupService _productGroupService;
        private readonly IProductionOrderService _productionOrderService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(
            IProductCategoryService productCategoryService,
            ISupplierService supplierService,
            IIngredientService ingredientService,
            IProductIngredientService productIngredientService,
            IProductService productService,
            IProductComponentService productComponentService,
            IProductGroupService productGroupService,
            IProductionOrderService productionOrderService,
            ILogger<ProductController> logger)
        {
            _productCategoryService = productCategoryService;
            _supplierService = supplierService;
            _ingredientService = ingredientService;
            _productIngredientService = productIngredientService;
            _productService = productService;
            _productComponentService = productComponentService;
            _productGroupService = productGroupService;
            _productionOrderService = productionOrderService;
            _logger = logger;
        }

        // GET: Product
        public async Task<IActionResult> Index()
        {
            try
            {
                var products = await _productService.GetAllAsync();
                var productViewModels = products.Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Code = p.Code,
                    Name = p.Name,
                    Description = p.Description,
                    ProductType = p.ProductType,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category?.Name,
                    Price = p.UnitPrice,
                    Cost = p.Cost,
                    Unit = p.Unit,
                    CurrentStock = null, // Product base não tem estoque
                    MinStock = null, // Product base não tem estoque
                    StateCode = (int)p.StateCode,
                    CreatedAt = p.CreatedAt,
                    ModifiedAt = p.LastModifiedAt
                }).ToList();
                
                var viewModel = new ProductIndexViewModel
                {
                    Products = productViewModels,
                    Statistics = new ProductStatisticsViewModel
                    {
                        TotalProducts = products.Count(),
                        ActiveProducts = products.Count(p => p.StateCode == ObjectState.Active),
                        InactiveProducts = products.Count(p => p.StateCode == ObjectState.Inactive),
                        SimpleProducts = products.Count(p => p.ProductType == ProductType.Simple),
                        CompositeProducts = products.Count(p => p.ProductType == ProductType.Composite),
                        GroupProducts = products.Count(p => p.ProductType == ProductType.Group),
                        ProductGroups = products.Count(p => p.ProductType == ProductType.Group),
                        LowStockProducts = 0, // Será calculado quando implementarmos estoque
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
        public async Task<IActionResult> FormularioCriacao(ProductType productType = ProductType.Simple)
        {
            try
            {
                await PopulateDropdownsAsync();

                var viewModel = new CreateProductViewModel
                {
                    ProductType = productType,
                    AvailableProductTypes = ProductTypeSelectionViewModel.GetProductTypes(),
                    AvailableCategories = new List<CategorySelectionViewModel>()
                };

                return PartialView("_Create", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de criação para tipo: {ProductType}", productType);
                return StatusCode(500, "Erro interno do servidor");
            }
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

                var product = await _productService.GetByIdAsync(id);
                if (product == null)
                {
                    return NotFound("Produto não encontrado");
                }

                await PopulateDropdownsAsync();

                var viewModel = new EditProductViewModel
                {
                    Id = product.Id,
                    Code = product.Code,
                    Name = product.Name,
                    Description = product.Description,
                    ProductType = product.ProductType,
                    CategoryId = product.CategoryId,
                    Price = product.UnitPrice,
                    Cost = product.Cost,
                    Unit = product.Unit,
                    StateCode = (int)product.StateCode,
                    AvailableCategories = new List<CategorySelectionViewModel>()
                };

                return PartialView("_Edit", viewModel);
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

                var product = await _productService.GetByIdAsync(id);
                if (product == null)
                {
                    return NotFound("Produto não encontrado");
                }

                var viewModel = new ProductDetailsViewModel
                {
                    Id = product.Id,
                    Code = product.Code,
                    Name = product.Name,
                    Description = product.Description,
                    ProductType = product.ProductType,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category?.Name,
                    Price = product.UnitPrice,
                    Cost = product.Cost,
                    Unit = product.Unit,
                    StateCode = (int)product.StateCode,
                    CreatedAt = product.CreatedAt,
                    ModifiedAt = product.LastModifiedAt
                };

                return PartialView("_Details", viewModel);
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
        public async Task<IActionResult> SalvarNovo(CreateProductViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Dados inválidos", errors = ModelState });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
                
                // Criar o produto baseado no tipo
                Product product = viewModel.ProductType switch
                {
                    ProductType.Simple => new SimpleProduct
                    {
                        Code = viewModel.Code,
                        Name = viewModel.Name,
                        Description = viewModel.Description,
                        CategoryId = viewModel.CategoryId,
                        UnitPrice = viewModel.Price ?? 0,
                        Cost = viewModel.Cost ?? 0,
                        Unit = viewModel.Unit,
                        StateCode = ObjectState.Active,
                        CreatedBy = userId,
                        CreatedAt = DateTime.UtcNow,
                        LastModifiedBy = userId,
                        LastModifiedAt = DateTime.UtcNow
                    },
                    ProductType.Composite => new CompositeProduct
                    {
                        Code = viewModel.Code,
                        Name = viewModel.Name,
                        Description = viewModel.Description,
                        CategoryId = viewModel.CategoryId,
                        UnitPrice = viewModel.Price ?? 0,
                        Cost = viewModel.Cost ?? 0,
                        Unit = viewModel.Unit,
                        StateCode = ObjectState.Active,
                        CreatedBy = userId,
                        CreatedAt = DateTime.UtcNow,
                        LastModifiedBy = userId,
                        LastModifiedAt = DateTime.UtcNow
                    },
                    ProductType.Group => new ProductGroup
                    {
                        Code = viewModel.Code,
                        Name = viewModel.Name,
                        Description = viewModel.Description,
                        CategoryId = viewModel.CategoryId,
                        UnitPrice = viewModel.Price ?? 0,
                        Cost = viewModel.Cost ?? 0,
                        Unit = viewModel.Unit,
                        StateCode = ObjectState.Active,
                        CreatedBy = userId,
                        CreatedAt = DateTime.UtcNow,
                        LastModifiedBy = userId,
                        LastModifiedAt = DateTime.UtcNow
                    },
                    _ => throw new ArgumentException("Tipo de produto inválido")
                };

                var productId = await _productService.CreateAsync(product);

                return Json(new { 
                    success = true, 
                    message = "Produto criado com sucesso!", 
                    productId = productId,
                    productType = (int)viewModel.ProductType,
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
        public async Task<IActionResult> SalvarEdicaoProduct(string id, EditProductViewModel viewModel)
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
                
                // Buscar o produto existente
                var existingProduct = await _productService.GetByIdAsync(id);
                if (existingProduct == null)
                {
                    return Json(new { success = false, message = "Produto não encontrado" });
                }

                // Atualizar propriedades
                existingProduct.Code = viewModel.Code;
                existingProduct.Name = viewModel.Name;
                existingProduct.Description = viewModel.Description;
                existingProduct.CategoryId = viewModel.CategoryId;
                existingProduct.UnitPrice = viewModel.Price ?? 0;
                existingProduct.Cost = viewModel.Cost ?? 0;
                existingProduct.Unit = viewModel.Unit;
                existingProduct.StateCode = (ObjectState)viewModel.StateCode;
                existingProduct.LastModifiedBy = userId;
                existingProduct.LastModifiedAt = DateTime.UtcNow;

                var success = await _productService.UpdateAsync(existingProduct);
                if (success)
                {
                    return Json(new { success = true, message = "Produto atualizado com sucesso!" });
                }
                else
                {
                    return Json(new { success = false, message = "Erro ao atualizar produto" });
                }
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

                var success = await _productService.DeleteAsync(id);
                if (success)
                {
                    return Json(new { success = true, message = "Produto excluído com sucesso!" });
                }
                else
                {
                    return Json(new { success = false, message = "Erro ao excluir produto ou produto não encontrado" });
                }
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
                var products = await _productService.GetAllAsync();
                
                // Aplicar filtros
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    products = products.Where(p => 
                        p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        p.Code.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        (p.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false));
                }

                if (!showInactive)
                {
                    products = products.Where(p => p.StateCode == ObjectState.Active);
                }

                if (productType.HasValue)
                {
                    products = products.Where(p => p.ProductType == productType.Value);
                }

                if (!string.IsNullOrWhiteSpace(categoryId))
                {
                    products = products.Where(p => p.CategoryId == categoryId);
                }

                return PartialView("_ListaProduct", products.ToList());
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

                var products = await _productService.SearchAsync(termo);
                var result = products.Select(p => new
                {
                    id = p.Id,
                    code = p.Code,
                    name = p.Name,
                    description = p.Description,
                    price = p.UnitPrice,
                    cost = p.Cost,
                    unit = p.Unit,
                    productType = p.ProductType.ToString(),
                    categoryName = p.Category?.Name,
                    isActive = p.StateCode == ObjectState.Active
                }).ToList();

                return Json(result);
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

                var products = await _productService.SearchAsync(termo);
                var result = products.Take(10).Select(p => new
                {
                    id = p.Id,
                    value = p.Name,
                    label = $"{p.Code} - {p.Name}",
                    code = p.Code,
                    price = p.UnitPrice,
                    unit = p.Unit
                }).ToList();

                return Json(result);
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
                var products = await _productService.GetAllAsync();
                
                // Aplicar filtros
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    products = products.Where(p => 
                        p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        p.Code.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        (p.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false));
                }

                if (!showInactive)
                {
                    products = products.Where(p => p.StateCode == ObjectState.Active);
                }

                if (productType.HasValue)
                {
                    products = products.Where(p => p.ProductType == productType.Value);
                }

                if (!string.IsNullOrWhiteSpace(categoryId))
                {
                    products = products.Where(p => p.CategoryId == categoryId);
                }

                // Paginação
                var totalItems = products.Count();
                var pagedProducts = products
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                ViewBag.CurrentPage = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalItems = totalItems;
                ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                return PartialView("_Grid", pagedProducts);
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

        #region Private Methods

        private async Task PopulateDropdownsAsync()
        {
            // TODO: Implementar quando os serviços estiverem completos
            // ViewBag.Categories = await _productCategoryService.GetAllAsync();
            // ViewBag.Suppliers = await _supplierService.GetAllAsync();
            // ViewBag.Ingredients = await _ingredientService.GetAllAsync();
            await Task.CompletedTask;
        }

        private List<SelectListItem> GetProductTypeSelectList(ProductType? selectedType = null)
        {
            return Enum.GetValues<ProductType>()
                .Select(pt => new SelectListItem
                {
                    Value = ((int)pt).ToString(),
                    Text = GetProductTypeDisplayName(pt),
                    Selected = pt == selectedType
                }).ToList();
        }

        private async Task<List<CategorySelectionViewModel>> GetCategoriesSelectListAsync()
        {
            try
            {
                // TODO: Implementar quando IProductCategoryService.GetAllAsync() estiver disponível
                // var categories = await _productCategoryService.GetAllAsync();
                // return categories.Select(c => new CategorySelectionViewModel
                // {
                //     Value = c.Id,
                //     Text = c.Name,
                //     IsSelected = false
                // }).ToList();
                
                await Task.CompletedTask;
                return new List<CategorySelectionViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar categorias");
                return new List<CategorySelectionViewModel>();
            }
        }

        private string GetProductTypeDisplayName(ProductType productType)
        {
            return productType switch
            {
                ProductType.Simple => "Produto Simples",
                ProductType.Composite => "Produto Composto",
                ProductType.Group => "Grupo de Produtos",
                _ => productType.ToString()
            };
        }

        #endregion
    }
} 