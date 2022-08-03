using NUnit.Framework;

namespace Cryptography.ECDSA.Tests
{
    public class Base58Test : BaseTest
    {
        [Test]
        [TestCase("5HueCGU8rMjxEXxiPuD5BDku4MkFqeZyd4dZ1jvhTVqvbTLvyTJ", "800c28fca386c7a227600b2fe50b7cae11ec86d3bf1fbe471be89827e19d72aa1d507a5b8d")]
        [TestCase("5KYZdUEo39z3FPrtuX2QbbwGnNP5zTd7yyr2SC1j299sBCnWjss", "80e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b8555c5bbb26")]
        [TestCase("5KfazyjBBtR2YeHjNqX5D6MXvqTUd2iZmWusrdDSUqoykTyWQZB", "80f3a375e00cc5147f30bee97bb5d54b31a12eee148a1ac31ac9edc4ecd13bc1f80cc8148e")]
        public void Base58DecodeTest(string key, string value)
        {
            var rez = Base58.Decode(key);
            var testrez = Hex.ToString(rez);
            Assert.IsTrue(testrez.Equals(value));
        }

        [Test]
        [TestCase("5HueCGU8rMjxEXxiPuD5BDku4MkFqeZyd4dZ1jvhTVqvbTLvyTJ", "800c28fca386c7a227600b2fe50b7cae11ec86d3bf1fbe471be89827e19d72aa1d507a5b8d")]
        [TestCase("5KYZdUEo39z3FPrtuX2QbbwGnNP5zTd7yyr2SC1j299sBCnWjss", "80e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b8555c5bbb26")]
        [TestCase("5KfazyjBBtR2YeHjNqX5D6MXvqTUd2iZmWusrdDSUqoykTyWQZB", "80f3a375e00cc5147f30bee97bb5d54b31a12eee148a1ac31ac9edc4ecd13bc1f80cc8148e")]
        public void Base58EncodeTest(string key, string value)
        {
            var b = Hex.HexToBytes(value);
            var rez = Base58.Encode(b);
            Assert.IsTrue(key.Equals(rez));
        }
    }
}