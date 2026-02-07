using FSK.Sensitivity.Core.Entity;
using FSK.Sensitivity.Core.HardWare.Drivers;
using FSK.Sensitivity.Core.HardWare.Peripherals;
using FSK.Sensitivity.Core.Infrastructure;
using FSK.Sensitivity.Core.Model;
using FSK.Sensitivity.Core.Repositories;
using Microsoft.Extensions.Logging;
using NetTaste;
using Serilog;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FSK.Sensitivity.Main.ViewModels
{
    public class ScanViewModel : BaseViewModel, IDialogAware
    {
        private string title="扫描登录";
        private readonly IBarcodeScannerService _scannerService;
        private readonly ILogger<ScanViewModel> logger;
        private readonly ICloudSyncService cloudSyncService;
        private readonly IConfigurationService configurationService;
       
        private ObservableCollection<string> _scanHistory;
        private string _scannedBarcode;
        public string ScannedBarcode
        {
            get => _scannedBarcode;
            set => SetProperty(ref _scannedBarcode, value);
        }
        private string _scannerStatus;
        public string ScannerStatus
        {
            get => _scannerStatus;
            set => SetProperty(ref _scannerStatus, value);
        }
        private bool _isScannerActive;
        public bool IsScannerActive
        {
            get => _isScannerActive;
            set => SetProperty(ref _isScannerActive, value);
        }
        public ObservableCollection<string> ScanHistory
        {
            get => _scanHistory;
            set => SetProperty(ref _scanHistory, value);
        }
        
        public ICommand ClearCommand { get; }
        public ScanViewModel(IBarcodeScannerService scanner,ILogger<ScanViewModel> logger
            , ICloudSyncService cloudSyncService, IConfigurationService configurationService
            )
        {
            this._scannerService = scanner;
            this.logger = logger;
            this.cloudSyncService = cloudSyncService;
            this.configurationService = configurationService;
          
            _scanHistory = new ObservableCollection<string>();
            
            ClearCommand = new DelegateCommand(() => ScannedBarcode = "");
            
            UpdateStatus();
        }


        private void StartScanning()
        {
            _scannerService.StartListening();
            _scannerService.BarcodeScanned += OnBarcodeScanned;
            UpdateStatus();
        }
        private void StopScanning()
        {
            _scannerService.StopListening();
            _scannerService.BarcodeScanned -= OnBarcodeScanned;
            UpdateStatus();
        }
        private void OnBarcodeScanned(object sender, BarcodeScannedEventArgs e)
        {
            CloudSolutionDataItem cloudSolution = _scannerService.ProcessCode(e.Barcode);
            if (cloudSolution == null) { return; }
            logger.LogDebug("*************************");
            logger.LogDebug($"解析到信息,guid:{cloudSolution.Guid},type:{cloudSolution.Type}");
            logger.LogDebug("*************************");
            
            _scannerService.StopListening();
            if (!string.IsNullOrEmpty(cloudSolution.Guid))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var result = new DialogResult(ButtonResult.OK);
                    result.Parameters.Add("scanResult", cloudSolution.Guid);
                    RequestClose.Invoke(result);
                });
            }

           
            
            
        }

        

        private void UpdateStatus()
        {
            IsScannerActive = _scannerService.IsListening;
            ScannerStatus = IsScannerActive ? "监听中..." : "已停止";
        }

        private void Scanner_ScanCompleted(string result)
        {
            Log.Information($"扫描结果:{result}");
        }

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }
        public DialogCloseListener RequestClose  {get;}

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            StopScanning();


        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            StartScanning();
        }

        public DelegateCommand CloseCommand => new DelegateCommand(Close);



        private void Close()
        {
            StopScanning();
            var result = new DialogResult(ButtonResult.Cancel);
            RequestClose.Invoke(result);
        }


        private void ExecuteConfirm()
        {
            // 执行确认逻辑

            //var result = new DialogResult(ButtonResult.OK);
            // 或者带参数的返回
             var result = new DialogResult(ButtonResult.OK);
             result.Parameters.Add("scanResult", "112233");

            RequestClose.Invoke(result);
        }
    }
}
