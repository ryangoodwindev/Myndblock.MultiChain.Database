using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Myndblock.MultiChain.Database.Tests
{
    public class MultiChainDbContextIConfigurationTest
    {
        private ITransactionRepo _contract;
        private const string MY_BLOCKCHAIN = "MyBlockchain";
        private const string ISSUE_MORE = "issuemore";

        /// <summary>
        /// Setup method will mimic an application dependency pipeline
        /// </summary>
        [SetUp]
        public void Setup()
        {
            // Stage - build our test configuration pipeline
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            
            // Stage - build configuration 
            IConfiguration configuration = builder.Build();

            // Stage - start service collection
            ServiceCollection services = new ServiceCollection();

            // Act - add the ConfigureMultiChainDbStorage services to the DI pipeline
            //       supports MultiChainDbContext local storage
            services.ConfigureMultiChainDbStorage(configuration);
            var provider = services.BuildServiceProvider();

            // Act - fetch required service from temp provider
            //       ITransactionRepo offers basic List and CRUD functions
            //       ITransactionRepo also offers direct access to the MultiChainDbContext and ILogger services, for ease of use
            _contract = provider.GetRequiredService<ITransactionRepo>();
        }

        /// <summary>
        /// Validate that our services are in order and available as expected
        /// </summary>
        [Test, Order(1)]
        public void EnsureServicesAreAvailable()
        {
            // Assert - ensure the Transactions context is not null and is an instance of MultiChainDbContext
            Assert.IsNotNull(_contract.Context);
            Assert.IsInstanceOf<MultiChainDbContext>(_contract.Context);

            // Assert - ensure the ILogger service is not null and is an instance of ILogger<ITransactionRepo>
            Assert.IsNotNull(_contract.Logger);
            Assert.IsInstanceOf<ILogger<ITransactionRepo>>(_contract.Logger);
        }

        /// <summary>
        /// Test will commit a single TransactionModel to local storage
        /// </summary>
        /// <returns></returns>
        [Test, Order(2)]
        public Task CreateAsyncTest()
        {
            // create one record
            var single = new TransactionModel(MY_BLOCKCHAIN, ISSUE_MORE, $"{Guid.NewGuid()}")
            {
                CreatedBy = nameof(CreateAsyncTest),
                LastModifiedBy = nameof(CreateAsyncTest)
            };

            /* .SaveChangesAsync() is called automatically within the contract */
            /* We don't need to call .SaveChangesAsync() again here for any reason */

            // Act & Assert commit to local storage
            Assert.DoesNotThrowAsync(async () => await _contract.CreateAsync(single));

            return Task.CompletedTask;
        }

        /// <summary>
        /// Test will commit a collection of TransactionModels to local storage
        /// </summary>
        /// <returns></returns>
        [Test, Order(3)]
        public Task CreateBatchAsyncTest()
        {
            //  Stage - number of for loops to complete & collectin of TransactionModels
            var rounds = 10000;
            var collection = new List<TransactionModel>(); 
            for (int i = 0; i < rounds; i++)
            {
                // instantiate a new TransactionModel
                var transaction = new TransactionModel(MY_BLOCKCHAIN, ISSUE_MORE, $"{Guid.NewGuid()}")
                {
                    CreatedBy = nameof(CreateAsyncTest),
                    LastModifiedBy = nameof(CreateAsyncTest)
                };
                collection.Add(transaction);
            }

            /* .SaveChangesAsync() is called automatically within the contract */
            /* We don't need to call .SaveChangesAsync() again here for any reason */

            // Act & Assert - create a collection of TransactionModels in local storage
            //                optional parameter of batchSize defaults to 1000
            //                e.g. Pass 20,000 records to the method but only commit 1000 at a time
            Assert.DoesNotThrowAsync(async () => await _contract.CreateBatchAsync(collection, BatchSize.OneThousand));

            return Task.CompletedTask;
        }

        /// <summary>
        /// Test will read a single TransactionModel from local storage
        /// </summary>
        /// <returns></returns>
        [Test, Order(4)]
        public async Task ReadAsyncTest()
        {
            // Stage - read first record available from local storage
            var transaction = await _contract.Context.Transactions.FirstOrDefaultAsync();

            /* .SaveChangesAsync() is called automatically within the contract */
            /* We don't need to call .SaveChangesAsync() again here for any reason */

            // Act - read record using actual ReadAsync() method
            transaction = await _contract.ReadAsync(transaction.Id);

            // Assert
            Assert.IsNotNull(transaction);
            Assert.IsInstanceOf<TransactionModel>(transaction);

            // Assert
            Assert.IsTrue(transaction.IsSuccess());
            Assert.IsNull(transaction.ErrorMessage);

            // Assert
            Assert.IsNotEmpty(transaction.Blockchain);
            Assert.IsNotEmpty(transaction.TargetMethod);
            Assert.IsNotEmpty(transaction.Txid);
            Assert.IsNotEmpty(transaction.CreatedBy);
            Assert.IsNotEmpty(transaction.LastModifiedBy);

            // Assert
            Assert.IsInstanceOf<Guid?>(transaction.Id);
            Assert.IsInstanceOf<DateTime>(transaction.CreatedOn);
            Assert.IsInstanceOf<DateTime>(transaction.LastModifiedOn);
            Assert.IsInstanceOf<byte[]>(transaction.RowVersion);

        }

        /// <summary>
        /// Test will update a single TransactionModel already present in local storage
        /// </summary>
        /// <returns></returns>
        [Test, Order(5)]
        public async Task UpdateAsyncTest()
        {
            // Stage - read first record available from local storage
            var transaction = await _contract.Context.Transactions.FirstOrDefaultAsync();

            // Stage - update record
            transaction.Blockchain = "SomeOtherBlockchain";
            transaction.LastModifiedBy = nameof(UpdateAsyncTest);
            transaction.LastModifiedOn = DateTime.UtcNow;

            /* .SaveChangesAsync() is called automatically within the contract */
            /* We don't need to call .SaveChangesAsync() again here for any reason */

            // Act & Assert - update a single TransactionModel alredy present in local storage
            Assert.DoesNotThrowAsync(async () => await _contract.UpdateAsync(transaction.Id, transaction));
        }

        /// <summary>
        /// Test will update a collection of TransactionModels already present in local storage
        /// </summary>
        /// <returns></returns>
        [Test, Order(6)]
        public async Task UpdateBatchAsyncTest()
        {
            // Stage - fetch the first 100 TransactionModels from local storage
            var batch = await _contract.PaginateAsync();

            // Stage - update each record in batch
            foreach (var item in batch)
            {
                item.Txid = $"{Guid.NewGuid()}";
                item.LastModifiedBy = nameof(UpdateAsyncTest);
                item.LastModifiedOn = DateTime.UtcNow;
            }

            /* .SaveChangesAsync() is called automatically within the contract */
            /* We don't need to call .SaveChangesAsync() again here for any reason */

            // Act & Assert - update a collection of TransactionModels alredy present in local storage
            Assert.DoesNotThrowAsync(async () => await _contract.UpdateBatchAsync(batch, BatchSize.OneThousand));
        }

        /// <summary>
        /// Test will delete a single TransactionModel from local storage
        /// </summary>
        /// <returns></returns>
        [Test, Order(7)]
        public async Task DeleteAsyncTest()
        {
            // Stage - read first record available from local storage
            var transaction = await _contract.Context.Transactions.FirstOrDefaultAsync();

            /* .SaveChangesAsync() is called automatically within the contract */
            /* We don't need to call .SaveChangesAsync() again here for any reason */

            // Act & Assert - delete a single TransactionModel from local storage
            Assert.DoesNotThrowAsync(async() => await _contract.DeleteAsync(transaction.Id, transaction));
        }

        /// <summary>
        /// Test will delete a collection of TransactionModels from local storage
        /// </summary>
        /// <returns></returns>
        [Test, Order(8)]
        public async Task DeleteBatchAsyncTest()
        {
            // Stage - fetch the first 100 TransactionModels from local storage
            var batch = await _contract.PaginateAsync();

            /* .SaveChangesAsync() is called automatically within the contract */
            /* We don't need to call .SaveChangesAsync() again here for any reason */

            // Act & Assert - delete a collection of TransactionModels from local storage
            Assert.DoesNotThrowAsync(async () => await _contract.DeleteBatchAsync(batch, BatchSize.OneThousand));
        }

        /// <summary>
        /// Test will read all TransactionModels from local storage
        /// Test method CreateAsyncTest must be ran before this method
        /// </summary>
        /// <returns></returns>
        [Test, Order(9)]
        public async Task ReadAllAsyncTest()
        {
            // Act & Assert - read all records
            var gen = await _contract.ReadAllAsync();
            Assert.Greater(gen.Count, 0);

            // Act & Assert - read all records for a specific blockchain name
            var exp = await _contract.ReadAllAsync(MY_BLOCKCHAIN);
            Assert.Greater(exp.Count, 0);
        }

        /// <summary>
        /// Test will read pages (batches) of TransactionModels from local storage
        /// Test method CreateAsyncTest must be ran before this method
        /// </summary>
        /// <returns></returns>
        [Test, Order(10)]
        public async Task PaginateAsyncTest()
        {
            // Act & Assert - default method calls first records 0 through 100 with no filters applied
            var page = await _contract.PaginateAsync();
            Assert.LessOrEqual(page.Count, 100);

            // Act & Assert - passing a blockchain name into the method calls records 0 through 99 for the target node
            var specificPage = await _contract.PaginateAsync(MY_BLOCKCHAIN);
            Assert.LessOrEqual(specificPage.Count, 100);

            // Act & Assert - passing a blockchain name into the method calls records 100 through 199 for the target node
            var pageTwo = await _contract.PaginateAsync(MY_BLOCKCHAIN, start: 100, rows: 100);
            Assert.LessOrEqual(pageTwo.Count, 100);

            // Act & Assert - passing a blockchain name into the method calls records 200 through 299 for the target node
            var pageThree = await _contract.PaginateAsync(MY_BLOCKCHAIN, start: 200, rows: 100);
            Assert.LessOrEqual(pageThree.Count, 100);
        }
    }
}