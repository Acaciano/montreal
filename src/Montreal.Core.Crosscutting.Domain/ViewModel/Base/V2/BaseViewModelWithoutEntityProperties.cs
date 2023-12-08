using System;
using Montreal.Core.Crosscutting.Common.Entity.Base.V2;

namespace Montreal.Core.Crosscutting.Domain.ViewModel.Base.V2
{
    public abstract class BaseViewModelWithoutEntityProperties<TEntity> : BaseViewModelEntity<TEntity> where TEntity : BaseEntity
    {
        public abstract void LoadFromEntity(TEntity entity);
        public abstract TEntity ConvertToEntity();
        public abstract void OverwriteEntity(TEntity entity);
    }
}
