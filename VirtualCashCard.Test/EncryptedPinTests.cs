using System;
using NUnit.Framework;

namespace VirtualCashCardService.Test
{
    [TestFixture]
    public class EncryptedPinTests
    {
        [Test]
        [TestCase("1234")]
        [TestCase("12")]
        [TestCase("123456")]
        public void Can_create_pin(string pin)
        {
            var encrypted = EncryptedPin.EncryptPin(pin);
            Assert.That(encrypted.PinsMatch(pin));
        }

        [Test]
        [TestCase("1234a")]
        [TestCase("bc")]
        [TestCase("12a3456")]
        public void Invalid_pin_throws(string pin)
        {
            Assert.Throws<ArgumentException>(() => EncryptedPin.EncryptPin(pin));
            
        }
    }
}
