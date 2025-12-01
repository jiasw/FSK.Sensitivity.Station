using FSK.Sensitivity.Core.Entity;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Repositories
{
    public class FitLogRepository : BaseRepository<FitLog>
    {
        private readonly ISqlSugarClient sqlSugarClient;

        public FitLogRepository(ISqlSugarClient sqlSugarClient) : base(sqlSugarClient)
        {
            this.sqlSugarClient = sqlSugarClient;
        }
    }
}
