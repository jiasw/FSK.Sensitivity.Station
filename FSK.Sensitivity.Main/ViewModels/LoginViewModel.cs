using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.Entity;
using FSK.Sensitivity.Core.Repositories;
using FSK.Sensitivity.Main.Controls;
using HandyControl.Controls;
using Prism.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Main.ViewModels
{


    public class UserList : BaseViewModel
    {
        private string name;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }
        private string loginName;
        public string LoginName
        {
            get => loginName;
            set => SetProperty(ref loginName, value);
        }

        private bool isSelected;
        public bool IsSelected
        {
            get => isSelected;
            set => SetProperty(ref isSelected, value);
        }

        public long Id { get; set; }
    }
    public class LoginViewModel : BaseViewModel, IDialogAware
    {
        private string title = "登录";
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        private string username;
        public string Username
        {
            get => username;
            set => SetProperty(ref username, value);
        }   
        private string password;
        private readonly IDialogService dialogService;
        private readonly PatientRepository patientRepository;

        private List<Patient> _patientList;
        private List<UserList> _patients;
        private long _seleedPatientId = 0;

        public List<UserList> Patients
        {
            get => _patients;
            set => SetProperty(ref _patients, value);
        }

        public LoginViewModel(IDialogService dialogService, PatientRepository patientRepository)
        {
            this.dialogService = dialogService;
            this.patientRepository = patientRepository;

            LoadPatientsAsync();
        }

        private async void LoadPatientsAsync()
        {
            _patientList = await patientRepository.Query();
            Patients= _patientList.Select(p => new UserList { Name = p.PatientName, IsSelected = false
                , LoginName = p.LoginName,Id=p.Id }).ToList();
        }

        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        public DialogCloseListener RequestClose { get; }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            
        }
        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose.Invoke(dialogResult);
        }

        public DelegateCommand<UserList> SelectCommand=> new DelegateCommand<UserList>(Select);

        private void Select(UserList patient)
        {
            foreach (var p in Patients)
            {
                p.IsSelected = false;
            }
            patient.IsSelected = true;
            Username = patient.Name;
            _seleedPatientId = patient.Id;
        }

        public DelegateCommand LoginCommand => new DelegateCommand(Login);

        private void Login()
        {
            if (Patients==null || Patients.Count==0||string.IsNullOrEmpty(Username))
            {
                MessageBoxService.Instance.Show("请选择用户");
                return;
            }
            AppData.Instance.CurrentPatient= _patientList.First(p => p.Id == _seleedPatientId);

            //TODO: 登录逻辑    
            RaiseRequestClose(new DialogResult(ButtonResult.OK));
        }

        public DelegateCommand CancelCommand => new DelegateCommand(Cancel);

        private void Cancel()
        {
            RaiseRequestClose(new DialogResult(ButtonResult.Cancel));
        }

        
    }
}
