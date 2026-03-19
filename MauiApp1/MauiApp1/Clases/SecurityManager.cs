using System.Security.Cryptography;
using System.Text;

namespace iAlmacen.Clases;

public class SecurityManager
{
    //static readonly string password = "m@p5@2025";
    //public static string Encrypt(string plainText)
    //{
    //	if(plainText == null)
    //	{
    //		return null;
    //	}

    //	//Get the bytes of the string
    //	var bytesToBeEncrypted = Encoding.UTF32.GetBytes(plainText);
    //	var passwordBytes = Encoding.UTF32.GetBytes(password);

    //	//Hash the password with SHA256
    //	passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

    //	var bytesEncrypted = Encrypt(bytesToBeEncrypted, passwordBytes);
    //	return Convert.ToBase64String(bytesEncrypted);
    //}

    //public static string Decrypt(string encryptedText)
    //{
    //	if (encryptedText == null)
    //		return null;

    //	//Get the bytes of the string
    //	var bytesToBeDecrypted = Convert.FromBase64String(encryptedText);
    //	var passwordBytes = Encoding.UTF8.GetBytes(password);

    //	passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

    //	var bytesDecrypted = Decrypt(bytesToBeDecrypted, passwordBytes);

    //	return Encoding.UTF8.GetString(bytesDecrypted);
    //}

    //private static byte[] Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
    //{
    //	byte[] encryptedBytes = null;
    //	var saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

    //	using (MemoryStream ms = new MemoryStream())
    //	{
    //		using (RijndaelManaged AES = new RijndaelManaged())
    //		{
    //			var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);

    //			AES.KeySize = 256;
    //			AES.BlockSize = 128;
    //			AES.Key = key.GetBytes(AES.KeySize / 8);
    //			AES.IV = key.GetBytes(AES.BlockSize / 8);
    //			AES.Mode = CipherMode.CBC;

    //			using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
    //			{
    //				cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
    //				cs.Close();
    //			}

    //			encryptedBytes = ms.ToArray();
    //		}
    //	}

    //	return encryptedBytes;
    //}

    //      private static byte[] Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
    //{
    //	byte[] decryptedBytes = null;
    //          var saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

    //	using (MemoryStream ms = new MemoryStream())
    //	{
    //		using (RijndaelManaged AES = new RijndaelManaged())
    //		{
    //                  var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);

    //                  AES.KeySize = 256;
    //                  AES.BlockSize = 128;
    //                  AES.Key = key.GetBytes(AES.KeySize / 8);
    //                  AES.IV = key.GetBytes(AES.BlockSize / 8);
    //                  AES.Mode = CipherMode.CBC;

    //                  using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
    //                  {
    //                      cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
    //                      cs.Close();
    //                  }

    //                  decryptedBytes = ms.ToArray();
    //              }
    //	}

    //	return decryptedBytes;
    //      }

    public static byte[] Encrypt(string plainText, byte[] Key, byte[] IV)
    {
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] encrypted;

        using (var aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            MemoryStream msEncrypt = new MemoryStream();
            CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

            csEncrypt.Write(plainBytes, 0, plainBytes.Length);
            csEncrypt.Close();
            csEncrypt.Flush();

            encrypted = msEncrypt.ToArray();
        }

        return encrypted;
    }

    public static byte[] Decrypt(byte[] encrypted, byte[] Key, byte[] IV)
    {
        using (var aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            MemoryStream msDecrypt = new MemoryStream(encrypted);
            CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            MemoryStream msDestination = new MemoryStream();

            csDecrypt.CopyTo(msDestination);

            return msDestination.ToArray();
        }
    }
}