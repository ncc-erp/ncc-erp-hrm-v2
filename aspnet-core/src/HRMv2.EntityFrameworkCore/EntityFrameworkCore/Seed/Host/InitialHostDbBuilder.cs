namespace HRMv2.EntityFrameworkCore.Seed.Host
{
    public class InitialHostDbBuilder
    {
        private readonly HRMv2DbContext _context;

        public InitialHostDbBuilder(HRMv2DbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            new DefaultEditionCreator(_context).Create();
            new DefaultLanguagesCreator(_context).Create();
            new HostRoleAndUserCreator(_context).Create();
            new DefaultSettingsCreator(_context).Create();
            new HostLevelCreator(_context).Create();
            new HostEmailTemplateCreator(_context).Create();

            _context.SaveChanges();
        }
    }
}
