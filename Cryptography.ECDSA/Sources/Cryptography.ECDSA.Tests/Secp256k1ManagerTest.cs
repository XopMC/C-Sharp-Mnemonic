using System;
using System.Diagnostics;
using System.Security.Cryptography;
using NUnit.Framework;

namespace Cryptography.ECDSA.Tests
{
    public class Secp256K1ManagerTest : BaseTest
    {
        [Test]
        public void SignCompressedCompactTest()
        {
            var key = Secp256K1Manager.GenerateRandomKey();
            var sw1 = new Stopwatch();
            var rand = new RNGCryptoServiceProvider();
            byte[] msg;
            for (int i = 1; i < 1000; i++)
            {
                msg = new byte[i];
                rand.GetBytes(msg);

                var hash = Sha256Manager.GetHash(msg);

                sw1.Start();
                var signature1 = Secp256K1Manager.SignCompressedCompact(hash, key);
                sw1.Stop();

                Assert.True(signature1.Length == 65);
                Assert.True(Secp256K1Manager.IsCanonical(signature1, 1));
                if (!Secp256K1Manager.IsCanonical(signature1, 1))
                {
                    WriteLine($"signature1 not canonical - skip [{i}]");
                }
            }

            WriteLine($"Secp256K1Manager time {sw1.ElapsedTicks}");
        }
    }
}
