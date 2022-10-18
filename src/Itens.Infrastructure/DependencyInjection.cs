using AutoMapper;
using Itens.Application.Common.Interfaces.Persistence;
using Itens.Infrastructure.AutoMapper;
using Itens.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Itens.Infrastructure
{
    public static class DependencyInjection
    {
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
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<IItemTipoRepository, ItemTipoRepository>();
            services.AddScoped<IComentarioRepository, ComentarioRepository>();

            return services;
        }
    }
}
