using FSK.Sensitivity.Core.Entity;
using FSK.Sensitivity.Core.Model;
using FSK.Sensitivity.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Main.ViewModels
{
    public class UserInfoViewModel : BaseViewModel
    {
        private readonly MangerRepository mangerRepository;
        private ObservableCollection<Manger> mangers = new ObservableCollection<Manger>();
        private int pageIndex = 1;
        private int pageSize = 10;


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

       

        private int totalPage;
        public int TotalPage
        {
            get { return totalPage; }
            set { SetProperty(ref totalPage, value); }
        }


        public UserInfoViewModel(MangerRepository mangerRepository)
        {
            this.mangerRepository = mangerRepository;
        }

        public DelegateCommand LoadCommand=>new DelegateCommand(async () =>
        {
            await LoadData(PageIndex);
        });

        public DelegateCommand PageUpdatedCmd=>new DelegateCommand(async () =>
        {
            await LoadData(PageIndex);
        });

        private async Task<PageModel<Manger>> LoadData(int pageIndex=1)
        {
            PageModel<Manger> pageModel = await mangerRepository.QueryPage(n => n.Id > 0, pageIndex, pageSize);
            TotalPage = pageModel.pageCount;
            Mangers = new ObservableCollection<Manger>(pageModel.data);
            return pageModel;
        }
        public DelegateCommand<Manger> DeleteCommand => new DelegateCommand<Manger>(async (n) =>
        {
            await mangerRepository.DeleteById(n.Id);
            await LoadData(PageIndex);
        });



        
        
    }
}
