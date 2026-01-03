using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace CrosscuttingUtiles
{
    public class Cifrado
    {
        private const string KEY = "5354D0F77642BE2A36B3D5BBA4FDFD";
        private const string password = "5354D0F77642BE2A36B3D5BBA4FDFD";
        private const string salt = "5354D0F77642BE2A36B3D5BBA4FDFD";

        public static string Encriptar(object value)
        {
            DeriveBytes rgb = new Rfc2898DeriveBytes(password, Encoding.Unicode.GetBytes(salt));
            SymmetricAlgorithm algorithm = new TripleDESCryptoServiceProvider();
            byte[] rgbKey = rgb.GetBytes(algorithm.KeySize >> 3);
            byte[] rgbIV = rgb.GetBytes(algorithm.BlockSize >> 3);
            ICryptoTransform transform = algorithm.CreateEncryptor(rgbKey, rgbIV);
            using (MemoryStream buffer = new MemoryStream())
            {
                using (CryptoStream stream = new CryptoStream(buffer, transform, CryptoStreamMode.Write))
                {
                    using (StreamWriter writer = new StreamWriter(stream, Encoding.Unicode))
                    {
                        writer.Write(value);
                    }
                }
                return Convert.ToBase64String(buffer.ToArray());
            }
        }

        public static string Desencriptar(string text)
        {
            DeriveBytes rgb = new Rfc2898DeriveBytes(password, Encoding.Unicode.GetBytes(salt));
            SymmetricAlgorithm algorithm = new TripleDESCryptoServiceProvider();
            byte[] rgbKey = rgb.GetBytes(algorithm.KeySize >> 3);
            byte[] rgbIV = rgb.GetBytes(algorithm.BlockSize >> 3);
            ICryptoTransform transform = algorithm.CreateDecryptor(rgbKey, rgbIV);
            using (MemoryStream buffer = new MemoryStream(Convert.FromBase64String(text)))
            {
                using (CryptoStream stream = new CryptoStream(buffer, transform, CryptoStreamMode.Read))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.Unicode))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        public static string Cifrar(string Input)
        {
            try
            {
                byte[] IV = ASCIIEncoding.ASCII.GetBytes("qualityi");
                byte[] EncryptionKey = Convert.FromBase64String("rpaSPvIcVLlrcSt9PK9/c67Gkj7yL1S5");
                byte[] buffer = Encoding.UTF8.GetBytes(Input);
                TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
                des.Key = EncryptionKey;
                des.IV = IV;

                return Convert.ToBase64String(des.CreateEncryptor().TransformFinalBlock(buffer, 0, buffer.Length));
            }
            catch 
            {
                return "";
            }
        }
        public string DesCifrar(string Input)
        {
            byte[] IV = ASCIIEncoding.ASCII.GetBytes("qualityi"); // La clave debe ser de 8 caracteres
            byte[] EncryptionKey = Convert.FromBase64String("rpaSPvIcVLlrcSt9PK9/c67Gkj7yL1S5"); // No se puede alterar la cantidad de caracteres pero si la clave
            byte[] buffer = Convert.FromBase64String(Input);
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Key = EncryptionKey;
            des.IV = IV;
            return Encoding.UTF8.GetString(des.CreateDecryptor().TransformFinalBlock(buffer, 0, buffer.Length));
        }

    }
}
