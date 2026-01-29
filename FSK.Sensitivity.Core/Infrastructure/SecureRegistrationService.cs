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
        void SaveRegistration();

        string LoadRegistration();

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
        public void SaveRegistration()
        {
            string licenseKey="FSK.Sensitivity.Core.Infrastructure.SecureRegistrationService";
            Directory.CreateDirectory(Path.GetDirectoryName(_registrationPath));

            byte[] dataToEncrypt = Encoding.UTF8.GetBytes(licenseKey);
            byte[] encryptedData = ProtectedData.Protect(
                dataToEncrypt,
                null,
                _scope);

            File.WriteAllBytes(_registrationPath, encryptedData);
        }
        public string LoadRegistration()
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

                return Encoding.UTF8.GetString(decryptedData);
            }
            catch
            {
                return null;
            }
        }

        public bool IsRegistered() {
            return !string.IsNullOrEmpty(LoadRegistration());
        
        }

    }
}
