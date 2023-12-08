using System;
using Montreal.Core.Crosscutting.Common.Extensions;

namespace Montreal.Core.Crosscutting.Common.Entity.Base.V2
{
    public abstract class BaseEntity : IEntity
    {
        public BaseEntity()
        {
            this.Id = Guid.NewGuid();
            this.Active = true;
        }

        public Guid Id { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow.ToBrazilianTimezone();
        public DateTime? UpdatedAt { get; private set; }
        public bool Active { get; private set; }
        public Guid? RegisteredById { get; private set; }
        public Guid? LastChangeById { get; private set; }

        public void SetId(Guid id) => Id = id;

        public void ChangeUpdatedAt() => UpdatedAt = DateTime.UtcNow.ToBrazilianTimezone();

        public void SetRegisteredById(Guid value) => RegisteredById = value;

        public void SetLastChangeById(Guid value) => LastChangeById = value;

        public void InactivateLogical() => Active = false;

        public void ActivateLogical() => Active = true;

        public void SetCreatedAt(DateTime createdAt) => CreatedAt = createdAt;
    }
}