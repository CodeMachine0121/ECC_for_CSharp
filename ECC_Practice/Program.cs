// See https://aka.ms/new-console-template for more information

using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

class Alice
{
    public static byte[] alicePublicKey;

    public static void Main(string[] args)
    {
        using (ECDiffieHellmanCng alice = new ECDiffieHellmanCng())
        {
            // Generate key
            alice.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
            alice.HashAlgorithm = CngAlgorithm.Sha256; // 選擇雜湊演算法
            alicePublicKey = alice.PublicKey.ToByteArray();

            Bob bob = new Bob();
            // 導出金鑰
            CngKey bobKey = CngKey.Import(bob.bobPublicKey, CngKeyBlobFormat.EccPublicBlob);
            byte[] aliceKey = alice.DeriveKeyMaterial(bobKey);

        }
    }

    private static void Send(byte[] key, string secretMessage, out byte[] encryptedMessage, out byte[] iv)
    {
        using (Aes aes = new AesCryptoServiceProvider())
        {
            aes.Key = key;
            iv = aes.IV;

            // Encrypt the message
            using (MemoryStream ciphertext = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ciphertext, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                byte[] plaintextMessage = Encoding.UTF8.GetBytes(secretMessage);
                cs.Write(plaintextMessage, 0, plaintextMessage.Length);
                cs.Close();
                encryptedMessage = ciphertext.ToArray();
            }
        }
    }

    public class Bob
    {
        public byte[] bobPublicKey;
        public byte[] bobKey;

        public Bob()
        {
            using (ECDiffieHellmanCng bob = new ECDiffieHellmanCng())
            {
                // 使物件內產生金鑰 
                bob.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
                bob.HashAlgorithm = CngAlgorithm.Sha256;
                bobPublicKey = bob.PublicKey.ToByteArray();
                // 倒出私鑰
                bobKey = bob.DeriveKeyMaterial(CngKey.Import(Alice.alicePublicKey, CngKeyBlobFormat.EccPublicBlob));

            }
        }
    }
}
