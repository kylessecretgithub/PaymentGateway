using Moq;
using NUnit.Framework;
using PaymentGateway.Gateway.Factories;
using PaymentGateway.Gateway.Proxies;
using PaymentGateway.Gateway.Services;
using System;

namespace PaymentGateway.Gateway.UnitTests.Services
{
    public class AesEncryptionTests
    {
        protected Mock<RandomNumberGeneratorProxyFactory> rngFactory;
        protected AesKey aesKey;
        protected Mock<RandomNumberGeneratorProxy> rngProxy;

        [SetUp]
        public void BaseSetUp()
        {
            rngProxy = new Mock<RandomNumberGeneratorProxy>();           
            rngFactory = new Mock<RandomNumberGeneratorProxyFactory>();
            rngFactory.Setup(f => f.Create()).Returns(rngProxy.Object);
            aesKey = new AesKey("hunter2");
        }

        [TestFixture]
        internal class EncryptString_SuccessfullyEncryptsData : AesEncryptionTests
        {
            private byte[] encryptedString;

            [SetUp]
            public void SetUp()
            {
                var aesEncrpytion = new AesEncryption(rngFactory.Object, aesKey);
                encryptedString = aesEncrpytion.EncryptString("12");
            }

            [Test]
            public void Symmetrically_encrypts_data()
            {
                byte[] expectedEncryption = new byte[32] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 195, 198, 173, 23, 80, 60, 232, 104, 223, 12, 201, 13, 76, 7, 5, 90 };
                Assert.That(encryptedString, Is.EqualTo(expectedEncryption));
            }

            [Test]
            public void Randomly_shuffles_bytes()
            {
                rngProxy.Verify(rng => rng.GetBytes(It.IsAny<byte[]>()), Times.Once);
            }
        }

        [TestFixture]
        internal class DecryptToString_SuccessfullyDecryptsData : AesEncryptionTests
        {
            private string decryptedString;

            [SetUp]
            public void SetUp()
            {
                byte[] encryptedString = new byte[32] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 195, 198, 173, 23, 80, 60, 232, 104, 223, 12, 201, 13, 76, 7, 5, 90 };
                var aesEncrpytion = new AesEncryption(rngFactory.Object, aesKey);
                decryptedString = aesEncrpytion.DecryptToString(encryptedString);
            }

            [Test]
            public void Symmetrically_encrypts_data()
            {
                Assert.That(decryptedString, Is.EqualTo("12"));
            }
        }
    }
}
