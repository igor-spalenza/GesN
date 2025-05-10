using GesN.Web.Interfaces.Repositories;
using GesN.Web.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace GesN.Web.PageModels
{
    public class PedidosIndexModel : PageModel
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IClienteRepository _clienteRepository;

        public PedidosIndexModel(
            IPedidoRepository pedidoRepository,
            IClienteRepository clienteRepository)
        {
            _pedidoRepository = pedidoRepository;
            _clienteRepository = clienteRepository;
        }

        [BindProperty]
        public Pedido Pedido { get; set; }

        public IEnumerable<Pedido> Pedidos { get; set; }
        public IEnumerable<Cliente> Clientes { get; set; }

        public void OnGet()
        {
            // A página principal é apenas um container
            // O conteúdo real será carregado via Ajax
        }

        public async Task<IActionResult> OnGetListaPedidosAsync()
        {
            Pedidos = await _pedidoRepository.GetAllAsync();
            return Partial("_ListaPedidos", Pedidos);
        }

        public async Task<IActionResult> OnGetFormularioCriacaoAsync()
        {
            Clientes = await _clienteRepository.GetAllAsync();
            Pedido = new Pedido { DataPedido = DateTime.Today };
            return Partial("_Create", this);
        }

        public async Task<IActionResult> OnGetFormularioEdicaoAsync(int id)
        {
            Pedido = await _pedidoRepository.GetByIdAsync(id);
            if (Pedido == null)
                return NotFound();

            Clientes = await _clienteRepository.GetAllAsync();
            return Partial("_Edit", this);
        }

        public async Task<IActionResult> OnGetDetalhesPedidoAsync(int id)
        {
            Pedido = await _pedidoRepository.GetByIdAsync(id);
            if (Pedido == null)
                return NotFound();

            return Partial("_Details", Pedido);
        }

        public async Task<IActionResult> OnPostSalvarNovoPedidoAsync()
        {
            if (!ModelState.IsValid)
            {
                Clientes = await _clienteRepository.GetAllAsync();
                return Partial("_Create", this);
            }

            await _pedidoRepository.AddAsync(Pedido);
            return new OkResult();
        }

        public async Task<IActionResult> OnPostSalvarEdicaoPedidoAsync(int id)
        {
            if (id != Pedido.PedidoId || !ModelState.IsValid)
            {
                Clientes = await _clienteRepository.GetAllAsync();
                return Partial("_Edit", this);
            }

            await _pedidoRepository.UpdateAsync(Pedido);
            return new OkResult();
        }
    }
}
