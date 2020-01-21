namespace Myndblock.MultiChain.Database
{
    /// <summary>
    /// MultiChain database configuration properties/settings
    /// </summary>
    public class MultiChainDbOptions
    {
        private MigrationOption _migrationOption;
        private StorageSystem _storageSystem;
        private string _connectionString;

        /// <summary>
        /// MultiChain database configuration properties/settings
        /// </summary>
        public MultiChainDbOptions() { }

        /// <summary>
        /// MultiChain database configuration properties/settings
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        /// <param name="storageSystem">Type of storage system/device (options are SqlServer and SqlLite)</param>
        /// <param name="migrationOption">Handle the MultchainContext migration either manually (Manual) or automatically (Auto)</param>
        public MultiChainDbOptions(string connectionString,
            StorageSystem storageSystem,
            MigrationOption migrationOption)
        {
            ConnectionString = connectionString;
            MigrationOption = migrationOption;
            StorageSystem = storageSystem;
        }

        /// <summary>
        /// Handle the MultchainContext migration either manually (Manual) or automatically (Auto)
        /// </summary>
        public MigrationOption MigrationOption
        {
            get => _migrationOption;
            set => _migrationOption = value;
        }

        /// <summary>
        /// Type of storage system/device (options are SqlServer and SqlLite)
        /// </summary>
        public StorageSystem StorageSystem
        {
            get => _storageSystem;
            set => _storageSystem = value;
        }

        /// <summary>
        /// Database connection string
        /// </summary>
        public string ConnectionString 
        { 
            get => _connectionString; 
            set => _connectionString = value; 
        }
    }
}
