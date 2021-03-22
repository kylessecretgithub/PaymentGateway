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
                byte[] expectedEncryption = new byte[32] { Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(195), Convert.ToByte(198), Convert.ToByte(173), Convert.ToByte(23), Convert.ToByte(80), Convert.ToByte(60), Convert.ToByte(232), Convert.ToByte(104), Convert.ToByte(223), Convert.ToByte(12), Convert.ToByte(201), Convert.ToByte(13), Convert.ToByte(76), Convert.ToByte(7), Convert.ToByte(5), Convert.ToByte(90) };
                for (int i = 0; i < expectedEncryption.Length; i++)
                {
                    Assert.That(encryptedString[i], Is.EqualTo(expectedEncryption[i]));
                }
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
                byte[] encryptedString = new byte[32] { Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(195), Convert.ToByte(198), Convert.ToByte(173), Convert.ToByte(23), Convert.ToByte(80), Convert.ToByte(60), Convert.ToByte(232), Convert.ToByte(104), Convert.ToByte(223), Convert.ToByte(12), Convert.ToByte(201), Convert.ToByte(13), Convert.ToByte(76), Convert.ToByte(7), Convert.ToByte(5), Convert.ToByte(90) };
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
