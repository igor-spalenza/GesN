using GesN.Web.Interfaces.Repositories;
using GesN.Web.Interfaces.Services;
using GesN.Web.Models.Entities.Sales;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Services
{
    public class ContractService : IContractService
    {
        private readonly IContractRepository _contractRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<ContractService> _logger;

        public ContractService(
            IContractRepository contractRepository,
            ICustomerRepository customerRepository,
            ILogger<ContractService> logger)
        {
            _contractRepository = contractRepository;
            _customerRepository = customerRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Contract>> GetAllContractsAsync()
        {
            try
            {
                return await _contractRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os contratos");
                throw;
            }
        }

        public async Task<Contract?> GetContractByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return null;

                return await _contractRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar contrato por ID: {Id}", id);
                throw;
            }
        }

        public async Task<Contract?> GetContractByNumberAsync(string contractNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(contractNumber))
                    return null;

                return await _contractRepository.GetByContractNumberAsync(contractNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar contrato por número: {ContractNumber}", contractNumber);
                throw;
            }
        }

        public async Task<IEnumerable<Contract>> GetContractsByCustomerAsync(string customerId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(customerId))
                    return Enumerable.Empty<Contract>();

                return await _contractRepository.GetByCustomerIdAsync(customerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar contratos por cliente: {CustomerId}", customerId);
                throw;
            }
        }

        public async Task<IEnumerable<Contract>> GetContractsByStatusAsync(ContractStatus status)
        {
            try
            {
                return await _contractRepository.GetByStatusAsync(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar contratos por status: {Status}", status);
                throw;
            }
        }

        public async Task<IEnumerable<Contract>> GetActiveContractsAsync()
        {
            try
            {
                return await _contractRepository.GetActiveAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar contratos ativos");
                throw;
            }
        }

        public async Task<IEnumerable<Contract>> GetExpiringContractsAsync(int days = 30)
        {
            try
            {
                return await _contractRepository.GetExpiringAsync(days);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar contratos próximos do vencimento: {Days} dias", days);
                throw;
            }
        }

        public async Task<IEnumerable<Contract>> SearchContractsAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return await GetActiveContractsAsync();

                return await _contractRepository.SearchAsync(searchTerm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao pesquisar contratos: {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<string> CreateContractAsync(Contract contract, string createdBy)
        {
            try
            {
                // Validar dados do contrato
                if (!await ValidateContractDataAsync(contract))
                {
                    throw new ArgumentException("Dados do contrato inválidos");
                }

                // Verificar se o cliente existe
                var customer = await _customerRepository.GetByIdAsync(contract.CustomerId);
                if (customer == null)
                {
                    throw new ArgumentException($"Cliente não encontrado: {contract.CustomerId}");
                }

                // Verificar se já existe contrato com o mesmo número
                if (!string.IsNullOrWhiteSpace(contract.ContractNumber))
                {
                    var existingContract = await _contractRepository.GetByContractNumberAsync(contract.ContractNumber);
                    if (existingContract != null)
                    {
                        throw new InvalidOperationException($"Já existe um contrato com o número: {contract.ContractNumber}");
                    }
                }

                // Gerar número do contrato se não fornecido
                if (string.IsNullOrWhiteSpace(contract.ContractNumber))
                {
                    contract.ContractNumber = GenerateContractNumber();
                }

                // Definir quem criou
                contract.SetCreatedBy(createdBy);

                // Criar contrato
                var contractId = await _contractRepository.CreateAsync(contract);

                _logger.LogInformation("Contrato criado com sucesso: {ContractId} ({ContractNumber}) por {CreatedBy}", 
                    contractId, contract.ContractNumber, createdBy);

                return contractId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar contrato: {ContractTitle}", contract.Title);
                throw;
            }
        }

        public async Task<bool> UpdateContractAsync(Contract contract, string modifiedBy)
        {
            try
            {
                // Verificar se o contrato existe
                var existingContract = await _contractRepository.GetByIdAsync(contract.Id);
                if (existingContract == null)
                {
                    throw new ArgumentException($"Contrato não encontrado: {contract.Id}");
                }

                // Validar dados do contrato
                if (!await ValidateContractDataAsync(contract))
                {
                    throw new ArgumentException("Dados do contrato inválidos");
                }

                // Verificar se o cliente existe
                var customer = await _customerRepository.GetByIdAsync(contract.CustomerId);
                if (customer == null)
                {
                    throw new ArgumentException($"Cliente não encontrado: {contract.CustomerId}");
                }

                // Verificar duplicação de número de contrato (excluindo o próprio contrato)
                if (!string.IsNullOrWhiteSpace(contract.ContractNumber))
                {
                    var contractWithNumber = await _contractRepository.GetByContractNumberAsync(contract.ContractNumber);
                    if (contractWithNumber != null && contractWithNumber.Id != contract.Id)
                    {
                        throw new InvalidOperationException($"Já existe outro contrato com o número: {contract.ContractNumber}");
                    }
                }

                // Atualizar modificação
                contract.UpdateModification(modifiedBy);

                // Atualizar contrato
                var result = await _contractRepository.UpdateAsync(contract);

                if (result)
                {
                    _logger.LogInformation("Contrato atualizado com sucesso: {ContractId} por {ModifiedBy}", 
                        contract.Id, modifiedBy);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar contrato: {ContractId}", contract.Id);
                throw;
            }
        }

        public async Task<bool> DeleteContractAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return false;

                // Verificar se o contrato existe
                var contract = await _contractRepository.GetByIdAsync(id);
                if (contract == null)
                {
                    throw new ArgumentException($"Contrato não encontrado: {id}");
                }

                // Verificar se o contrato pode ser deletado
                if (contract.Status == ContractStatus.Signed || contract.Status == ContractStatus.Active)
                {
                    throw new InvalidOperationException("Não é possível deletar um contrato ativo ou assinado. Cancele-o primeiro.");
                }

                // Soft delete (desativar)
                var result = await _contractRepository.DeleteAsync(id);

                if (result)
                {
                    _logger.LogInformation("Contrato deletado (desativado) com sucesso: {ContractId}", id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar contrato: {ContractId}", id);
                throw;
            }
        }

        public async Task<bool> ContractExistsAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return false;

                return await _contractRepository.ExistsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência do contrato: {ContractId}", id);
                throw;
            }
        }

        public async Task<int> GetContractCountAsync()
        {
            try
            {
                return await _contractRepository.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar contratos");
                throw;
            }
        }

        public async Task<IEnumerable<Contract>> GetPagedContractsAsync(int page, int pageSize)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 100) pageSize = 100; // Limite máximo

                return await _contractRepository.GetPagedAsync(page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar contratos paginados: Page={Page}, PageSize={PageSize}", page, pageSize);
                throw;
            }
        }

        public async Task<bool> ConfirmContractAsync(string contractId, string confirmedBy)
        {
            try
            {
                var contract = await _contractRepository.GetByIdAsync(contractId);
                if (contract == null)
                {
                    throw new ArgumentException($"Contrato não encontrado: {contractId}");
                }

                contract.Confirm(confirmedBy);
                var result = await _contractRepository.UpdateAsync(contract);

                if (result)
                {
                    _logger.LogInformation("Contrato confirmado: {ContractId} por {ConfirmedBy}", contractId, confirmedBy);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao confirmar contrato: {ContractId}", contractId);
                throw;
            }
        }

        public async Task<bool> SignContractAsync(string contractId, string signedBy, DateTime? signDate = null)
        {
            try
            {
                var contract = await _contractRepository.GetByIdAsync(contractId);
                if (contract == null)
                {
                    throw new ArgumentException($"Contrato não encontrado: {contractId}");
                }

                contract.Sign(signedBy, signDate);
                var result = await _contractRepository.UpdateAsync(contract);

                if (result)
                {
                    _logger.LogInformation("Contrato assinado: {ContractId} por {SignedBy}", contractId, signedBy);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao assinar contrato: {ContractId}", contractId);
                throw;
            }
        }

        public async Task<bool> SuspendContractAsync(string contractId, string suspendedBy)
        {
            try
            {
                var contract = await _contractRepository.GetByIdAsync(contractId);
                if (contract == null)
                {
                    throw new ArgumentException($"Contrato não encontrado: {contractId}");
                }

                contract.Suspend(suspendedBy);
                var result = await _contractRepository.UpdateAsync(contract);

                if (result)
                {
                    _logger.LogInformation("Contrato suspenso: {ContractId} por {SuspendedBy}", contractId, suspendedBy);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao suspender contrato: {ContractId}", contractId);
                throw;
            }
        }

        public async Task<bool> CancelContractAsync(string contractId, string cancelledBy)
        {
            try
            {
                var contract = await _contractRepository.GetByIdAsync(contractId);
                if (contract == null)
                {
                    throw new ArgumentException($"Contrato não encontrado: {contractId}");
                }

                contract.Cancel(cancelledBy);
                var result = await _contractRepository.UpdateAsync(contract);

                if (result)
                {
                    _logger.LogInformation("Contrato cancelado: {ContractId} por {CancelledBy}", contractId, cancelledBy);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cancelar contrato: {ContractId}", contractId);
                throw;
            }
        }

        public async Task<bool> CompleteContractAsync(string contractId, string completedBy)
        {
            try
            {
                var contract = await _contractRepository.GetByIdAsync(contractId);
                if (contract == null)
                {
                    throw new ArgumentException($"Contrato não encontrado: {contractId}");
                }

                contract.Complete(completedBy);
                var result = await _contractRepository.UpdateAsync(contract);

                if (result)
                {
                    _logger.LogInformation("Contrato finalizado: {ContractId} por {CompletedBy}", contractId, completedBy);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao finalizar contrato: {ContractId}", contractId);
                throw;
            }
        }

        public async Task<bool> RenewContractAsync(string contractId, DateTime newEndDate, string renewedBy)
        {
            try
            {
                var contract = await _contractRepository.GetByIdAsync(contractId);
                if (contract == null)
                {
                    throw new ArgumentException($"Contrato não encontrado: {contractId}");
                }

                contract.Renew(newEndDate, renewedBy);
                var result = await _contractRepository.UpdateAsync(contract);

                if (result)
                {
                    _logger.LogInformation("Contrato renovado: {ContractId} até {NewEndDate} por {RenewedBy}", 
                        contractId, newEndDate, renewedBy);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao renovar contrato: {ContractId}", contractId);
                throw;
            }
        }

        public async Task<bool> ValidateContractDataAsync(Contract contract)
        {
            // Validações básicas
            if (string.IsNullOrWhiteSpace(contract.Title))
                return false;

            if (string.IsNullOrWhiteSpace(contract.CustomerId))
                return false;

            if (contract.TotalValue <= 0)
                return false;

            if (contract.StartDate == default)
                return false;

            // Validar datas
            if (contract.EndDate.HasValue && contract.EndDate.Value <= contract.StartDate)
                return false;

            await Task.CompletedTask; // Para manter assinatura async
            return true;
        }

        public async Task<decimal> CalculateContractValueAsync(string contractId)
        {
            try
            {
                var contract = await _contractRepository.GetByIdAsync(contractId);
                if (contract == null)
                {
                    return 0;
                }

                // Por enquanto, retorna o valor total do contrato
                // Futuramente, pode calcular baseado em itens do contrato
                return contract.TotalValue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao calcular valor do contrato: {ContractId}", contractId);
                throw;
            }
        }

        public async Task<IEnumerable<Contract>> GetContractsNearExpirationAsync()
        {
            try
            {
                return await _contractRepository.GetExpiringAsync(30);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar contratos próximos do vencimento");
                throw;
            }
        }

        private static string GenerateContractNumber()
        {
            var year = DateTime.Now.Year;
            var timestamp = DateTime.Now.ToString("MMddHHmmss");
            return $"CONT{year}{timestamp}";
        }
    }
} 