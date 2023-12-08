using System;
using Montreal.Core.Crosscutting.Common.Entity.Base.V2;

namespace Montreal.Core.Crosscutting.Domain.ViewModel.Base.V2
{
    public abstract class BaseViewModelWithEntityProperties<TEntity> : BaseViewModelEntity<TEntity> where TEntity : BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? RegisteredById { get; set; }
        public Guid? LastChangeById { get; set; }
        public bool Active { get; set; }

        public abstract void LoadFromEntity(TEntity entity);

        public abstract TEntity ConvertToEntity();

        public abstract void OverwriteEntity(TEntity entity);
    }
}
