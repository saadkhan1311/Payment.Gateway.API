using AutoMapper;
using Flurl.Http;
using PaymentGateway.API.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PaymentGateway.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PaymentGateway.Business;
using System.Reflection;
using FluentValidation.AspNetCore;
using FluentValidation;
using PaymentGateway.Domain.DTOs;
using PaymentGateway.Domain.Validation;

namespace PaymentGateway.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private const string _apiKey_HeaderName = "Authorization";
        public IConfiguration Configuration { get; }

        public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
        {
            readonly IApiVersionDescriptionProvider provider;
            public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
            {
                this.provider = provider;
            }

            public void Configure(SwaggerGenOptions options)
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(
                      description.GroupName,
                        new Microsoft.OpenApi.Models.OpenApiInfo()
                        {
                            Title = $"Payment Gateway API - {description.ApiVersion}",
                            Version = description.ApiVersion.ToString(),
                        });
                }
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<AppDbContext>(options => options.UseSqlite(Configuration.GetConnectionString("DBConnection")), ServiceLifetime.Scoped);
            //services.AddControllers().AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<Startup>());
            //AES-256-CBC Encryption Algortithm is used by default which is PCI DSS Compliant. It can be overriden if needed
            services.AddDataProtection();

            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
                o.ApiVersionReader = new UrlSegmentApiVersionReader();
            });
            FlurlHttp.Configure(opt => opt.BeforeCall = call => { call.Request.SetHeader("content-type", "application\\json"); });
            services.AddVersionedApiExplorer(options => { options.GroupNameFormat = "'v'VVV"; options.SubstituteApiVersionInUrl = true; });
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddAutoMapper();

            #region Repositories
            services.AddScoped<IRequestInfo, RequestInfo>();
            services.AddScoped<ICardRepository, CardRepository>();
            services.AddScoped<ICurrencyRepository, CurrencyRepository>();
            services.AddScoped<IMerchantRepository, MerchantRepository>();
            services.AddScoped<IPaymentResponseRepository, PaymentResponseRepository>();
            #endregion

            #region Services
            services.AddScoped<IPaymentsService, PaymentsService>();
            services.AddScoped<ICardService, CardService>();
            //Mock Acquirer Service
            services.AddScoped<IAcquirerService, MockAcquiringBankService>();
            //Actual AcquiringBank Service (Switch before prodcution or use directives)
            //services.AddScoped<IAcquirerService, AcquiringBankService>();
            #endregion

            #region Validators
            services.AddSingleton<IValidator<PaymentRequestDTO>, PaymentRequestDTOValidator>();
            services.AddSingleton<IValidator<CardRequestDTO>, CardDTOValidator>();
            #endregion

            //Add swagger documents
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition(_apiKey_HeaderName, new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Api key needed to access the endpoints. Authorization: Merchant_Secret_Key",
                    Name = _apiKey_HeaderName,
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement{
                    {
                    new OpenApiSecurityScheme{
                        Name = _apiKey_HeaderName,
                        Type = SecuritySchemeType.ApiKey,
                        In = ParameterLocation.Header,
                        Reference = new OpenApiReference{
                        Id = _apiKey_HeaderName,
                        Type = ReferenceType.SecurityScheme}
                        },new List<string>()
                    }});
                GetXmlCommentsPath().ForEach(xmlFile => c.IncludeXmlComments(xmlFile, true));              
                
            });

            //Setup CORS
            services.AddCors(options =>
            {
                options.AddPolicy(name: "AllOrigins",
                                  builder =>
                                  {
                                      builder.AllowAnyOrigin();
                                      builder.AllowAnyMethod();
                                      builder.AllowAnyHeader();
                                  });
            });

        }
        private List<string> GetXmlCommentsPath()
        {
            List<string> xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.AllDirectories).ToList();
            return xmlFiles;
        }

       
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint(
                            $"/swagger/{description.GroupName}/swagger.json",
                            description.GroupName.ToUpperInvariant());
                        options.RoutePrefix = String.Empty;
                    }
                });

            app.UseRouting();
            app.UseAuthentication();
            app.UseCors("AllOrigins");

            app.UseApiKeyAuthenticatonMiddleWare();
            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
            });

            //Redirect default version of Swagger
            app.UseWhen(context => !context.Request.Path.Value.Contains("/api/v"), appBuilder =>
            {

                var options = new RewriteOptions()
                    .AddRedirect("api/(.*)", "api/v1/$1", 302);
                appBuilder.UseRewriter(options);
            });
        }

       
        
    }


}
