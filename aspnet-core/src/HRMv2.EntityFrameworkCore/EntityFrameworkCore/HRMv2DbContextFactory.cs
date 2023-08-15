using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using HRMv2.Configuration;
using HRMv2.Web;

namespace HRMv2.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class HRMv2DbContextFactory : IDesignTimeDbContextFactory<HRMv2DbContext>
    {
        public HRMv2DbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<HRMv2DbContext>();
            
            /*
             You can provide an environmentName parameter to the AppConfigurations.Get method. 
             In this case, AppConfigurations will try to read appsettings.{environmentName}.json.
             Use Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") method or from string[] args to get environment if necessary.
             https://docs.microsoft.com/en-us/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli#args
             */
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            HRMv2DbContextConfigurer.Configure(builder, configuration.GetConnectionString(HRMv2Consts.ConnectionStringName));

            return new HRMv2DbContext(builder.Options);
        }
    }
}
