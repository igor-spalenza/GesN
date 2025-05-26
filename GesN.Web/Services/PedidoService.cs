using GesN.Web.Interfaces.Repositories;
using GesN.Web.Interfaces.Services;
using GesN.Web.Models;
using GesN.Web.Models.DTOs;

namespace GesN.Web.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _pedidoRepository;

        public PedidoService(IPedidoRepository pedidoRepository)
        {
            _pedidoRepository = pedidoRepository;
        }

        public async Task<Pedido> GetByIdAsync(int id)
        {
            var pedido = await _pedidoRepository.GetByIdAsync(id);
            return new Pedido
            {
                PedidoId = pedido.PedidoId,
                ClienteId = pedido.ClienteId,
                DataCadastro = pedido.DataCadastro,
                DataPedido = pedido.DataPedido,
                DataModificacao = pedido.DataModificacao,
                NomeCliente = pedido.NomeCliente
            };
        }

        public async Task<int> AddAsync(Pedido pedidoDto)
        {
            var dataAtual = DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"));
            var pedido = new Pedido
            {
                ClienteId = pedidoDto.ClienteId,
                DataPedido = pedidoDto.DataPedido,
                DataCadastro = dataAtual,
                DataModificacao = dataAtual
            };
            return await _pedidoRepository.AddAsync(pedido);
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Pedido>> GetAllAsync()
        {
            var pedidos = await _pedidoRepository.GetAllAsync();
            return pedidos.Select(p => new Pedido
            {
                PedidoId = p.PedidoId,
                ClienteId = p.ClienteId,
                DataCadastro = p.DataCadastro,
                DataPedido = p.DataPedido,
                DataModificacao = p.DataModificacao
            });
        }

        public async Task<(bool Success, string ErrorMessage)> UpdateAsync(Pedido pedidoDto)
        {
            try
            {
                var pedido = new Pedido
                {
                    PedidoId = pedidoDto.PedidoId,
                    ClienteId = pedidoDto.ClienteId,
                    DataPedido = pedidoDto.DataPedido,
                    DataModificacao = DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"))
                };
                
                return await _pedidoRepository.UpdateAsync(pedido);
            }
            catch (Exception ex)
            {
                // Capturar quaisquer exceções inesperadas no nível de serviço
                return (false, $"Erro no serviço: {ex.Message}");
            }
        }
    }
}
