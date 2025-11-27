using FSK.Sensitivity.Core.Entity;
using FSK.Sensitivity.Core.Enums;
using FSK.Sensitivity.Core.Repositories;
using FSK.Sensitivity.Main.Controls;
using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Main.ViewModels
{
    public class RegisterViewModel : BaseViewModel, IDialogAware
    {
       

        public List<string> SexList =>new List<string> { "男", "女" };

        public List<string> GradeList => new List<string> { "一年级", "二年级", "三年级", "四年级", "五年级", "六年级", "七年级", "八年级", "九年级", "高一", "高二", "高三","大一","大二","大三","大四" };

        private string title = "首次登录";
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }
        private string _loginAccount;
        public string LoginAccount
        {
            get { return _loginAccount; }
            set { SetProperty(ref _loginAccount, value); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _phone;
        public string Phone
        {
            get { return _phone; }  
            set { SetProperty(ref _phone, value); }
        }

        private string _idCard;
        public string IdCard
        {
            get { return _idCard; }
            set { SetProperty(ref _idCard, value); }
        }
        private string _age;
        public string Age
        {
            get { return _age; }
            set { SetProperty(ref _age, value); }
        }

        private string _gender;
        public string Gender
        {
            get { return _gender; }
            set { SetProperty(ref _gender, value); }
        }

        private string _school;
        public string School
        {
            get { return _school; }
            set { SetProperty(ref _school, value); }
        }

        private string _grade;
        public string Grade
        {
            get { return _grade; }
            set { SetProperty(ref _grade, value); }
        }

        private string _className;
        private readonly PatientRepository patientRepository;

        public RegisterViewModel(PatientRepository patientRepository)
        {
            this.patientRepository = patientRepository;
        }

        public string ClassName
        {
            get { return _className; }  
            set { SetProperty(ref _className, value); }
        }

        public DialogCloseListener RequestClose { get; }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }
        private long id = 0;
        public async void OnDialogOpened(IDialogParameters parameters)
        {
            parameters.TryGetValue("id", out id);
            if (id != 0)
            {
                var manger = await patientRepository.QueryById(id);
                if (manger == null)
                {
                    return;
                }
                LoginAccount= manger.LoginName;
                Name = manger.PatientName;
                Phone = manger.Phone;
                IdCard = manger.IdCard;
                Age = manger.Age.ToString();
                Gender = manger.Sex == 1 ? "男" : "女";
                School = manger.School;
                Grade = manger.Grade;
                ClassName = manger.Class;
            }
        }

        public DelegateCommand CloseCommand=>new DelegateCommand(Close);

        private void Close()
        {
            RequestClose.Invoke(new DialogResult(ButtonResult.Cancel));
        }

        public DelegateCommand RegisterCommand => new DelegateCommand(async () => await Register());

        private async Task Register()
        {
            if (string.IsNullOrEmpty(LoginAccount))
            {
                AlertMessageBox.Show("请输入登录账号！");
                return;
            }
            if (string.IsNullOrEmpty(Name))
            {
                AlertMessageBox.Show("请输入姓名！");
                return;
            }

            Patient addPatient = new Patient
            {
                LoginName = LoginAccount,
                PatientName = Name,
                Phone = Phone,
                IdCard = IdCard,
                Age = int.Parse(Age),
                Sex = Gender == "男"? 1 : 0,
                School = School,
                Grade = Grade,
                Class=ClassName
            };
            long id= await patientRepository.Add(addPatient);
            if (id > 0)
            {
                AlertMessageBox.Show("注册成功！");
                addPatient.Id = id;
                AppData.Instance.CurrentPatient = addPatient;
                Close();
            }
            else
            {
                AlertMessageBox.Show("注册失败！");
            }

        }


    }
}
