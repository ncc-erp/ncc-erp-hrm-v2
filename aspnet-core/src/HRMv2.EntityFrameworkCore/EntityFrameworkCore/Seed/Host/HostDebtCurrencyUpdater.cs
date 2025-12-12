using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.EntityFrameworkCore.Seed.Host
{
    public class HostDebtCurrencyUpdater
    {
        private readonly HRMv2DbContext _context;
        public HostDebtCurrencyUpdater(HRMv2DbContext context)
        {
            _context = context;
        }

        public void Update()
        {
            var debts = _context.Debts
                .Where(d => d.Currency == null)
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
