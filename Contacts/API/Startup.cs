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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<ILiteDbConnection>(x =>
                    new LiteDbConnection(Configuration.GetSection("DbDirectoryPath").Value))
                .AddScoped<IContactRepository, ContactLiteDbRepository>()
                .AddScoped<IContactService, ContactService>()
                .AddScoped<IRequestAccessor, RequestAccessor>()
                .AddAutoMapper(mc => mc.AddProfile<MapperConfiguration>())
                .AddFluentValidation(fv => fv
                    .RegisterValidatorsFromAssemblyContaining<ContactValidator>()
                    .RegisterValidatorsFromAssemblyContaining<PhoneValidator>()
                )
                .AddControllers()
                .AddJsonOptions(opts => opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
