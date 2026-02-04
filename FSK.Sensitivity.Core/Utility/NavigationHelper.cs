using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FSK.Sensitivity.Core.Utility
{
    public static class NavigationHelper
    {
        /// <summary>
        /// 获取当前区域中显示的视图名称
        /// </summary>
        public static string GetCurrentViewName(IRegionManager regionManager, string regionName)
        {
            var region = regionManager.Regions[regionName];
            if(region.ActiveViews.Count()==0)
            {
                return string.Empty;
            }

            if (region?.ActiveViews.FirstOrDefault() is FrameworkElement view)
            {
                return view.Name ?? view.GetType().Name;
            }
            return string.Empty;
        }
        /// <summary>
        /// 检查当前视图是否是指定的视图
        /// </summary>
        public static bool IsCurrentView(IRegionManager regionManager, string regionName, string viewName)
        {
            var currentView = GetCurrentViewName(regionManager, regionName);
            return currentView.Equals(viewName, StringComparison.OrdinalIgnoreCase);
        }
    }
}
