using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Myndblock.MultiChain.Database
{
    public class TransactionLog : ITransactionRepo
    {
        // local service contracts
        private readonly ILogger<ITransactionRepo> _logger;
        private readonly MultiChainDbContext _transactions;

        /// <summary>
        /// Transaction log implements ITransactionRepo
        /// </summary>
        /// <param name="transactions"></param>
        /// <param name="logger"></param>
        public TransactionLog(MultiChainDbContext transactions, 
            ILogger<ITransactionRepo> logger)
        {
            _transactions = transactions;
            _logger = logger;
        }

        /// <summary>
        /// Transaction log database context
        /// </summary>
        public MultiChainDbContext Transactions { get => _transactions; }

        /// <summary>
        /// Event logging service
        /// </summary>
        public ILogger<ITransactionRepo> Logger { get => _logger; }


        /// <summary>
        /// Create a new TransactionModel log item
        /// </summary>
        /// <param name="model">Target TransactionModel entity to be created in local storage</param>
        /// <returns></returns>
        public Task CreateAsync(TransactionModel model)
        {
            _transactions.Entry(model).State = EntityState.Added;
            _logger.LogInformation("", $"Create action requested for MultiChain node transaction {model.Id}");
            return _transactions.SaveChangesAsync();
        }

        /// <summary>
        /// Read a single TransactioModel log item
        /// </summary>
        /// <param name="id">TransactionModel primary key value</param>
        /// <returns></returns>
        public Task<TransactionModel> ReadAsync(Guid? id)
        {
            _logger.LogInformation("", $"Read action requested for MultiChain node transaction {id}");
            return _transactions.Transactions.FirstOrDefaultAsync(first => first.Id == id);
        }

        /// <summary>
        /// Update an existing TransactionModel log item
        /// </summary>
        /// <param name="id">TransactionModel primary key value</param>
        /// <param name="model">Target TransactionModel entity to be updated in local storage</param>
        /// <returns></returns>
        public Task UpdateAsync(Guid? id, TransactionModel model)
        {
            if (id != model.Id)
                throw EntityIdMismatchException();

            _transactions.Entry(model).State = EntityState.Modified;
            _logger.LogInformation("", $"Update action requested for MultiChain node transaction {id}");
            return _transactions.SaveChangesAsync();
        }

        /// <summary>
        /// Delete an existing TransactionModel log item
        /// </summary>
        /// <param name="id">TransactionModel primary key value</param>
        /// <param name="model">Target TransactionModel entity to be removed from local storage</param>
        /// <returns></returns>
        public Task DeleteAsync(Guid? id, TransactionModel model)
        {
            if (id != model.Id)
                throw EntityIdMismatchException();

            _transactions.Entry(model).State = EntityState.Deleted;
            _logger.LogInformation("", $"Delete action requested for MultiChain node transaction {id}");
            return _transactions.SaveChangesAsync();
        }


        /// <summary>
        /// Read all transaction from the transction log
        /// </summary>
        /// <returns></returns>
        public async Task<IList<TransactionModel>> ReadAllAsync()
        {
            _logger.LogInformation("", $"All MultiChain node transactions requested");
            return await _transactions.Transactions.ToListAsync();
        }

        /// <summary>
        /// Log and throw ArgumentException
        /// </summary>
        /// <param name="errorMessage">Error message to be thrown with the new ArgumentException</param>
        /// <returns></returns>
        private ArgumentException EntityIdMismatchException(string errorMessage = "The id and entity id do not match.")
        {
            _logger.LogError("", errorMessage);
            throw new ArgumentException(errorMessage);
        }
    }
}