using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace HRMv2.EntityFrameworkCore
{
    public static class HRMv2DbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<HRMv2DbContext> builder, string connectionString)
        {
            builder.UseNpgsql(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<HRMv2DbContext> builder, DbConnection connection)
        {
            builder.UseNpgsql(connection);
        }
    }
}
