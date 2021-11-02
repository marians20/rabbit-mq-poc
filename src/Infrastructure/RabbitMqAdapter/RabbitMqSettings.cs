namespace RabbitMqAdapter
{
    public sealed class RabbitMqSettings
    {
        public RabbitMqSettings() { }
        public RabbitMqSettings(string hostName, string userName, string password, string virtualHost)
        {
            HostName = hostName;
            UserName = userName;
            Password = password;
            VirtualHost = virtualHost;
        }

        public string HostName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string VirtualHost { get; set; }
    }
}
