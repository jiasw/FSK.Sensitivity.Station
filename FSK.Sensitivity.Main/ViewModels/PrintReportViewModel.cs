using FSK.Sensitivity.Core;
using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.Enums;
using FSK.Sensitivity.Core.Infrastructure;
using FSK.Sensitivity.Core.Model;
using FSK.Sensitivity.Core.Repositories;
using FSK.Sensitivity.Core.Utility;
using Prism.Navigation.Regions;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
        private readonly IConfigurationService configurationService;
        private List<ChartCSFDataModel> _chartsData = new List<ChartCSFDataModel>();
        AppSetting appSetting;
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


        private string result = "";
        public string Result
        {
            get { return result; }
            set
            {
                SetProperty(ref result, value);
            }
        }

        private bool _isAnalysis = false;
        public bool IsAnalysis
        {
            get { return _isAnalysis; }
            set
            {
                SetProperty(ref _isAnalysis, value);
            }
        }



        public PrintReportViewModel(IRegionManager regionManager, CheckResultRepository checkResultRepository
            , FitLogRepository fitLogRepository, IConfigurationService configurationService)
        {
            this.regionManager = regionManager;
            this.checkResultRepository = checkResultRepository;
            this.fitLogRepository = fitLogRepository;
            this.configurationService = configurationService;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }



        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }
        List<AnalysisPromt> analysisPromts = new List<AnalysisPromt>();
        private async Task BindData()
        {
            List<ChartCSFDataModel> list = new List<ChartCSFDataModel>();
            analysisPromts.Clear();
            if (request.DataType == CheckItem.CSF)//对比敏感度
            {
                List<Core.Entity.CheckResult> results= await checkResultRepository.Query(n => n.CheckId == request.DataID);

                foreach (var result in results) {

                    #region 图表数据
                    ChartCSFDataModel chart = new ChartCSFDataModel();
                    chart.Eye = result.Eye;
                    chart.DazzleLight = result.LightStatus;
                    chart.IsDay = result.DayOrNight;

                    List<CSFPointModel> points = new List<CSFPointModel>();

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


                    if (result.CheckDistance == CheckDistance.Short)
                    {
                        chart.MicrospurPoints = points;
                    }
                    else if (result.CheckDistance == CheckDistance.Medium)
                    {
                        chart.ShortRangePoints = points;
                    }
                    else if (result.CheckDistance == CheckDistance.Long)
                    {
                        chart.MidrangePoints = points;
                    }
                    else
                    {
                        chart.LongRangePoints = points;
                    }
                    chart.Title = chart.Eye.GetDescription() + "-" + chart.IsDay.GetDescription();
                    if (chart.DazzleLight == LightStatus.Strong)
                    {
                        chart.Title += "-强光";
                    }
                    list.Add(chart);
                    #endregion

                    #region ai promt
                    AnalysisPromt analysisPromt = new AnalysisPromt();
                    analysisPromt.Eyes = result.Eye.GetDescription();
                    analysisPromt.mode= result.DayOrNight.GetDescription();
                    analysisPromt.light= result.LightStatus.GetDescription();
                    analysisPromt.distance= result.CheckDistance.GetDescription();
                    analysisPromt.Data = new AnalysisData()
                    {
                        VA1 = ((CSFValue)result.VA06).GetDescription(),
                        VA2 = ((CSFValue)result.VA10).GetDescription(),
                        VA3 = ((CSFValue)result.VA20).GetDescription(),
                        VA4 = ((CSFValue)result.VA40).GetDescription(),
                        VA5 = ((CSFValue)result.VA60).GetDescription(),
                        VA6 = ((CSFValue)result.VA80).GetDescription(),

                    };
                    analysisPromts.Add(analysisPromt);
                    #endregion
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


        public DelegateCommand ResultCommand=>new DelegateCommand(async ()=> await GetResult());

        private async Task GetResult()
        {
            if (analysisPromts.Count <= 0)
            {
                return;
            }
            Result = "";
            IsAnalysis = true;

            using HttpClient client = new();
           string prompt = analysisPromts.ToJson();
            
            var request = new HttpRequestMessage(HttpMethod.Get, $"{appSetting.WebAnalysisServer}/api/Sensitivity/stream?prompt={prompt}");
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (response.IsSuccessStatusCode)
            {
                IsAnalysis = false;
                Result = "诊断分析：" + "\n";
                using var stream = await response.Content.ReadAsStreamAsync();
                using var reader = new StreamReader(stream);
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    
                    Result += line+"\n";
                }
            }
            else
            {
                Result = "网络异常,请稍候重试！";
            }
            IsAnalysis = false;
        }
        private string BuildPrompt()
        {
            return "";
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
            appSetting = configurationService.LoadSetting();
           Task.Run( BindData);
        }
    }
}
