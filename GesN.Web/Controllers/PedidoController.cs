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

        // Action para a Etapa 1 de criação do Pedido
        public IActionResult NovoPedido()
        {
            return PartialView("_Create");
        }
        public async Task<IActionResult> ListaPedidos()
        {
            var pedidos = await _pedidoService.GetAllAsync();
            return PartialView("_ListaPedidos", pedidos);
        }

        // GET: VendaController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var pedido = await _pedidoService.GetByIdAsync(id);
            return View("_Details", pedido);
        }
        
        public async Task<IActionResult> DetailsPartialView(int id)
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

        [HttpGet]
        public async Task<IActionResult> Criar()
        {
            var clientes = await _clienteService.GetAllAsync();
            ViewBag.Clientes = clientes;
            return PartialView("_Create");
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

        [HttpGet]
        public async Task<IActionResult> BuscarClienteNomeTel(string termo)
        {
            if (string.IsNullOrWhiteSpace(termo))
                return Json(new List<object>());

            var clientes = await _clienteService.BuscarPorNomeOuTelefoneAsync(termo);

            var resultado = clientes.Select(c => new {
                id = c.ClienteId,
                nome = c.Nome + " " + c.Sobrenome,
                telefonePrincipal = c.TelefonePrincipal
            });

            return Json(resultado);
        }

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

        // GET: Pedido/Editar/5 - Retorna a partial view de edição
        public async Task<IActionResult> Editar(int id)
        {
            var pedido = await _pedidoService.GetByIdAsync(id);
            if (pedido == null)
                return NotFound();

            return PartialView("_Edit", pedido);
        }

        // POST: Pedido/SalvarEdicao - Salva a edição de um pedido via AJAX
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarEdicao(int id, [FromForm]Pedido pedido)
        {
            if (id != pedido.PedidoId)
                return NotFound();
                
            if (!ModelState.IsValid)
            {
                // Retorna os erros de validação do modelo
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return Json(new { 
                    success = false, 
                    message = "Erro de validação", 
                    errors = errors 
                });
            }

            try
            {
                var (success, errorMessage) = await _pedidoService.UpdateAsync(pedido);
                
                if (!success)
                {
                    return Json(new { 
                        success = false, 
                        message = errorMessage ?? "Erro ao atualizar o pedido"
                    });
                }
                
                var pedidoAtualizado = await _pedidoService.GetByIdAsync(id);
                return Json(new
                {
                    success = true,
                    message = "Pedido atualizado com sucesso",
                    pedido = new
                    {
                        pedidoId = pedidoAtualizado.PedidoId,
                        clienteId = pedidoAtualizado.ClienteId,
                        dataPedido = pedidoAtualizado.DataPedido,
                        dataCadastro = pedidoAtualizado.DataCadastro,
                        dataModificacao = pedidoAtualizado.DataModificacao
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    message = "Erro ao atualizar pedido: " + ex.Message 
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarNovo([FromForm]Pedido pedido)
        {
            ModelState.Remove("PedidoId");
            ModelState.Remove("DataCadastro");
            ModelState.Remove("DataModificacao");
            ModelState.Remove("NomeCliente");

            if (!ModelState.IsValid)
            {
                return PartialView("_Create", pedido);
            }

            try
            {
                var id = await _pedidoService.AddAsync(pedido);
                return Json(new { success = true, id = id, message = "Pedido criado com sucesso!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro ao criar pedido: " + ex.Message });
            }
        }
    }
}
