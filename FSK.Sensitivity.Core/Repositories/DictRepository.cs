using FSK.Sensitivity.Core.Entity;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Repositories
{
    public class DictRepository : BaseRepository<Dict>
    {
        private readonly ISqlSugarClient sqlSugarClient;

        public DictRepository(ISqlSugarClient sqlSugarClient) : base(sqlSugarClient)
        {
            this.sqlSugarClient = sqlSugarClient;
        }

        
    }
}
