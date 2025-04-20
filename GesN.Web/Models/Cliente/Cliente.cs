using System.ComponentModel.DataAnnotations;

namespace GesN.Web.Models
{
    public class Cliente
    {
        public int ClienteId { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }

        [Display(Name = "CPF")]
        public string Cpf { get; set; }

        [Display(Name = "Telefone Principal")]
        public string TelefonePrincipal { get; set; }

        [Display(Name = "Data de Criação")]
        public DateTime DataCadastro { get; set; }

        [Display(Name = "Data de Modificação")]
        public DateTime DataModificacao { get; set; }
    }
}
