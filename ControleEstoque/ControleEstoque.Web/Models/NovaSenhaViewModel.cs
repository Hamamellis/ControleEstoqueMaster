using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ControleEstoque.Web.Models
{
    public class NovaSenhaViewModel
    {
        [Required(ErrorMessage = "Informe a Senha")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha:")]
        public string Senha { get; set; }

        [Compare("Senha", ErrorMessage = "Informe a confirmação da Senha")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmação da Senha:")]
        public string ConfirmacaoSenha { get; set; }

        public int Usuario { get; set; }
    }
}