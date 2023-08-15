using Abp.Modules;
using HRMv2.Core.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Application.Tests
{
    [DependsOn(
        typeof(HRMv2CoreTestModule)
        )]
    public class HRMv2ApplicationTestModule : AbpModule
    {
    }
}
