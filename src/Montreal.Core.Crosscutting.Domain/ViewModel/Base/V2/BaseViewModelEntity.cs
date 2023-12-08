using System;
using Montreal.Core.Crosscutting.Common.Entity.Base.V2;

namespace Montreal.Core.Crosscutting.Domain.ViewModel.Base.V2
{
    public abstract class BaseViewModelEntity<TEntity> where TEntity : BaseEntity
    {
        private TEntity _originalEntity; 

        protected void LoadFromEntity(TEntity entity, Action<TEntity> converter)
        {
            this._originalEntity = entity;

            converter(this._originalEntity);
        }

        public TEntity ConvertToEntity(Action<TEntity> converter)
        {
            converter(this._originalEntity);

            return this._originalEntity;
        }
    }
}
