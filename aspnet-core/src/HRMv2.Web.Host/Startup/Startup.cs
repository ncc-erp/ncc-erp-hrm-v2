using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Castle.Facilities.Logging;
using Abp.AspNetCore;
using Abp.AspNetCore.Mvc.Antiforgery;
using Abp.Castle.Logging.Log4Net;
using Abp.Extensions;
using HRMv2.Configuration;
using HRMv2.Identity;
using Abp.AspNetCore.SignalR.Hubs;
using Abp.Dependency;
using Abp.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System.IO;
using HRMv2.Constants;
using Amazon.Runtime.CredentialManagement;
using Amazon;
using Amazon.S3;
using HRMv2.WebServices.Project;
using HRMv2.WebServices;
using HRMv2.WebServices.Timesheet;
using HRMv2.UploadFileServices;
using HRMv2.Hubs;
using Microsoft.AspNetCore.Http.Connections;

namespace HRMv2.Web.Host.Startup
{
    public class Startup
    {
        private const string _defaultCorsPolicyName = "localhost";

        private const string _apiVersion = "v1";

        private readonly IConfigurationRoot _appConfiguration;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public Startup(IWebHostEnvironment env)
        {
            _hostingEnvironment = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //MVC
            services.AddControllersWithViews(
                options => { options.Filters.Add(new AbpAutoValidateAntiforgeryTokenAttribute()); }
            ).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new AbpMvcContractResolver(IocManager.Instance)
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                };
            });

            IdentityRegistrar.Register(services);
            AuthConfigurer.Configure(services, _appConfiguration);

            WebServiceRegistrar.AddWebServices(services, _appConfiguration);

            services.AddSignalR(options => {
                options.EnableDetailedErrors = true;
            });

            // Configure CORS for angular2 UI
            services.AddCors(
                options => options.AddPolicy(
                    _defaultCorsPolicyName,
                    builder => builder
                        .WithOrigins(
                            // App:CorsOrigins in appsettings.json can contain more than one address separated by comma.
                            _appConfiguration["App:CorsOrigins"]
                                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .Select(o => o.RemovePostFix("/"))
                                .ToArray()
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                )
            );

            // Swagger - Enable this line and the related lines in Configure method to enable swagger UI
            ConfigureSwagger(services);

            // Configure Abp and Dependency Injection
            services.AddAbpWithoutCreatingServiceProvider<HRMv2WebHostModule>(
                // Configure Log4Net logging
                options => options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                    f => f.UseAbpLog4Net().WithConfig(_hostingEnvironment.IsDevelopment()
                        ? "log4net.config"
                        : "log4net.Production.config"
                    )
                )
            );
            RegisterFileService(services);

            services.Configure<TimesheetConfig>(_appConfiguration.GetSection("TimesheetService"));
            HRMv2Consts.EnableBackgroundJobExecution = _appConfiguration.GetValue<bool?>("App:EnableBackgroundJobExecution") ?? true ;
            HRMv2Consts.HRM_Uri = _appConfiguration.GetValue<string>("App:ClientRootAddress");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseAbp(options => { options.UseAbpRequestLocalization = false; }); // Initializes ABP framework.

            app.UseCors(_defaultCorsPolicyName); // Enable CORS!

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAbpRequestLocalization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<AbpCommonHub>("/signalr", options => {
                    options.Transports = HttpTransportType.LongPolling | HttpTransportType.WebSockets;
                });
                endpoints.MapHub<CalculateSalaryHub>("/signalr-calculateSalaryHub", options => {
                    options.Transports = HttpTransportType.LongPolling | HttpTransportType.WebSockets;
                });
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute("defaultWithArea", "{area}/{controller=Home}/{action=Index}/{id?}");
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger(c => { c.RouteTemplate = "swagger/{documentName}/swagger.json"; });

            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUI(options =>
            {
                // specifying the Swagger JSON endpoint.
                options.SwaggerEndpoint($"/swagger/{_apiVersion}/swagger.json", $"HRMv2 API {_apiVersion}");
                options.IndexStream = () => Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("HRMv2.Web.Host.wwwroot.swagger.ui.index.html");
                options.DisplayRequestDuration(); // Controls the display of the request duration (in milliseconds) for "Try it out" requests.  
            }); // URL: /swagger
        }
        
        private void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(_apiVersion, new OpenApiInfo
                {
                    Version = _apiVersion,
                    Title = "HRMv2 API",
                    Description = "HRMv2",
                    // uncomment if needed TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "HRMv2",
                        Email = string.Empty,
                        Url = new Uri("https://twitter.com/aspboilerplate"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://github.com/aspnetboilerplate/aspnetboilerplate/blob/dev/LICENSE"),
                    }
                });
                options.DocInclusionPredicate((docName, description) => true);

                // Define the BearerAuth scheme that's in use
                options.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme()
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                //add summaries to swagger
                bool canShowSummaries = _appConfiguration.GetValue<bool>("Swagger:ShowSummaries");
                if (canShowSummaries)
                {
                    var hostXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var hostXmlPath = Path.Combine(AppContext.BaseDirectory, hostXmlFile);
                    options.IncludeXmlComments(hostXmlPath);

                    var applicationXml = $"HRMv2.Application.xml";
                    var applicationXmlPath = Path.Combine(AppContext.BaseDirectory, applicationXml);
                    options.IncludeXmlComments(applicationXmlPath);

                    var webCoreXmlFile = $"HRMv2.Web.Core.xml";
                    var webCoreXmlPath = Path.Combine(AppContext.BaseDirectory, webCoreXmlFile);
                    options.IncludeXmlComments(webCoreXmlPath);
                }
            });
        }

        private void RegisterFileService(IServiceCollection services)
        {
            LoadUploadFileConfig();
            if (UploadFileConstant.UploadFileProvider == UploadFileConstant.AmazoneS3)
            {
                CreateAWSCredentialProfile();
                services.AddAWSService<IAmazonS3>();
                services.AddTransient<IUploadFileService, AmazonS3Service>();
            }
            else
            {
                services.AddTransient<IUploadFileService, InternalUploadFileService>();
            }

        }
        void CreateAWSCredentialProfile()
        {
            var options = new CredentialProfileOptions
            {
                AccessKey = AmazoneS3Constant.AccessKeyId,
                SecretKey = AmazoneS3Constant.SecretKeyId
            };
            var profile = new CredentialProfile(AmazoneS3Constant.Profile, options);
            profile.Region = RegionEndpoint.GetBySystemName(AmazoneS3Constant.Region);

            var sharedFile = new SharedCredentialsFile();
            sharedFile.RegisterProfile(profile);
        }
        private void LoadUploadFileConfig()
        {
            AmazoneS3Constant.Profile = _appConfiguration.GetValue<string>("AWS:Profile");
            AmazoneS3Constant.AccessKeyId = _appConfiguration.GetValue<string>("AWS:AccessKeyId");
            AmazoneS3Constant.SecretKeyId = _appConfiguration.GetValue<string>("AWS:SecretKeyId");
            AmazoneS3Constant.Region = _appConfiguration.GetValue<string>("AWS:Region");
            AmazoneS3Constant.BucketName = _appConfiguration.GetValue<string>("AWS:BucketName");
            AmazoneS3Constant.Prefix = _appConfiguration.GetValue<string>("AWS:Prefix");
            AmazoneS3Constant.CloudFront = _appConfiguration.GetValue<string>("AWS:CloudFront");
            UploadFileConstant.RootUrl = _appConfiguration.GetValue<string>("App:ServerRootAddress");
            UploadFileConstant.UploadFileProvider = _appConfiguration.GetValue<string>("UploadFile:Provider");
            UploadFileConstant.MaxSizeFile = _appConfiguration.GetValue<long>("UploadFile:MaxSizeFile");
            UploadFileConstant.AvatarFolder = _appConfiguration.GetValue<string>("UploadFile:AvatarFolder");
            var strAllowImageFileType = _appConfiguration.GetValue<string>("UploadFile:AllowImageFileTypes");
            if (string.IsNullOrEmpty(strAllowImageFileType))
            {
                strAllowImageFileType = "jpg,jpeg,png";
            }
            UploadFileConstant.AllowImageFileTypes = strAllowImageFileType.Split(",");
            var strAllowFileType = _appConfiguration.GetValue<string>("UploadFile:AllowFileTypes");
            if (string.IsNullOrEmpty(strAllowFileType))
            {
                strAllowFileType = "xlsx,xltx,docx,pdf";
            }
            UploadFileConstant.AllowFileTypes = strAllowFileType.Split(",");
            UploadFileConstant.FileFolder = _appConfiguration.GetValue<string>("UploadFile:FileFolder");
        }
        
    }
}
