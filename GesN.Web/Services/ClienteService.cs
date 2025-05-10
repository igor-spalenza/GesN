using GesN.Web.Interfaces.Repositories;
using GesN.Web.Interfaces.Services;
using GesN.Web.Models;
using GesN.Web.Models.DTOs;

namespace GesN.Web.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<Cliente> GetByIdAsync(int id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            return new Cliente
            {
                ClienteId = cliente.ClienteId,
                Nome = cliente.Nome,
                Sobrenome = cliente.Sobrenome,
                Cpf = cliente.Cpf,
                TelefonePrincipal = cliente.TelefonePrincipal,
                DataCadastro = cliente.DataCadastro
            };
        }

        public async Task<IEnumerable<Cliente>> GetAllAsync()
        {
            var clientes = await _clienteRepository.GetAllAsync();
            return clientes.Select(c => new Cliente
            {
                ClienteId = c.ClienteId,
                Nome = c.Nome,
                Sobrenome = c.Sobrenome,
                Cpf = c.Cpf,
                TelefonePrincipal = c.TelefonePrincipal,
                DataCadastro = c.DataCadastro
            });
        }

        public async Task AddAsync(ClienteCreateDto clienteDto)
        {
            var dataAtual = DateTime.Now;
            var cliente = new Cliente
            {
                Nome = clienteDto.Nome,
                Sobrenome = clienteDto.Sobrenome,
                Cpf = clienteDto.Cpf,
                TelefonePrincipal = clienteDto.TelefonePrincipal,
                DataCadastro = dataAtual,
                DataModificacao = dataAtual
            };
            await _clienteRepository.AddAsync(cliente);
        }

        public async Task UpdateAsync(Cliente clienteDto)
        {
            var cliente = new Cliente
            {
                ClienteId = clienteDto.ClienteId,
                Nome = clienteDto.Nome,
                Sobrenome = clienteDto.Sobrenome,
                Cpf = clienteDto.Cpf,
                TelefonePrincipal = clienteDto.TelefonePrincipal,
            };
            await _clienteRepository.UpdateAsync(cliente);
        }

        public async Task DeleteAsync(int id)
        {
            await _clienteRepository.DeleteAsync(id);
        }
    }
}
