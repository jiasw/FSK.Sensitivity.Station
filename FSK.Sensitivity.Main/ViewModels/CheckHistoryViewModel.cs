using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.Enums;
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

        private string _id;
        public string Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _age;
        public string Age
        {
            get { return _age; }
            set { SetProperty(ref _age, value); }
        }

        private string _date;
        public string Date
        {
            get { return _date; }
            set { SetProperty(ref _date, value); }
        }



        public CheckHistoryViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
            List<CheckInfo> lists = new List<CheckInfo>();
            lists.Add(new CheckInfo() { Type = CheckItem.CSF, Result = "不合格", Status = "查看" });
            lists.Add(new CheckInfo() { Type = CheckItem.DCK, Result = "不合格", Status = "查看" });
            lists.Add(new CheckInfo() { Type = CheckItem.CSF, Result = "不合格", Status = "查看" });
            lists.Add(new CheckInfo() { Type = CheckItem.DCK, Result = "不合格", Status = "查看" });
            CheckInfos = lists;
List<CheckDateItem> dateItems = new List<CheckDateItem>();
            dateItems.Add(new CheckDateItem() { Date = "2021-01-01" });
            dateItems.Add(new CheckDateItem() { Date = "2021-01-02" });
            dateItems.Add(new CheckDateItem() { Date = "2021-01-03" });
            CheckDateItems = dateItems;
        }

        void BindDateList()
        {

        }


        public DelegateCommand BackCommand => new DelegateCommand(Back);

        private void Back()
        {
            regionManager.Regions[AppConst.MainRegion].NavigationService.Journal.GoBack();
        }
    }
}
