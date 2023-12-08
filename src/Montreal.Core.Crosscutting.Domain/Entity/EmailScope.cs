namespace Montreal.Core.Crosscutting.Domain.Entity
{
    public class EmailScope
    {
        public string Subject { get; set; }
        public string[] To { get; set; }
        public string From { get; set; }
        public string Body { get; set; }
    }
}