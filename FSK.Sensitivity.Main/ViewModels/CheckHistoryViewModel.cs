using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.Model;
using Prism.Navigation.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Main.ViewModels
{
    public class CheckHistoryViewModel : BaseViewModel
    {
        private readonly IRegionManager regionManager;
        private List<CheckInfo> _checkInfos;

        public List<CheckInfo> CheckInfos
        {
            get { return _checkInfos; }
            set { SetProperty(ref _checkInfos, value); }
        }

        private List<CheckDateItem> _checkDateItems;

        public List<CheckDateItem> CheckDateItems
        {
            get { return _checkDateItems; }
            set { SetProperty(ref _checkDateItems, value); }
        }

        public CheckHistoryViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
            List<CheckInfo> lists = new List<CheckInfo>();
            lists.Add(new CheckInfo() { Item = "对比敏感度检测", Result = "不合格", Status = "已检查" });
            lists.Add(new CheckInfo() { Item = "暗适应检测", Result = "不合格", Status = "未检查" });
            lists.Add(new CheckInfo() { Item = "对比敏感度检测", Result = "不合格", Status = "已检查" });
            lists.Add(new CheckInfo() { Item = "暗适应检测", Result = "不合格", Status = "已检查" });
            CheckInfos = lists;
List<CheckDateItem> dateItems = new List<CheckDateItem>();
            dateItems.Add(new CheckDateItem() { Date = "2021-01-01" });
            dateItems.Add(new CheckDateItem() { Date = "2021-01-02" });
            dateItems.Add(new CheckDateItem() { Date = "2021-01-03" });
            CheckDateItems = dateItems;
        }

        public DelegateCommand BackCommand => new DelegateCommand(Back);

        private void Back()
        {
            regionManager.Regions[AppConst.MainRegion].NavigationService.Journal.GoBack();
        }
    }
}
