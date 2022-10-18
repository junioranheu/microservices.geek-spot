using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Usuarios.Application.Common.Interfaces.Authentication;
using Usuarios.Application.Common.Interfaces.Persistence;
using Usuarios.Infrastructure.Authentication;
using Usuarios.Infrastructure.AutoMapper;
using Usuarios.Infrastructure.Persistence;

namespace Usuarios.Infrastructure
{
    public static class DependencyInjection
    {
        // Como importar o parâmetro "WebApplicationBuilder" caso aconteça algum erro: https://stackoverflow.com/questions/71146292/how-import-webapplicationbuilder-in-a-class-library
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, WebApplicationBuilder builder)
        {
            ConfigurationManager configuration = builder.Configuration;

            // =-=-=-=-=-=-=-=-=-= Configuração do JWT =-=-=-=-=-=-=-=-=-=
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

            // =-=-=-=-=-=-=-=-=-= Configuração de depêndencia do AutoMapper =-=-=-=-=-=-=-=-=-=
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapperConfig());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            // =-=-=-=-=-=-=-=-=-= Serviços =-=-=-=-=-=-=-=-=-=
            services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

            // Interfaces e repositórios;
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IUsuarioSeguirRepository, UsuarioSeguirRepository>();

            // =-=-=-=-=-=-=-=-=-= Autenticação JWT para a API: https://balta.io/artigos/aspnet-5-autenticacao-autorizacao-bearer-jwt =-=-=-=-=-=-=-=-=-=
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
                x.SaveToken = true;
                x.IncludeErrorDetails = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:Secret"])),
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            return services;
        }
    }
}
