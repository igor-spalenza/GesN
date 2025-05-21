using GesN.Web.Models.Enumerators;
using System.ComponentModel.DataAnnotations;

namespace GesN.Web.Models
{
    public class Pedido
    {
        public int PedidoId { get; set; }

        public int ClienteId { get; set; }

        public int ColaboradorId { get; set; }

        [Display(Name = "Data do Pedido")]
        public DateTime DataPedido { get; set; }

        [Display(Name = "Data de Criação")]
        public DateTime DataCadastro { get; set; }

        [Display(Name = "Data de Modificação")]
        public DateTime DataModificacao { get; set; }
        
        public Status Status { get; set; }

        public string NomeCliente { get; set; }
    }
}
