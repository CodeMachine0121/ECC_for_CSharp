using ECC_Practice.Models;
using Newtonsoft.Json;
using Org.BouncyCastle.Math;
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
        var rOrder = receiver.getPublicKey().Curve.Order;
        var r = receiverCH.Signing(msg, rOrder);
        var result = senderCH.Verifying(msg, r, receiver.getPublicKey());
        Console.WriteLine(result);

        
        // API Testing
        var api = new APITesting();
        var x=receiver.getPublicKey().XCoord.ToString();
        var y=receiver.getPublicKey().YCoord.ToString();


        var requestPublicKey = api.getPublicKey_from_API();
        var serverPublickey = receiver.getPublicKey().Curve.CreatePoint(
            new BigInteger(requestPublicKey.Result[0], 16),
            new BigInteger(requestPublicKey.Result[1], 16));
        receiverCH.setSessionkey(serverPublickey);
        api.sessionRequest(x, y);
        
        // send message
        string sendMSG = "Hello world";
        var order = receiver.getPublicKey().Curve.Order;
        var signature= receiverCH.Signing(sendMSG, order).ToString(16);
        var publicKeyObj = new PointObject() { x = receiver.getPublicKey().XCoord.ToString(), y = receiver.getPublicKey().YCoord.ToString()};
        MessageObject response = api.dataRequest(sendMSG, signature,publicKeyObj).Result;
        var verifyResult = receiverCH.Verifying(response.message, new BigInteger(response.signature, 16), serverPublickey);

    }
}