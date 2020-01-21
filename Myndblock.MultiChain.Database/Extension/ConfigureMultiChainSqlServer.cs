using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Myndblock.MultiChain.Database
{
    /// <summary>
    /// Offers extension methods used to incorporate the MultiChainDbContext
    /// into a DI pipeline and/or application service container
    /// </summary>
    public static class MultiChainSqlServerExtension
    {
        /// <summary>
        /// The ConfigureMultiChainDbStorage extension method creates a MultiChainDbContext
        /// that can be used to create a transactin log external to the blockchain node.
        /// Configure the MultiChainDbContext storage mechanisms within the application
        /// <paramref name="serviceCollection"/> DI pipeline using the
        /// <paramref name="multiChainDbOptionsAction"/> parameter.
        /// 
        /// <para>
        ///     The MultiChainDbContext may be injected directly. Or, the provided 
        ///     ITransactionRepo contract may be used instead, which offers CRUD and
        ///     ReadAll functionality along with exposing the MultiChainDbContext
        ///     and ILogging services as public properties, for ease of use.
        /// </para>
        /// 
        /// </summary>
        /// <param name="serviceCollection">Dependency injection pipeline services collection</param>
        /// <param name="multiChainDbOptionsAction">
        ///     Use this parameter to configure the ConnectionString, StorageSystem, and MigrationOption
        ///     properties required to enable the MultiChainDbContext. We support SQL Server and SQL Lite.
        /// </param>
        /// <returns></returns>
        public static IServiceCollection ConfigureMultiChainDbStorage(this IServiceCollection serviceCollection, Action<MultiChainDbOptions> multiChainDbOptionsAction)
        {
            // invoke MultiChainDbOptions
            var dbOptions = new MultiChainDbOptions();
            multiChainDbOptionsAction.Invoke(dbOptions);

            // configure Sql Server or Sql Lite DbContext
            switch (dbOptions.StorageSystem)
            {
                // SQL Server case
                case StorageSystem.SqlServer:
                    serviceCollection.AddDbContext<MultiChainDbContext>(options => options.UseSqlServer(dbOptions.ConnectionString));
                    break;

                // SQL Lite case
                case StorageSystem.SqlLite:
                    serviceCollection.AddDbContext<MultiChainDbContext>(options => options.UseSqlite(dbOptions.ConnectionString));
                    break;
                default:break;
            }

            // detect and handle auto migration of MultiChainDbContext
            if (dbOptions.MigrationOption == MigrationOption.Auto)
            {
                // build an on the fly ServiceProvider
                var provider = serviceCollection.BuildServiceProvider();

                // fetch the MultiChainDbContext
                var context = provider.GetRequiredService<MultiChainDbContext>();

                // We use migrate instead of EnsureCreated 
                // so we can use migration features to ensure the 
                // database has been created and the migration applied.
                context.Database.Migrate();
            }

            // add transaction model repository to service container
            serviceCollection.AddTransient(typeof(ITransactionRepo), typeof(TransactionLog));

            return serviceCollection;
        }
    }
}