using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Myndblock.MultiChain.Database.Tests
{
    public class MultiChainDbContextIConfigurationTest
    {
        private ITransactionRepo _contract;

        /// <summary>
        /// Setup method will mimic an application dependency pipeline
        /// </summary>
        [SetUp]
        public void Setup()
        {
            // build our test configuration pipeline
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            IConfiguration configuration = builder.Build();

            // start service collection
            ServiceCollection services = new ServiceCollection();

            // add the ConfigureMultiChainDbStorage services to the DI pipeline
            // supports MultiChainDbContext local storage
            services.ConfigureMultiChainDbStorage(configuration);
            var provider = services.BuildServiceProvider();

            // fetch required service from temp provider
            // this repo offers basic List and CRUD functions
            // also offers direct access to the MultiChainDbContext and ILogger services, for ease of use
            _contract = provider.GetRequiredService<ITransactionRepo>();
        }

        [Test]
        public void EnsureServicesAreAvailable()
        {
            // ensure the Transactions context is not null and is an instance of MultiChainDbContext
            Assert.IsNotNull(_contract.Transactions);
            Assert.IsInstanceOf<MultiChainDbContext>(_contract.Transactions);

            // ensure the ILogger service is not null and is an instance of ILogger<ITransactionRepo>
            Assert.IsNotNull(_contract.Logger);
            Assert.IsInstanceOf<ILogger<ITransactionRepo>>(_contract.Logger);
        }

        [Test]
        public async Task CreateAsyncTest()
        {
            var rounds = 40;
            for (int i = 0; i < rounds; i++)
            {
                var transaction = new TransactionModel("MyBlockchain", "issuemore", "some txid we could store")
                {
                    CreatedBy = nameof(CreateAsyncTest),
                    LastModifiedBy = nameof(CreateAsyncTest)
                };

                await _contract.CreateAsync(transaction);
            }

            Assert.Pass();
        }

        [Test]
        public async Task ReadAsyncTest()
        {

        }

        [Test]
        public async Task UpdateAsyncTest()
        {

        }

        [Test]
        public async Task DeleteAsyncTest()
        {

        }

        [Test]
        public async Task ReadAllAsyncTest()
        {

        }
    }
}