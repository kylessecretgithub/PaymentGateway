using NUnit.Framework;
using PaymentGateway.Gateway.Services;

namespace PaymentGateway.Gateway.UnitTests.Services
{
    public class AesKeyTests
    {
        [TestFixture]
        public class ConvertPasswordToKey_ConvertsPassword
        {
            [Test]
            public void Password_hashed_using_md5()
            {
                var expectedKeyHashed = new byte[] { 42, 185, 99, 144, 199, 219, 227, 67, 157, 231, 77, 12, 155, 11, 23, 103 };
                var aesKey = new AesKey("hunter2");
                Assert.That(aesKey.Key, Is.EqualTo(expectedKeyHashed));
            }
        }
    }
}
