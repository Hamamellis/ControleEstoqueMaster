using System.ComponentModel.DataAnnotations;

namespace ControleEstoque.Web.Models
{
    public class EsqueciMinhaSenhaViewModel
    {
        [Required(ErrorMessage = "Por favor, Informe o Login")]
        public string Login { get; set; }
    }
}