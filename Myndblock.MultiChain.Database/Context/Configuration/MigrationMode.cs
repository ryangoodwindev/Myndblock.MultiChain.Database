namespace Myndblock.MultiChain.Database
{
    /// <summary>
    /// Available and supported migration options for SQL service providers
    /// </summary>
    public enum MigrationMode
    {
        /// <summary>
        /// Use this option if the consumer will trigger the "Add-Migration" feature
        /// manually. This option WILL NOT create the database or apply any pending
        /// migration(s) to local storage automatically.
        /// </summary>
        Manual,

        /// <summary>
        /// Use this option if the application will trigger the .Migrate() feature
        /// automatically each time the application starts. This option WILL create 
        /// the database and apply any pending migration(s) to local storage.
        /// </summary>
        Auto
    }
}
