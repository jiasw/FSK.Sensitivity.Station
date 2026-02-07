using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.Entity;
using FSK.Sensitivity.Core.HardWare.Drivers;
using FSK.Sensitivity.Core.HardWare.Peripherals;
using FSK.Sensitivity.Core.Infrastructure;
using FSK.Sensitivity.Core.Repositories;
using FSK.Sensitivity.Main.ViewModels;
using FSK.Sensitivity.Main.Views;
using FSK.Sensitivity.Main.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Prism.Ioc;
using Prism.Navigation.Regions;

using SqlSugar;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Main
{
    public class SensitivityMainModule : IModule
    {
        private readonly IRegionManager regionManager;
        private readonly ILogger<SensitivityMainModule> logger;

        public SensitivityMainModule(IRegionManager regionManager,ILogger<SensitivityMainModule> logger)
        {
            this.regionManager = regionManager;
            this.logger = logger;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            logger.LogDebug("开始加载模块");
            regionManager.RegisterViewWithRegion(AppConst.MainRegion, typeof(MainMenu));
            regionManager.RegisterViewWithRegion(AppConst.MainRegion, typeof(HardWareTest));
            regionManager.RegisterViewWithRegion(AppConst.MainRegion, typeof(TrainFrame));
            regionManager.RegisterViewWithRegion(AppConst.MainRegion, typeof(Manger));
            regionManager.RegisterViewWithRegion(AppConst.TrainRegion, typeof(SystemSetting));
            regionManager.RegisterViewWithRegion(AppConst.TrainRegion, typeof(SensitivityConfig));
            regionManager.RegisterViewWithRegion(AppConst.TrainRegion, typeof(SensitivityTraining));
            regionManager.RegisterViewWithRegion(AppConst.TrainRegion, typeof(ContrastConfig));
            regionManager.RegisterViewWithRegion(AppConst.TrainRegion, typeof(ContrastTraining));
            regionManager.RegisterViewWithRegion(AppConst.TrainRegion, typeof(CheckHistory));
            regionManager.RegisterViewWithRegion(AppConst.TrainRegion, typeof(SystemSetting));
            regionManager.RegisterViewWithRegion(AppConst.TrainRegion, typeof(PrintReport));
            regionManager.RegisterViewWithRegion(AppConst.TrainRegion, typeof(SerialPortConfig));
            regionManager.RegisterViewWithRegion(AppConst.TrainRegion, typeof(Store));
            regionManager.RegisterViewWithRegion(AppConst.TrainRegion, typeof(Patients));
            regionManager.RegisterViewWithRegion(AppConst.TrainRegion, typeof(NetWork));
            regionManager.RegisterViewWithRegion(AppConst.TrainRegion, typeof(UserInfo));
            regionManager.RegisterViewWithRegion(AppConst.SignRegion, typeof(SecondaryInit));
            regionManager.RegisterViewWithRegion(AppConst.SignRegion, typeof(SecondaryContrast));
            regionManager.RegisterViewWithRegion(AppConst.SignRegion, typeof(SecondarySensitivity));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            logger.LogDebug("开始注册类型");
            containerRegistry.RegisterSingleton<IConfigurationService, ConfigurationService>();
            containerRegistry.RegisterSingleton<IEventAggregator, EventAggregator>();

           

            // 默认情况下，每次都创建新实例
            //containerRegistry.RegisterForNavigation<Patients, PatientsViewModel>();
            
            //containerRegistry.RegisterForNavigation<ContrastConfig, ContrastConfigViewModel>();
            var container = containerRegistry.GetContainer();
            var appSettingService = container.Resolve<IConfigurationService>();
            var appConfig =  appSettingService.LoadSetting();
            #region 注册数据操作层
            var DB = new SqlSugarScope(new ConnectionConfig
            {
                ConnectionString =$"Data Source={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, appConfig.SqlLiteDb)};" ,
                DbType = SqlSugar.DbType.Sqlite,
                IsAutoCloseConnection = true,
            },
          db =>
          {
              db.QueryFilter.AddTableFilter<FSK.Sensitivity.Core.Entity.Patient>(it => it.IsDeleted == false);
              db.QueryFilter.AddTableFilter<FSK.Sensitivity.Core.Entity.Manger>(it => it.IsDeleted == false);
              db.QueryFilter.AddTableFilter<FSK.Sensitivity.Core.Entity.FitLog>(it => it.IsDeleted == false);
              db.Aop.OnLogExecuting = (sql, pars) =>
              {
                  //获取原生SQL推荐 5.1.4.63  性能OK
                  Console.WriteLine(UtilMethods.GetNativeSql(sql, pars));
              };
              db.Aop.OnError = (exp) =>
              {
                  Console.WriteLine(exp.Message);
              };

          }
          );

            // 注册数据库服务
            containerRegistry.RegisterScoped<ISqlSugarClient>(a => DB);
            containerRegistry.RegisterInstance<PatientRepository>(new PatientRepository(DB));
            containerRegistry.RegisterInstance<CheckResultRepository>(new CheckResultRepository(DB));
            containerRegistry.RegisterInstance<MangerRepository>(new MangerRepository(DB));
            containerRegistry.RegisterInstance<DictRepository>(new DictRepository(DB));
            #endregion
            #region 生成数据库
            DB.DbMaintenance.CreateDatabase();
            DB.CodeFirst.InitTables<FSK.Sensitivity.Core.Entity.Patient,
                FSK.Sensitivity.Core.Entity.Manger,
                FSK.Sensitivity.Core.Entity.FitLog,
                 FSK.Sensitivity.Core.Entity.CheckResult,
                FSK.Sensitivity.Core.Entity.Dict>();

            #endregion
            #region 注册弹窗
            containerRegistry.RegisterDialog<Login, LoginViewModel>();
            containerRegistry.RegisterDialog<Register, RegisterViewModel>();
            containerRegistry.RegisterDialog<ShowItemsDialog, ShowItemsDialogViewModel>();
            containerRegistry.RegisterDialog<Scan, ScanViewModel>();
            containerRegistry.RegisterDialog<UserInfoAdd, UserInfoAddViewModel>();
            containerRegistry.RegisterDialog<HardWareTest, HardWareTestViewModel>();
            containerRegistry.RegisterDialog<Finish, FinishViewModel>();

            #endregion

            #region 注册工具类
            // 创建 ServiceCollection 并添加 HttpClient 支持
            var services = new ServiceCollection();
            services.AddHttpClient();

            // 获取 IHttpClientFactory
            var httpClientFactory = services
                .BuildServiceProvider()
                .GetRequiredService<IHttpClientFactory>();

            containerRegistry.RegisterInstance(httpClientFactory);
            

            // 注册 远程 客户端
            containerRegistry.RegisterInstance<HttpClient>(
                new HttpClient { BaseAddress = new Uri(appConfig.RemoteServer) }, "RemoteClient");

            // 注册 本地 客户端
            containerRegistry.RegisterInstance<HttpClient>(
                new HttpClient { BaseAddress = new Uri(appConfig.LocalServer) }, "LocalClient");
            
            containerRegistry.RegisterSingleton<ISecureRegistrationService, SecureRegistrationService>();
            containerRegistry.RegisterSingleton<ICloudSyncService, CloudSyncService>();
            containerRegistry.RegisterSingleton<ITrainingAndCheckService, TrainingAndCheckService>();

            #endregion

            #region 注册硬件操作类
            containerRegistry.RegisterSingleton<IModbusService, ModbusService>();
            containerRegistry.RegisterSingleton<ILight, LightController>();
            containerRegistry.RegisterSingleton<IJoystick, JoystickController>();
            containerRegistry.RegisterSingleton<IMotor, MotorController>();
            containerRegistry.RegisterSingleton<Iwifi, WifiService>();
            containerRegistry.RegisterSingleton<IBarcodeScannerService, BarcodeScannerService>();
            containerRegistry.RegisterSingleton<ISpeechService, SpeechService>();
            #endregion


        }
    }
}
