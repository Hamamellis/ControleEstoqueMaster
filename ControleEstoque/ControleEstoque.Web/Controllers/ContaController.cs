using ControleEstoque.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ControleEstoque.Web.Controllers
{
    public class ContaController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel login, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }
            var usuario = UsuarioModel.ValidarUsuario(login.Usuario, login.Senha);

            if (usuario != null)
            {
                var tiket = FormsAuthentication.Encrypt(new FormsAuthenticationTicket(
                    1, usuario.Nome, DateTime.Now, DateTime.Now.AddHours(12), login.LembrarMe, usuario.Id + "|" + usuario.RecuperarStringNomePerfis()));
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, tiket);
                Response.Cookies.Add(cookie);

                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("", "Login inválido.");
            }
            return View(login);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult AlterarSenhaUsuario(AlteracaoSenhaUsuarioViewModel model)
        {
            ViewBag.Mensagem = null;

            if (HttpContext.Request.HttpMethod.ToUpper() == "POST")
            {
                var usuarioLogado = (HttpContext.User as AplicacaoPrincipal); // - Usuario no Contexto do Http convertido...

                var alterou = false; // - não está validado...

                if (usuarioLogado != null)
                {
                    // - ABAIXO: -- > vamos verificar se a Senha atual é igual a que está no Banco de Dados...
                    if (!usuarioLogado.Dados.ValidarSenhaAtual(model.SenhaAtual)) // - SE NÃO CONSEGUIU...
                    {   // - EXIBE ESTA MENSAGEM...
                        ModelState.AddModelError("SenhaAtual", "A senha Atual não Confere com os registros...");
                    }
                    else
                    {
                        alterou = usuarioLogado.Dados.AlterarSenha(model.NovaSenha);
                        // - Se a Senha confere... e autenticado o Usuario Logado...                                                                                 
                        // - recebe a validação para trocar por nova senha...

                        if (alterou)
                        {
                            ViewBag.Mensagem = new string[] { "OK", "Senha alterada com sucesso." };
                        }
                        else
                        {
                            ViewBag.Mensagem = new string[] { "Erro", "Não foi possível alterar a senha." };
                        }
                    }
                }
                return View();
            }
            else
            {
                ModelState.Clear(); // - QUANDO FOR UM ""GET"" - O ESTADO DEVE FICAR LIMPO...
                return View();
            }
        }

        [AllowAnonymous]
        public ActionResult EsqueciMinhaSenha(EsqueciMinhaSenhaViewModel model)
        {
            ViewBag.EmailEnviado = true;
            // - Se o Contexto do HTTP na requisição for GET - usando o metodo do tipo ToUpper()... falso, então limpe...
            if (HttpContext.Request.HttpMethod.ToUpper() == "GET")
            {
                ViewBag.EmailEnviado = false;
                ModelState.Clear();
            }
            // - Se NÃO for 'Get', será 'Post' automáticamente.
            else
            {
                var usuario = UsuarioModel.RecuperarPeloLogin(model.Login);

                if (usuario != null)
                {
                    EnviarEmailRedefinicaoSenha(usuario);
                }
            }

            return View(model);  // - O 'model' serve para 'GET e Post' *** ou Um ou Outro...
        }

        [AllowAnonymous]
        public ActionResult RedefinirSenha(int id)
        {
            var usuario = UsuarioModel.RecuperarPeloId(id);
            if (usuario == null)
            {
                id = -1;
            }

            var model = new NovaSenhaViewModel() { Usuario = id };

            ViewBag.Mensagem = null;

            return View(model); // - Carregamos o id do Usuário aqui para chamá-lo abaixo... RenefinirSenha()
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult RedefinirSenha(NovaSenhaViewModel model)
        {
            ViewBag.Mensagem = null; // - Se não houver nenhum problema, não vai exibir mensagem... 

            if (!ModelState.IsValid) // - Se não estiver válido...
            {
                return View(model); // - Vamos retornar com o model...
            }
            var usuario = UsuarioModel.RecuperarPeloId(model.Usuario); // - Vamos carregar o id pelo Usuário...
            if(usuario != null)
            {
                var ok = usuario.AlterarSenha(model.Senha);
                ViewBag.Mensagem = ok ? 
                    "Senha Alterada com Sucesso!!!" : 
                    "Não foi possível Alterar a sua Senha!!!";
            }

            return View(); // - Se deixarmos com este view, ele não vai realizar o Post...
        }

        private void EnviarEmailRedefinicaoSenha(UsuarioModel usuario)
        {
            var callbackUrl = Url.Action("RedefinirSenha", "Conta", new { id = usuario.Id }, protocol: Request.Url.Scheme);
            var client = new SmtpClient() // - devemos importar a diretiva using "System.Net.Mail"... -SE NÃO CONFIGURAR EM WEN.CONFIG OS PARAMS NÃO FUNCIONA
            {
                Host = ConfigurationManager.AppSettings["EmailServidor"], // - Configuração do Servidor...
                Port = Convert.ToInt32(ConfigurationManager.AppSettings["EmailPorta"]), // - Qual Porta de acesso...
                EnableSsl = (ConfigurationManager.AppSettings["EmailSsl"] == "S"), // - verificar se esta com S de Sim...
                UseDefaultCredentials = false, // - falsa - Pois iremos informar a credencial abaixo...

                // - devemos importar a diretiva using "System.Net"...
                Credentials = new NetworkCredential(
                    ConfigurationManager.AppSettings["EmailUsuario"], // - Informa o Usuário
                    ConfigurationManager.AppSettings["EmailSenha"])  // - Informa o Senha
            };

            var mensagem = new MailMessage();

            mensagem.From = new MailAddress(ConfigurationManager.AppSettings["EmailOrigem"], "Controle de Estoque - Aprendendo a Redefinir a senha do usuário");
            mensagem.To.Add(usuario.Email);
            mensagem.Subject = "Redefinição de Senha";
            mensagem.Body = string.Format("Redefina a sua Senha <a href='{0}'>Aqui</a>", callbackUrl); // - O link Vai esta aqui que direcionará para o ActionLink...
            mensagem.IsBodyHtml = true; // - Informamos que será do tipo Html...

            client.Send(mensagem);
        }
    }
}

/* - OBSERVAÇÃO:
 * - O envio do E-mal FAKE, só vai funcionar se estiver executando o Rnwood.Smtp4Dev.exe em:
 * E:\DOWNLOAD app´s\RnWood Smtp 4 Developer Executável e-mail Fake\Rnwood Fake Smtp4Dev 
*/