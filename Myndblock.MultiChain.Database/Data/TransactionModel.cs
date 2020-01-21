using Myndblock.MultiChain.Database.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Myndblock.MultiChain.Database
{
    /// <summary>
    /// Record entity used to store a MultiChain transaction
    /// </summary>
    [Table("multichain_transaction_log")]
    public class TransactionModel : BaseModel
    {
        /// <summary>
        /// Record entity used to store a MultiChain transaction
        /// </summary>
        public TransactionModel() { }

        /// <summary>
        /// Record entity used to store a MultiChain transaction
        /// </summary>
        /// <param name="blockchain">Target MultiChain blockchain node name</param>
        /// <param name="targetMethod">Target MultiChain blockchain node method</param>
        public TransactionModel(string blockchain, string targetMethod)
        {
            Blockchain = blockchain;
            TargetMethod = targetMethod;
        }

        /// <summary>
        /// Record entity used to store a MultiChain transaction
        /// </summary>
        /// <param name="blockchain">Target MultiChain blockchain node name</param>
        /// <param name="targetMethod">Target MultiChain blockchain node method</param>
        /// <param name="txid">Transaction id resulting from a successful MultiChain blockchain node transaction or interaction</param>
        public TransactionModel(string blockchain, string targetMethod, string txid) 
            : this(blockchain, targetMethod) => Txid = txid;

        /// <summary>
        /// Target blockchain node
        /// </summary>
        public string Blockchain { get; set; }

        /// <summary>
        /// MultiChain transaction id value
        /// </summary>
        public string Txid { get; set; }

        /// <summary>
        /// MultiChain blockchain method called for this transaction
        /// </summary>
        public string TargetMethod { get; set; }

        /// <summary>
        /// MultiChain error message
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Determines if the transaction is or was successful
        /// </summary>
        /// <returns></returns>
        public bool IsSuccess() => string.IsNullOrEmpty(ErrorMessage) && !string.IsNullOrEmpty(Txid);
    }
}