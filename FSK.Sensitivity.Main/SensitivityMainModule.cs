using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.Entity;
using FSK.Sensitivity.Core.Infrastructure;
using FSK.Sensitivity.Core.Repositories;
using FSK.Sensitivity.Main.ViewModels;
using FSK.Sensitivity.Main.Views;
using FSK.Sensitivity.Main.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using Prism.Ioc;
using Prism.Navigation.Regions;
using SqlSugar;
using System.IO;

namespace FSK.Sensitivity.Main
{
    public class SensitivityMainModule : IModule
    {
        private readonly IRegionManager regionManager;

        public SensitivityMainModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {


            regionManager.RegisterViewWithRegion(AppConst.MainRegion, typeof(MainMenu));
            regionManager.RegisterViewWithRegion(AppConst.MainRegion, typeof(TrainFrame));
            regionManager.RegisterViewWithRegion(AppConst.MainRegion, typeof(Manger));

            
            regionManager.RegisterViewWithRegion(AppConst.TrainRegion, typeof(SystemSetting));
            regionManager.RegisterViewWithRegion(AppConst.TrainRegion, typeof(SensitivityConfig));
            regionManager.RegisterViewWithRegion(AppConst.TrainRegion, typeof(SensitivityTraining));
            regionManager.RegisterViewWithRegion(AppConst.TrainRegion, typeof(ContrastConfig));
            regionManager.RegisterViewWithRegion(AppConst.TrainRegion, typeof(CheckHistory));
            regionManager.RegisterViewWithRegion(AppConst.TrainRegion, typeof(ContrastTraining));
            regionManager.RegisterViewWithRegion(AppConst.TrainRegion, typeof(SystemSetting));
            regionManager.RegisterViewWithRegion(AppConst.TrainRegion, typeof(SerialPortConfig));
            regionManager.RegisterViewWithRegion(AppConst.TrainRegion, typeof(Store));
            regionManager.RegisterViewWithRegion(AppConst.TrainRegion, typeof(UserInfo));


            regionManager.RegisterViewWithRegion(AppConst.SignRegion, typeof(SecondaryInit));
            regionManager.RegisterViewWithRegion(AppConst.SignRegion, typeof(SecondaryContrast));
            regionManager.RegisterViewWithRegion(AppConst.SignRegion, typeof(SecondarySensitivity));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            ConfigurationHelper configurationHelper = new ConfigurationHelper();
            configurationHelper.BuildAppConfiguration();
            var appConfig = configurationHelper.GetAppConfiguration();
            containerRegistry.RegisterInstance(appConfig);
            containerRegistry.RegisterSingleton<IEventAggregator, EventAggregator>();

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
              db.QueryFilter.AddTableFilter<FSK.Sensitivity.Core.Entity.CheckResult>(it => it.IsDeleted == false);
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
                FSK.Sensitivity.Core.Entity.CheckResult,
                FSK.Sensitivity.Core.Entity.Dict>();

            #endregion
            #region 注册弹窗
            containerRegistry.RegisterDialog<Login, LoginViewModel>();
            containerRegistry.RegisterDialog<Register, RegisterViewModel>();
            containerRegistry.RegisterDialog<ShowItemsDialog, ShowItemsDialogViewModel>();
            containerRegistry.RegisterDialog<Scan, ScanViewModel>();
            containerRegistry.RegisterDialog<UserInfoAdd, UserInfoAddViewModel>();
            containerRegistry.RegisterDialog<AlertMsg, AlertMsgViewModel>();
            #endregion
            #region 注册界面

            #endregion

        }
    }
}
