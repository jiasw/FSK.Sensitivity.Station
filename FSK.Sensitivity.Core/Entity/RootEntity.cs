using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Entity
{
    public class RootEntity
    {
        /// <summary>
        /// ID
        /// 泛型主键Tkey
        /// </summary>
        [SugarColumn(IsNullable = false, IsPrimaryKey = true)]
        public long Id { get; set; }

        [SugarColumn(InsertServerTime = true)]
        public DateTime CreateTime { get; set; }
        [SugarColumn(UpdateServerTime = true)]
        public DateTime UpdateTime { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime DeleteTime { get; set; }


    }
}
