using System;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using API.Helpers;
using API.Helpers.Interfaces;
using DataAccess.LiteDbInfrastructure;
using DataAccess.LiteDbInfrastructure.Interfaces;
using DataAccess.Repositories;
using DataAccess.Repositories.Interfaces;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using Services;
using Services.Interfaces;
using Services.Validators;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using MapperConfiguration = API.Mappers.MapperConfiguration;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "Contacts API",
                        Version = "v1",
                        Description = "Api for CRUD operations against a contact list.",
                        Contact = new OpenApiContact
                        {
                            Name = "Jordan Upperman",
                            Email = "Jordaneupperman@gmail.com",
                            Url = new Uri("https://github.com/Jupperman/ContactsApi"),
                        },
                    });
                    c.EnableAnnotations();
                    c.AddServer(new OpenApiServer() {Description = "IIS Express", Url = "http://localhost:21268" });
                    c.AddServer(new OpenApiServer() {Description = "API", Url = "http://localhost:5000" });
                })
                .AddSingleton<ILiteDbConnection>(x =>
                    new LiteDbConnection(Configuration.GetSection("DbDirectoryPath").Value))
                .AddScoped<IContactRepository, ContactLiteDbRepository>()
                .AddScoped<IContactService, ContactService>()
                .AddScoped<IRequestAccessor, RequestAccessor>()
                .AddAutoMapper(mc => mc.AddProfile<MapperConfiguration>())
                .AddFluentValidation(fv => fv
                    .RegisterValidatorsFromAssemblyContaining<ContactValidator>()
                )
                .AddControllers()
                .AddJsonOptions(opts => opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Contacts API V1");
                    c.RoutePrefix = string.Empty;
                });
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
