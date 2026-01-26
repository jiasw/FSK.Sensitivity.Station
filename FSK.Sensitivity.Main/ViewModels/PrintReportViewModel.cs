using FSK.Sensitivity.Core;
using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.Enums;
using FSK.Sensitivity.Core.Model;
using FSK.Sensitivity.Core.Repositories;
using Prism.Navigation.Regions;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace FSK.Sensitivity.Main.ViewModels
{
    
    public class PrintReportViewModel : BaseViewModel, INavigationAware
    {

        ReportRequest request;
        private readonly IRegionManager regionManager;
        private readonly CheckResultRepository checkResultRepository;
        private readonly FitLogRepository fitLogRepository;


        private List<ChartCSFDataModel> _chartsData = new List<ChartCSFDataModel>();

        public List<ChartCSFDataModel> ChartsData
        {
            get { return _chartsData; }
            set { SetProperty(ref _chartsData, value); }
        }

        private List<DCKPointModel> _points = new List<DCKPointModel>();
        public List<DCKPointModel> Points
        {
            get { return _points; }
            set { SetProperty(ref _points, value); }
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

        private Visibility _showDck = Visibility.Collapsed;

        public Visibility ShowDck
        {
            get { return _showDck; }
            set { SetProperty(ref _showDck, value); }
        }

        public Visibility _showCsf = Visibility.Collapsed;

        public Visibility ShowCsf
        {
            get { return _showCsf; }
            set { SetProperty(ref _showCsf, value); }
        }




        public PrintReportViewModel(IRegionManager regionManager, CheckResultRepository checkResultRepository, FitLogRepository fitLogRepository)
        {
            this.regionManager = regionManager;
            this.checkResultRepository = checkResultRepository;
            this.fitLogRepository = fitLogRepository;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }



        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        private async Task BindData()
        {
            List<ChartCSFDataModel> list = new List<ChartCSFDataModel>();
            if (request.DataType == CheckItem.CSF)//对比敏感度
            {
                List<Core.Entity.CheckResult> results= await checkResultRepository.Query(n => n.CheckId == request.DataID);

                foreach (var result in results) {

                    ChartCSFDataModel chart=new ChartCSFDataModel();
                    chart.Eye = result.Eye;
                    chart.DazzleLight = result.LightStatus;
                    chart.IsDay = result.DayOrNight;
                    
                    List< CSFPointModel> points=new List< CSFPointModel>();

                    points.Add(new CSFPointModel()
                    {
                        Value = CSFVA.VA06,
                        CS = (CSFValue)result.VA06,
                    });
                    points.Add(new CSFPointModel()
                    {
                        Value = CSFVA.VA10,
                        CS = (CSFValue)result.VA10,
                    });

                    points.Add(new CSFPointModel()
                    {
                        Value = CSFVA.VA20,
                        CS = (CSFValue)result.VA20,
                    });
                    points.Add(new CSFPointModel()
                    {
                        Value = CSFVA.VA40,
                        CS = (CSFValue)result.VA40,
                    });
                    points.Add(new CSFPointModel()
                    {
                        Value = CSFVA.VA60,
                        CS = (CSFValue)result.VA60,
                    });
                    points.Add(new CSFPointModel()
                    {
                        Value = CSFVA.VA80,
                        CS = (CSFValue)result.VA80,
                    });


                    if (result.CheckDistance== CheckDistance.Short)
                    {
                        chart.MicrospurPoints = points;
                    }else if(result.CheckDistance== CheckDistance.Medium)
                    {
                        chart.ShortRangePoints = points;
                    }
                    else if(result.CheckDistance== CheckDistance.Long)
                    {
                        chart.MidrangePoints = points;
                    }
                    else
                    {
                        chart.LongRangePoints= points;
                    }
                    chart.Title = chart.Eye.GetDescription()+"-"+chart.IsDay.GetDescription();
                    if (chart.DazzleLight == LightStatus.Strong)
                    {
                        chart.Title += "-强光";
                    }
                    list.Add(chart);
                }
                ChartsData = list;
            }
            else if (request.DataType == CheckItem.DCK)//暗环境适应
            { 
                Core.Entity.FitLog fitlog = await fitLogRepository.QueryById(request.DataID);
                if (fitlog != null) {
                    Points = null;
                    Points = new List<DCKPointModel>()
                    {new DCKPointModel(){DCKTime = (DCKTime)fitlog.FitDrution,
                        DCKValue = (DCKValue)fitlog.FitResult
                    }
                    };
                }
            }

            
        }


        public DelegateCommand BackCommand => new DelegateCommand(Back);

        private void Back()
        {
            if (request != null) {
                if (request.FromPage == 0|| request.FromPage==1)
                {
                    regionManager.Regions[AppConst.MainRegion].NavigationService.RequestNavigate(AppConst.Main_Page_Menu);
                }
                else
                {
                    regionManager.Regions[AppConst.MainRegion].NavigationService.Journal.GoBack();
                }

            }
            else
            {
                regionManager.Regions[AppConst.MainRegion].NavigationService.Journal.GoBack();
            }
            
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Name = AppData.Instance.CurrentPatient.PatientName;
            Age = AppData.Instance.CurrentPatient.Age.ToString();
            Id = AppData.Instance.CurrentPatient.Id.ToString();
            

            request = navigationContext.Parameters.GetValue<ReportRequest>("request");
            Date = request.Date;
            ShowCsf=request.DataType== CheckItem.CSF? Visibility.Visible:Visibility.Collapsed;
            ShowDck = request.DataType == CheckItem.DCK ? Visibility.Visible : Visibility.Collapsed;

           Task.Run( BindData);
        }
    }
}
