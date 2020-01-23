using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
        /// <paramref name="configuration"/> parameter.
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
        /// <param name="configuration">IConfiguration pipeline</param>
        /// <returns></returns>
        public static IServiceCollection ConfigureMultiChainDbStorage(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddLogging();

            // configure MultiChainDbOptions
            serviceCollection.Configure<MultiChainDbOptions>(configuration);

            // create temporary service provider to extract a configured instance of MultiChainDbOptions
            var tempProvider = serviceCollection.BuildServiceProvider();
            var dbOptions = tempProvider.GetRequiredService<IOptions<MultiChainDbOptions>>().Value;

            // a connection string is required, so we throw if null/empty
            if (string.IsNullOrEmpty(dbOptions.ConnectionString))
                throw new ArgumentNullException($"{nameof(dbOptions.ConnectionString)} cannot be null or empty.");

            // configure Sql Server or Sqlite DbContext
            switch (dbOptions.StoragePlatform)
            {
                // SQL Server case
                case StoragePlatform.SqlServer:
                    serviceCollection.AddDbContext<MultiChainDbContext>(options => options.UseSqlServer(dbOptions.ConnectionString));
                    break;

                // SQlite case
                case StoragePlatform.SqlLite:
                    serviceCollection.AddDbContext<MultiChainDbContext>(options => options.UseSqlite(dbOptions.ConnectionString));
                    break;
                default:break;
            }

            // detect and handle auto migration of MultiChainDbContext
            if (dbOptions.MigrationMode == MigrationMode.Auto)
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

        /// <summary>
        /// The ConfigureMultiChainDbStorage extension method creates a MultiChainDbContext
        /// that can be used to create a transactin log external to the blockchain node.
        /// Configure the MultiChainDbContext storage mechanisms within the application
        /// <paramref name="serviceCollection"/> DI pipeline using the
        /// <paramref name="configureDbOptions"/> parameter.
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
        /// <param name="configureDbOptions">
        ///     Use this optional parameter to explicitly configure the ConnectionString, StorageSystem, and MigrationOption
        ///     MultiChainDbOptions properties, required to enable the MultiChainDbContext. Otherwise we look
        ///     to the IConfiguration pipeline to configure the MultiChainDbOptions. We support SQL Server and SQlite.
        /// </param>
        /// <returns></returns>
        public static IServiceCollection ConfigureMultiChainDbStorage(this IServiceCollection serviceCollection, Action<MultiChainDbOptions> configureDbOptions)
        {
            serviceCollection.AddLogging();

            // placeholder for db options
            var dbOptions = new MultiChainDbOptions();
            configureDbOptions.Invoke(dbOptions);

            if (string.IsNullOrEmpty(dbOptions.ConnectionString))
                throw new ArgumentNullException($"{nameof(dbOptions.ConnectionString)} cannot be null or empty.");

            // configure Sql Server or Sqllite DbContext
            switch (dbOptions.StoragePlatform)
            {
                // SQL Server case
                case StoragePlatform.SqlServer:
                    serviceCollection.AddDbContext<MultiChainDbContext>(options => options.UseSqlServer(dbOptions.ConnectionString));
                    break;

                // SQlite case
                case StoragePlatform.SqlLite:
                    serviceCollection.AddDbContext<MultiChainDbContext>(options => options.UseSqlite(dbOptions.ConnectionString));
                    break;
                default: break;
            }

            // detect and handle auto migration of MultiChainDbContext
            if (dbOptions.MigrationMode == MigrationMode.Auto)
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