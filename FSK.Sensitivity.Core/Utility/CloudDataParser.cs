using FSK.Sensitivity.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Utility
{

    public class AdvancedDataParser
    {
        /// <summary>
        /// 解析不规范的字符串数据，支持开头/结尾缺失字段
        /// </summary>
        public static List<CloudSolutionDataItem> ParseData(string input)
        {
            var result = new List<CloudSolutionDataItem>();

            try
            {
                // 第一步：规范化输入
                var normalized = NormalizeInput(input);

                // 第二步：提取所有块（以]['分界）
                var blocks = ExtractBlocks(normalized);

                // 第三步：解析每个块
                foreach (var block in blocks)
                {
                    var dataItem = ParseSingleItem(block);
                    if (dataItem != null)
                    {
                        result.Add(dataItem);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"解析异常: {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// 规范化输入字符串
        /// </summary>
        private static string NormalizeInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // 移除首尾空白
            input = input.Trim();

            // 如果开头是 ']，说明前面有缺失的 [
            if (input.StartsWith("]"))
            {
                input = "[" + input;
            }

            // 如果末尾是 [，说明后面有缺失的 ]
            if (input.EndsWith("["))
            {
                input = input + "]";
            }

            return input;
        }

        /// <summary>
        /// 提取数据块
        /// </summary>
        private static List<string> ExtractBlocks(string input)
        {
            var blocks = new List<string>();

            // 使用正则表达式匹配 ['...',...]模式
            var pattern = @"\['([^]]*?)'\]";
            var matches = System.Text.RegularExpressions.Regex.Matches(input, pattern);

            if (matches.Count > 0)
            {
                // 如果正则匹配成功
                foreach (System.Text.RegularExpressions.Match match in matches)
                {
                    blocks.Add(match.Groups[1].Value);
                }
            }
            else
            {
                // 降级方案：按 ]['分割
                blocks = input
                    .Trim('[', ']')
                    .Split(new[] { "][" }, StringSplitOptions.None)
                    .ToList();
            }

            return blocks;
        }

        /// <summary>
        /// 解析单个数据块
        /// </summary>
        private static CloudSolutionDataItem ParseSingleItem(string item)
        {
            if (string.IsNullOrWhiteSpace(item))
                return null;

            var data = new CloudSolutionDataItem();

            // 按 , 分割键值对
            var pairs = item.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var pair in pairs)
            {
                if (string.IsNullOrWhiteSpace(pair))
                    continue;

                // 按 ; 分割键和值
                var parts = pair.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length >= 2)
                {
                    var key = parts[0].Trim('\'', ' ', '"').ToLower();
                    var value = parts[1].Trim('\'', ' ', '"');

                    switch (key)
                    {
                        case "guid":
                            data.Guid = value;
                            break;
                        case "type":
                            if (int.TryParse(value, out int typeValue))
                            {
                                data.Type = typeValue;
                            }
                            break;
                    }
                }
            }

            // 至少需要 GUID 字段
            return !string.IsNullOrEmpty(data.Guid) ? data : null;
        }

        /// <summary>
        /// 对数据进行去重
        /// </summary>
        public static List<CloudSolutionDataItem> Deduplicate(List<CloudSolutionDataItem> items)
        {
            return items.Distinct().ToList();
        }

        /// <summary>
        /// 一键处理：解析 + 去重
        /// </summary>
        public static List<CloudSolutionDataItem> ProcessData(string input)
        {
            var parsed = ParseData(input);
            return Deduplicate(parsed);
        }
    }

}
