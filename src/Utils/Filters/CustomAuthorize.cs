using Microsoft.AspNetCore.Authorization;
using Utils.Enums;

namespace Utils.Filters
{
    public class CustomAuthorize : AuthorizeAttribute
    {
        // https://stackoverflow.com/questions/1148312/asp-net-mvc-decorate-authorize-with-multiple-enums
        public CustomAuthorize(params UsuarioTipoEnum[] roles)
        {
            string resultadoFinal = string.Empty;

            foreach (var role in roles)
            {
                resultadoFinal += (int)role + ", ";
            }

            if (resultadoFinal.EndsWith(", "))
            {
                resultadoFinal = resultadoFinal.Remove(resultadoFinal.Length - 2);
            }

            Roles = resultadoFinal;
        }
    }
}
