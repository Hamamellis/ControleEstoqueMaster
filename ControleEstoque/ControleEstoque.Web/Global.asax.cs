using System;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace ControleEstoque.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError(); // - ESTAMOS PEGANDO O ÚLTIMO ERRO...

            if (ex is HttpRequestValidationException) // - VERIFICANDO SE ESTE ERRO É VALIDADO NA REQUISIÇÃO...
            {
                Response.Clear();
                Response.StatusCode = 200;
                Response.ContentType = "application/json";
                Response.Write("{ \"Resultado\":\"AVISO\",\"Mensagens\":[\"Somente texto sem caracteres especiais pode ser enviado.\"],\"IdSalvo\":\"\"}");
                Response.End();
            }
            else if (ex is HttpAntiForgeryException) // - VERIFICANDO SE ESTE ERRO É GERADO NA FORGERY... SE HOUVER ATAQUE NÃO RESPONDE...
            {
                Response.Clear();
                Response.StatusCode = 200;
                Response.End();
                // gravar LOG
            }
        }

        protected void Application_AuthenticationRequest(Object sender, EventArgs e)
        {
            var cookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];

            if (cookie != null && cookie.Value != string.Empty)
            {
                FormsAuthenticationTicket tiket;
                try
                {
                    tiket = FormsAuthentication.Decrypt(cookie.Value);
                }
                catch
                {
                    return;
                }

                var partes = tiket.UserData.Split('|');
                var id = Convert.ToInt32(partes[0]); // - PEGAMOS O ID NA 1ª PARTE... PELO "|"
                var perfis = partes[1].Split(';'); // - PEGAMOS OS PERFIS NA 2ª PARTE... PELO ";"

                if(Context.User != null)
                {
                    Context.User = new AplicacaoPrincipal(Context.User.Identity, perfis, id);
                }

            }
        }

    }
}
