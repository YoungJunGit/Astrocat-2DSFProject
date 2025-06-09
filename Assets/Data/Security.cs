using System.Security.Cryptography;
using System;
using System.Text;
using System.IO;

public class Security
{
    private static readonly string SecurityPassword = "[패스워드 입력]";

    //AES_256 복호화
    public static string AESDecrypt256(string InputText)
    {
        RijndaelManaged RijndaelCipher = new RijndaelManaged();

        byte[] EncryptedData = Convert.FromBase64String(InputText);
        byte[] Salt = Encoding.ASCII.GetBytes(SecurityPassword.Length.ToString());

        PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(SecurityPassword, Salt);

        // Decryptor 객체를 만든다.
        ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));

        MemoryStream memoryStream = new MemoryStream(EncryptedData);

        // 데이터 읽기 용도의 cryptoStream객체
        CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);

        // 복호화된 데이터를 담을 바이트 배열을 선언한다.
        byte[] PlainText = new byte[EncryptedData.Length];

        int DecryptedCount = cryptoStream.Read(PlainText, 0, PlainText.Length);

        memoryStream.Close();
        cryptoStream.Close();

        string DecryptedData = Encoding.Unicode.GetString(PlainText, 0, DecryptedCount);

        return DecryptedData;
    }
}
