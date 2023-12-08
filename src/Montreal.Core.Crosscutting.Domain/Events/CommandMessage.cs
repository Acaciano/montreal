using MediatR;
using Newtonsoft.Json;

namespace Montreal.Core.Crosscutting.Domain.Events
{
    public abstract class CommandMessage : IRequest, IRequestBase
    {
        [JsonIgnore()]
        public string MessageType { get; protected set; }

        protected CommandMessage()
        {
            MessageType = GetType().Name;
        }
    }
}