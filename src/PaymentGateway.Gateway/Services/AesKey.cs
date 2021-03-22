using System.Security.Cryptography;
using System.Text;

namespace PaymentGateway.Gateway.Services
{
    public class AesKey
    {
        public AesKey(string password)
        {
            Key = ConvertPasswordToKey(password);
        }

        public byte[] Key { get; private set; }

        private byte[] ConvertPasswordToKey(string password)
        {
            var keyBytes = Encoding.UTF8.GetBytes(password);
            using var md5 = MD5.Create();            
            return md5.ComputeHash(keyBytes);            
        }
    }
}
