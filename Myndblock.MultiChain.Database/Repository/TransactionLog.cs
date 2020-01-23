using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Myndblock.MultiChain.Database
{
    public class TransactionLog : ITransactionRepo
    {
        /// <summary>
        /// Transaction log implements ITransactionRepo
        /// </summary>
        /// <param name="transactions"></param>
        /// <param name="logger"></param>
        public TransactionLog(MultiChainDbContext transactions,
            ILogger<ITransactionRepo> logger)
        {
            _context = transactions;
            _logger = logger;
        }

        /// <summary>
        /// Transaction log database context
        /// </summary>
        public MultiChainDbContext Context { get => _context; }
        private readonly MultiChainDbContext _context;

        /// <summary>
        /// Event logging service
        /// </summary>
        public ILogger<ITransactionRepo> Logger { get => _logger; }
        private readonly ILogger<ITransactionRepo> _logger;

        /// <summary>
        /// Create a new TransactionModel log item
        /// </summary>
        /// <param name="model">Target TransactionModel entity to be created in local storage</param>
        /// <returns></returns>
        public Task CreateAsync(TransactionModel model)
        {
            _context.Entry(model).State = EntityState.Added;
            _logger.LogInformation("", $"Create action requested for MultiChain node {model.Blockchain} with transaction {model.Id}");
            return _context.SaveChangesAsync();
        }

        /// <summary>
        /// Create a batch of TransactionModels log items
        /// </summary>
        /// <param name="models">Collection of TransactionModels that will be created in local storatge</param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        public async Task CreateBatchAsync(IList<TransactionModel> models, int batchSize = BatchSize.OneThousand)
        {
            var pages = GetPages(models.Count, batchSize);

            for (int i = 0; i < pages; i++)
            {
                var batch = models.Skip(i * batchSize).Take(batchSize);
                _context.AddRange(batch);
                await _context.SaveChangesAsync();
            }

            _logger.LogInformation("", $"{models.Count} MultiChain node transactions commited to the transaction log.");
        }

        /// <summary>
        /// Read a single TransactioModel log item
        /// </summary>
        /// <param name="id">TransactionModel primary key value</param>
        /// <returns></returns>
        public Task<TransactionModel> ReadAsync(Guid? id)
        {
            _logger.LogInformation("", $"Read action requested for MultiChain node with transaction {id}");
            return _context.Transactions.FirstOrDefaultAsync(first => first.Id == id);
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

            _context.Entry(model).State = EntityState.Modified;
            _logger.LogInformation("", $"Update action requested for MultiChain node {model.Blockchain} with transaction {id}");
            return _context.SaveChangesAsync();
        }

        /// <summary>
        /// Update a batch of TransactionModels log items
        /// </summary>
        /// <param name="models">Collection of TransactionModels that will be created in local storatge</param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        public async Task UpdateBatchAsync(IList<TransactionModel> models, int batchSize = BatchSize.OneThousand)
        {
            var pages = GetPages(models.Count, batchSize);

            for (int i = 0; i < pages; i++)
            {
                var batch = models.Skip(i * batchSize).Take(batchSize);
                _context.UpdateRange(batch);
                await _context.SaveChangesAsync();
            }

            _logger.LogInformation("", $"{models.Count} MultiChain node transactions updated on the transaction log.");
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

            _context.Entry(model).State = EntityState.Deleted;
            _logger.LogInformation("", $"Delete action requested for MultiChain node {model.Blockchain} with transaction {id}");
            return _context.SaveChangesAsync();
        }

        /// <summary>
        /// Delete a batch of TransactionModels log items
        /// </summary>
        /// <param name="models">Collection of TransactionModels that will be created in local storatge</param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        public async Task DeleteBatchAsync(IList<TransactionModel> models, int batchSize = BatchSize.OneThousand)
        {
            var pages = GetPages(models.Count, batchSize);

            for (int i = 0; i < pages; i++)
            {
                var batch = models.Skip(i * batchSize).Take(batchSize);
                _context.RemoveRange(batch);
                await _context.SaveChangesAsync();
            }

            _logger.LogInformation("", $"{models.Count} MultiChain node transactions removed from the transaction log.");
        }

        /// <summary>
        /// Read all transaction from the transction log
        /// </summary>
        /// <param name="blockchain">Filter transaction by blockchain name</param>
        /// <returns></returns>
        public async Task<IList<TransactionModel>> ReadAllAsync(string blockchain = "")
        {
            _logger.LogInformation("", $"All MultiChain node transactions requested");
            if (string.IsNullOrEmpty(blockchain))
                return await _context.Transactions.ToListAsync();
            else
                return await _context.Transactions.Where(where => where.Blockchain == blockchain).ToListAsync();
        }

        /// <summary>
        /// Built-in pagination support. Default values will pull the first 100 records for all blockchain names.
        /// </summary>
        /// <param name="blockchain">(Optional) Filter by target MultiChain blockchain name</param>
        /// <param name="start">(Optional) Start index for page result. Default is 0 (zero).</param>
        /// <param name="rows">(Optional) Number of rows to take using <paramref name="start"/> as a reference point. Default is 100.</param>
        public async Task<IList<TransactionModel>> PaginateAsync(string blockchain = "", int start = Pagination.Zero, int rows = Pagination.OneHundred)
        {
            _logger.LogInformation("", $"A page of MultiChain transactions have been requested from {blockchain}, {rows} rows pulled starting at index {start}");
            if (string.IsNullOrEmpty(blockchain))
                return await _context.Transactions.Skip(start).Take(rows).ToListAsync();
            else
                return await _context.Transactions.Skip(start).Take(rows).Where(where => where.Blockchain == blockchain).ToListAsync();
        }

        /// <summary>
        /// Batch size helper
        /// </summary>
        /// <param name="count">How many total records that will be divided by the <paramref name="batchSize"/> to give us a page count</param>
        /// <param name="batchSize">Quantity of records to commit at once</param>
        /// <returns></returns>
        private static int GetPages(int count, int batchSize)
        {
            var pages = Math.DivRem(count, batchSize, out int remainder);
            return remainder > 0 ? pages + 1 : pages;
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