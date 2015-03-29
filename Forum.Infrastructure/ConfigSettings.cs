using System.Configuration;

namespace Forum.Infrastructure
{
    public class ConfigSettings
    {
        public static string Part_DBConnectionString { get; set; }
        public static string Forum_DBConnectionString { get; set; }

        public static void Initialize()
        {
            Part_DBConnectionString = ConfigurationManager.ConnectionStrings["Part_DB"].ConnectionString;
            Forum_DBConnectionString = ConfigurationManager.ConnectionStrings["Forum_DB"].ConnectionString;
        }
    }
}
