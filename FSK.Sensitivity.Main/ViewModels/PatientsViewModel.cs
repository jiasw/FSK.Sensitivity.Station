using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.Entity;
using FSK.Sensitivity.Core.Model;
using FSK.Sensitivity.Core.Repositories;
using FSK.Sensitivity.Main.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FSK.Sensitivity.Main.ViewModels
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class PatientsViewModel : BaseViewModel
    {
        private readonly PatientRepository patientRepository;
        private readonly IDialogService dialogService;
        private ObservableCollection<Patient> mangers = new ObservableCollection<Patient>();
        private int pageIndex = 1;
        private int pageSize = 10;
        

        public ObservableCollection<Patient> Mangers
        {
            get { return mangers; }
            set { SetProperty(ref mangers, value); }
        }
        public int PageIndex
        {
            get { return pageIndex; }
            set { SetProperty(ref pageIndex, value); }
        }



        private int totalPage;
        public int TotalPage
        {
            get { return totalPage; }
            set { SetProperty(ref totalPage, value); }
        }


        public PatientsViewModel(PatientRepository patientRepository, IDialogService dialogService)
        {
            this.patientRepository = patientRepository;
            this.dialogService = dialogService;
        }

        public DelegateCommand LoadCommand => new DelegateCommand(async () =>
        {
            await LoadData(PageIndex);
        });

        public DelegateCommand PrevCmd => new DelegateCommand(async () =>
        {
            PageIndex--;
            await LoadData(PageIndex);
        });
public DelegateCommand NextCmd => new DelegateCommand(async () =>
        {
            PageIndex++;
            await LoadData(PageIndex);
        });

        private async Task<PageModel<Patient>> LoadData(int pageIndex = 1)
        {
            PageModel<Patient> pageModel = await patientRepository.QueryPage(n => n.IsDeleted == false, pageIndex, pageSize);
            TotalPage = pageModel.pageCount;
            Mangers = new ObservableCollection<Patient>(pageModel.data);
            return pageModel;
        }
        public DelegateCommand<Patient> DeleteMangerCommand => new DelegateCommand<Patient>(async (n) =>
        {
           if( MessageBoxService.Instance.ShowConfirm("确定删除吗？")== MessageBoxResult.Yes)
            {
                n.IsDeleted = true;
                await patientRepository.Update(n);
                await LoadData(PageIndex);
            }
        });

        public DelegateCommand<Patient> EditMangerCommand => new DelegateCommand<Patient>(async (n) =>
        {
            dialogService.ShowDialog(AppConst.Main_Dialog_Register, new DialogParameters()
            {
                { "id", n.Id }
            }, async result =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    await LoadData(PageIndex);
                }

            });
        });

        public DelegateCommand AddMangerCommand => new DelegateCommand(async () =>
        {
            dialogService.ShowDialog(AppConst.Main_Dialog_Register, new DialogParameters(), async result =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    await LoadData(PageIndex);
                }

            });
        });



    }
}
