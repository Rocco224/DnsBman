using System.Text;

namespace DnsBman.Utilities.Cryptation
{
    public class RC4Encryption
    {
        public static byte[] Encrypt(string plainText, byte[] key)
        {
            try
            {
                using (RC4 rc4 = new RC4(key))
                {
                    byte[] plaintextBytes = Encoding.UTF8.GetBytes(plainText);
                    return rc4.TransformFinalBlock(plaintextBytes, 0, plaintextBytes.Length);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Si è verificato un errore durante la crittografia RC4.", ex);
            }
        }

        public static string Decrypt(byte[] cipherText, byte[] key)
        {
            try
            {
                using (RC4 rc4 = new RC4(key))
                {
                    byte[] decryptedBytes = rc4.TransformFinalBlock(cipherText, 0, cipherText.Length);
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Si è verificato un errore durante la decrittografia RC4.", ex);
            }
        }
    }
}
