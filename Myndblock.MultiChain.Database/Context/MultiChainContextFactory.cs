using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Myndblock.MultiChain.Database
{
    /// <summary>
    /// Do not use this factory directly, it is only used to create a migration (Add-Migration)
    /// during design-time in the absence of an actual Dependency Injection Pipeline container.
    /// </summary>
    public sealed class MultiChainContextFactory : IDesignTimeDbContextFactory<MultiChainDbContext>
    {
        /// <summary>
        /// Dummy ConnectionString value since we just need to use this for scaffolding a migration during design-time
        /// </summary>
        private const string TestConnectionString = @"Connection string only necessary if we are trying to apply the migration, in this case we are simply using it to generated the migration at design-time.";

        /// <summary>
        /// Create a new isntace of MultiChainDbContext
        /// </summary>
        /// <param name="args">Do not use this, thanks.</param>
        /// <returns></returns>
        public MultiChainDbContext CreateDbContext(string[] args)
        {
            // create new OptionsBuilder instance based on MultiChainDbContext
            var optionsBuilder = new DbContextOptionsBuilder<MultiChainDbContext>();
            optionsBuilder.UseSqlServer(TestConnectionString); // use SqlServer connection type

            return new MultiChainDbContext(optionsBuilder.Options);
        }
    }
}