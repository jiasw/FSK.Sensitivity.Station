using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Entity
{
    public class Dict : RootEntity
    {
        /// <summary>
        /// 字典类型编码
        /// </summary>
        [SugarColumn(Length = 5)]
        public string TypeCode { get; set; }

        /// <summary>
        /// 字典类型名称
        /// </summary>  
        public string Name { get; set; }

        /// <summary>
        /// 字典编码
        /// </summary>
        [SugarColumn(Length = 5)]
        public string Code { get; set; }

        [SugarColumn(IsNullable = true)]
        /// <summary>
        /// 字典说明
        /// </summary>
        public string Description { get; set; }

    }
}
