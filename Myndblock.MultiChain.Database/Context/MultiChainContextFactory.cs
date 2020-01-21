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
        public MultiChainContextFactory() { }

        /// <summary>
        /// Create a new isntace of MultiChainDbContext
        /// </summary>
        /// <param name="args">Do not use this, thanks.</param>
        /// <returns></returns>
        public MultiChainDbContext CreateDbContext(string[] args)
        {
            // create new OptionsBuilder instance based on MultiChainDbContext
            var optionsBuilder = new DbContextOptionsBuilder<MultiChainDbContext>();
            optionsBuilder.UseSqlServer("No connection string necessary since we are just using this factory to create migrations.");

            return new MultiChainDbContext(optionsBuilder.Options);
        }
    }
}