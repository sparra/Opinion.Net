namespace WorkerRoleCommandProcessor
{
    using System.Data.Entity;
    using Opinion.Infrastructure.Common.Entity;
    using Opinion.Infrastructure.Sql.BlobStorage;
    using Opinion.Infrastructure.Sql.EventSourcing;
    using Opinion.Infrastructure.Sql.MessageLog;
    
    /// <summary>
    /// Initializes the EF infrastructure.
    /// </summary>
    internal static class DatabaseSetup
    {
        public static void Initialize()
        {
            Database.DefaultConnectionFactory = new ServiceConfigurationSettingConnectionFactory(Database.DefaultConnectionFactory);
            Database.SetInitializer<EventStoreDbContext>(null);
            Database.SetInitializer<MessageLogDbContext>(null);
            Database.SetInitializer<BlobStorageDbContext>(null);
        }
    }
}
