using System.ComponentModel.DataAnnotations;

namespace GesN.Web.Models
{
    public class Pedido
    {
        public string PedidoId { get; set; }
        public string ClienteId { get; set; }
        public string ColaboradorId { get; set; }

        [Display(Name = "Data do Pedido")]
        public DateTime DataPedido { get; set; }

        [Display(Name = "Data de Criação")]
        public DateTime DataCadastro { get; set; }

        [Display(Name = "Data de Modificação")]
        public DateTime DataModificacao { get; set; }
    }
}
