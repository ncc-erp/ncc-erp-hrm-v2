using HRMv2.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.EntityFrameworkCore.Seed.Tenants
{
    public class TenantLevelBuilder
    {
        private readonly HRMv2DbContext _context;
        private int? _tenantId;

        public TenantLevelBuilder(HRMv2DbContext context, int? tenantId)
        {
            _context = context;
            _tenantId = tenantId;
        }
        public void Create()
        {
            CreateDefaultLevel();
        }

        public void CreateDefaultLevel()
        {
            var existLevel = _context.Levels.IgnoreQueryFilters()
                .Where(q => q.TenantId == _tenantId)
                .FirstOrDefault();

            if (existLevel != default)
            {
                return;
            }

            var seedDataLevel = new List<Level>{
                new Level
                {
                    Name = "Intern_0",
                    ShortName = "I0",
                    Code = "0",
                    Color = "#B2BEB5",
                    TenantId = _tenantId,
                },
                new Level
                {
                    Name = "Intern_1",
                    ShortName = "I1",
                    Code = "1",
                    Color = "#8F9779",
                    TenantId = _tenantId,
                },
                new Level
                {
                    Name = "Intern_2",
                    ShortName = "I2",
                    Code = "2",
                    Color = "#665D1E",
                    TenantId = _tenantId,
                },
                new Level
                {
                    Name = "Intern_3",
                    ShortName = "I3",
                    Code = "3",
                    Color = "#777",
                    TenantId = _tenantId,
                },
                new Level
                {
                    Name = "Fresher-",
                    ShortName = "F-",
                    Code = "4",
                    Color = "#60b8ff",
                    TenantId = _tenantId,
                },
                new Level
                {
                    Name = "Fresher",
                    ShortName = "F",
                    Code = "5",
                    Color = "#318CE7",
                    TenantId = _tenantId,
                },
                new Level
                {
                    Name = "Fresher+",
                    ShortName = "F+",
                    Code = "6",
                    Color = "#1f75cb",
                    TenantId = _tenantId,
                },
                new Level
                {
                    Name = "Junior-",
                    ShortName = "J-",
                    Code = "7",
                    Color = "#ad9fa1",
                    TenantId = _tenantId,
                },
                new Level
                {
                    Name = "Junior",
                    ShortName = "J",
                    Code = "8",
                    Color = "#A57164",
                    TenantId = _tenantId,
                },
                new Level
                {
                    Name = "Junior+",
                    ShortName = "J+",
                    Code = "9",
                    Color = "#3B2F2F",
                    TenantId = _tenantId,
                },
                new Level
                {
                    Name = "Middle-",
                    ShortName = "M-",
                    Code = "10",
                    Color = "#A4C639",
                    TenantId = _tenantId,
                },
                new Level
                {
                    Name = "Middle",
                    ShortName = "M",
                    Code = "11",
                    Color = "#3bab17",
                    TenantId = _tenantId,
                },
                new Level
                {
                    Name = "Middle+",
                    ShortName = "M+",
                    Code = "12",
                    Color = "#008000",
                    TenantId = _tenantId,
                },
                new Level
                {
                    Name = "Senior-",
                    ShortName = "S-",
                    Code = "13",
                    Color = "#c36285",
                    TenantId = _tenantId,
                },
                new Level
                {
                    Name = "Senior",
                    ShortName = "S",
                    Code = "14",
                    Color = "#AB274F",
                    TenantId = _tenantId,
                },
                new Level
                {
                    Name = "Principal",
                    ShortName = "P",
                    Code = "15",
                    Color = "#902ee1",
                    TenantId = _tenantId,
                },
            };

            _context.AddRange(seedDataLevel);
            _context.SaveChanges();
        }
    }
}
