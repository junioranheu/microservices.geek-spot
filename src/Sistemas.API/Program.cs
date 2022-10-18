using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Sistemas.Infrastructure;
using Sistemas.Infrastructure.Data;
using Swashbuckle.AspNetCore.SwaggerUI;
using Utils.Filters;

var builder = WebApplication.CreateBuilder(args);
{
    // T�cnica para adicionar as classes de Infrastructure em uma classe centralizada: https://youtu.be/fhM0V2N1GpY?t=2149
    builder.Services.AddInfrastructure(builder);

    // Filtro de erros;
    builder.Services.AddControllers(o => o.Filters.Add<ErrorHandlingFilterAttribute>());

    // Inserir as informa��es do banco na vari�vel builder antes de build�-la;
    var secretSenhaBancoDados = builder.Configuration["SecretSenhaBancoDados"]; // secrets.json;
    string con = builder.Configuration.GetConnectionString("BaseDadosGeekSpot");
    con = con.Replace("[secretSenhaBancoDados]", secretSenhaBancoDados); // Alterar pela senha do secrets.json;
    builder.Services.AddDbContext<Context>(options => options.UseMySql(con, ServerVersion.AutoDetect(con)));

    // Swagger;
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() { Title = "GeekSpot", Version = "v1" });

        // https://stackoverflow.com/questions/43447688/setting-up-swagger-asp-net-core-using-the-authorization-headers-bearer
        var jwtSecurityScheme = new OpenApiSecurityScheme
        {
            Scheme = "bearer",
            BearerFormat = "JWT",
            Name = "JWT Authentication",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Description = "Coloque **_apenas_** o token (JWT Bearer) abaixo!",

            Reference = new OpenApiReference
            {
                Id = JwtBearerDefaults.AuthenticationScheme,
                Type = ReferenceType.SecurityScheme
            }
        };

        c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            { jwtSecurityScheme, Array.Empty<string>() }
        });
    });

    // Cors;
    builder.Services.AddCors(options =>
        options.AddPolicy(name: builder.Configuration["CORSSettings:Cors"], builder =>
        {
            builder.AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed((host) => true)
                .AllowCredentials();
        })
    );

    // Adicionar comando "AddNewtonsoftJson" para ignorar "erros" de object cycle - https://stackoverflow.com/questions/59199593/net-core-3-0-possible-object-cycle-was-detected-which-is-not-supported;
    // E tamb�m formatar o resultado JSON retornado pelas APIs;
    builder.Services.AddControllers().AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
    });
}

var app = builder.Build();
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "GeekSpot v1");
            c.DocExpansion(DocExpansion.None); // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/279
        });

        app.UseDeveloperExceptionPage();
    }

    // Redirecionar sempre para HTTPS;
    if (app.Environment.IsProduction())
    {
        app.UseHttpsRedirection();
    }

    // Cors;
    app.UseCors(builder.Configuration["CORSSettings:Cors"]);

    // Outros;
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}

//  Program.cs public para o xUnit;
public partial class Program { }