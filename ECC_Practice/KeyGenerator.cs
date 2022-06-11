using System.Numerics;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
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
        BigInteger aliceRandom = keygen.getBigRandomInteger(256);
        /// random * P
        var alicePrivateKey = (ECPrivateKeyParameters)keypair.Private;
        var Alice_rp = alicePrivateKey.Parameters.G.Multiply(aliceRandom );
        
        //Console.WriteLine("[+] rpX: "+Alice_rp.XCoord.ToString());
        //Console.WriteLine("[+] rpY: "+Alice_rp.YCoord.ToString());
        
        // Bob
        Console.WriteLine("[+] Bob's Phase##################################################");
        var bobkeygen = new KeyGenerator();
        var bobkeypair = bobkeygen.GenerateKey_by_Curve(256);
        BigInteger bobRandom = keygen.getBigRandomInteger(256);
        // random *P
        var bobPrivateKey = (ECPrivateKeyParameters) bobkeypair.Private;
        var Bob_rp = bobPrivateKey.Parameters.G.Multiply(bobRandom);
        //Console.WriteLine("[+] rpX: "+Bob_rp.XCoord.ToString());
        //Console.WriteLine("[+] rpY: "+Bob_rp.YCoord.ToString());
        
        // Remember to Normalize the Point!
        // Alice receive Bob's rp
        var aliceSessionKey = Bob_rp.Multiply(aliceRandom).Normalize();
        // Bob receive Alice's rp
        var bobSessionKey = Alice_rp.Multiply(bobRandom).Normalize();
        
        Console.WriteLine("[+] Random Number: ");
        Console.WriteLine("\t[-]Alice r: "+aliceRandom);
        Console.WriteLine("\t[-]Bob   r: "+bobRandom);
        Console.WriteLine("\t[-]Alice*Bob r: "+(aliceRandom.Multiply(bobRandom)));


        
        Console.WriteLine("[+] Session Key Comparation: ");
        var result = aliceSessionKey.Equals(bobSessionKey);
        result = aliceSessionKey.XCoord.ToBigInteger().Equals(bobSessionKey.XCoord.ToBigInteger());
        Console.WriteLine("\t[-] Alice : "+ aliceSessionKey.XCoord.ToBigInteger()+" , "+aliceSessionKey.YCoord.ToBigInteger());
        Console.WriteLine("\t[-] Bob   : "+bobSessionKey.XCoord.ToBigInteger()+" , "+bobSessionKey.YCoord.ToBigInteger());
        Console.WriteLine("\t[-]Result: "+result);
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
        //Console.WriteLine("[+] Private Key in pem Format:\n "+ privateKey);

        textWriter = new StringWriter();
        pemWriter = new PemWriter(textWriter);
        pemWriter.WriteObject(keypair.Public);
        pemWriter.Writer.Flush();
        string publicKey = textWriter.ToString();
        //.WriteLine("[+] Public key in pem Format: \n"+ publicKey);
        
        // 輸出各項參數
        var privateParameter = (ECPrivateKeyParameters)keypair.Private;
        var privateKeyValue = privateParameter.D.ToString(16);
        //Console.WriteLine("[+] Private Key Value: "+ privateKeyValue);

        var publicParameter = (ECPublicKeyParameters)keypair.Public;
        var publicX = publicParameter.Q.XCoord.ToString();
        var publicY = publicParameter.Q.YCoord.ToString();
        //Console.WriteLine("[+] Public Key X-value: "+ publicX);
        //Console.WriteLine("[+] Public Key Y-value: "+publicY);

        var Px = privateParameter.Parameters.G.XCoord.ToString();
        var Py = privateParameter.Parameters.G.YCoord.ToString();
        //Console.WriteLine("[+] Px: "+Px);
        //Console.WriteLine("[+] Py: "+Py);
        return keypair;
    }


    public BigInteger getBigRandomInteger(int size)
    {
        BigInteger ans;
        SecureRandom secureRandom = new SecureRandom();
        byte[] data = new byte[size/8];
        secureRandom.NextBytes(data);

        ans = new BigInteger(data);
        ans.Abs();
        //Console.WriteLine("[+] Random: "+new BigInteger(data));
        return ans;
    }
}