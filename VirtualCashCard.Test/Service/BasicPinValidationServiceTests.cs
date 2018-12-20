using NUnit.Framework;
using VirtualCashCardService.Service;

namespace VirtualCashCardService.Test.Service
{
    [TestFixture]
    public class BasicPinValidationServiceTests
    {
        [Test]
        public void Required_number_of_digits_correct()
        {
            var sut = new BasicPinValidationService();
            Assert.That(sut.RequiredPinDigits, Is.EqualTo(4));
        }

        [Test]
        public void Is_pin_correct()
        {
            var sut = new BasicPinValidationService();
            var encrypted = EncryptedPin.EncryptPin("1234");
            Assert.That(sut.IsPinCorrect(encrypted, "1234"));
        }

        [Test]
        public void Is_pin_incorrect()
        {
            var sut = new BasicPinValidationService();
            var encrypted = EncryptedPin.EncryptPin("1234");
            Assert.That(!sut.IsPinCorrect(encrypted, "1233"));
        }

        [Test]
        public void Can_get_encrypted_pin()
        {
            var sut = new BasicPinValidationService();
            var encrypted = sut.GetEncryptedPin("1234");
            Assert.NotNull(encrypted);
            Assert.That(encrypted.PinsMatch("1234"));
        }
    }
}
