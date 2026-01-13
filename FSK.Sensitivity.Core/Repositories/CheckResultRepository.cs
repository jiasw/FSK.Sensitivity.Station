using FSK.Sensitivity.Core.Entity;
using FSK.Sensitivity.Core.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Repositories
{
    public class CheckResultRepository : BaseRepository<CheckResult>
    {
        private readonly ISqlSugarClient sqlSugarClient;

        public CheckResultRepository(ISqlSugarClient sqlSugarClient) : base(sqlSugarClient)
        {
            this.sqlSugarClient = sqlSugarClient;
        }
        /// <summary>
        /// 获取检查记录
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<string>> GetCheckDatesAsync(int pageIndex, int pageSize,long userId=0)
        {
            // 1. 处理可选参数的过滤 SQL
            // CheckResult 和 FitLog 表中都有 UserId 和 IsDeleted 字段
            string userFilter = userId>0 ? " AND UserId = @userId " : "";

            // 2. 构建基础 UNION SQL
            // 提取 FitTime 的日期部分
            string baseSql = $@"
        SELECT date(FitTime) AS CheckDate FROM CheckResult WHERE IsDeleted = 0 {userFilter}
        UNION
        SELECT date(FitTime) AS CheckDate FROM FitLog WHERE IsDeleted = 0 {userFilter}";

            // 3. 构建完整的分页 SQL
            string finalSql = $@"
        SELECT CheckDate FROM ({baseSql}) AS T
        ORDER BY CheckDate DESC
        LIMIT @pageSize OFFSET @offset";

            // 4. 设置参数
            var parameters = new
            {
                userId = userId,
                pageSize = pageSize,
                offset = (pageIndex - 1) * pageSize
            };

            // 5. 执行查询
            return await sqlSugarClient.Ado.SqlQueryAsync<string>(finalSql, parameters);
        }

        public async Task<List<CheckInfo>> GetCheckInfosAsync(int pageIndex, int pageSize, DateTime date, long userId = 0) { 
        
            return new List<CheckInfo>();
        }

    }
}
