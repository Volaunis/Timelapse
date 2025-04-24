namespace BNF.Timelapse.Repositories.Configuration;

public class MongoDbConfiguration
{
    public required string ConnectionString { get; set; }
    public required string Database { get; set; }
}
