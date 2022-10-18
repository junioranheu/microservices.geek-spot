using Microsoft.Extensions.DependencyInjection;
using Usuarios.Application.Common.Interfaces.Authentication;
using Usuarios.Application.Services.Authentication;

namespace Usuarios.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAutenticarService, AutenticarService>();

            return services;
        }
    }
}
