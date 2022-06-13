using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;

namespace ECC_Practice;

public interface SignatureInterface
{
    public BigInteger Signing(string msg, BigInteger order);
    public bool Verifying(string msg, BigInteger r, ECPoint PublicKey);
}