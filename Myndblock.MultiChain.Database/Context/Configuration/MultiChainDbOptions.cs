namespace Myndblock.MultiChain.Database
{
    /// <summary>
    /// MultiChain database configuration properties/settings
    /// </summary>
    public class MultiChainDbOptions
    {
        private string _connectionString;
        private MigrationMode _migration;
        private StoragePlatform _storage;

        public MultiChainDbOptions() { }

        /// <summary>
        /// Database connection string
        /// </summary>
        public string ConnectionString
        {
            get => _connectionString;
            set => _connectionString = value;
        }

        /// <summary>
        /// Handle the MultchainContext migration either manually (Manual) or automatically (Auto)
        /// </summary>
        public MigrationMode MigrationMode
        {
            get => _migration;
            set => _migration = value;
        }

        /// <summary>
        /// Type of storage system/device (options are SqlServer and SqlLite)
        /// </summary>
        public StoragePlatform StoragePlatform
        {
            get => _storage;
            set => _storage = value;
        }
    }
}