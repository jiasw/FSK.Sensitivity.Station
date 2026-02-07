using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Utility
{
    public static class StringExtend
    {
        
            // DES IV (初始化向量)
            private static readonly byte[] DesKeys = { 0xE2, 0xF4, 0xC6, 0xA8, 0xB0, 0xDB, 0xC3, 0x19 };
            /// <summary>
            /// DES加密字符串
            /// </summary>
            /// <param name="encryptString">待加密的字符串</param>
            /// <param name="encryptKey">加密密钥,要求为8位</param>
            /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
            public static string DesEncrypt(this string encryptString, string encryptKey = "FSKGROUP")
            {
                try
                {
                    if (string.IsNullOrEmpty(encryptString))
                        return encryptString;
                    // 取前8位作为密钥
                    byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, Math.Min(8, encryptKey.Length)).PadRight(8, ' '));
                    byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                    using (var des = DES.Create())
                    {
                        des.Key = rgbKey;
                        des.IV = DesKeys;
                        des.Mode = CipherMode.CBC;
                        des.Padding = PaddingMode.PKCS7;
                        using (var encryptor = des.CreateEncryptor(des.Key, des.IV))
                        using (var mStream = new MemoryStream())
                        {
                            using (var cStream = new CryptoStream(mStream, encryptor, CryptoStreamMode.Write))
                            {
                                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                                cStream.FlushFinalBlock();
                                return Convert.ToBase64String(mStream.ToArray());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 失败返回源串
                    return encryptString;
                }
            }
            /// <summary>
            /// DES解密字符串
            /// </summary>
            /// <param name="decryptString">待解密的字符串</param>
            /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
            /// <returns>解密成功返回解密后的字符串，失败返回源串</returns>
            public static string DesDecrypt(this string decryptString, string decryptKey = "FSKGROUP")
            {
                try
                {
                    if (string.IsNullOrEmpty(decryptString))
                        return decryptString;
                    byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey.Substring(0, Math.Min(8, decryptKey.Length)).PadRight(8, ' '));
                    byte[] inputByteArray = Convert.FromBase64String(decryptString);
                    using (var des = DES.Create())
                    {
                        des.Key = rgbKey;
                        des.IV = DesKeys;
                        des.Mode = CipherMode.CBC;
                        des.Padding = PaddingMode.PKCS7;
                        using (var decryptor = des.CreateDecryptor(des.Key, des.IV))
                        using (var mStream = new MemoryStream())
                        {
                            using (var cStream = new CryptoStream(mStream, decryptor, CryptoStreamMode.Write))
                            {
                                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                                cStream.FlushFinalBlock();
                                return Encoding.UTF8.GetString(mStream.ToArray());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 失败返回源串
                    return decryptString;
                }
            }
            /// <summary>
            /// TripleDES加密（安全性更高）
            /// </summary>
            /// <param name="encryptString">待加密的字符串</param>
            /// <param name="encryptKey">加密密钥,要求为16位或24位</param>
            /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
            public static string TripleDesEncrypt(this string encryptString, string encryptKey = "FSKGROUPFSKGROUP")
            {
                try
                {
                    if (string.IsNullOrEmpty(encryptString))
                        return encryptString;
                    // 密钥必须是16或24字节
                    var keyBytes = Encoding.UTF8.GetBytes(encryptKey);
                    if (keyBytes.Length < 16)
                        Array.Resize(ref keyBytes, 16);
                    else if (keyBytes.Length < 24)
                        Array.Resize(ref keyBytes, 24);
                    else if (keyBytes.Length > 24)
                        Array.Resize(ref keyBytes, 24);
                    byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                    using (var des = TripleDES.Create())
                    {
                        des.Key = keyBytes;
                        des.IV = DesKeys;
                        des.Mode = CipherMode.CBC;
                        des.Padding = PaddingMode.PKCS7;
                        using (var encryptor = des.CreateEncryptor(des.Key, des.IV))
                        using (var mStream = new MemoryStream())
                        {
                            using (var cStream = new CryptoStream(mStream, encryptor, CryptoStreamMode.Write))
                            {
                                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                                cStream.FlushFinalBlock();
                                return Convert.ToBase64String(mStream.ToArray());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return encryptString;
                }
            }
            /// <summary>
            /// TripleDES解密
            /// </summary>
            /// <param name="decryptString">待解密的字符串</param>
            /// <param name="decryptKey">解密密钥,要求为16位或24位</param>
            /// <returns>解密成功返回解密后的字符串，失败返回源串</returns>
            public static string TripleDesDecrypt(this string decryptString, string decryptKey = "FSKGROUPFSKGROUP")
            {
                try
                {
                    if (string.IsNullOrEmpty(decryptString))
                        return decryptString;
                    var keyBytes = Encoding.UTF8.GetBytes(decryptKey);
                    if (keyBytes.Length < 16)
                        Array.Resize(ref keyBytes, 16);
                    else if (keyBytes.Length < 24)
                        Array.Resize(ref keyBytes, 24);
                    else if (keyBytes.Length > 24)
                        Array.Resize(ref keyBytes, 24);
                    byte[] inputByteArray = Convert.FromBase64String(decryptString);
                    using (var des = TripleDES.Create())
                    {
                        des.Key = keyBytes;
                        des.IV = DesKeys;
                        des.Mode = CipherMode.CBC;
                        des.Padding = PaddingMode.PKCS7;
                        using (var decryptor = des.CreateDecryptor(des.Key, des.IV))
                        using (var mStream = new MemoryStream())
                        {
                            using (var cStream = new CryptoStream(mStream, decryptor, CryptoStreamMode.Write))
                            {
                                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                                cStream.FlushFinalBlock();
                                return Encoding.UTF8.GetString(mStream.ToArray());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return decryptString;
                }
            }

        private const string aesKey = "zzjs32415fskgroup987xiejingguang";
        private const string aesIv = "jsxxxiejingguang";
        // <summary>
        /// AES加密
        /// </summary>
        /// <param name="aesModel"></param>
        /// <returns></returns>
        public static string AESEncrypt(this string source, string key = aesKey)
        {

            if (string.IsNullOrEmpty(source))
            {
                return source;
            }
            //使用32位密钥
            byte[] key32 = new byte[32];
            //如果我们的密钥不是32为，则自动补全到32位
            byte[] byteKey = Encoding.UTF8.GetBytes(key.PadRight(key32.Length));
            //复制密钥
            Array.Copy(byteKey, key32, key32.Length);

            //使用16位向量
            byte[] iv16 = new byte[16];
            //如果我们的向量不是16为，则自动补全到16位
            byte[] byteIv = Encoding.UTF8.GetBytes(aesIv.PadRight(iv16.Length));
            //复制向量
            Array.Copy(byteIv, iv16, iv16.Length);

            // 创建加密对象,Rijndael 算法
            //Rijndael RijndaelAes = Rijndael.Create();
            RijndaelManaged RijndaelAes = new RijndaelManaged();
            RijndaelAes.Mode = CipherMode.CBC;
            RijndaelAes.Padding = PaddingMode.PKCS7;
            RijndaelAes.Key = key32;
            RijndaelAes.IV = iv16;
            byte[] result = null;
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream EncryptStream = new CryptoStream(ms, RijndaelAes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        var data = Encoding.UTF8.GetBytes(source);
                        EncryptStream.Write(data, 0, data.Length);
                        EncryptStream.FlushFinalBlock();
                        result = ms.ToArray();
                    }
                }
                return Convert.ToBase64String(result);
            }
            catch { }
            return string.Empty;
        }


        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="aesModel"></param>
        /// <returns></returns>
        public static string AESDecrypt(this string source, string key = aesKey)
        {
            if (key == null)
            {
                key = aesKey;
            }
            if (string.IsNullOrEmpty(source))
            {
                return source;
            }
            //使用32位密钥
            byte[] key32 = new byte[32];
            //如果我们的密钥不是32为，则自动补全到32位
            byte[] byteKey = Encoding.UTF8.GetBytes(key.PadRight(key32.Length));
            //复制密钥
            Array.Copy(byteKey, key32, key32.Length);

            //使用16位向量
            byte[] iv16 = new byte[16];
            //如果我们的向量不是16为，则自动补全到16位
            byte[] byteIv = Encoding.UTF8.GetBytes(aesIv.PadRight(iv16.Length));
            //复制向量
            Array.Copy(byteIv, iv16, iv16.Length);

            // 创建解密对象,Rijndael 算法
            //Rijndael RijndaelAes = Rijndael.Create();
            RijndaelManaged RijndaelAes = new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                Key = key32,
                IV = iv16
            };
            byte[] result = null;
            try
            {
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(source)))
                {
                    using (CryptoStream DecryptStream = new CryptoStream(ms, RijndaelAes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (MemoryStream msResult = new MemoryStream())
                        {
                            byte[] temp = new byte[1024 * 1024];
                            int len = 0;
                            while ((len = DecryptStream.Read(temp, 0, temp.Length)) > 0)
                            {
                                msResult.Write(temp, 0, len);
                            }

                            result = msResult.ToArray();
                        }
                    }
                }
                return Encoding.UTF8.GetString(result);
            }
            catch { }
            return string.Empty;
        }


        public static void CopyTo(this object obj, object target)
        {
            if (obj != null)
            {
                var pArray = obj.GetType().GetProperties();
                foreach (var p in pArray)
                {
                    try
                    {
                        var tp = target.GetType().GetProperty(p.Name);
                        if (tp != null && p.GetValue(obj) != null)
                        {
                            var value = p.GetValue(obj);
                            if (value is Nullable<long>)
                            {
                                value = (value as Nullable<long>).Value;
                            }
                            if (value is Nullable<int>)
                            {
                                value = (value as Nullable<int>).Value;
                            }
                            if (value is Nullable<DateTime>)
                            {
                                value = (value as Nullable<DateTime>).Value;
                            }
                            if (value is Nullable<decimal>)
                            {
                                value = (value as Nullable<decimal>).Value;
                            }
                            if (value is Nullable<float>)
                            {
                                value = (value as Nullable<float>).Value;
                            }
                            tp.SetValue(target, value);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message);
                    }

                }
            }
        }
        public static DateTime ToDateTime(this object obj)
        {
            if (obj != null)
            {
                bool result = DateTime.TryParse(obj.ToString(), out DateTime value);
                return result ? value : DateTime.Now;
            }
            return DateTime.Now;
        }

        public static bool BaseValueEquels(this object obj, object taget)
        {
            if (obj is Int32 intA && taget is Int32 intB)
            {
                return intA == intB;
            }
            if (obj is Decimal decA && taget is Decimal decB)
            {
                return decA == decB;
            }
            if (obj is Int64 longA && taget is Int64 longB)
            {
                return longA == longB;
            }
            if (obj is Int16 shortA && taget is Int16 shortB)
            {
                return shortA == shortB;
            }
            if (obj is String strA && taget is String strB)
            {
                return strA == strB;
            }
            return false;
        }
        public static bool IsEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static string ToLensName(this string code)
        {
            if (code.IsEmpty())
            {
                return "空";
            }
            else
            {
                return code;
            }
        }

        public static int ParseLensType(this string str)
        {
            if (str.IsNotEmpty())
            {
                if (str.StartsWith("+"))
                {
                    return 0;
                }
                else if (str.StartsWith("-"))
                {
                    return 1;
                }
                else if (str.ToUpper().EndsWith("BI"))
                {
                    return 2;
                }
                else if (str.ToUpper().EndsWith("BO"))
                {
                    return 3;
                }
            }
            return -1;
        }

        public static bool IsNotEmpty(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }

        public static int ToInt(this object obj, int? defValue = null)
        {
            if (obj == null)
            {
                if (defValue != null)
                {
                    return defValue.Value;
                }
                return 0;
            }

            if (obj is Double dou)
            {
                return Int32.Parse(dou.ToString("F0"));
            }
            if (obj is Decimal dec)
            {
                return Int32.Parse(dec.ToString("F0"));
            }
            if (obj is float f)
            {
                return Int32.Parse(f.ToString("F0"));
            }
            var r = int.TryParse(obj.ToString().ClearSymbol(), out int s);
            return r ? (s == 0 && defValue != null ? defValue.Value : s) : (defValue != null ? defValue.Value : 0);
        }

        public static T ToValue<T>(this string value)
        {
            if (typeof(T) == typeof(Int32))
            {
                var val = value.ToInt();
                if (val is T t)
                {
                    return t;
                }
            }
            else if (typeof(T) == typeof(string) && value is T t)
            {
                return t;
            }
            
            return default;
        }

        public static decimal ToDecimal(this object obj, decimal? defValue = null)
        {
            if (obj == null)
            {
                if (defValue != null)
                {
                    return defValue.Value;
                }
                return 0M;
            }
            if (obj is Int32 i3)
            {
                return i3 * 1.0M;
            }
            if (obj is Int16 i1)
            {
                return i1 * 1.0M;
            }
            if (obj is Int64 i6)
            {
                return i6 * 1.0M;
            }
            var r = decimal.TryParse(obj.ToString().ClearSymbol(), out decimal s);
            return r ? (s == 0 && defValue != null ? defValue.Value : s) : (defValue != null ? defValue.Value : 0M);
        }
        public static double ToDouble(this object obj, double? defValue = null)
        {
            if (obj == null)
            {
                if (defValue != null)
                {
                    return defValue.Value;
                }
                return 0D;
            }
            var r = double.TryParse(obj.ToString().ClearSymbol(), out double s);
            return r ? (s == 0 && defValue != null ? defValue.Value : s) : (defValue != null ? defValue.Value : 0D);
        }

        public static bool ToBool(this string src)
        {
            if (src.ToLower() == "true" || src == "1" || src == "是" || src.ToLower() == "on")
            {
                return true;
            }
            return false;
        }

        public static bool IsNumber(this string code)
        {
            Regex r = new Regex("^(-)?\\d+(\\.)?\\d+$");
            return r.IsMatch(code.Replace("+", ""));
        }

        public static string ClearSymbol(this string src)
        {
            Regex r = new Regex("\\+|\\s+|[A-Z|a-z]|_");

            return r.Replace(src, "");
        }

        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        public static T ToObject<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static string ToHexString(this byte[] data)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(" ");
                }
                sb.Append(data[i].ToString("x2"));
            }
            return sb.ToString();
        }
        

        public static List<T> ToListObj<T>(this DataTable table) where T : class, new()
        {
            var list = new List<T>();
            foreach (DataRow row in table.Rows)
            {
                var entity = new T();
                foreach (var item in entity.GetType().GetProperties())
                {
                    if (row.Table.Columns.Contains(item.Name))
                    {

                        if (DBNull.Value != row[item.Name])
                        {
                            var value = row[item.Name];
                            if (value is long l && item.PropertyType == typeof(Nullable<long>))
                            {
                                value = new Nullable<long>(l);
                            }
                            if (value is int i && item.PropertyType == typeof(Nullable<int>))
                            {
                                value = new Nullable<int>(i);
                            }
                            if (value is DateTime d && item.PropertyType == typeof(Nullable<DateTime>))
                            {
                                value = new Nullable<DateTime>(d);
                            }
                            if (value is decimal dec && item.PropertyType == typeof(Nullable<decimal>))
                            {
                                value = new Nullable<decimal>(dec);
                            }
                            if (value is float flt && item.PropertyType == typeof(Nullable<float>))
                            {
                                value = new Nullable<float>(flt);
                            }
                            if (value is double db && item.PropertyType == typeof(Nullable<double>))
                            {
                                value = new Nullable<double>(db);
                            }
                            if (item.PropertyType == typeof(string))
                            {
                                value = value.ToString();
                            }
                            item.SetValue(entity, value);
                        }
                    }
                }
                list.Add(entity);
            }
            return list;
        }

    }

}