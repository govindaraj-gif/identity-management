namespace Auth.Service
{
    public class AppSettings
    {
        public Logging Logging { get; set; } = new Logging();
        public string AllowedHosts { get; set; } = string.Empty;
        public ConnectionStrings ConnectionStrings { get; set; } = new ConnectionStrings();
        public JWT JWT { get; set; } = new JWT();
    }

    public class ConnectionStrings
    {
        public string DefaultConnection { get; set; } = string.Empty;
    }

    public class JWT
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string SigningKey { get; set; } = string.Empty;
    }

    public class Logging
    {
        public LogLevel LogLevel { get; set; } = new LogLevel();
    }
}
