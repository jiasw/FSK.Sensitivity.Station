using FSK.Sensitivity.Core;
using FSK.Sensitivity.Core.Enums;
using FSK.Sensitivity.Core.Infrastructure;
using FSK.Sensitivity.Core.Model;
using FSK.Sensitivity.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Main.ViewModels
{
   

    public class ShowItemsDialogViewModel : BaseViewModel, IDialogAware
    {

        private double itemWidth = 100;
        public double ItemWidth
        {
            get { return itemWidth; }
            set { SetProperty(ref itemWidth, value); }
        }

        private double itenHeight = 20;
        public double ItenHeight
        {
            get { return itenHeight; }
            set { SetProperty(ref itenHeight, value); }
        }

        private List<ShowItemsModel> items;
        public List<ShowItemsModel> Items
        {
            get { return items; }
            set { SetProperty(ref items, value); }
        }

        private ItemsType itemsType;

        private string title;
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

        public void OnDialogOpened(IDialogParameters parameters)
        {
            Title = "选择";
            itemsType = parameters.GetValue<ItemsType>(nameof(ItemsType));
            if (new List<ItemsType>() { ItemsType.DayTypes, ItemsType.Eyes, ItemsType.Distance,ItemsType.DarkTime }.Contains(itemsType))
            {
                ItemWidth = 200;
                ItenHeight = 80;
                if (itemsType == ItemsType.Eyes)
                {
                    Title = "眼别";
                    Items = Utils.GetEnumDisplayList<Eye>();
                }
                else if (itemsType == ItemsType.DayTypes)
                {
                    Title = "日夜";
                    Items = Utils.GetEnumDisplayList<DayOrNight>();
                }
                else if (itemsType == ItemsType.Distance)
                {
                    Title = "检查距离";
                    Items = EnumExtensions.ToEnumModelList<CheckDistance>().Select(i => new ShowItemsModel() { Name = i.Description, Value = i.Value.ToString() }).ToList();
                }
                else if (itemsType == ItemsType.DarkTime)
                {
                    Title = "暗环境时长";
                    Items = EnumExtensions.ToEnumModelList<DCKTime>().Select(i => new ShowItemsModel() { Name = i.Description, Value = i.Value.ToString() }).ToList();
                }
            }
            else
            {
                Title = "瞳距";
                ItemWidth = 130;
                ItenHeight = 60;
                Items = Enumerable.Range(50, 31).Select(i => new ShowItemsModel() { Name = i.ToString(), Value = i.ToString() }).ToList();
            }
        }

        public DelegateCommand CloseCommand => new DelegateCommand(Close);

        private void Close()
        {
            DialogResult dialogResult = new DialogResult(ButtonResult.Cancel);
            RequestClose.Invoke(dialogResult);
        }

        public DelegateCommand<ShowItemsModel> SelectCommand => new DelegateCommand<ShowItemsModel>(Select);

        private void Select(ShowItemsModel item)
        {
            var parameters = new DialogParameters {
                { nameof(ItemsType), itemsType },
        { "selectedOption", item }
            };
            DialogResult dialogResult = new DialogResult(ButtonResult.OK);
            dialogResult.Parameters = parameters;
            RequestClose.Invoke(dialogResult);
        }

    }
}
