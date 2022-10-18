using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// Como criar um BaseController: https://stackoverflow.com/questions/58735503/creating-base-controller-for-asp-net-core-to-do-logging-but-something-is-wrong-w;
// Como fazer os metódos da BaseController não bugar a API ([NonAction]): https://stackoverflow.com/questions/35788911/500-error-when-setting-up-swagger-in-asp-net-core-mvc-6-app
// Ou então usar "protected";
namespace Itens.API.Controllers
{
    public abstract class BaseController<T> : Controller
    {
        protected async Task<bool> IsUsuarioSolicitadoMesmoDoToken(int id)
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            if (token != null)
            {
                // var nomeUsuarioSistema = User.FindFirstValue(ClaimTypes.Name);          
                // var usuarioTipoid = User.FindFirstValue(ClaimTypes.Role);
                var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (usuarioId != id.ToString())
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }
    }
}
