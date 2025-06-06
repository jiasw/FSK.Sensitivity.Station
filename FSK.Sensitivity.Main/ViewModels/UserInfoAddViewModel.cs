using FSK.Sensitivity.Core;
using FSK.Sensitivity.Core.Entity;
using FSK.Sensitivity.Core.Repositories;
using FSK.Sensitivity.Main.Controls;
using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Main.ViewModels
{
    public class UserInfoAddViewModel : BaseViewModel, IDialogAware
    {


        public List<KeyValuePair<string, string>> UserTypes { get; } = EnumExtensions.UserTypes;

        public List<KeyValuePair<string, string>> Genders { get; } = EnumExtensions.Genders;


        private string name;

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }
        private string gender;

        public string Gender
        {
            get { return gender; }
            set { SetProperty(ref gender, value); }
        }
        private string age;

        public string Age
        {
            get { return age; }
            set { SetProperty(ref age, value); }
        }
        private string phone;

        public string Phone
        {
            get { return phone; }
            set { SetProperty(ref phone, value); }
        }
        private string usertype;
        public string UserType
        {
            get { return usertype; }
            set { SetProperty(ref usertype, value); }
        }


        private string title = "人员信息";
        private readonly MangerRepository mangerRepository;

        public UserInfoAddViewModel(MangerRepository mangerRepository)
        {
            this.mangerRepository = mangerRepository;
        }

        private long id=0;

        public DelegateCommand SaveCommand => new DelegateCommand(async () => await Save());

        private async Task Save()
        {
            if (CheckInput())
            {
                Manger manger = new Manger()
                {
                    name = Name,
                    gender = Gender,
                    age = age,
                    phone = Phone,
                    type = UserType,
                };
                if (id == 0)
                {
                    await mangerRepository.Add(manger);
                }
                else
                {
                    manger.Id = id;
                    await mangerRepository.Update(manger);
                }
                AlertMessageBox.Show("保存成功！");
                RequestClose.Invoke(new DialogResult(ButtonResult.OK));
            }
            
        }

        private bool CheckInput()
        {
            if (string.IsNullOrEmpty(Name))
            {
                AlertMessageBox.Show("姓名不能为空！");
                return false;
            }
           
            return true;
        }


        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }
        public DialogCloseListener RequestClose { get; }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            
        }
        public DelegateCommand CloseCommand => new DelegateCommand(Close);

        private void Close()
        {
            RequestClose.Invoke();
        }
        public async void OnDialogOpened(IDialogParameters parameters)
        {
            parameters.TryGetValue("id", out id);
            if (id!= 0)
            {
                var manger = await mangerRepository.QueryById(id);
                if (manger == null)
                {
                    return;
                }
                Name = manger.name;
                Gender = manger.gender;
                Age = manger.age;
                Phone = manger.phone;
                UserType = manger.type;
            }
        }
    }
}
