using System.ComponentModel.DataAnnotations;
using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Models.ViewModels.Production
{
    // ViewModels para Listagem e Index
    public class SupplierIndexViewModel
    {
        public IEnumerable<Supplier> Suppliers { get; set; } = new List<Supplier>();
        public SupplierStatisticsViewModel Statistics { get; set; } = new();
        public SupplierSearchViewModel Search { get; set; } = new();
    }

    public class SupplierStatisticsViewModel
    {
        public int TotalSuppliers { get; set; }
        public int ActiveSuppliers { get; set; }
        public int InactiveSuppliers { get; set; }
        public DateTime LastUpdate { get; set; }
    }

    public class SupplierSearchViewModel
    {
        public string SearchTerm { get; set; } = string.Empty;
        public bool ShowInactive { get; set; } = false;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    // ViewModels para CRUD
    public class CreateSupplierViewModel
    {
        [Required(ErrorMessage = "O nome do fornecedor é obrigatório")]
        [Display(Name = "Nome")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Razão Social")]
        [StringLength(150, ErrorMessage = "A razão social deve ter no máximo 150 caracteres")]
        public string? CompanyName { get; set; }

        [Display(Name = "Documento")]
        [StringLength(20, ErrorMessage = "O documento deve ter no máximo 20 caracteres")]
        public string? DocumentNumber { get; set; }

        [Display(Name = "Tipo de Documento")]
        public DocumentType? DocumentType { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "O email deve ter no máximo 100 caracteres")]
        public string? Email { get; set; }

        [Display(Name = "Telefone")]
        [StringLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres")]
        public string? Phone { get; set; }

        [Display(Name = "Endereço")]
        public string? AddressId { get; set; }

        [Display(Name = "Ativo")]
        public bool IsActive { get; set; } = true;
    }

    public class EditSupplierViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome do fornecedor é obrigatório")]
        [Display(Name = "Nome")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Razão Social")]
        [StringLength(150, ErrorMessage = "A razão social deve ter no máximo 150 caracteres")]
        public string? CompanyName { get; set; }

        [Display(Name = "Documento")]
        [StringLength(20, ErrorMessage = "O documento deve ter no máximo 20 caracteres")]
        public string? DocumentNumber { get; set; }

        [Display(Name = "Tipo de Documento")]
        public DocumentType? DocumentType { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "O email deve ter no máximo 100 caracteres")]
        public string? Email { get; set; }

        [Display(Name = "Telefone")]
        [StringLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres")]
        public string? Phone { get; set; }

        [Display(Name = "Endereço")]
        public string? AddressId { get; set; }

        [Display(Name = "Ativo")]
        public bool IsActive { get; set; }

        // Metadados
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? LastModifiedAt { get; set; }
        public string? LastModifiedBy { get; set; }
    }

    public class SupplierDetailsViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? CompanyName { get; set; }
        public string? DocumentNumber { get; set; }
        public DocumentType? DocumentType { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? AddressId { get; set; }
        public bool IsActive { get; set; }

        // Metadados
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? LastModifiedAt { get; set; }
        public string? LastModifiedBy { get; set; }

        // Estatísticas
        public int IngredientCount { get; set; }
    }

    // ViewModel para Autocomplete
    public class SupplierAutocompleteViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? CompanyName { get; set; }
        public string? DocumentNumber { get; set; }
    }
}

// Extension Methods para conversão
namespace GesN.Web.Models.ViewModels.Production
{
    public static class SupplierMappingExtensions
    {
        // Entity → ViewModel
        public static SupplierDetailsViewModel ToDetailsViewModel(this Supplier supplier)
        {
            return new SupplierDetailsViewModel
            {
                Id = supplier.Id,
                Name = supplier.Name,
                CompanyName = supplier.CompanyName,
                DocumentNumber = supplier.DocumentNumber,
                DocumentType = supplier.DocumentType,
                Email = supplier.Email,
                Phone = supplier.Phone,
                AddressId = supplier.AddressId,
                IsActive = supplier.IsActive,
                CreatedAt = supplier.CreatedAt,
                CreatedBy = supplier.CreatedBy,
                LastModifiedAt = supplier.LastModifiedAt,
                LastModifiedBy = supplier.LastModifiedBy,
                IngredientCount = 0 // TODO: Implementar contagem quando necessário
            };
        }

        public static EditSupplierViewModel ToEditViewModel(this Supplier supplier)
        {
            return new EditSupplierViewModel
            {
                Id = supplier.Id,
                Name = supplier.Name,
                CompanyName = supplier.CompanyName,
                DocumentNumber = supplier.DocumentNumber,
                DocumentType = supplier.DocumentType,
                Email = supplier.Email,
                Phone = supplier.Phone,
                AddressId = supplier.AddressId,
                IsActive = supplier.IsActive,
                CreatedAt = supplier.CreatedAt,
                CreatedBy = supplier.CreatedBy,
                LastModifiedAt = supplier.LastModifiedAt,
                LastModifiedBy = supplier.LastModifiedBy
            };
        }

        public static SupplierAutocompleteViewModel ToAutocompleteViewModel(this Supplier supplier)
        {
            return new SupplierAutocompleteViewModel
            {
                Id = supplier.Id,
                Name = supplier.Name,
                CompanyName = supplier.CompanyName,
                DocumentNumber = supplier.DocumentNumber
            };
        }

        // ViewModel → Entity
        public static Supplier ToEntity(this CreateSupplierViewModel viewModel)
        {
            var supplier = new Supplier
            {
                Name = viewModel.Name,
                CompanyName = viewModel.CompanyName,
                DocumentNumber = viewModel.DocumentNumber,
                DocumentType = viewModel.DocumentType,
                Email = viewModel.Email,
                Phone = viewModel.Phone,
                AddressId = viewModel.AddressId
            };

            if (viewModel.IsActive)
                supplier.Activate();
            else
                supplier.Deactivate();

            return supplier;
        }

        public static Supplier ToEntity(this EditSupplierViewModel viewModel)
        {
            var supplier = new Supplier
            {
                Id = viewModel.Id,
                Name = viewModel.Name,
                CompanyName = viewModel.CompanyName,
                DocumentNumber = viewModel.DocumentNumber,
                DocumentType = viewModel.DocumentType,
                Email = viewModel.Email,
                Phone = viewModel.Phone,
                AddressId = viewModel.AddressId
            };

            if (viewModel.IsActive)
                supplier.Activate();
            else
                supplier.Deactivate();

            return supplier;
        }

        // Collections
        public static IEnumerable<SupplierDetailsViewModel> ToDetailsViewModels(this IEnumerable<Supplier> suppliers)
        {
            return suppliers.Select(s => s.ToDetailsViewModel());
        }

        public static IEnumerable<SupplierAutocompleteViewModel> ToAutocompleteViewModels(this IEnumerable<Supplier> suppliers)
        {
            return suppliers.Select(s => s.ToAutocompleteViewModel());
        }
    }
} 