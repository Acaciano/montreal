namespace Montreal.Core.Crosscutting.Infrastructure.Communication
{
    public class SmtpConfig
    {
        public SmtpConfig(string username, string domain, string password, string name, int port)
        {
            Username = username;
            Domain = domain;
            Password = password;
            Name = name;
            Port = port;
        }

        public string Username { get; private set; }
        public string Domain { get; private set; }
        public string Password { get; private set; }
        public string Name { get; private set; }
        public int Port { get; private set; }

        public static SmtpConfig GetConfigDefault()
        {
           return new SmtpConfig(string.Empty, string.Empty, string.Empty, string.Empty, 0);
        }
    }
}
