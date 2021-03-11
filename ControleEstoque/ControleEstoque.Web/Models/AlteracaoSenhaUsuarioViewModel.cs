using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ControleEstoque.Web.Models
{
    public class AlteracaoSenhaUsuarioViewModel
    {
        [Required(ErrorMessage ="Por favor, preencha a Senha atual...")]
        [Display(Name = "Senha Atual")]

        public string SenhaAtual { get; set; }

        [Required(ErrorMessage = "Por favor, preencha a Nova Senha...")]
        [MinLength(3, ErrorMessage ="A Nova Senha deve ter no mínino 3 Caracteres")]
        [Display(Name = "Nova Senha")]

        public string NovaSenha { get; set; }

        [Required(ErrorMessage = "Por favor, Confirme a Nova Senha...")]
        [Compare("NovaSenha", ErrorMessage ="A Nova Senha e a Confirmação devem ser IGUAIS")]
        [Display(Name = "Confirmação da Nova Senha")]

        public string ConfirmacaoNovaSenha { get; set; }

    }
}