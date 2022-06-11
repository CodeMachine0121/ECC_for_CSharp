using System.Numerics;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using BigInteger = Org.BouncyCastle.Math.BigInteger;


namespace ECC_Practice;

public class KeyGenerator
{
    public static void Main(string[] args)
    {
        
        // Alice
        Console.WriteLine("[+] Alice's Phase##################################################");
        var keygen = new KeyGenerator();
        var keypair = keygen.GenerateKey_by_Curve(256);
        BigInteger bigRandomInteger = keygen.getBigRandomInteger(256);
        /// random * P
        var privateParameter = (ECPrivateKeyParameters)keypair.Private;
        var Alice_rp = privateParameter.Parameters.G.Multiply(bigRandomInteger);
        Console.WriteLine("[+] rpX: "+Alice_rp.XCoord.ToString());
        Console.WriteLine("[+] rpY: "+Alice_rp.YCoord.ToString());
        
        
    }
    public AsymmetricCipherKeyPair generateKeys(int keySize)
    {
        var gen = new ECKeyPairGenerator("ECDSA");
        
        //  隨機亂數
        var secureRandom = new SecureRandom();
        
        // 創立ECC參數物件
        var keyGenParam = new KeyGenerationParameters(secureRandom, keySize);
        
        gen.Init(keyGenParam);

        return gen.GenerateKeyPair();
    }
    
    public AsymmetricCipherKeyPair GenerateKey_by_Curve(int keysize)
    {
        var keypair = this.generateKeys(keysize);
        
        // 寫成 pem格式
        TextWriter textWriter = new StringWriter();
        PemWriter pemWriter = new PemWriter(textWriter);
        pemWriter.WriteObject(keypair.Private);
        pemWriter.Writer.Flush();
        string privateKey = textWriter.ToString();
        Console.WriteLine("[+] Private Key in pem Format:\n "+ privateKey);

        textWriter = new StringWriter();
        pemWriter = new PemWriter(textWriter);
        pemWriter.WriteObject(keypair.Public);
        pemWriter.Writer.Flush();
        string publicKey = textWriter.ToString();
        Console.WriteLine("[+] Public key in pem Format: \n"+ publicKey);
        
        // 輸出各項參數
        var privateParameter = (ECPrivateKeyParameters)keypair.Private;
        var privateKeyValue = privateParameter.D.ToString(16);
        Console.WriteLine("[+] Private Key Value: "+ privateKeyValue);

        var publicParameter = (ECPublicKeyParameters)keypair.Public;
        var publicX = publicParameter.Q.XCoord.ToString();
        var publicY = publicParameter.Q.YCoord.ToString();
        Console.WriteLine("[+] Public Key X-value: "+ publicX);
        Console.WriteLine("[+] Public Key Y-value: "+publicY);

        var Px = privateParameter.Parameters.G.XCoord.ToString();
        var Py = privateParameter.Parameters.G.YCoord.ToString();
        Console.WriteLine("[+] Px: "+Px);
        Console.WriteLine("[+] Py: "+Py);
        return keypair;
    }


    public BigInteger getBigRandomInteger(int size)
    {
        SecureRandom secureRandom = new SecureRandom();
        byte[] data = new byte[size/8];
        secureRandom.NextBytes(data);
        
        
        Console.WriteLine("[+] Random: "+new BigInteger(data));
        return new BigInteger(data);
    }
}