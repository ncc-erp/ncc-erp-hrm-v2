using Abp.Modules;
using HRMv2.Tests;

namespace HRMv2.Core.Tests
{
    [DependsOn(
        typeof(HRMv2TestModule)
        )]
    public class HRMv2CoreTestModule : AbpModule
    {
    }
}
