using System;
using Montreal.Core.Crosscutting.Common.Extensions;

namespace Montreal.Core.Crosscutting.Common.Entity.Base.V1
{
    public class BaseEntity : IEntity
    {
        public BaseEntity()
        {
            Activate();
            Id = Guid.NewGuid();
        }

        public Guid Id { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow.ToBrazilianTimezone();
        public DateTime? UpdatedAt { get; private set; }
        public bool Active { get; private set; }
        public byte Status { get; private set; }
        public string SmartSearch { get; private set; }
        public Guid? RegisteredById { get; private set; }
        public Guid? LastChangeById { get; private set; }

        public void SetId(Guid id) => Id = id;

        public void ChangeUpdatedAt() => UpdatedAt = DateTime.UtcNow.ToBrazilianTimezone();

        public void SetRegisteredById(Guid value) => RegisteredById = value;

        public void SetLastChangeById(Guid value) => LastChangeById = value;

        public void InactivateLogical() => Active = false;

        public void ActivateLogical() => Active = true;

        public void Activate() => Status = (int)Enum.EntityStatus.Active;

        public void Inactivate() => Status = (int)Enum.EntityStatus.Inactive;

        public void Block() => Status = (int)Enum.EntityStatus.Blocked;

        public void SetSmartSearch(object entity) => SmartSearch = entity.GetSearchField();

        public void SetCreatedAt(DateTime createdAt) => CreatedAt = createdAt;
    }
}