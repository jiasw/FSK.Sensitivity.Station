using FSK.Sensitivity.Core.Const;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Main.Controls
{
    public class AlertMessageBox
    {
        public AlertMessageBox()
        {
        }

        public static void Show(string message,Action onConfirm=null)
        {
            DialogParameters parameters = new DialogParameters();
            parameters.Add(AppConst.Main_Dialog_AlertMsg, message);
            AppData.Instance.DialogService.ShowDialog(AppConst.Main_Dialog_AlertMsg, parameters, result =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    if (onConfirm!= null)
                    {
                        onConfirm();
                    }
                }
            });
        }
    }
}
