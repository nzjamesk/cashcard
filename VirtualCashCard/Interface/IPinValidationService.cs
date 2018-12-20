namespace VirtualCashCardService.Interface
{
    public interface IPinValidationService
    {
        int RequiredPinDigits { get; }
        bool IsPinCorrect(EncryptedPin encryptedPin, string input);
        EncryptedPin GetEncryptedPin(string input);
    }
}
