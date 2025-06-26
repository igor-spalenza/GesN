using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GesN.Web.Interfaces.Services;
using GesN.Web.Models.ViewModels.Production;
using System.Security.Claims;

namespace GesN.Web.Controllers
{
    [Authorize]
    public class SupplierController : Controller
    {
        private readonly ISupplierService _supplierService;
        private readonly ILogger<SupplierController> _logger;

        public SupplierController(
            ISupplierService supplierService,
            ILogger<SupplierController> logger)
        {
            _supplierService = supplierService;
            _logger = logger;
        }

        // GET: Supplier
        public async Task<IActionResult> Index()
        {
            try
            {
                var suppliers = await _supplierService.GetAllSuppliersAsync();
                var activeSuppliers = await _supplierService.GetActiveSuppliersAsync();

                var viewModel = new SupplierIndexViewModel
                {
                    Suppliers = suppliers,
                    Statistics = new SupplierStatisticsViewModel
                    {
                        TotalSuppliers = suppliers.Count(),
                        ActiveSuppliers = activeSuppliers.Count(),
                        InactiveSuppliers = suppliers.Count() - activeSuppliers.Count(),
                        LastUpdate = DateTime.Now
                    }
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar página de fornecedores");
                TempData["ErrorMessage"] = "Erro ao carregar os fornecedores. Tente novamente.";
                return View(new SupplierIndexViewModel());
            }
        }

        // GET: Supplier/CreatePartial
        [HttpGet]
        public IActionResult CreatePartial()
        {
            var viewModel = new CreateSupplierViewModel();
            return PartialView("_Create", viewModel);
        }

        // GET: Supplier/EditPartial/5
        [HttpGet]
        public async Task<IActionResult> EditPartial(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest("ID do fornecedor é obrigatório");
                }

                var supplier = await _supplierService.GetSupplierByIdAsync(id);
                if (supplier == null)
                {
                    return NotFound("Fornecedor não encontrado");
                }

                var viewModel = supplier.ToEditViewModel();
                return PartialView("_Edit", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de edição do fornecedor: {SupplierId}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        // GET: Supplier/DetailsPartial/5
        [HttpGet]
        public async Task<IActionResult> DetailsPartial(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest("ID do fornecedor é obrigatório");
                }

                var supplier = await _supplierService.GetSupplierByIdAsync(id);
                if (supplier == null)
                {
                    return NotFound("Fornecedor não encontrado");
                }

                var viewModel = supplier.ToDetailsViewModel();
                return PartialView("_Details", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar detalhes do fornecedor: {SupplierId}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        // POST: Supplier/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSupplierViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Dados inválidos", errors = ModelState });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
                var supplier = viewModel.ToEntity();
                var supplierId = await _supplierService.CreateSupplierAsync(supplier, userId);

                return Json(new { 
                    success = true, 
                    message = "Fornecedor criado com sucesso!", 
                    id = supplierId,
                    numberSequence = supplier.Name
                });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar fornecedor");
                return Json(new { success = false, message = "Erro interno do servidor" });
            }
        }

        // POST: Supplier/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditSupplierViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Dados inválidos", errors = ModelState });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
                var supplier = viewModel.ToEntity();
                var success = await _supplierService.UpdateSupplierAsync(supplier, userId);

                if (success)
                {
                    return Json(new { success = true, message = "Fornecedor atualizado com sucesso!" });
                }

                return Json(new { success = false, message = "Erro ao atualizar fornecedor" });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar fornecedor: {Id}", viewModel.Id);
                return Json(new { success = false, message = "Erro interno do servidor" });
            }
        }

        // POST: Supplier/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return Json(new { success = false, message = "ID do fornecedor é obrigatório" });
                }

                var success = await _supplierService.DeleteSupplierAsync(id);

                if (success)
                {
                    return Json(new { success = true, message = "Fornecedor excluído com sucesso!" });
                }

                return Json(new { success = false, message = "Fornecedor não encontrado" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir fornecedor: {SupplierId}", id);
                return Json(new { success = false, message = "Erro interno do servidor" });
            }
        }

        // GET: Supplier/Grid
        [HttpGet]
        public async Task<IActionResult> Grid()
        {
            try
            {
                var suppliers = await _supplierService.GetAllSuppliersAsync();
                var viewModels = suppliers.ToDetailsViewModels();
                return PartialView("_Grid", viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar grid de fornecedores");
                return PartialView("_Grid", new List<SupplierDetailsViewModel>());
            }
        }

        // GET: Supplier/ListaSupplier
        [HttpGet]
        public async Task<IActionResult> ListaSupplier(string? searchTerm = null, bool showInactive = false)
        {
            try
            {
                IEnumerable<GesN.Web.Models.Entities.Production.Supplier> suppliers;

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    suppliers = await _supplierService.SearchSuppliersAsync(searchTerm);
                }
                else
                {
                    suppliers = showInactive 
                        ? await _supplierService.GetAllSuppliersAsync()
                        : await _supplierService.GetActiveSuppliersAsync();
                }

                return PartialView("_ListaSupplier", suppliers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar fornecedores");
                return PartialView("_ListaSupplier", new List<GesN.Web.Models.Entities.Production.Supplier>());
            }
        }

        // GET: Supplier/BuscarSupplier
        [HttpGet]
        public async Task<IActionResult> BuscarSupplier(string termo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(termo))
                {
                    return Json(new List<object>());
                }

                var suppliers = await _supplierService.SearchSuppliersAsync(termo);
                var result = suppliers.Select(s => new
                {
                    id = s.Id,
                    name = s.Name,
                    companyName = s.CompanyName,
                    documentNumber = s.DocumentNumber,
                    email = s.Email,
                    phone = s.Phone,
                    isActive = s.IsActive
                });

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar fornecedores: {Termo}", termo);
                return Json(new List<object>());
            }
        }

        // GET: Supplier/BuscaSupplierAutocomplete
        [HttpGet]
        public async Task<IActionResult> BuscaSupplierAutocomplete(string termo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(termo))
                {
                    return Json(new List<object>());
                }

                var suppliers = await _supplierService.SearchSuppliersForAutocompleteAsync(termo);
                var result = suppliers.ToAutocompleteViewModels().Select(s => new
                {
                    id = s.Id,
                    text = s.Name,
                    companyName = s.CompanyName,
                    documentNumber = s.DocumentNumber
                });

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar fornecedores para autocomplete: {Termo}", termo);
                return Json(new List<object>());
            }
        }

        // GET: Supplier/_Grid
        [HttpGet]
        public async Task<IActionResult> _Grid(string? searchTerm = null, bool showInactive = false, int page = 1, int pageSize = 10)
        {
            try
            {
                IEnumerable<GesN.Web.Models.Entities.Production.Supplier> suppliers;

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    suppliers = await _supplierService.SearchSuppliersAsync(searchTerm);
                }
                else
                {
                    suppliers = showInactive 
                        ? await _supplierService.GetAllSuppliersAsync()
                        : await _supplierService.GetActiveSuppliersAsync();
                }

                // Aplicar paginação se necessário
                var pagedSuppliers = suppliers.Skip((page - 1) * pageSize).Take(pageSize);

                return PartialView("_Grid", pagedSuppliers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar grid de fornecedores");
                return PartialView("_Grid", new List<GesN.Web.Models.Entities.Production.Supplier>());
            }
        }
    }
} 