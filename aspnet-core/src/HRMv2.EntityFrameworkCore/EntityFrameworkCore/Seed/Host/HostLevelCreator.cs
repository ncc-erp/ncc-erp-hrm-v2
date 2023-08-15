using HRMv2.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.EntityFrameworkCore.Seed.Host
{
    public class HostLevelCreator
    {
        private readonly HRMv2DbContext _context;

        public HostLevelCreator(HRMv2DbContext context)
        {
            _context = context;
        }
        public void Create()
        {
            CreateDefaultLevel();
        }

        public void CreateDefaultLevel()
        {
            var seedDataLevel = new List<Level>{
                new Level
                {
                    Name = "Intern_0",
                    ShortName = "I0",
                    Code = "0",
                    Color = "#B2BEB5",
                },
                new Level
                {
                    Name = "Intern_1",
                    ShortName = "I1",
                    Code = "1",
                    Color = "#8F9779",
                },
                new Level
                {
                    Name = "Intern_2",
                    ShortName = "I2",
                    Code = "2",
                    Color = "#665D1E",
                },
                new Level
                {
                    Name = "Intern_3",
                    ShortName = "I3",
                    Code = "3",
                    Color = "#777",
                },
                new Level
                {
                    Name = "Fresher-",
                    ShortName = "F-",
                    Code = "4",
                    Color = "#60b8ff",
                },
                new Level
                {
                    Name = "Fresher",
                    ShortName = "F",
                    Code = "5",
                    Color = "#318CE7",
                },
                new Level
                {
                    Name = "Fresher+",
                    ShortName = "F+",
                    Code = "6",
                    Color = "#1f75cb",
                },
                new Level
                {
                    Name = "Junior-",
                    ShortName = "J-",
                    Code = "7",
                    Color = "#ad9fa1",
                },
                new Level
                {
                    Name = "Junior",
                    ShortName = "J",
                    Code = "8",
                    Color = "#A57164",
                },
                new Level
                {
                    Name = "Junior+",
                    ShortName = "J+",
                    Code = "9",
                    Color = "#3B2F2F",
                },
                new Level
                {
                    Name = "Middle-",
                    ShortName = "M-",
                    Code = "10",
                    Color = "#A4C639",
                },
                new Level
                {
                    Name = "Middle",
                    ShortName = "M",
                    Code = "11",
                    Color = "#3bab17",
                },
                new Level
                {
                    Name = "Middle+",
                    ShortName = "M+",
                    Code = "12",
                    Color = "#008000",
                },
                new Level
                {
                    Name = "Senior-",
                    ShortName = "S-",
                    Code = "13",
                    Color = "#c36285",
                },
                new Level
                {
                    Name = "Senior",
                    ShortName = "S",
                    Code = "14",
                    Color = "#AB274F",
                },
                new Level
                {
                    Name = "Principal",
                    ShortName = "P",
                    Code = "15",
                    Color = "#902ee1",
                },
            };

            var existLevel = _context.Levels.IgnoreQueryFilters()
                .Where(x => x.TenantId == null)
                .FirstOrDefault();

            if (existLevel != default)
            {
                return;
            }

            _context.AddRange(seedDataLevel);
            _context.SaveChanges();
        }
    }
}
