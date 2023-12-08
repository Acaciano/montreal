using System;
using System.Collections.Generic;
using System.Linq;

namespace Montreal.Core.Crosscutting.Common.Pagination
{
    public class PaginationConfiguratorFactory<TEntity>
    {
        private static PaginationConfiguratorFactory<TEntity> _instance;

        private ICollection<PaginationConfiguratorObject<TEntity>> _configurations;

        internal static PaginationConfiguratorFactory<TEntity> Instance
        {
            get
            {
                if (_instance == null) _instance = new PaginationConfiguratorFactory<TEntity>();
                return _instance;
            }

            private set { Instance = value; }
        }

        private PaginationConfiguratorFactory() => _configurations = new List<PaginationConfiguratorObject<TEntity>>();

        internal static PaginationConfiguratorObject<TEntity> GetByProperty(string property) =>
            Instance._configurations.FirstOrDefault(x => x.Property.ToLower() == property?.ToLower());

        internal static PaginationConfiguratorObject<TEntity> GetByTypeAndProperty(Type type, string property) =>
            Instance._configurations.FirstOrDefault(x => x.Type == type && x.Property.ToLower() == property?.ToLower());

        internal static PaginationConfiguratorObject<TEntity> GetOrderConfiguration(string property) =>
            Instance._configurations.FirstOrDefault(x => x.Type == typeof(TEntity) && x.Property.ToLower() == property?.ToLower() && (x.OrderExpressions != null && x.OrderExpressions.Any()));

        internal static PaginationConfiguratorObject<TEntity> GetFilterConfiguration(string property) =>
            Instance._configurations.FirstOrDefault(x => x.Type == typeof(TEntity) && x.Property.ToLower() == property?.ToLower() && x.FilterExpression != null);

        internal static PaginationConfiguratorObject<TEntity> GetSelectConfiguration(string property) =>
            Instance._configurations.FirstOrDefault(x => x.Type == typeof(TEntity) && x.Property.ToLower() == property?.ToLower() && x.SelectExpression != null);

        internal static PaginationConfiguratorObject<TEntity> AddConfiguration(PaginationConfiguratorObject<TEntity> configuration)
        {
            Instance._configurations.Add(configuration);
            return configuration;
        }
    }
}
