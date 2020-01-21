using System;

namespace Myndblock.MultiChain.Database.Data
{
    /// <summary>
    /// Any data model class in Myndblock.MultiChain.Database should inherit this base class,
    /// so as to inherit the necessary backing fields
    /// </summary>
    public abstract class BaseModel
    {
        /// <summary>
        /// Primary key for this MultiChain transaction
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// Date and time this record was created.
        /// Should alwyas be UTC time to handle consumer locales correctly.
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Date and time this record was last modified
        /// </summary>
        public DateTime LastModifiedOn { get; set; }

        /// <summary>
        /// Who or what created this record.
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Who or what modified this record last.
        /// </summary>
        public string LastModifiedBy { get; set; }

        /// <summary>
        /// Concurrency token
        /// </summary>
        public byte[] RowVersion { get; set; }
    }
}