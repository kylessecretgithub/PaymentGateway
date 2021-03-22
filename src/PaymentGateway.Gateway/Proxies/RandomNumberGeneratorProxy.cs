using System;
using System.Security.Cryptography;

namespace PaymentGateway.Gateway.Proxies
{
    public class RandomNumberGeneratorProxy : IDisposable
    {
        private bool isDisposed;

        private RandomNumberGenerator randomNumberGenerator;
        public RandomNumberGeneratorProxy()
        {
            this.randomNumberGenerator = RandomNumberGenerator.Create();
        }

        public virtual void GetBytes(byte[] bytes)
        {
            randomNumberGenerator.GetBytes(bytes);
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }
            if (disposing)
            {
                randomNumberGenerator.Dispose();
            }
            isDisposed = true;
        }
    }
}
