using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.Repositories;
using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Main.ViewModels
{
    public class StoreViewModel : BaseViewModel
    {
        public StoreViewModel(DictRepository dictRepository)
        {
            this.dictRepository = dictRepository;
        }

        private string _name;
        private readonly DictRepository dictRepository;

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public DelegateCommand LoadStoreCommand => new DelegateCommand( async () => await Load());

        private async Task Load()
        {
            var storeDict = await dictRepository.Query(n => n.TypeCode == AppConst.Dict_TypeCode_Store);
            if (storeDict!= null && storeDict.Count > 0)
            {
                Name = storeDict[0].Name;
            }
        }

        public DelegateCommand SaveCommand => new DelegateCommand( async () => await Save());

        private async Task Save()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return;
            }
            var storeDict = await dictRepository.Query(n => n.TypeCode == AppConst.Dict_TypeCode_Store);
            if (storeDict != null && storeDict.Count > 0)
            {
                storeDict[0].Name = Name;
                await dictRepository.Update(storeDict[0]);
            }
            else
            {
                await dictRepository.Add(new Core.Entity.Dict()
                {
                    Code = "01",
                    Name = Name,
                    TypeCode= AppConst.Dict_TypeCode_Store,
                    IsDeleted = false
                });
                    
            }
            MessageBox.Show("保存成功");
                
        }
    }
}
