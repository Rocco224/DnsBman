using DnsBman.Utilities.Cryptation;
using System.Text;

namespace DnsBman.Utilities
{
    public static class ConfigurationHandler
    {
        private static IConfiguration _configuration;
        private static string key = "Msd34AFpkmKEA!@Ksd";
        private static byte[] keyBytes = Encoding.UTF8.GetBytes(key);

        static ConfigurationHandler()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        public static string GetConnectionString()
        {
            try
            {
                string dataSource = RC4Encryption.Decrypt(Convert.FromBase64String(_configuration.GetConnectionString("DataSource")), keyBytes);
                string initialCatalog = RC4Encryption.Decrypt(Convert.FromBase64String(_configuration.GetConnectionString("InitialCatalog")), keyBytes);
                string username = RC4Encryption.Decrypt(Convert.FromBase64String(_configuration.GetConnectionString("Username")), keyBytes);
                string password = RC4Encryption.Decrypt(Convert.FromBase64String(_configuration.GetConnectionString("Password")), keyBytes);

                return $"Data Source={dataSource};" +
                    $"Initial Catalog={initialCatalog};" +
                    $"Persist Security Info=True;" +
                    $"User ID={username};" +
                    $"Password={password};" +
                    $"MultipleActiveResultSets=True;" +
                    $"Max Pool Size=100;";
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante la lettura della stringa di connessione", ex);
            }
        }

        public static string GetArubaAuthKey()
        {
            try
            {
                return RC4Encryption.Decrypt(Convert.FromBase64String(_configuration.GetSection("ArubaAPI")["AuthorizationKey"]), keyBytes);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante la lettura dell'Authorization Key di Aruba", ex);
            }
        }

        public static string GetArubaCredentials()
        {
            try
            {
                return RC4Encryption.Decrypt(Convert.FromBase64String(_configuration.GetSection("ArubaAPI")["Credentials"]), keyBytes);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante la lettura delle credenziali di Aruba", ex);
            }
        }

        public static string GetApiKey()
        {
            try
            {
                return RC4Encryption.Decrypt(Convert.FromBase64String(_configuration.GetValue<string>("ApiKey")), keyBytes);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante la lettura delle credenziali di Aruba", ex);
            }
        }
    }
}
