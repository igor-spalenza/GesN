using GesN.Web.Interfaces.Services;
using GesN.Web.Models;
using GesN.Web.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GesN.Web.Controllers
{
    public class PedidoController : Controller
    {
        private readonly IPedidoService _pedidoService;
        private readonly IClienteService _clienteService;

        public PedidoController(IPedidoService pedidoService, IClienteService clienteService)
        {
            _clienteService = clienteService;
            _pedidoService = pedidoService;
        }
        // GET: VendaController
        public async Task<ActionResult> Index()
        {
            //var pedidos = await _pedidoService.GetAllAsync();
            //return View("Index", pedidos);
            return View();
        }

        public async Task<IActionResult> OnGetListaPedidos()
        {
            var pedidos = await _pedidoService.GetAllAsync();
            return new PartialViewResult
            {
                ViewName = "_ListaPedidos",
                Model = pedidos
            };
        }

        // Action para a Etapa 1 de criação do Pedido
        public IActionResult NovoPedido()
        {
            return PartialView("_Create");
        }

        /*public async Task<IActionResult> AnotarPedido(int idCliente)
        {
            var cliente = await _clienteService.GetByIdAsync(1);
            var pedido = new PedidoCreateDto
            {
                ClienteId = cliente.ClienteId
            };
            return PartialView("_PedidoCriacao", pedido);
        }
        */

        // GET: VendaController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var pedido = await _pedidoService.GetByIdAsync(id);
            return View("_Details", pedido);
        }
        
        public async Task<ActionResult> DetailsPartialView(int id)
        {
            var pedido = await _pedidoService.GetByIdAsync(id);
            return PartialView("_Details", pedido);
        }

        // GET: VendaController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: VendaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Pedido pedidoCreateDto)
        {

            ModelState.Remove("PedidoId");
            ModelState.Remove("DataCadastro");
            ModelState.Remove("DataModificacao");
            if (ModelState.IsValid)
            {
                if (pedidoCreateDto.PedidoId != null /*== 0*/)
                {
                    try
                    {
                        await _pedidoService.AddAsync(pedidoCreateDto);
                        //var pedido = _pedidoService.GetByIdAsync(1);
                        return RedirectToAction("EditPartial", 2);
                        //return PartialView("_Edit", pedido);
                    }
                    catch (Exception ex)
                    {
                        return PartialView();
                    }
                }
            }
            return PartialView();
        }

        // POST: VendaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePartial([FromForm]Pedido pedidoCreateDto)
        {

            ModelState.Remove("PedidoId");
            ModelState.Remove("DataCadastro");
            ModelState.Remove("DataModificacao");
            if (ModelState.IsValid)
            {
                if (pedidoCreateDto.PedidoId != null /*== 0*/)
                {
                    try
                    {
                        var Id = await _pedidoService.AddAsync(pedidoCreateDto);
                        //var pedido = _pedidoService.GetByIdAsync(1);
                        return RedirectToAction("EditPartial", Id);
                        //return PartialView("_Edit", pedido);
                    }
                    catch (Exception ex)
                    {
                        return PartialView();
                    }
                }
            }
            return PartialView();
        }

        // GET: VendaController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var pedido = await _pedidoService.GetByIdAsync(id);
            return View("_Edit", pedido);
        }

        // GET: VendaController/Edit/5
        public async Task<IActionResult> EditPartial(int id)
         {
            var pedido = await _pedidoService.GetByIdAsync(id);
            if (pedido == null)
                return NotFound();

            return PartialView("_Edit", pedido);
        }

        // POST: VendaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: VendaController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: VendaController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
