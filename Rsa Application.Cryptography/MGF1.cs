using System.Security.Cryptography;

namespace Rsa_Application.Cryptography
{
    internal class MGF1
    {
        internal static byte[] GetMask(HashAlgorithm hash, byte[] mgfSeed, int maskLen)
        {
            if (maskLen >= Math.Pow(2, 32))
                throw new Exception("Длина маски слишком большая.");
            byte[] T = new byte[0];
            int hLen = hash.HashSize / 8;
            for (int i = 0; i < maskLen / hLen; ++i)
            {
                var C = Primitives.I2OSP(i, 4);
                T = Primitives.Concate(T, hash.ComputeHash
                    (Primitives.Concate(mgfSeed, C)));
            }
            Array.Resize(ref T, maskLen);
            return T;
        }
    }
}
