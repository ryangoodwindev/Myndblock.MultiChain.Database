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
        MultiChainDbContext Transactions { get; }


        /// <summary>
        /// Create a new TransactionModel log item
        /// </summary>
        /// <param name="model">Target TransactionModel entity to be created in local storage</param>
        /// <returns></returns>
        Task CreateAsync(TransactionModel model);

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
        /// Delete an existing TransactionModel log item
        /// </summary>
        /// <param name="id">TransactionModel primary key value</param>
        /// <param name="model">Target TransactionModel entity to be removed from local storage</param>
        /// <returns></returns>
        Task DeleteAsync(Guid? id, TransactionModel model);


        /// <summary>
        /// Read all transaction from the transction log
        /// </summary>
        /// <returns></returns>
        Task<IList<TransactionModel>> ReadAllAsync();
    }
}