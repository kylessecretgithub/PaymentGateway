using NUnit.Framework;
using PaymentGateway.Gateway.Models;

namespace PaymentGateway.Gateway.UnitTests.Models
{
    internal class PaymentTests
    {
        [Test]
        internal void CardNumber_is_masked()
        {
            var payment = new Payment
            {
                CardNumber = 12345678
            };
            payment.MaskCardNumber();

            Assert.That(payment.CardNumber, Is.EqualTo(123));
        }

        [Test]
        internal void CardNumber_not_enough_length_to_bemasked()
        {
            var payment = new Payment
            {
                CardNumber = 12
            };
            payment.MaskCardNumber();

            Assert.That(payment.CardNumber, Is.EqualTo(12));
        }

        [Test]
        internal void CardNumber_Is_Null()
        {
            var payment = new Payment();
            payment.MaskCardNumber();

            Assert.That(payment.CardNumber, Is.Null);
        }
    }
}
