using System;
using System.Collections.Generic;
using System.Text;

namespace Montreal.Core.Crosscutting.Common.Data
{
    public class ExportObject<TEntity> where TEntity : class
    {
        public IEnumerable<TEntity> Results { get; set; }
    }
}
