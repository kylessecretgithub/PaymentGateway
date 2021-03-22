using PaymentGateway.Gateway.Proxies;

namespace PaymentGateway.Gateway.Factories
{
    public class RandomNumberGeneratorProxyFactory
    {
        public virtual RandomNumberGeneratorProxy Create()
        {
            return new RandomNumberGeneratorProxy();
        }
    }
}
