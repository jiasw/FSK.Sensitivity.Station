using FSK.Sensitivity.Core.Entity;
using FSK.Sensitivity.Core.Enums;
using FSK.Sensitivity.Core.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

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
        public async Task<List<string>> GetCheckDatesAsync(int pageIndex, int pageSize,int userId=0)
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

        public async Task<int> GetTotalCountAsync(int userId = 0)
        {
            
            // 1. 处理可选参数的过滤 SQL
            // CheckResult 和 FitLog 表中都有 UserId 和 IsDeleted 字段
            string userFilter = userId > 0 ? " AND UserId = @userId " : "";
            // 2. 构建基础 UNION SQL
            // 提取 FitTime 的日期部分
            string baseSql = $@"
        SELECT date(FitTime) AS CheckDate FROM CheckResult WHERE IsDeleted = 0 {userFilter}
        UNION
        SELECT date(FitTime) AS CheckDate FROM FitLog WHERE IsDeleted = 0 {userFilter}";

            // 3. 构建完整的分页 SQL
            string finalSql = $@"
        SELECT count(*) FROM ({baseSql}) AS T
        ";
            var parameters = new
            {
                userId = userId
            };
            return await sqlSugarClient.Ado.GetIntAsync(finalSql, parameters);
        }





        public async Task<List<CheckInfo>> GetCheckInfosAsync( string date, long userId = 0) {
            //1.查询对比敏感度数据
            //2.查询暗环境适应数据
            //3.数据整合
            string sql = $@"SELECT 
    CheckId ,
    UserId,
    MAX(FitTime) AS FitTime,  -- 取记录的时间
    -- 左眼数据汇总
    MAX(CASE WHEN Eye = 1 THEN VA06 END) AS Left_VA06,
    MAX(CASE WHEN Eye = 1 THEN VA10 END) AS Left_VA10,
    MAX(CASE WHEN Eye = 1 THEN VA20 END) AS Left_VA20,
    MAX(CASE WHEN Eye = 1 THEN VA40 END) AS Left_VA40,
    MAX(CASE WHEN Eye = 1 THEN VA60 END) AS Left_VA60,
    MAX(CASE WHEN Eye = 1 THEN VA80 END) AS Left_VA80,
    -- 右眼数据汇总
    MAX(CASE WHEN Eye = 2 THEN VA06 END) AS Right_VA06,
    MAX(CASE WHEN Eye = 2 THEN VA10 END) AS Right_VA10,
    MAX(CASE WHEN Eye = 2 THEN VA20 END) AS Right_VA20,
    MAX(CASE WHEN Eye = 2 THEN VA40 END) AS Right_VA40,
    MAX(CASE WHEN Eye = 2 THEN VA60 END) AS Right_VA60,
    MAX(CASE WHEN Eye = 2 THEN VA80 END) AS Right_VA80,
    -- 其他公共字段
    DayOrNight,
    PupilDistance,
		CheckDistance,
		CheckDuration,
		LightStatus
FROM CheckResult
WHERE IsDeleted = 0  
  AND date(FitTime) ='{date}' and UserId={userId}
GROUP BY CheckId;";
            List<CheckResultDataView> list= await sqlSugarClient.Ado.SqlQueryAsync<CheckResultDataView>(sql);
            string fitsql = $@"SELECT * from FitLog where  IsDeleted = 0   and UserId={userId}
  AND date(FitTime) ='{date}' ";
            List<FitLog> fitLogs = await sqlSugarClient.Ado.SqlQueryAsync<FitLog>(fitsql);
            List<CheckInfo> mergelist= list.Select(x => new CheckInfo()
            {
                DataId=x.CheckId,
                Type= CheckItem.CSF,
                Result=x.Result,
                CheckTime=x.FitTime,
            }).Concat(fitLogs.Select(x => new CheckInfo() {
                DataId = x.Id,
                Type = CheckItem.DCK,
                Result = "合格",
                CheckTime = x.FitTime,
            })).OrderBy(x => x.CheckTime).ToList();

            return mergelist;

        }

    }
}
