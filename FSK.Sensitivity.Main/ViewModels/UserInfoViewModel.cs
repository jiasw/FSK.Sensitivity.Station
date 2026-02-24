using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.Entity;
using FSK.Sensitivity.Core.Model;
using FSK.Sensitivity.Core.Repositories;
using Prism.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Main.ViewModels
{
    public class UserInfoViewModel : BaseViewModel, INavigationAware
    {
        private readonly MangerRepository mangerRepository;
        private readonly IDialogService dialogService;
        private ObservableCollection<Manger> mangers = new ObservableCollection<Manger>();
        private int pageIndex = 1;
        private int pageSize = 14;


        public ObservableCollection<Manger> Mangers
        {
            get { return mangers; }
            set { SetProperty(ref mangers, value); }
        }
        public int PageIndex
        {
            get { return pageIndex; }
            set { SetProperty(ref pageIndex, value); }
        }

       

        private int totalPage=-1;
        public int TotalPage
        {
            get { return totalPage; }
            set { SetProperty(ref totalPage, value); }
        }


        public UserInfoViewModel(MangerRepository mangerRepository, IDialogService dialogService)
        {
            this.mangerRepository = mangerRepository;
            this.dialogService = dialogService;
        }

        public DelegateCommand LoadCommand=>new DelegateCommand(async () =>
        {
            await LoadData(PageIndex);
        });

        public DelegateCommand PageUpdatedCmd=>new DelegateCommand(async () =>
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

        private async Task<PageModel<Manger>> LoadData(int pageIndex=1)
        {
            PageModel<Manger> pageModel = await mangerRepository.QueryPage(n => n.Id > 0, pageIndex, pageSize);
            TotalPage = pageModel.pageCount;
            Mangers = new ObservableCollection<Manger>(pageModel.data);
            return pageModel;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _= LoadData();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        public DelegateCommand<Manger> DeleteMangerCommand => new DelegateCommand<Manger>(async (n) =>
        {
            await mangerRepository.DeleteById(n.Id);
            await LoadData(PageIndex);
        });

        public DelegateCommand<Manger> EditMangerCommand => new DelegateCommand<Manger>(async (n) =>
        {
            dialogService.ShowDialog(AppConst.Main_Dialog_Setting_UserInfo_Add, new DialogParameters()
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
            dialogService.ShowDialog(AppConst.Main_Dialog_Setting_UserInfo_Add, new DialogParameters(), async result =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    await LoadData(PageIndex);
                }

            });
        });

        
        
    }
}
