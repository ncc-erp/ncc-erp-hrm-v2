using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.EntityFrameworkCore.Seed.Tenants
{
    public class DebtCurrencyFixed
    {
        private readonly HRMv2DbContext _context;
        private readonly int _tenantId;
        public DebtCurrencyFixed(HRMv2DbContext context, int tenantId)
        {
            _context = context;
            _tenantId = tenantId;
        }

        public void Fixed()
        {
            var debts = _context.Debts
                .Where(d => d.TenantId == _tenantId && d.Currency == null)
                .ToList();

            if (!debts.Any())
                return;

            foreach (var debt in debts)
            {
                debt.Currency = "VND";
            }    

            _context.SaveChanges();
        }
    }
}
