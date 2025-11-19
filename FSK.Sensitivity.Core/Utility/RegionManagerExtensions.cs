using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Utility
{
    public static class RegionManagerExtensions
    {
        public static void SafeRequestNavigate(this IRegionManager regionManager, string regionName, string targetViewName)
        {
            // 获取指定区域
            var region = regionManager.Regions[regionName];

            // 获取当前活动的视图名称
            var currentView = region.ActiveViews.FirstOrDefault()?.GetType().Name;

            // 比较当前视图和目标视图名称
            if (currentView != targetViewName)
            {
                regionManager.RequestNavigate(regionName, targetViewName);
            }
            // 如果相同则不做任何操作
        }
    }

}
