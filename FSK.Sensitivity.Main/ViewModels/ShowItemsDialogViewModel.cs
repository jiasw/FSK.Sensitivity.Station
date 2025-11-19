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

        private string itemsType;

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
            LogHelper.Instance.LogInformation("ShowItemsDialogViewModel opened");
            Title = "选择";
            itemsType = parameters.GetValue<string>("itemsType");
            if (new List<string>() { "1", "2" , "3" }.Contains(itemsType))
            {
                ItemWidth = 200;
                ItenHeight = 80;
                if (itemsType == "1")
                {
                    Title = "眼别";
                    Items = Utils.GetEnumDisplayList<Eye>();
                }
                else if (itemsType == "2")
                {
                    Title = "日夜";
                    Items = Utils.GetEnumDisplayList<DayOrNight>();
                }
                else if (itemsType == "3")
                {
                    Title = "检查距离";
                    Items = Utils.GetEnumDisplayList<CheckDistance>();
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
                { "itemsType", itemsType },
        { "selectedOption", item }
            };
            DialogResult dialogResult = new DialogResult(ButtonResult.OK);
            dialogResult.Parameters = parameters;
            RequestClose.Invoke(dialogResult);
        }

    }
}
