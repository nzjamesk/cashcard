using VirtualCashCardService.Interface;

namespace VirtualCashCardService.Service
{
    public class BasicPinValidationService : IPinValidationService
    {
        public int RequiredPinDigits => 4;

        public bool IsPinCorrect(EncryptedPin encryptedPin, string input)
        {
            return encryptedPin.PinsMatch(input);
        }

        public EncryptedPin GetEncryptedPin(string input)
        {
            return EncryptedPin.EncryptPin(input);
        }
    }
}
