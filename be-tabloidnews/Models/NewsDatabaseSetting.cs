namespace be_tabloidnews.Models
{
    public class NewsDatabaseSetting : INewsDatabaseSettings
    {
        public string NewsConnectionName { get; set; } = String.Empty;
        public string ConnectionString { get; set; } = String.Empty;
        public string DatabaseName { get; set; } = String.Empty;


    }
}
