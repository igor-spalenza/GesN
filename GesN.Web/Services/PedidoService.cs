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
                ColaboradorId = pedido.ColaboradorId,
                DataCadastro = pedido.DataCadastro,
                DataPedido = pedido.DataPedido,
                DataModificacao = pedido.DataModificacao
            };
        }

        public async Task<int> AddAsync(Pedido clienteDto)
        {
            var dataAtual = DateTime.Now;
            var pedido = new Pedido
            {
                ClienteId = clienteDto.ClienteId,
                ColaboradorId = clienteDto.ColaboradorId,
                DataPedido =  clienteDto.DataPedido,
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

        public Task UpdateAsync(Pedido clienteDto)
        {
            throw new NotImplementedException();
        }
    }
}
