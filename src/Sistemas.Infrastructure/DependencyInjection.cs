using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sistemas.Application.Common.Interfaces.Persistence;
using Sistemas.Infrastructure.AutoMapper;
using Sistemas.Infrastructure.Persistence;

namespace Sistemas.Infrastructure
{
    public static class DependencyInjection
    {
        // Como importar o parâmetro "WebApplicationBuilder" caso aconteça algum erro: https://stackoverflow.com/questions/71146292/how-import-webapplicationbuilder-in-a-class-library
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, WebApplicationBuilder builder)
        {
            ConfigurationManager configuration = builder.Configuration;

            // =-=-=-=-=-=-=-=-=-= Configuração de depêndencia do AutoMapper =-=-=-=-=-=-=-=-=-=
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapperConfig());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            // Interfaces e repositórios;
            services.AddScoped<ISistemaRepository, SistemaRepository>();
            services.AddScoped<IAjudaTopicoRepository, AjudaTopicoRepository>();
            services.AddScoped<IAjudaItemRepository, AjudaItemRepository>();

            return services;
        }
    }
}
