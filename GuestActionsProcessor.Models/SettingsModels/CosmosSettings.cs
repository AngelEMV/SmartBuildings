namespace GuestActionsProcessor.Models.SettingsModels
{
    public class CosmosSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string ContainerName { get; set; }
    }
}