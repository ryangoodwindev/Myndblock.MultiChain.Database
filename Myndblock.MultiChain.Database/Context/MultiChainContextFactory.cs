using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Myndblock.MultiChain.Database
{
    public class MultiChainContextFactory : IDesignTimeDbContextFactory<MultiChainDbContext>
    {
        private const string TestConnectionString = @"Connection string only necessary if we are trying to apply the migration, in this case we are simply using it to generated the migration.";

        public MultiChainDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MultiChainDbContext>();
            optionsBuilder.UseSqlServer(TestConnectionString);

            return new MultiChainDbContext(optionsBuilder.Options);
        }
    }
}