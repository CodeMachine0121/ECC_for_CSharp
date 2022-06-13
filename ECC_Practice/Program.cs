using System.Security.Cryptography;
using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.OpenSsl;
using ECCurve = Org.BouncyCastle.Math.EC.ECCurve;
using ECPoint = Org.BouncyCastle.Math.EC.ECPoint;


namespace ECC_Practice;

public class Program
{
    public static void Main(string[] args)
    {
        // 發送方的
        KeyGenerator sender = new KeyGenerator(256);
        ECPoint senderPublicKey = sender.getPublicKey();
        ChameleonHash senderCH = new ChameleonHash(sender.getBasePoint(), sender.getPrivateKey(), senderPublicKey);
        var senderRP = senderPublicKey;
        

        KeyGenerator receiver = new KeyGenerator(256);
        ECPoint receiverPublicKey = receiver.getPublicKey();
        ChameleonHash receiverCH = new ChameleonHash(receiver.getBasePoint(), receiver.getPrivateKey(), receiverPublicKey);
        var recevierRP = receiverPublicKey;
        
        // 發送方設定 sessionkey
        senderCH.setSessionkey(recevierRP);
        receiverCH.setSessionkey(senderRP);


        string msg = "Hall";
        var r = receiverCH.Signing(msg);

        var result = senderCH.Verifying(msg, r, receiver.getPublicKey());
        
        Console.WriteLine(result);

        var api = new APITesting();
        var x=receiver.getPublicKey().XCoord.ToString();
        var y=receiver.getPublicKey().YCoord.ToString();
        api.GET();
        api.POST(x,y);

    }
}