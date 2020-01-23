using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Myndblock.MultiChain.Database
{
    /// <summary>
    /// ITransactionRepo contract defines methods that should be
    /// available to interact with a MultiChain transaction repository
    /// </summary>
    public interface ITransactionRepo
    {
        /// <summary>
        /// Event logging service
        /// </summary>
        ILogger<ITransactionRepo> Logger { get; }

        /// <summary>
        /// Transaction log database context
        /// </summary>
        MultiChainDbContext Context { get; }

        /// <summary>
        /// Create a new TransactionModel log item
        /// </summary>
        /// <param name="model">Target TransactionModel entity to be created in local storage</param>
        /// <returns></returns>
        Task CreateAsync(TransactionModel model);

        /// <summary>
        /// Create a batch of TransactionModels log items
        /// </summary>
        /// <param name="models">Collection of TransactionModels that will be created in local storatge</param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        Task CreateBatchAsync(IList<TransactionModel> models, int batchSize = BatchSize.OneThousand);

        /// <summary>
        /// Read a single TransactioModel log item
        /// </summary>
        /// <param name="id">TransactionModel primary key value</param>
        /// <returns></returns>
        Task<TransactionModel> ReadAsync(Guid? id);

        /// <summary>
        /// Update an existing TransactionModel log item
        /// </summary>
        /// <param name="id">TransactionModel primary key value</param>
        /// <param name="model">Target TransactionModel entity to be updated in local storage</param>
        /// <returns></returns>
        Task UpdateAsync(Guid? id, TransactionModel model);

        /// <summary>
        /// Update a batch of TransactionModels log items
        /// </summary>
        /// <param name="models">Collection of TransactionModels that will be created in local storatge</param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        Task UpdateBatchAsync(IList<TransactionModel> models, int batchSize = BatchSize.OneThousand);

        /// <summary>
        /// Delete an existing TransactionModel log item
        /// </summary>
        /// <param name="id">TransactionModel primary key value</param>
        /// <param name="model">Target TransactionModel entity to be removed from local storage</param>
        /// <returns></returns>
        Task DeleteAsync(Guid? id, TransactionModel model);

        /// <summary>
        /// Delete a batch of TransactionModels log items
        /// </summary>
        /// <param name="models">Collection of TransactionModels that will be created in local storatge</param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        Task DeleteBatchAsync(IList<TransactionModel> models, int batchSize = BatchSize.OneThousand);

        /// <summary>
        /// Read all transaction from the transction log
        /// </summary>
        /// <returns></returns>
        /// <param name="blockchain">(Optional) Filter by target MultiChain blockchain name</param>
        Task<IList<TransactionModel>> ReadAllAsync(string blockchain = "");

        /// <summary>
        /// Built-in pagination support.
        /// </summary>
        /// <param name="blockchain">(Optional) Filter by target MultiChain blockchain name</param>
        /// <param name="start">(Optional) Start index for page result. Default is 0 (zero).</param>
        /// <param name="rows">(Optional) Number of rows to take using <paramref name="start"/> as a reference point. Default is 100.</param>
        /// <returns></returns>
        Task<IList<TransactionModel>> PaginateAsync(string blockchain = "", int start = Pagination.Zero, int rows = Pagination.OneHundred);
    }
}