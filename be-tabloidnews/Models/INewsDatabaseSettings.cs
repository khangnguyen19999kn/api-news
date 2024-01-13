namespace be_tabloidnews.Models
{
    public interface INewsDatabaseSettings
    {
        string NewsConnectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
