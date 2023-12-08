namespace Montreal.Core.Crosscutting.Domain.Events
{
    public interface IRequestBase
    {
        string MessageType { get; }
    }
}