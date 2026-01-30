using FSK.Sensitivity.Core.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Infrastructure
{

    public interface ISecureRegistrationService
    {
        void SaveRegistration(DeviceRegisterModel model);

        DeviceRegisterModel? LoadRegistration();

        bool IsRegistered();

    }

    public class SecureRegistrationService: ISecureRegistrationService
    {
        private readonly string _registrationPath;
        private readonly DataProtectionScope _scope =
            DataProtectionScope.CurrentUser;
        public SecureRegistrationService()
        {
            string appDataPath = Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData);

            _registrationPath = Path.Combine(
                appDataPath,
                "FSK.Sensitivity",
                "registration.bin");
        }
        public void SaveRegistration(DeviceRegisterModel model)
        {
            string licenseKey= JsonConvert.SerializeObject(model);
            Directory.CreateDirectory(Path.GetDirectoryName(_registrationPath));

            byte[] dataToEncrypt = Encoding.UTF8.GetBytes(licenseKey);
            byte[] encryptedData = ProtectedData.Protect(
                dataToEncrypt,
                null,
                _scope);

            File.WriteAllBytes(_registrationPath, encryptedData);
        }
        public DeviceRegisterModel? LoadRegistration()
        {
            if (!File.Exists(_registrationPath))
                return null;
            try
            {
                byte[] encryptedData = File.ReadAllBytes(_registrationPath);
                byte[] decryptedData = ProtectedData.Unprotect(
                    encryptedData,
                    null,
                    _scope);

                string jsonData = Encoding.UTF8.GetString(decryptedData);
                return JsonConvert.DeserializeObject<DeviceRegisterModel>(jsonData);
            }
            catch
            {
                return null;
            }
        }

        public bool IsRegistered()
        {
            DeviceRegisterModel? registration = LoadRegistration();
            if (registration == null) return false;
            return true;
        }

    }
}
