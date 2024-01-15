namespace be_tabloidnews.Models
{
    public interface IUserDatabaseSettings
    {
        string UserConnectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
