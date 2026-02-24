using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.Enums;
using FSK.Sensitivity.Core.EventBus;
using FSK.Sensitivity.Core.Model;
using FSK.Sensitivity.Core.Repositories;
using HandyControl.Controls;
using Prism.Events;
using Prism.Navigation.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FSK.Sensitivity.Main.ViewModels
{
    public class CheckHistoryViewModel : BaseViewModel, INavigationAware
    {
        private readonly IRegionManager regionManager;
        private readonly CheckResultRepository checkResultRepository;
        private readonly IEventAggregator eventAggregator;
        private List<CheckInfo> _checkInfos;

        public List<CheckInfo> CheckInfos
        {
            get { return _checkInfos; }
            set { SetProperty(ref _checkInfos, value); }
        }

        private List<CheckDateItem> _checkDateItems=new List<CheckDateItem>();

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

        private int pagesize=10;

        private int page=1;

        private int totalpage=1;

        private int total=0;

        
        private bool prepageenable=false;
        public bool Prepageenable
        {
            get
            {
                if (page == 1)
                {
                    prepageenable = false;
                }
                else
                {
                    prepageenable = true;
                }
                return prepageenable;
            }
            set { SetProperty(ref prepageenable, value); }
        }

        private bool nextpageenable=false;
        public bool Nextpageenable
        {
            get
            {
                if (page == totalpage)
                {
                    nextpageenable = false;
                }
                else
                {
                    nextpageenable = true;
                }
                return nextpageenable;
            }
            set { SetProperty(ref nextpageenable, value); }

        }


        public CheckHistoryViewModel(IRegionManager regionManager, CheckResultRepository checkResultRepository,IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            this.checkResultRepository = checkResultRepository;
            this.eventAggregator = eventAggregator;
        }

        async Task BindDateList()
        {
            CheckDateItems.Clear();
            List<string> query = await checkResultRepository.GetCheckDatesAsync(page, pagesize, AppData.Instance.CurrentPatient.Id);
            if (query != null)
            {
                CheckDateItems =query.Select(x => new CheckDateItem() { Date = x }).ToList();
                if (CheckDateItems.Count > 0)
                {
                    Date= CheckDateItems.First().Date;
                    CheckInfos = await checkResultRepository.GetCheckInfosAsync(CheckDateItems.First().Date, AppData.Instance.CurrentPatient.Id);
                }

            }
            total = await checkResultRepository.GetTotalCountAsync(AppData.Instance.CurrentPatient.Id);
            totalpage=(int)Math.Ceiling((double)total/pagesize);
            RaisePropertyChanged(nameof(Nextpageenable));
            RaisePropertyChanged(nameof(Prepageenable));
        }

        public DelegateCommand PrePageCommand => new DelegateCommand(async () => await PrePage());

        private async Task PrePage()
        {
            if (page > 1)
            {
                page-=1;
            }
            await BindDateList();
            
        }

        public DelegateCommand NextPageCommand => new DelegateCommand(async () => await NextPage());

        private async Task NextPage()
        {
            if (page < totalpage)
            {
                page+=1;
            }
            await BindDateList();
        }

        public DelegateCommand<string> ItemClickCommand=>new DelegateCommand<string>(async (date) => await ItemClick(date));

        private async Task ItemClick(string date)
        {
            Date = date;
            CheckInfos = await checkResultRepository.GetCheckInfosAsync(date, AppData.Instance.CurrentPatient.Id);
        }

        public DelegateCommand<CheckInfo> ShowDetailCommand => new DelegateCommand<CheckInfo>(ShowDetail);
        private void ShowDetail(CheckInfo info)
        {
            ReportRequest request = new ReportRequest();
            
            request.DataID = info.DataId;
            request.DataType = info.Type;
            request.Date = Date;
            
            regionManager.RequestNavigate(AppConst.TrainRegion, AppConst.Main_Page_Report,new NavigationParameters() { { "request", request } });
        }


        public DelegateCommand BackCommand => new DelegateCommand(Back);

        private void Back()
        {
            regionManager.Regions[AppConst.MainRegion].NavigationService.Journal.GoBack();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            eventAggregator.GetEvent<CheckResultPageLoadRvent>().Publish();
            page = 1;
            totalpage = 1;
            Name = AppData.Instance.CurrentPatient.PatientName;
            Age= AppData.Instance.CurrentPatient.Age.ToString();
            Id=AppData.Instance.CurrentPatient.Id.ToString();
            _ = Task.Run(BindDateList);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }
    }
}
