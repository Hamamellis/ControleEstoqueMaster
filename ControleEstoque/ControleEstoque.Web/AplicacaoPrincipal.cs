using ControleEstoque.Web.Models;
using System.Security.Principal;

namespace ControleEstoque.Web
{
    public class AplicacaoPrincipal : GenericPrincipal // - Esta Classe herda de GenericPrincipal...
    {
        public UsuarioModel Dados { get; set; } // - Ele contem uma Propriedade Dados do Tipo UsuarioModel...

        public AplicacaoPrincipal(IIdentity identity, string[] roles, int id) : base(identity, roles)
        {
            Dados = UsuarioModel.RecuperarPeloId(id);
        }
        
    }
}