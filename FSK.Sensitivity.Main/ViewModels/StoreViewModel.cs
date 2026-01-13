using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.Repositories;
using FSK.Sensitivity.Main.Controls;
using HandyControl.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Xps.Packaging;

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

        private string _code;
        public string Code
        {
            get { return _code; }
            set { SetProperty(ref _code, value); }
        }

        private string _image;
        public string Image
        {
            get => _image;
            set
            {
               SetProperty(ref _image, value);
                RaisePropertyChanged(nameof(HasImage));
                RaisePropertyChanged(nameof(FullImagePath));
            }
        }
        // 辅助属性：用于判断是否存在图片
        public bool HasImage => !string.IsNullOrEmpty(_image);

        // 拼接后的完整路径
        public string FullImagePath
        {
            get
            {
                if (string.IsNullOrEmpty(Image)) return null;

                // 假设图片存储在程序运行目录下的 Images 文件夹中
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                return System.IO.Path.Combine(baseDirectory, "Resource", "Images", Image);
            }
        }

        public DelegateCommand LoadStoreCommand => new DelegateCommand( async () => await Load());

        private async Task Load()
        {
            string[] dictcodes=new string[] { AppConst.Dict_TypeCode_StoreName, AppConst.Dict_TypeCode_StoreCode, AppConst.Dict_TypeCode_StoreLogo };

            var storeDict = await dictRepository.Query(n => dictcodes.Contains(n.TypeCode));
            if (storeDict!= null && storeDict.Count > 0)
            {
                Name = GetDictNameByTypeCode(AppConst.Dict_TypeCode_StoreName, storeDict);
                Code = GetDictNameByTypeCode(AppConst.Dict_TypeCode_StoreCode, storeDict);
                Image = GetDictNameByTypeCode(AppConst.Dict_TypeCode_StoreLogo, storeDict);
            }
        }

        public DelegateCommand UploadLogoCommand => new DelegateCommand(  UploadLogo);

        private void UploadLogo()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
            ofd.Multiselect = false;
            if (ofd.ShowDialog()==true)
            {
                //将文件保存到指定目录
                string fileName = System.IO.Path.GetFileName(ofd.FileName);
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string destDir = System.IO.Path.Combine(baseDirectory, "Resource", "Images");
                if (!System.IO.Directory.Exists(destDir))
                {
                    System.IO.Directory.CreateDirectory(destDir);
                }
                string destPath = System.IO.Path.Combine(destDir, fileName);
                System.IO.File.Copy(ofd.FileName, destPath, true);
                Image = fileName;
            }

        }

        public string GetDictNameByTypeCode(string typeCode, List<Core.Entity.Dict> dicts)
        {
            var dict = dicts.FirstOrDefault(d => d.TypeCode == typeCode);
            return dict != null ? dict.Name : string.Empty;
        }


        public DelegateCommand SaveCommand => new DelegateCommand( async () => await Save());

        private async Task saveName()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return;
            }
            var storeDict = await dictRepository.Query(n => n.TypeCode == AppConst.Dict_TypeCode_StoreName);
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
                    TypeCode = AppConst.Dict_TypeCode_StoreName,
                    IsDeleted = false
                });
            }
        }
        private async Task SaveCode()
        {
            if (string.IsNullOrEmpty(Code))
            {
                return;
            }
            var storeDict = await dictRepository.Query(n => n.TypeCode == AppConst.Dict_TypeCode_StoreCode);
            if (storeDict != null && storeDict.Count > 0)
            {
                storeDict[0].Name = Code;
                await dictRepository.Update(storeDict[0]);
            }
            else
            {
                await dictRepository.Add(new Core.Entity.Dict()
                {
                    Code = "01",
                    Name = Code,
                    TypeCode = AppConst.Dict_TypeCode_StoreCode,
                    IsDeleted = false
                });
            }
        }

        private async Task SaveLogo()
        {
            if (string.IsNullOrEmpty(Image))
            {
                return;
            }
            var storeDict = await dictRepository.Query(n => n.TypeCode == AppConst.Dict_TypeCode_StoreLogo);
            if (storeDict != null && storeDict.Count > 0)
            {
                storeDict[0].Name = Image;
                await dictRepository.Update(storeDict[0]);
            }
            else
            {
                await dictRepository.Add(new Core.Entity.Dict()
                {
                    Code = "01",
                    Name = Image,
                    TypeCode = AppConst.Dict_TypeCode_StoreLogo,
                    IsDeleted = false
                });
            }
        }

        private async Task Save()
        {
            _ = saveName();
            _ = SaveCode();
            _ = SaveLogo();


            MessageBoxService.Instance.Show("保存成功");

        }
    }
}
