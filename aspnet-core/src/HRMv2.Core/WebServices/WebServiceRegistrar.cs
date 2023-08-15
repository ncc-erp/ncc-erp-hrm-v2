using HRMv2.WebServices.Finfast;
using HRMv2.WebServices.IMS;
using HRMv2.WebServices.Komu;
using HRMv2.WebServices.Project;
using HRMv2.WebServices.Timesheet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.WebServices
{
    public static class WebServiceRegistrar
    {
        public static IServiceCollection AddWebServices(this IServiceCollection services, IConfigurationRoot _appConfiguration)
        {
            services.AddHttpClient<TimesheetWebService>(option =>
            {
                option.BaseAddress = new Uri(_appConfiguration.GetValue<string>("TimesheetService:BaseAddress"));
                option.DefaultRequestHeaders.Add("X-Secret-Key", _appConfiguration.GetValue<string>("TimesheetService:SecurityCode"));
            });

            services.AddHttpClient<ProjectService>(option =>
            {
                option.BaseAddress = new Uri(_appConfiguration.GetValue<string>("ProjectService:BaseAddress"));
                option.DefaultRequestHeaders.Add("X-Secret-Key", _appConfiguration.GetValue<string>("ProjectService:SecurityCode"));
            });

            services.AddHttpClient<Talent.TalentWebService>(option =>
            {
                option.BaseAddress = new Uri(_appConfiguration.GetValue<string>("TalentService:BaseAddress"));
                option.DefaultRequestHeaders.Add("X-Secret-Key", _appConfiguration.GetValue<string>("TalentService:SecurityCode"));
            });

            services.AddHttpClient<IMSWebService>(option =>
            {
                option.BaseAddress = new Uri(_appConfiguration.GetValue<string>("IMSService:BaseAddress"));
                option.DefaultRequestHeaders.Add("X-Secret-Key", _appConfiguration.GetValue<string>("IMSService:SecurityCode"));
            });

            services.AddHttpClient<FinfastWebService>(option =>
            {
                option.BaseAddress = new Uri(_appConfiguration.GetValue<string>("FinfastService:BaseAddress"));
                option.DefaultRequestHeaders.Add("X-Secret-Key", _appConfiguration.GetValue<string>("FinfastService:SecurityCode"));
            });

            services.AddHttpClient<KomuService>(option =>
            {
                option.BaseAddress = new Uri(_appConfiguration.GetValue<string>("KomuService:BaseAddress", ""));
                option.DefaultRequestHeaders.Add("X-Secret-Key", _appConfiguration.GetValue<string>("KomuService:SecurityCode", ""));
            });


            return services;
        }
    }
}
