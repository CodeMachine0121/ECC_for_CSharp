using ECC_Practice.Models;
using Newtonsoft.Json;
using Org.BouncyCastle.Math;
using ECPoint = Org.BouncyCastle.Math.EC.ECPoint;


namespace ECC_Practice;

public class Program
{
    public static void Main(string[] args)
    {
        
        var receiver = new KeyGenerator(256);
        var receiverCH = new ChameleonHash(receiver.getBasePoint(), receiver.getPrivateKey(), receiver.getPublicKey());
        
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

        string msg = DataGenerator.DatatoString(DataGenerator.getData());
       
       
        string sendMSG = AESCipher.Encryption( msg, receiverCH.sessionKey.ToString(16)); // encryption
        var order = receiver.getPublicKey().Curve.Order;
        var signature= receiverCH.Signing(msg, order).ToString(16);
        var publicKeyObj = new PointObject() { x = receiver.getPublicKey().XCoord.ToString(), y = receiver.getPublicKey().YCoord.ToString()};
        
        // send Reqeust
        MessageObject response = api.dataRequest(sendMSG, signature,publicKeyObj).Result;
        var demsg = AESCipher.Decryption(response.message, receiverCH.sessionKey.ToString(16));
        var verifyResult = receiverCH.Verifying(demsg, new BigInteger(response.signature, 16), serverPublickey);
      
    }
}