using PaymentGateway.Gateway.Factories;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PaymentGateway.Gateway.Services
{
    public class AesEncryption
    {
        private const int AesBlockByteSize = 128 / 8;
        private readonly RandomNumberGeneratorProxyFactory randomGeneratorFactory;
        private AesKey aesKey;

        public AesEncryption(RandomNumberGeneratorProxyFactory randomGeneratorFactory, AesKey aesKey)
        {
            this.aesKey = aesKey;
            this.randomGeneratorFactory = randomGeneratorFactory;
        }

        public byte[] EncryptString(string toEncrypt)
        {
            using var aes = Aes.Create();            
            var initializationVector = GenerateRandomBytes(AesBlockByteSize);
            using var encryptor = aes.CreateEncryptor(aesKey.Key, initializationVector);          
            var plainText = Encoding.UTF8.GetBytes(toEncrypt);
            var cipherText = encryptor.TransformFinalBlock(plainText, 0, plainText.Length);
            var result = new byte[initializationVector.Length + cipherText.Length];
            initializationVector.CopyTo(result, 0);
            cipherText.CopyTo(result, initializationVector.Length);
            return result;                       
        }

        public string DecryptToString(byte[] encryptedData)
        {
            using var aes = Aes.Create();            
            var initializationVector = encryptedData.Take(AesBlockByteSize).ToArray();
            var cipherText = encryptedData.Skip(AesBlockByteSize).ToArray();
            using var encryptor = aes.CreateDecryptor(aesKey.Key, initializationVector);                
            var decryptedBytes = encryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
            return Encoding.UTF8.GetString(decryptedBytes);                           
        }

        private byte[] GenerateRandomBytes(int numberOfBytes)
        {
            var randomBytes = new byte[numberOfBytes];
            using var random = randomGeneratorFactory.Create();
            random.GetBytes(randomBytes);
            return randomBytes;
        }
    }
}
