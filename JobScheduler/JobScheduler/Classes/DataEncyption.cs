using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.IO;
using System.Text;

public static class DataEncyption
{
    /// <summary>
    /// Encryption Key.
    /// </summary>
    static byte[] key = { 55, 66, 77, 88, 99, 88, 99, 88, 99, 88, 99, 88, 99, 88, 99, 55 };
    public static string ErrorMessage = string.Empty;


    /// <summary>
    /// Encrypts data before passing it to the server.
    /// </summary>
    /// <param name="TextField">String to be encrypted.</param>
    /// <returns>Encrypted value of the string.</returns>
    public static string EncryptString(string TextField)
    {
        AesManaged encryptor = new AesManaged();
        encryptor.Key = key;
        encryptor.IV = key;
        using (MemoryStream encryptionStream = new MemoryStream())
        {
            using (CryptoStream encrypt = new CryptoStream(encryptionStream, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
            {
                byte[] utfD1 = UTF8Encoding.UTF8.GetBytes(TextField);
                encrypt.Write(utfD1, 0, utfD1.Length);
                encrypt.FlushFinalBlock();
                encrypt.Close();
                return Convert.ToBase64String(encryptionStream.ToArray());
            }
        }
    }

    public static string EncryptString(string TextField, string CryptoKey)
    {
        if (TextField == string.Empty)
        {
            return TextField;
        }
        AesManaged encryptor = new AesManaged();
        encryptor.Key = GetBytes(CryptoKey);
        encryptor.IV = GetBytes(CryptoKey);
        using (MemoryStream encryptionStream = new MemoryStream())
        {
            using (CryptoStream encrypt = new CryptoStream(encryptionStream, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
            {
                byte[] utfD1 = UTF8Encoding.UTF8.GetBytes(TextField);
                encrypt.Write(utfD1, 0, utfD1.Length);
                encrypt.FlushFinalBlock();
                encrypt.Close();
                return Convert.ToBase64String(encryptionStream.ToArray());
            }
        }
    }

    public static string DecryptString(string TextField)
    {
        if (TextField == string.Empty || TextField == "Error")
        {
            return TextField;
        }
        AesManaged decryptor = new AesManaged();
        byte[] encryptedData = Convert.FromBase64String(TextField);
        decryptor.Key = key;
        decryptor.IV = key;
        using (MemoryStream decryptionStream = new MemoryStream())
        {
            using (CryptoStream decrypt = new CryptoStream(decryptionStream, decryptor.CreateDecryptor(), CryptoStreamMode.Write))
            {
                decrypt.Write(encryptedData, 0, encryptedData.Length);
                decrypt.Flush();
                decrypt.Close();
                byte[] decryptedData = decryptionStream.ToArray();
                return UTF8Encoding.UTF8.GetString(decryptedData, 0, decryptedData.Length);
            }
        }
    }

    public static string DecryptString(string TextField, string Cryptokey)
    {
        AesManaged decryptor = new AesManaged();
        byte[] encryptedData = Convert.FromBase64String(TextField);
        decryptor.Key = GetBytes(Cryptokey);
        decryptor.IV = GetBytes(Cryptokey);
        using (MemoryStream decryptionStream = new MemoryStream())
        {
            using (CryptoStream decrypt = new CryptoStream(decryptionStream, decryptor.CreateDecryptor(), CryptoStreamMode.Write))
            {
                decrypt.Write(encryptedData, 0, encryptedData.Length);
                decrypt.Flush();
                decrypt.Close();
                byte[] decryptedData = decryptionStream.ToArray();
                return UTF8Encoding.UTF8.GetString(decryptedData, 0, decryptedData.Length);
            }
        }
    }

    public static byte[] GetBytes(string str)
    {
        byte[] bytes = new byte[str.Length * sizeof(char)];
        System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
        return bytes;
    }

    public static string GetString(byte[] bytes)
    {
        char[] chars = new char[bytes.Length / sizeof(char)];
        System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
        return new string(chars);
    }

}