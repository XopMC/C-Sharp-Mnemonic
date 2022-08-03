using NUnit.Framework;

namespace Cryptography.ECDSA.Tests
{
    public class HexTest : BaseTest
    {
        [Test]
        [TestCase(0, "00")]
        [TestCase(0, "0000")]
        [TestCase(1, "1")]
        [TestCase(1, "01")]
        [TestCase(16, "10")]
        [TestCase(127, "7f")]
        [TestCase(128, "80")]
        [TestCase(255, "ff")]
        [TestCase(256, "100")]
        [TestCase(257, "101")]
        [TestCase(2048, "800")]
        [TestCase(65536, "10000")]
        [TestCase(int.MaxValue, "7fffffff")]
        [TestCase(int.MinValue, "80000000")]
        public void ConvertIntTest(int key, string hex)
        {
            var hexStr = Hex.HexToBytes(hex);
            var value = Hex.HexToInteger(hexStr);
            Assert.True(key == value);
        }
    }
}