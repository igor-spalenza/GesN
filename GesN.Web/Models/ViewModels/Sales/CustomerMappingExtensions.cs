using GesN.Web.Models.Entities.Sales;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Models.ViewModels.Sales
{
    public static class CustomerMappingExtensions
    {
        // Entity -> ViewModel
        public static CustomerViewModel ToViewModel(this Customer customer)
        {
            return new CustomerViewModel
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email ?? string.Empty,
                DocumentNumber = customer.DocumentNumber ?? string.Empty,
                DocumentType = customer.DocumentType ?? DocumentType.CPF,
                Phone = customer.Phone,
                StateCode = customer.StateCode,
                CreatedAt = customer.CreatedAt,
                LastModifiedAt = customer.LastModifiedAt
            };
        }

        public static CustomerDetailsViewModel ToDetailsViewModel(this Customer customer)
        {
            return new CustomerDetailsViewModel
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email ?? string.Empty,
                DocumentNumber = customer.DocumentNumber ?? string.Empty,
                DocumentType = customer.DocumentType ?? DocumentType.CPF,
                Phone = customer.Phone,
                StateCode = customer.StateCode,
                CreatedAt = customer.CreatedAt,
                LastModifiedAt = customer.LastModifiedAt
            };
        }

        public static EditCustomerViewModel ToEditViewModel(this Customer customer)
        {
            var viewModel = new EditCustomerViewModel
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email ?? string.Empty,
                DocumentNumber = customer.DocumentNumber ?? string.Empty,
                DocumentType = customer.DocumentType ?? DocumentType.CPF,
                Phone = customer.Phone,
                StateCode = customer.StateCode,
                CreatedAt = customer.CreatedAt,
                LastModifiedAt = customer.LastModifiedAt
            };

            // Configurar tipos de documento disponíveis
            foreach (var docType in viewModel.AvailableDocumentTypes)
            {
                docType.IsSelected = docType.Value == customer.DocumentType;
            }

            return viewModel;
        }

        // ViewModels -> Entity
        public static Customer ToEntity(this CreateCustomerViewModel viewModel)
        {
            return new Customer
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Email = viewModel.Email,
                DocumentNumber = viewModel.DocumentNumber,
                DocumentType = viewModel.DocumentType,
                Phone = viewModel.Phone,
                StateCode = ObjectState.Active,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System", // TODO: Pegar do contexto do usuário logado
                LastModifiedAt = DateTime.UtcNow,
                LastModifiedBy = "System" // TODO: Pegar do contexto do usuário logado
            };
        }

        public static void UpdateEntity(this EditCustomerViewModel viewModel, Customer customer)
        {
            customer.FirstName = viewModel.FirstName;
            customer.LastName = viewModel.LastName;
            customer.Email = viewModel.Email;
            customer.DocumentNumber = viewModel.DocumentNumber;
            customer.DocumentType = viewModel.DocumentType;
            customer.Phone = viewModel.Phone;
            customer.StateCode = viewModel.StateCode;
            customer.LastModifiedAt = DateTime.UtcNow;
            customer.LastModifiedBy = "System"; // TODO: Pegar do contexto do usuário logado
        }

        // Coleções
        public static List<CustomerViewModel> ToViewModelList(this IEnumerable<Customer> customers)
        {
            return customers.Select(c => c.ToViewModel()).ToList();
        }

        public static CustomerIndexViewModel ToIndexViewModel(this IEnumerable<Customer> customers, 
            int currentPage = 1, int pageSize = 10, int totalCustomers = 0)
        {
            var customerList = customers.ToViewModelList();
            var totalPages = (int)Math.Ceiling((double)totalCustomers / pageSize);

            return new CustomerIndexViewModel
            {
                Customers = customerList,
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalCustomers = totalCustomers,
                Statistics = new CustomerStatisticsViewModel
                {
                    TotalCustomers = totalCustomers,
                    ActiveCustomers = customerList.Count(c => c.StateCode == ObjectState.Active),
                    IndividualCustomers = customerList.Count(c => c.DocumentType == DocumentType.CPF),
                    CorporateCustomers = customerList.Count(c => c.DocumentType == DocumentType.CNPJ)
                }
            };
        }
    }
} 