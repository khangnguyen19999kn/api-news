namespace be_tabloidnews.Models
{
    public class UserDatabaseSetting : IUserDatabaseSettings
    {
        public string UserConnectionName { get; set; } = String.Empty;
        public string ConnectionString { get; set; } = String.Empty;
        public string DatabaseName { get; set; } = String.Empty;
    }
}
