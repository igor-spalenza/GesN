using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using GesN.Web.Interfaces.Services;
using GesN.Web.Models.ViewModels.Production;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace GesN.Web.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de Hierarquias de Componentes de Produto
    /// </summary>
    [Authorize]
    public class ProductComponentHierarchyController : Controller
    {
        private readonly IProductComponentHierarchyService _hierarchyService;
        private readonly IProductComponentService _componentService;
        private readonly ICompositeProductXHierarchyService _compositeProductHierarchyService;
        private readonly IProductService _productService;
        private readonly ILogger<ProductComponentHierarchyController> _logger;

        public ProductComponentHierarchyController(
            IProductComponentHierarchyService hierarchyService,
            IProductComponentService componentService,
            ICompositeProductXHierarchyService compositeProductHierarchyService,
            IProductService productService,
            ILogger<ProductComponentHierarchyController> logger)
        {
            _hierarchyService = hierarchyService;
            _componentService = componentService;
            _compositeProductHierarchyService = compositeProductHierarchyService;
            _productService = productService;
            _logger = logger;
        }

        #region Views Principais

        /// <summary>
        /// Página principal de listagem de hierarquias
        /// </summary>
        public async Task<IActionResult> Index()
        {
            try
            {
                // Consultar todas as hierarquias
                var hierarchies = await _hierarchyService.GetAllAsync();
                
                // Converter para ViewModels
                var hierarchyViewModels = new List<ProductComponentHierarchyViewModel>();
                foreach (var hierarchy in hierarchies)
                {
                    var viewModel = hierarchy.ToViewModel();
                    hierarchyViewModels.Add(viewModel);
                }

                // Inicializar IndexViewModel
                var indexViewModel = new ProductComponentHierarchyIndexViewModel
                {
                    Hierarchies = hierarchyViewModels
                };

                return View(indexViewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro ao carregar hierarquias: {ex.Message}";
                return View(new ProductComponentHierarchyIndexViewModel());
            }
        }

        /// <summary>
        /// Exibir detalhes de uma hierarquia
        /// </summary>
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { success = false, message = "ID da hierarquia não informado." });
            }

            try
            {
                var hierarchy = await _hierarchyService.GetByIdAsync(id);
                if (hierarchy == null)
                {
                    return Json(new { success = false, message = "Hierarquia não encontrada." });
                }

                var viewModel = hierarchy.ToDetailsViewModel();

                // Carregar dados relacionados
                await PopulateDetailsViewModelAsync(viewModel, hierarchy);

                // Verificar permissões do usuário
                var userId = GetCurrentUserId();
                await PopulateActionPermissionsAsync(viewModel, hierarchy, userId);

                return PartialView("_Details", viewModel);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erro ao carregar detalhes da hierarquia: {ex.Message}" });
            }
        }

        #endregion

        #region Grid Assíncrono

        /// <summary>
        /// Carrega grid de hierarquias de forma assíncrona
        /// </summary>
        public async Task<IActionResult> Grid()
        {
            try
            {
                var hierarchies = await _hierarchyService.GetAllAsync();
                var hierarchyViewModels = new List<ProductComponentHierarchyViewModel>();

                foreach (var hierarchy in hierarchies)
                {
                    var viewModel = hierarchy.ToViewModel();
                    hierarchyViewModels.Add(viewModel);
                }

                return PartialView("_Grid", hierarchyViewModels);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erro ao carregar hierarquias: {ex.Message}" });
            }
        }

        #endregion

        #region CRUD Operations

        /// <summary>
        /// Formulário de criação (GET)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Create(string? templateId = null, string? sourceHierarchyId = null)
        {
            try
            {
                var viewModel = new CreateProductComponentHierarchyViewModel();

                // Pre-popular campos se fornecidos
                if (!string.IsNullOrEmpty(templateId))
                {
                    viewModel.TemplateId = templateId;
                }

                if (!string.IsNullOrEmpty(sourceHierarchyId))
                {
                    viewModel.SourceHierarchyId = sourceHierarchyId;
                    
                    // Buscar dados da hierarquia fonte
                    var sourceHierarchy = await _hierarchyService.GetByIdAsync(sourceHierarchyId);
                    if (sourceHierarchy != null)
                    {
                        viewModel.Name = $"Cópia de {sourceHierarchy.Name}";
                        viewModel.Description = sourceHierarchy.Description;
                    }
                }

                // Carregar listas para dropdowns
                //await PopulateCreateViewModelListsAsync(viewModel);

                return PartialView("_Create", viewModel);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erro ao carregar formulário de criação: {ex.Message}" });
            }
        }

        /// <summary>
        /// Criar hierarquia (POST)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductComponentHierarchyViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCreateViewModelListsAsync(viewModel);
                return PartialView("_Create", viewModel);
            }

            try
            {
                string hierarchyId;

                if (!string.IsNullOrEmpty(viewModel.TemplateId))
                {
                    // Criar a partir de template
                    var userId = GetCurrentUserId();
                    await _hierarchyService.CreateFromTemplateAsync(viewModel.TemplateId, viewModel.Name, userId);
                    hierarchyId = ""; // Será retornado pela implementação
                }
                else if (!string.IsNullOrEmpty(viewModel.SourceHierarchyId))
                {
                    // Duplicar hierarquia existente
                    var userId = GetCurrentUserId();
                    hierarchyId = await _hierarchyService.DuplicateHierarchyAsync(
                        viewModel.SourceHierarchyId, viewModel.Name, userId);
                }
                else
                {
                    // Criar nova hierarquia
                    var hierarchy = viewModel.ToEntity();
                    hierarchy.CreatedBy = GetCurrentUserId();
                    hierarchyId = await _hierarchyService.CreateAsync(hierarchy);
                }

                TempData["SuccessMessage"] = "Hierarquia criada com sucesso!";
                return Json(new { success = true, message = "Hierarquia criada com sucesso!", hierarchyId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Erro ao criar hierarquia: {ex.Message}");
                await PopulateCreateViewModelListsAsync(viewModel);
                return PartialView("_Create", viewModel);
            }
        }

        /// <summary>
        /// Formulário de edição (GET)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { success = false, message = "ID da hierarquia não informado." });
            }

            try
            {
                var hierarchy = await _hierarchyService.GetByIdAsync(id);
                if (hierarchy == null)
                {
                    return Json(new { success = false, message = "Hierarquia não encontrada." });
                }

                var viewModel = new EditProductComponentHierarchyViewModel
                {
                    Id = hierarchy.Id,
                    Name = hierarchy.Name,
                    Description = hierarchy.Description ?? "",
                    StateCode = hierarchy.StateCode,
                    Notes = hierarchy.Notes ?? "",
                    CreatedAt = hierarchy.CreatedAt,
                    CreatedBy = hierarchy.CreatedBy ?? "",
                    LastModifiedAt = hierarchy.LastModifiedAt,
                    LastModifiedBy = hierarchy.LastModifiedBy ?? "",
                    OriginalName = hierarchy.Name
                };

                // Verificar permissões de edição
                //var userId = GetCurrentUserId();
                //await PopulateEditPermissionsAsync(viewModel, hierarchy, userId);

                // Carregar dados adicionais
                //await PopulateEditViewModelDataAsync(viewModel, hierarchy);

                return PartialView("_Edit", viewModel);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erro ao carregar hierarquia: {ex.Message}" });
            }
        }

        /// <summary>
        /// Atualizar hierarquia (POST)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProductComponentHierarchyViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_Edit", viewModel);
            }

            try
            {
                var hierarchy = await _hierarchyService.GetByIdAsync(viewModel.Id);
                if (hierarchy == null)
                {
                    ModelState.AddModelError("", "Hierarquia não encontrada.");
                    return PartialView("_Edit", viewModel);
                }

                // Atualizar entidade
                viewModel.UpdateEntity(hierarchy);
                hierarchy.LastModifiedBy = GetCurrentUserId();

                var success = await _hierarchyService.UpdateAsync(hierarchy);
                if (success)
                {
                    TempData["SuccessMessage"] = "Hierarquia atualizada com sucesso!";
                    return Json(new { success = true, message = "Hierarquia atualizada com sucesso!" });
                }
                else
                {
                    ModelState.AddModelError("", "Erro ao atualizar hierarquia.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Erro ao atualizar hierarquia: {ex.Message}");
            }

            return PartialView("_Edit", viewModel);
        }

        /// <summary>
        /// Excluir hierarquia
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { success = false, message = "ID da hierarquia não informado." });
            }

            try
            {
                var hierarchy = await _hierarchyService.GetByIdAsync(id);
                if (hierarchy == null)
                {
                    return Json(new { success = false, message = "Hierarquia não encontrada." });
                }

                if (!await _hierarchyService.CanDeleteAsync(id))
                {
                    return Json(new { success = false, message = "Hierarquia não pode ser excluída - está sendo utilizada por produtos compostos." });
                }

                var success = await _hierarchyService.DeleteAsync(id);

                if (success)
                {
                    TempData["SuccessMessage"] = "Hierarquia excluída com sucesso!";
                    return Json(new { success = true, message = "Hierarquia excluída com sucesso!" });
                }
                else
                {
                    return Json(new { success = false, message = "Erro ao excluir hierarquia." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erro ao excluir hierarquia: {ex.Message}" });
            }
        }

        #endregion

        #region Business Operations

        /// <summary>
        /// Ativar hierarquia
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(string id)
        {
            return await ExecuteStatusChangeAsync(id,
                async (hierarchyId, userId) => await _hierarchyService.ActivateHierarchyAsync(hierarchyId, userId),
                "Hierarquia ativada com sucesso!",
                "Erro ao ativar hierarquia");
        }

        /// <summary>
        /// Desativar hierarquia
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(string id)
        {
            return await ExecuteStatusChangeAsync(id,
                async (hierarchyId, userId) => await _hierarchyService.DeactivateHierarchyAsync(hierarchyId, userId),
                "Hierarquia desativada com sucesso!",
                "Erro ao desativar hierarquia");
        }

        /// <summary>
        /// Atualizar observações
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateNotes(string id, string notes)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { success = false, message = "ID da hierarquia não informado." });
            }

            try
            {
                var userId = GetCurrentUserId();
                var success = await _hierarchyService.UpdateNotesAsync(id, notes, userId);

                if (success)
                {
                    return Json(new { success = true, message = "Observações atualizadas com sucesso!" });
                }
                else
                {
                    return Json(new { success = false, message = "Erro ao atualizar observações." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erro ao atualizar observações: {ex.Message}" });
            }
        }

        /// <summary>
        /// Duplicar hierarquia
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Duplicate(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { success = false, message = "ID da hierarquia não informado." });
            }

            try
            {
                var hierarchy = await _hierarchyService.GetByIdAsync(id);
                if (hierarchy == null)
                {
                    return Json(new { success = false, message = "Hierarquia não encontrada." });
                }

                var viewModel = new DuplicateHierarchyViewModel
                {
                    SourceHierarchyId = hierarchy.Id,
                    SourceHierarchyName = hierarchy.Name,
                    NewName = $"Cópia de {hierarchy.Name}",
                    NewDescription = hierarchy.Description ?? "",
                    SourceDescription = hierarchy.Description ?? "",
                    ComponentCount = await _hierarchyService.GetComponentCountAsync(hierarchy.Id),
                    SourceStateCode = hierarchy.StateCode
                };

                return PartialView("_Duplicate", viewModel);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erro ao carregar formulário de duplicação: {ex.Message}" });
            }
        }

        /// <summary>
        /// Executar duplicação (POST)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Duplicate(DuplicateHierarchyViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_Duplicate", viewModel);
            }

            try
            {
                var userId = GetCurrentUserId();
                var newHierarchyId = await _hierarchyService.DuplicateHierarchyAsync(
                    viewModel.SourceHierarchyId, viewModel.NewName, userId);

                TempData["SuccessMessage"] = "Hierarquia duplicada com sucesso!";
                return Json(new { success = true, message = "Hierarquia duplicada com sucesso!", newHierarchyId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Erro ao duplicar hierarquia: {ex.Message}");
                return PartialView("_Duplicate", viewModel);
            }
        }

        #endregion

        #region API Endpoints

        /// <summary>
        /// Buscar hierarquias para autocomplete
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Search(string term, bool activeOnly = true, int limit = 10)
        {
            try
            {
                var hierarchies = await _hierarchyService.SearchAsync(term);
                
                if (activeOnly)
                {
                    hierarchies = hierarchies.Where(h => h.IsActive());
                }

                var results = hierarchies.Take(limit).Select(h => new
                {
                    id = h.Id,
                    text = h.Name,
                    description = h.Description ?? "",
                    isActive = h.IsActive(),
                    componentCount = _hierarchyService.GetComponentCountAsync(h.Id).Result
                });

                return Json(results);
            }
            catch (Exception)
            {
                return Json(new object[] { });
            }
        }

        /// <summary>
        /// Buscar hierarquias disponíveis para autocomplete na criação de CompositeProductXHierarchy
        /// Retorna apenas hierarquias que não estão associadas ao produto
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> BuscarHierarchiaDisponivel(string termo, string? productId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(termo) || termo.Length < 2)
                    return Json(new List<object>());

                // Buscar hierarquias que correspondem ao termo
                var hierarchies = await _hierarchyService.SearchAsync(termo);
                
                // Filtrar apenas hierarquias ativas
                var activeHierarchies = hierarchies.Where(h => h.IsActive()).ToList();

                // Se um productId foi fornecido, filtrar hierarquias já associadas
                if (!string.IsNullOrWhiteSpace(productId))
                {
                    var associatedHierarchyIds = (await _compositeProductHierarchyService.GetProductHierarchiesAsync(productId))
                        .Select(r => r.ProductComponentHierarchyId)
                        .ToHashSet();
                        
                    activeHierarchies = activeHierarchies
                        .Where(h => !associatedHierarchyIds.Contains(h.Id))
                        .ToList();
                }

                var result = activeHierarchies
                    .Take(10) // Limitar a 10 resultados
                    .Select(h => new
                    {
                        id = h.Id,
                        name = h.Name,
                        description = h.Description ?? "",
                        label = !string.IsNullOrWhiteSpace(h.Description) ? 
                            $"{h.Name} - {h.Description}" : h.Name,
                        value = h.Name
                    })
                    .ToList();

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar hierarquias disponíveis para autocomplete com termo: {Termo}, ProductId: {ProductId}", termo, productId);
                return Json(new List<object>());
            }
        }

        /// <summary>
        /// Obter estatísticas do dashboard
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var allHierarchies = await _hierarchyService.GetAllAsync();
                var activeHierarchies = await _hierarchyService.GetActiveHierarchiesAsync();

                var stats = new
                {
                    total = allHierarchies.Count(),
                    active = activeHierarchies.Count(),
                    inactive = allHierarchies.Count() - activeHierarchies.Count(),
                    unused = (await _hierarchyService.GetUnusedHierarchiesAsync()).Count()
                };

                return Json(stats);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Obter hierarquias para seleção
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetForSelection(bool activeOnly = true)
        {
            try
            {
                var hierarchies = activeOnly ? 
                    await _hierarchyService.GetActiveHierarchiesAsync() : 
                    await _hierarchyService.GetAllAsync();

                var results = hierarchies.Select(h => h.ToSelectionViewModel());

                return Json(results);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        #endregion

        #region Bulk Operations

        /// <summary>
        /// Operações em lote
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkOperation(BulkHierarchyOperationViewModel viewModel)
        {
            if (!viewModel.SelectedIds.Any())
            {
                return Json(new { success = false, message = "Nenhuma hierarquia selecionada." });
            }

            try
            {
                var userId = GetCurrentUserId();
                bool success = false;
                string message = "";

                switch (viewModel.Operation.ToLower())
                {
                    case "activate":
                        success = await _hierarchyService.ActivateBatchAsync(viewModel.SelectedIds, userId);
                        message = success ? "Hierarquias ativadas com sucesso!" : "Erro ao ativar hierarquias.";
                        break;

                    case "deactivate":
                        success = await _hierarchyService.DeactivateBatchAsync(viewModel.SelectedIds, userId);
                        message = success ? "Hierarquias desativadas com sucesso!" : "Erro ao desativar hierarquias.";
                        break;

                    case "delete":
                        success = await _hierarchyService.DeleteBatchAsync(viewModel.SelectedIds, userId);
                        message = success ? "Hierarquias excluídas com sucesso!" : "Erro ao excluir hierarquias.";
                        break;

                    default:
                        return Json(new { success = false, message = "Operação não reconhecida." });
                }

                return Json(new { success, message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erro na operação em lote: {ex.Message}" });
            }
        }

        #endregion

        #region Helper Methods

        private async Task<IEnumerable<GesN.Web.Models.Entities.Production.ProductComponentHierarchy>> GetFilteredHierarchiesAsync(
            string? searchTerm, bool? isActiveFilter, int? minUsageCount, bool showUnusedOnly)
        {
            IEnumerable<GesN.Web.Models.Entities.Production.ProductComponentHierarchy> hierarchies;

            if (showUnusedOnly)
            {
                hierarchies = await _hierarchyService.GetUnusedHierarchiesAsync();
            }
            else if (isActiveFilter == true)
            {
                hierarchies = await _hierarchyService.GetActiveHierarchiesAsync();
            }
            else if (!string.IsNullOrEmpty(searchTerm))
            {
                hierarchies = await _hierarchyService.SearchAsync(searchTerm);
            }
            else
            {
                hierarchies = await _hierarchyService.GetAllAsync();
            }

            // Aplicar filtro de status se especificado
            if (isActiveFilter.HasValue && !showUnusedOnly)
            {
                hierarchies = hierarchies.Where(h => h.IsActive() == isActiveFilter.Value);
            }

            // Aplicar filtro de uso mínimo
            if (minUsageCount.HasValue)
            {
                var filteredList = new List<GesN.Web.Models.Entities.Production.ProductComponentHierarchy>();
                foreach (var hierarchy in hierarchies)
                {
                    var usageCount = await _hierarchyService.GetUsageCountAsync(hierarchy.Id);
                    if (usageCount >= minUsageCount.Value)
                    {
                        filteredList.Add(hierarchy);
                    }
                }
                hierarchies = filteredList;
            }

            return hierarchies;
        }

        private IEnumerable<GesN.Web.Models.Entities.Production.ProductComponentHierarchy> ApplySorting(
            IEnumerable<GesN.Web.Models.Entities.Production.ProductComponentHierarchy> hierarchies, 
            string sortBy, string sortDirection)
        {
            var isAscending = sortDirection.ToLower() == "asc";

            return sortBy.ToLower() switch
            {
                "name" => isAscending ?
                    hierarchies.OrderBy(h => h.Name) :
                    hierarchies.OrderByDescending(h => h.Name),
                "isactive" => isAscending ?
                    hierarchies.OrderBy(h => h.IsActive()) :
                    hierarchies.OrderByDescending(h => h.IsActive()),
                "createdat" => isAscending ?
                    hierarchies.OrderBy(h => h.CreatedAt) :
                    hierarchies.OrderByDescending(h => h.CreatedAt),
                _ => isAscending ?
                    hierarchies.OrderBy(h => h.Name) :
                    hierarchies.OrderByDescending(h => h.Name)
            };
        }

        private IEnumerable<GesN.Web.Models.Entities.Production.ProductComponentHierarchy> ApplyPagination(
            IEnumerable<GesN.Web.Models.Entities.Production.ProductComponentHierarchy> hierarchies, 
            int page, int pageSize)
        {
            return hierarchies.Skip((page - 1) * pageSize).Take(pageSize);
        }

        private async Task PopulateViewModelAdditionalDataAsync(ProductComponentHierarchyViewModel viewModel)
        {
            // Carregar contadores
            viewModel.ComponentCount = await _hierarchyService.GetComponentCountAsync(viewModel.Id);
            viewModel.UsageCount = await _hierarchyService.GetUsageCountAsync(viewModel.Id);

            // Verificar permissões
            viewModel.CanActivate = await _hierarchyService.CanActivateAsync(viewModel.Id);
            viewModel.CanDeactivate = await _hierarchyService.CanDeactivateAsync(viewModel.Id);
            viewModel.CanEdit = true; // Sempre pode editar por enquanto
            viewModel.CanDelete = await _hierarchyService.CanDeleteAsync(viewModel.Id);
            viewModel.CanDuplicate = true; // Sempre pode duplicar
        }

        private async Task PopulateDashboardStatsAsync(ProductComponentHierarchyIndexViewModel viewModel)
        {
            var allHierarchies = await _hierarchyService.GetAllAsync();
            var activeHierarchies = await _hierarchyService.GetActiveHierarchiesAsync();
            var unusedHierarchies = await _hierarchyService.GetUnusedHierarchiesAsync();

            viewModel.ActiveCount = activeHierarchies.Count();
            viewModel.InactiveCount = allHierarchies.Count() - activeHierarchies.Count();
            viewModel.UsedCount = allHierarchies.Count() - unusedHierarchies.Count();
            viewModel.UnusedCount = unusedHierarchies.Count();
        }

        private async Task PopulateFilterListsAsync(ProductComponentHierarchyIndexViewModel viewModel)
        {
            viewModel.AvailableStatusFilters = new List<SelectListItem>
            {
                new() { Value = "", Text = "Todos" },
                new() { Value = "true", Text = "Ativas" },
                new() { Value = "false", Text = "Inativas" }
            };
        }

        private async Task PopulateCreateViewModelListsAsync(CreateProductComponentHierarchyViewModel viewModel)
        {
            // Templates (pode ser implementado posteriormente)
            viewModel.AvailableTemplates = new List<SelectListItem>();

            // Hierarquias existentes para duplicação
            var existingHierarchies = await _hierarchyService.GetAllAsync();
            viewModel.AvailableHierarchies = existingHierarchies
                .Select(h => new SelectListItem
                {
                    Value = h.Id,
                    Text = $"{h.Name} ({(h.IsActive() ? "Ativa" : "Inativa")})"
                }).ToList();
        }

        private async Task PopulateDetailsViewModelAsync(ProductComponentHierarchyDetailsViewModel viewModel,
            GesN.Web.Models.Entities.Production.ProductComponentHierarchy hierarchy)
        {
            // Carregar contadores
            viewModel.ComponentCount = await _hierarchyService.GetComponentCountAsync(hierarchy.Id);
            viewModel.UsageCount = await _hierarchyService.GetUsageCountAsync(hierarchy.Id);

            // Carregar componentes relacionados
            viewModel.Components = (await _hierarchyService.GetComponentsAsync(hierarchy.Id)).ToList();

            // Carregar produtos compostos relacionados
            viewModel.CompositeProducts = (await _hierarchyService.GetCompositeProductRelationsAsync(hierarchy.Id)).ToList();

            // Definir descrições
            viewModel.UsageDescription = viewModel.UsageCount > 0 ?
                $"Utilizada em {viewModel.UsageCount} produto(s) composto(s)" :
                "Não está sendo utilizada por nenhum produto";
        }

        private async Task PopulateActionPermissionsAsync(ProductComponentHierarchyDetailsViewModel viewModel,
            GesN.Web.Models.Entities.Production.ProductComponentHierarchy hierarchy, string userId)
        {
            viewModel.CanActivate = await _hierarchyService.CanActivateAsync(hierarchy.Id);
            viewModel.CanDeactivate = await _hierarchyService.CanDeactivateAsync(hierarchy.Id);
            viewModel.CanEdit = true; // Sempre pode editar por enquanto
            viewModel.CanDelete = await _hierarchyService.CanDeleteAsync(hierarchy.Id);
            viewModel.CanDuplicate = true; // Sempre pode duplicar
        }

        private async Task PopulateEditPermissionsAsync(EditProductComponentHierarchyViewModel viewModel,
            GesN.Web.Models.Entities.Production.ProductComponentHierarchy hierarchy, string userId)
        {
            viewModel.CanChangeActiveStatus = !await _hierarchyService.IsUsedByCompositeProductsAsync(hierarchy.Id);
            viewModel.IsReadonly = false; // Por enquanto não há restrições
        }

        private async Task PopulateEditViewModelDataAsync(EditProductComponentHierarchyViewModel viewModel,
            GesN.Web.Models.Entities.Production.ProductComponentHierarchy hierarchy)
        {
            viewModel.ComponentCount = await _hierarchyService.GetComponentCountAsync(hierarchy.Id);
            viewModel.UsageCount = await _hierarchyService.GetUsageCountAsync(hierarchy.Id);
        }

        private async Task<IActionResult> ExecuteStatusChangeAsync(string id,
            Func<string, string, Task<bool>> operation, string successMessage, string errorMessage)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { success = false, message = "ID da hierarquia não informado." });
            }

            try
            {
                var userId = GetCurrentUserId();
                var success = await operation(id, userId);

                if (success)
                {
                    return Json(new { success = true, message = successMessage });
                }
                else
                {
                    return Json(new { success = false, message = errorMessage });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"{errorMessage}: {ex.Message}" });
            }
        }

        #region Integrated Product Management

        /// <summary>
        /// Buscar hierarquias para um produto composto específico (integrado)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ProductHierarchies(string productId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productId))
                {
                    return BadRequest("ID do produto é obrigatório");
                }

                // Buscar hierarquias associadas ao produto
                var productHierarchies = await _hierarchyService.GetByCompositeProductIdAsync(productId);
                var viewModels = new List<ProductComponentHierarchyViewModel>();
                
                foreach (var hierarchy in productHierarchies)
                {
                    var viewModel = hierarchy.ToViewModel();
                    await PopulateViewModelAdditionalDataAsync(viewModel);
                    viewModels.Add(viewModel);
                }

                ViewBag.ProductId = productId;
                return PartialView("~/Views/Product/_CompositeProductXHierarchy.cshtml", viewModels);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Formulário para criar hierarquia no contexto de produto (integrado)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> FormularioHierarquia(string productId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productId))
                {
                    return BadRequest("ID do produto é obrigatório");
                }

                var viewModel = new CreateProductComponentHierarchyViewModel();
                await PopulateCreateViewModelListsAsync(viewModel);
                
                ViewBag.ProductId = productId;
                return PartialView("~/Views/ProductComponentHierarchy/_CreateHierarchy.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Salvar hierarquia no contexto de produto (integrado)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarHierarquia(CreateProductComponentHierarchyViewModel viewModel)
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

                var hierarchy = viewModel.ToEntity();
                hierarchy.CreatedBy = GetCurrentUserId();
                var hierarchyId = await _hierarchyService.CreateAsync(hierarchy);

                return Json(new { success = true, message = "Hierarquia criada com sucesso!", hierarchyId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erro ao criar hierarquia: {ex.Message}" });
            }
        }

        /// <summary>
        /// Formulário para associar hierarquia existente ao produto (integrado)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> FormularioAssociarHierarquia(string productId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productId))
                {
                    return BadRequest("ID do produto é obrigatório");
                }

                // Buscar hierarquias disponíveis (ativas e não associadas ao produto)
                var availableHierarchies = await _hierarchyService.GetAvailableForProductAsync(productId);
                
                var viewModel = new AssignHierarchyToProductViewModel
                {
                    ProductId = productId,
                    AvailableHierarchies = availableHierarchies.Select(h => new SelectListItem
                    {
                        Value = h.Id,
                        Text = h.Name,
                        Selected = false
                    }).ToList()
                };

                return PartialView("~/Views/ProductComponentHierarchy/_AssignHierarchy.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Associar hierarquia ao produto (integrado)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssociarHierarquia(AssignHierarchyToProductViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Dados inválidos" });
                }

                var userId = GetCurrentUserId();
                var success = await _hierarchyService.AssignToCompositeProductAsync(
                    viewModel.HierarchyId, viewModel.ProductId, userId);

                if (success)
                {
                    return Json(new { 
                        success = true, 
                        message = "Hierarquia associada com sucesso!",
                        productId = viewModel.ProductId
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Erro ao associar hierarquia" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erro ao associar hierarquia: {ex.Message}" });
            }
        }

        /// <summary>
        /// Desassociar hierarquia do produto (integrado)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DesassociarHierarquia(string productId, string hierarchyId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productId) || string.IsNullOrWhiteSpace(hierarchyId))
                {
                    return Json(new { success = false, message = "IDs são obrigatórios" });
                }

                var userId = GetCurrentUserId();
                var success = await _hierarchyService.UnassignFromCompositeProductAsync(
                    hierarchyId, productId, userId);

                if (success)
                {
                    return Json(new { 
                        success = true, 
                        message = "Hierarquia desassociada com sucesso!",
                        productId = productId
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Erro ao desassociar hierarquia" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erro ao desassociar hierarquia: {ex.Message}" });
            }
        }

        /// <summary>
        /// Detalhes da hierarquia no contexto de produto (integrado)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> DetalhesHierarquia(string hierarchyId, string productId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hierarchyId))
                {
                    return BadRequest("ID da hierarquia é obrigatório");
                }

                var hierarchy = await _hierarchyService.GetByIdAsync(hierarchyId);
                if (hierarchy == null)
                {
                    return NotFound("Hierarquia não encontrada");
                }

                var viewModel = hierarchy.ToDetailsViewModel();
                await PopulateDetailsViewModelAsync(viewModel, hierarchy);

                // Se estiver no contexto de produto, carregar informações específicas
                if (!string.IsNullOrWhiteSpace(productId))
                {
                    ViewBag.ProductId = productId;
                    var productRelation = await _hierarchyService.GetProductRelationAsync(hierarchyId, productId);
                    if (productRelation != null)
                    {
                        // CompositeProductXHierarchy não possui mais propriedades de auditoria
                        // pois não herda mais de Entity, então não criamos o ProductRelationInfo
                        // ou criamos com valores padrão se necessário
                        viewModel.ProductRelationInfo = null;
                    }
                }

                return PartialView("~/Views/ProductComponentHierarchy/_HierarchyDetails.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Gerenciar componentes de uma hierarquia no contexto de produto (integrado)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GerenciarComponentesHierarquia(string hierarchyId, string productId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hierarchyId))
                {
                    return BadRequest("ID da hierarquia é obrigatório");
                }

                var hierarchy = await _hierarchyService.GetByIdAsync(hierarchyId);
                if (hierarchy == null)
                {
                    return NotFound("Hierarquia não encontrada");
                }

                var components = await _hierarchyService.GetComponentsAsync(hierarchyId);
                var viewModel = new HierarchyComponentManagementViewModel
                {
                    HierarchyId = hierarchyId,
                    HierarchyName = hierarchy.Name,
                    ProductId = productId,
                    Components = components.Select(c => c.ToViewModel()).ToList()
                };

                return PartialView("~/Views/ProductComponentHierarchy/_ManageHierarchyComponents.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        #endregion

        #region CompositeProductXHierarchy Management

        /// <summary>
        /// Obter relações CompositeProductXHierarchy para um produto
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ProductHierarchyRelations(string productId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productId))
                {
                    return BadRequest("ID do produto é obrigatório");
                }

                var relations = await _compositeProductHierarchyService.GetProductHierarchiesAsync(productId);
                
                ViewBag.ProductId = productId;
                return PartialView("~/Views/Product/_CompositeProductXHierarchy.cshtml", relations.ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Formulário para associar hierarquia com configurações específicas
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> FormularioAssociarHierarchiaCompleta(string productId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productId))
                {
                    return BadRequest("ID do produto é obrigatório");
                }

                var viewModel = await _compositeProductHierarchyService.PrepareCreateViewModelAsync(productId);
                
                ViewBag.ProductId = productId;
                return PartialView("~/Views/ProductComponentHierarchy/_CreateCompositeHierarchyRelation.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Salvar associação de hierarquia com configurações
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarAssociacaoHierarquia(CreateCompositeProductXHierarchyViewModel viewModel)
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

                var userId = GetCurrentUserId();
                var relationId = await _compositeProductHierarchyService.CreateRelationAsync(viewModel, userId);

                return Json(new { 
                    success = true, 
                    message = "Hierarquia associada com sucesso!",
                    relationId = relationId,
                    productId = viewModel.ProductId 
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erro ao associar hierarquia: {ex.Message}" });
            }
        }

        /// <summary>
        /// Formulário para editar configurações de uma relação
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> FormularioEditarRelacao(string relationId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(relationId))
                {
                    return BadRequest("ID da relação é obrigatório");
                }

                if (!int.TryParse(relationId, out int relationIdInt))
                {
                    return BadRequest("ID da relação deve ser um número válido");
                }

                var viewModel = await _compositeProductHierarchyService.PrepareEditViewModelAsync(relationIdInt);
                if (viewModel == null)
                {
                    return NotFound("Relação não encontrada");
                }

                return PartialView("~/Views/ProductComponentHierarchy/_EditCompositeHierarchyRelation.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Salvar edição de configurações de relação
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarEdicaoRelacao(EditCompositeProductXHierarchyViewModel viewModel)
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

                var userId = GetCurrentUserId();
                var success = await _compositeProductHierarchyService.UpdateRelationAsync(viewModel, userId);

                if (success)
                {
                    return Json(new { 
                        success = true, 
                        message = "Configurações atualizadas com sucesso!",
                        productId = viewModel.ProductId 
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Erro ao atualizar configurações" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erro ao atualizar configurações: {ex.Message}" });
            }
        }

        /// <summary>
        /// Excluir relação CompositeProductXHierarchy
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirRelacao(string relationId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(relationId))
                {
                    return Json(new { success = false, message = "ID da relação não informado." });
                }

                if (!int.TryParse(relationId, out int relationIdInt))
                {
                    return Json(new { success = false, message = "ID da relação deve ser um número válido." });
                }

                var userId = GetCurrentUserId();
                var success = await _compositeProductHierarchyService.DeleteRelationAsync(relationIdInt, userId);

                if (success)
                {
                    return Json(new { 
                        success = true, 
                        message = "Relação excluída com sucesso!" 
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Erro ao excluir relação" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erro ao excluir relação: {ex.Message}" });
            }
        }

        /// <summary>
        /// Alternar status ativo/inativo de uma relação
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AlternarStatusRelacao(string relationId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(relationId))
                {
                    return Json(new { success = false, message = "ID da relação não informado." });
                }

                var userId = GetCurrentUserId();
                var success = await _compositeProductHierarchyService.ToggleActiveStatusAsync(relationId, userId);

                if (success)
                {
                    return Json(new { 
                        success = true, 
                        message = "Status alterado com sucesso!" 
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Erro ao alterar status" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erro ao alterar status: {ex.Message}" });
            }
        }

        /// <summary>
        /// Reordenar hierarquias de um produto
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReordenarHierarquias(string productId, Dictionary<string, int> newOrders)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productId))
                {
                    return Json(new { success = false, message = "ID do produto não informado." });
                }

                var userId = GetCurrentUserId();
                var success = await _compositeProductHierarchyService.ReorderProductHierarchiesAsync(productId, newOrders, userId);

                if (success)
                {
                    return Json(new { 
                        success = true, 
                        message = "Hierarquias reordenadas com sucesso!" 
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Erro ao reordenar hierarquias" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erro ao reordenar hierarquias: {ex.Message}" });
            }
        }

        /// <summary>
        /// Detalhes de uma relação CompositeProductXHierarchy
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> DetalhesRelacao(string relationId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(relationId))
                {
                    return BadRequest("ID da relação é obrigatório");
                }

                if (!int.TryParse(relationId, out int relationIdInt))
                {
                    return BadRequest("ID da relação deve ser um número válido");
                }

                var viewModel = await _compositeProductHierarchyService.GetRelationByIdAsync(relationIdInt);
                if (viewModel == null)
                {
                    return NotFound("Relação não encontrada");
                }

                return PartialView("~/Views/ProductComponentHierarchy/_CompositeHierarchyRelationDetails.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Formulário para criar/associar CompositeProductXHierarchy
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> FormularioCompositeProductXHierarchy(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest("ID do produto é obrigatório");
                }

                var viewModel = await _compositeProductHierarchyService.PrepareCreateViewModelAsync(id);
                
                // Garantir que o ProductId seja passado para a view
                ViewBag.ProductId = id;
                viewModel.ProductId = id; // Garantir que o ViewModel também tenha o ProductId
                
                return PartialView("~/Views/ProductComponentHierarchy/_CreateCompositeProductXHierarchy.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Salvar nova relação CompositeProductXHierarchy
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarCompositeProductXHierarchy(CreateCompositeProductXHierarchyViewModel viewModel)
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
                    return Json(new CreateCompositeProductXHierarchyResult("Dados inválidos", errors));
                }

                var userId = GetCurrentUserId();
                var relationId = await _compositeProductHierarchyService.CreateRelationAsync(viewModel, userId);

                // ✅ Buscar dados completos da relação criada para retorno estruturado
                var createdRelation = await _compositeProductHierarchyService.GetRelationByIdAsync(relationId);
                if (createdRelation == null)
                {
                    return Json(new CreateCompositeProductXHierarchyResult(false, "Erro: Relação criada mas não foi possível recuperar os dados"));
                }

                // ✅ Contar total de registros para o produto
                var totalCount = await _compositeProductHierarchyService.GetRelationsCountByProductIdAsync(viewModel.ProductId);

                // ✅ Buscar informações adicionais para exibição
                var product = await _productService.GetByIdAsync(viewModel.ProductId);
                var hierarchy = await _hierarchyService.GetByIdAsync(viewModel.ProductComponentHierarchyId);
                
                // ✅ Converter DetailsViewModel para ViewModel (para uso nos templates JavaScript)
                var createdViewModel = new CompositeProductXHierarchyViewModel
                {
                    Id = createdRelation.Id,
                    ProductComponentHierarchyId = createdRelation.ProductComponentHierarchyId,
                    ProductId = createdRelation.ProductId,
                    MinQuantity = createdRelation.MinQuantity,
                    MaxQuantity = createdRelation.MaxQuantity,
                    IsOptional = createdRelation.IsOptional,
                    AssemblyOrder = createdRelation.AssemblyOrder,
                    Notes = createdRelation.Notes,
                    ProductName = product?.Name ?? "",
                    HierarchyName = hierarchy?.Name ?? "",
                    HierarchyDescription = hierarchy?.Description ?? hierarchy?.Name ?? ""
                };

                return Json(new CreateCompositeProductXHierarchyResult(
                    success: true,
                    message: "Hierarquia adicionada com sucesso!",
                    data: createdViewModel,
                    totalCount: totalCount
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar relação CompositeProductXHierarchy");
                return Json(new CreateCompositeProductXHierarchyResult(false, "Erro interno do servidor ao criar relação"));
            }
        }

        /// <summary>
        /// Carregar formulário de edição de relação CompositeProductXHierarchy
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> EditarCompositeProductXHierarchy(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("ID da relação é obrigatório");
                }

                var viewModel = await _compositeProductHierarchyService.PrepareEditViewModelAsync(id);
                if (viewModel == null)
                {
                    return NotFound("Relação não encontrada");
                }

                // Buscar informações adicionais para a view
                var product = await _productService.GetByIdAsync(viewModel.ProductId);
                var hierarchy = await _hierarchyService.GetByIdAsync(viewModel.ProductComponentHierarchyId);
                
                ViewBag.ProductName = product?.Name ?? "Produto não encontrado";
                ViewBag.HierarchyName = hierarchy?.Name ?? "Hierarquia não encontrada";
                
                return PartialView("~/Views/ProductComponentHierarchy/_EditCompositeHierarchyRelation.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de edição de CompositeProductXHierarchy para ID: {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Salvar alterações na relação CompositeProductXHierarchy
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtualizarCompositeProductXHierarchy(EditCompositeProductXHierarchyViewModel viewModel)
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

                var userId = GetCurrentUserId();
                var success = await _compositeProductHierarchyService.UpdateRelationAsync(viewModel, userId);

                if (success)
                {
                    return Json(new { success = true, message = "Relação atualizada com sucesso!" });
                }
                else
                {
                    return Json(new { success = false, message = "Erro ao atualizar relação" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar CompositeProductXHierarchy ID: {Id}", viewModel.Id);
                return Json(new { success = false, message = $"Erro ao atualizar relação: {ex.Message}" });
            }
        }

        #endregion

        /// <summary>
        /// Exibe detalhes de uma relação CompositeProductXHierarchy
        /// </summary>
        /// <param name="id">ID da relação</param>
        /// <returns>Partial view com detalhes</returns>
        [HttpGet]
        public async Task<IActionResult> DetalhesCompositeProductXHierarchy(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("ID da relação é obrigatório");
                }

                var detailsViewModel = await _compositeProductHierarchyService.GetRelationByIdAsync(id);
                if (detailsViewModel == null)
                {
                    return NotFound("Relação não encontrada");
                }
                
                return PartialView("~/Views/ProductComponentHierarchy/_CompositeHierarchyRelationDetails.cshtml", detailsViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar detalhes de CompositeProductXHierarchy para ID: {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Deleta uma relação CompositeProductXHierarchy
        /// </summary>
        /// <param name="id">ID da relação</param>
        /// <returns>JSON com resultado da operação</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletarCompositeProductXHierarchy(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return Json(new { success = false, message = "ID da relação é obrigatório" });
                }

                var userId = GetCurrentUserId();
                var success = await _compositeProductHierarchyService.DeleteRelationAsync(id, userId);

                if (success)
                {
                    return Json(new { success = true, message = "Relação removida com sucesso!" });
                }
                else
                {
                    return Json(new { success = false, message = "Erro ao remover relação" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar CompositeProductXHierarchy ID: {Id}", id);
                return Json(new { success = false, message = $"Erro ao remover relação: {ex.Message}" });
            }
        }

        #endregion

        #region general

        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        }

        #endregion
    }
} 