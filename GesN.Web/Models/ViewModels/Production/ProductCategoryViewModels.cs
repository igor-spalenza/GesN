using System.ComponentModel.DataAnnotations;
using GesN.Web.Models.Entities.Production;

namespace GesN.Web.Models.ViewModels.Production
{
    // ViewModels para Listagem e Index
    public class ProductCategoryIndexViewModel
    {
        public IEnumerable<ProductCategory> Categories { get; set; } = new List<ProductCategory>();
        public ProductCategoryStatisticsViewModel Statistics { get; set; } = new();
        public ProductCategorySearchViewModel Search { get; set; } = new();
    }

    public class ProductCategoryStatisticsViewModel
    {
        public int TotalCategories { get; set; }
        public int ActiveCategories { get; set; }
        public int InactiveCategories { get; set; }
        public DateTime LastUpdate { get; set; }
    }

    public class ProductCategorySearchViewModel
    {
        public string SearchTerm { get; set; } = string.Empty;
        public bool ShowInactive { get; set; } = false;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    // ViewModels para CRUD
    public class CreateProductCategoryViewModel
    {
        [Required(ErrorMessage = "O nome da categoria é obrigatório")]
        [Display(Name = "Nome da Categoria")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Descrição")]
        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
        public string? Description { get; set; }

        [Display(Name = "Ativa")]
        public bool IsActive { get; set; } = true;
    }

    public class EditProductCategoryViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome da categoria é obrigatório")]
        [Display(Name = "Nome da Categoria")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Descrição")]
        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
        public string? Description { get; set; }

        [Display(Name = "Ativa")]
        public bool IsActive { get; set; }

        // Metadados
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? LastModifiedAt { get; set; }
        public string? LastModifiedBy { get; set; }
    }

    public class ProductCategoryDetailsViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }

        // Metadados
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? LastModifiedAt { get; set; }
        public string? LastModifiedBy { get; set; }

        // Estatísticas (se necessário)
        public int ProductCount { get; set; }
    }

    // ViewModel para Autocomplete
    public class ProductCategoryAutocompleteViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}

// Extension Methods para conversão
namespace GesN.Web.Models.ViewModels.Production
{
    public static class ProductCategoryMappingExtensions
    {
        // Entity → ViewModel
        public static ProductCategoryDetailsViewModel ToDetailsViewModel(this ProductCategory category)
        {
            return new ProductCategoryDetailsViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
                CreatedBy = category.CreatedBy,
                LastModifiedAt = category.LastModifiedAt,
                LastModifiedBy = category.LastModifiedBy,
                ProductCount = 0 // TODO: Implementar contagem quando necessário
            };
        }

        public static EditProductCategoryViewModel ToEditViewModel(this ProductCategory category)
        {
            return new EditProductCategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
                CreatedBy = category.CreatedBy,
                LastModifiedAt = category.LastModifiedAt,
                LastModifiedBy = category.LastModifiedBy
            };
        }

        public static ProductCategoryAutocompleteViewModel ToAutocompleteViewModel(this ProductCategory category)
        {
            return new ProductCategoryAutocompleteViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }

        // ViewModel → Entity
        public static ProductCategory ToEntity(this CreateProductCategoryViewModel viewModel)
        {
            var category = new ProductCategory
            {
                Name = viewModel.Name,
                Description = viewModel.Description
            };

            if (viewModel.IsActive)
                category.Activate();
            else
                category.Deactivate();

            return category;
        }

        public static ProductCategory ToEntity(this EditProductCategoryViewModel viewModel)
        {
            var category = new ProductCategory
            {
                Id = viewModel.Id,
                Name = viewModel.Name,
                Description = viewModel.Description
            };

            if (viewModel.IsActive)
                category.Activate();
            else
                category.Deactivate();

            return category;
        }

        // Collections
        public static IEnumerable<ProductCategoryDetailsViewModel> ToDetailsViewModels(this IEnumerable<ProductCategory> categories)
        {
            return categories.Select(c => c.ToDetailsViewModel());
        }

        public static IEnumerable<ProductCategoryAutocompleteViewModel> ToAutocompleteViewModels(this IEnumerable<ProductCategory> categories)
        {
            return categories.Select(c => c.ToAutocompleteViewModel());
        }
    }
} 