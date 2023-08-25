using HRMv2.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HRMv2.Tests.Seeders
{
    public interface ISeeder
    {
        void Seed(HRMv2DbContext context);
    }
}
