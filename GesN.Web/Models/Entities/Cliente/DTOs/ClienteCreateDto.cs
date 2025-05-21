using System.ComponentModel.DataAnnotations;

namespace GesN.Web.Models.DTOs
{
    public class ClienteCreateDto
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

        public ClienteCreateDto()
        {

        }

        public ClienteCreateDto(Cliente cliente)
        {
            Nome = cliente.Nome;
            Sobrenome = cliente.Sobrenome;
            Cpf = cliente.Cpf;
            TelefonePrincipal = cliente.TelefonePrincipal;
            DataCadastro = DateTime.Now;
            DataModificacao = DateTime.Now;
        }
    }
}
