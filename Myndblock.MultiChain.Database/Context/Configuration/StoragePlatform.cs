namespace Myndblock.MultiChain.Database
{
    /// <summary>
    /// Available and supported options for SQL service providers
    /// </summary>
    public enum StoragePlatform
    {
        /// <summary>
        /// Use this option if a SQL storage device is running behind the application.
        /// </summary>
        SqlServer, 
        
        /// <summary>
        /// Use this option is a SQL Lite storage device is running behind the application.
        /// </summary>
        SqlLite
    }
}