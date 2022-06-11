// See https://aka.ms/new-console-template for more information

using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

class Alice
{
    public static byte[] alicePublicKey;

    public Alice()
    {
        using (ECDiffieHellmanCng alice = new ECDiffieHellmanCng())
        {
            // Generate key
            alice.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
            alice.HashAlgorithm = CngAlgorithm.Sha256; // 選擇雜湊演算法
            alicePublicKey = alice.PublicKey.ToByteArray();

            Bob bob = new Bob();
            // KeyExchange
            CngKey bobKey = CngKey.Import(bob.bobPublicKey, CngKeyBlobFormat.EccPublicBlob); 
            byte[] aliceKey = alice.DeriveKeyMaterial(bobKey);
           
            byte[] encryptedMessage = null;
            byte[] iv = null;
            Send(aliceKey, "Secret message", out encryptedMessage, out iv);
            bob.Receive(encryptedMessage, iv);
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
                byte[] plaintextMessage = Encoding.UTF8.GetBytes(secretMessage); // 將明文->byte array
                cs.Write(plaintextMessage, 0, plaintextMessage.Length); // E(plainText)
                cs.Close();
                encryptedMessage = ciphertext.ToArray(); // 密文 -> byte array 
                
                Console.WriteLine($"CipherText: {BitConverter.ToString(encryptedMessage).Replace("-","")}");
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

        public void Receive(byte[] encryptedMessage, byte[] iv)
        {
            using (Aes aes = new AesCryptoServiceProvider())
            {
                aes.Key = bobKey;
                aes.IV = iv;
                
                // Decrypt cipherText
                using (MemoryStream plaintext = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(plaintext, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(encryptedMessage, 0, encryptedMessage.Length);
                        cs.Close();

                        string message = Encoding.UTF8.GetString(plaintext.ToArray());
                        Console.WriteLine($"PlainText: {message}");
                    }
                }
            }
        }
        
    }
}
