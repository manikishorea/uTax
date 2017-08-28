using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CrossLinkAPIService
{
    public static class PasswordManager
    {

        private static byte[] key = new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };
        private static byte[] iv = new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };

        private static string PasswordKey = "PAS$WO&3";
        private static string PasswordIV = "PAS$WO&3";
        /// <summary>
        /// This method is used to hash the input string based on a Key specified
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="Key"></param>
        /// <param name="hasher"></param>
        /// <returns></returns>
        public static string HashText(string inputString, string Key)
        {
            try
            {
                byte[] textWithSaltBytes = Encoding.UTF8.GetBytes(string.Concat(inputString, Key));
                HashAlgorithm hasher = new SHA1CryptoServiceProvider();
                byte[] hashedBytes = hasher.ComputeHash(textWithSaltBytes);
                hasher.Clear();
                return Convert.ToBase64String(hashedBytes);
            }
            finally
            {
            }
        }

        /// <summary>
        /// To get a random 8 digit alphanumeric string
        /// </summary>
        /// <returns>random 8 digit alphanumeric string</returns>
        public static string GetRandomString()
        {
            try
            {
                return Guid.NewGuid().ToString().Substring(0, 8);
            }
            finally
            {
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string CryptText(string text)
        {
            byte[] KeySaltBytes = Encoding.UTF8.GetBytes(PasswordKey);
            byte[] VISaltBytes = Encoding.UTF8.GetBytes(PasswordIV);

            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateEncryptor(KeySaltBytes, VISaltBytes);
            byte[] inputbuffer = Encoding.Unicode.GetBytes(text);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Convert.ToBase64String(outputBuffer);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string DecryptText(string text)
        {
            byte[] KeySaltBytes = Encoding.UTF8.GetBytes(PasswordKey);
            byte[] VISaltBytes = Encoding.UTF8.GetBytes(PasswordIV);

            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateDecryptor(KeySaltBytes, VISaltBytes);
            byte[] inputbuffer = Convert.FromBase64String(text);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Encoding.Unicode.GetString(outputBuffer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Crypt(this string text)
        {
            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateEncryptor(key, iv);
            byte[] inputbuffer = Encoding.Unicode.GetBytes(text);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Convert.ToBase64String(outputBuffer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Decrypt(this string text)
        {
            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateDecryptor(key, iv);
            byte[] inputbuffer = Convert.FromBase64String(text);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Encoding.Unicode.GetString(outputBuffer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        private static byte[] KEY_192 = {
        42,
        16,
        93,
        156,
        78,
        4,
        218,
        32,
        15,
        167,
        44,
        80,
        26,
        250,
        155,
        112,
        2,
        94,
        11,
        204,
        119,
        35,
        184,
        197
    };

        /// <summary>
        /// 
        /// </summary>
        private static byte[] IV_192 = {
        55,
        103,
        246,
        79,
        36,
        99,
        167,
        3,
        42,
        5,
        62,
        83,
        184,
        7,
        209,
        13,
        145,
        23,
        200,
        58,
        173,
        10,
        121,
        222
    };
        //    //TRIPLE DES encryption
        //    private static string EncryptTripleDES(string value)
        //    {
        //        TripleDESCryptoServiceProvider cryptoProvider = new TripleDESCryptoServiceProvider();
        //        MemoryStream ms = new MemoryStream();
        //        CryptoStream cs = new CryptoStream(ms, cryptoProvider.CreateEncryptor(KEY_192, IV_192), CryptoStreamMode.Write);
        //        StreamWriter sw = new StreamWriter(cs);

        //        sw.Write(value);
        //        sw.Flush();
        //        cs.FlushFinalBlock();
        //        ms.Flush();

        //        //convert back to a string
        //        return Convert.ToBase64String(ms.GetBuffer(), 0, Convert.ToInt32(ms.Length));
        //    }

        //TRIPLE DES decryption 
        private static string DecryptTripleDES(string value)
        {
            TripleDESCryptoServiceProvider cryptoProvider = new TripleDESCryptoServiceProvider();
            //convert from string to byte array
            byte[] buffer = Convert.FromBase64String(value);
            MemoryStream ms = new MemoryStream(buffer);
            CryptoStream cs = new CryptoStream(ms, cryptoProvider.CreateDecryptor(KEY_192, IV_192), CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ActivationKey"></param>
        /// <returns></returns>
        public static string[] GetAttributesFromKey(string ActivationKey)
        {
            string DecriptActivationKey = DecryptTripleDES(ActivationKey);
            string[] ActivationKeysWithAttributes = DecriptActivationKey.Split('^');
            string[] ArrayAttributes = null;
            string stringDeCriptedAttributes = null;
            string[] ArrayDeCriptedAttributes = null;
            string AttributeKey = string.Empty;
            if (ActivationKeysWithAttributes.Length > 1)
            {
                ActivationKey = ActivationKeysWithAttributes[0].ToString();
                AttributeKey = ActivationKeysWithAttributes[1].ToString();
            }
            if (!string.IsNullOrEmpty(AttributeKey))
            {
                ArrayAttributes = AttributeKey.Split('*');
                foreach (string itemAttribute in ArrayAttributes)
                {
                    if (string.IsNullOrEmpty(stringDeCriptedAttributes))
                    {
                        stringDeCriptedAttributes = DecryptTripleDES(itemAttribute);
                    }
                    else
                    {
                        stringDeCriptedAttributes += "*" + DecryptTripleDES(itemAttribute);
                    }

                }

                if (!string.IsNullOrEmpty(stringDeCriptedAttributes))
                {
                    ArrayDeCriptedAttributes = stringDeCriptedAttributes.Split('*');
                }


            }
            return ArrayDeCriptedAttributes;

        }

    }
}
