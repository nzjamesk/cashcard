using System;
using System.Linq;

namespace VirtualCashCardService
{
    // I have created this concept as a placeholder for potential future functionality, to allow keeping PIN information separately to the card and potentially move it out into 
    // a secondary, secure library with no access to the implementation. Here it is a simple unencrypted implementation but this mechanism allows for more involved implemenations
    // later, or even distributing the service remotely to keep the pin validation logic secure.
    public sealed class EncryptedPin
    {
        private readonly string _pin;

        private EncryptedPin(string pin)
        {
            _pin = pin;
        }

        public static EncryptedPin EncryptPin(string pin)
        {
            if (!pin.All(p => int.TryParse(p.ToString(), out _)))
                throw new ArgumentException("Invalid PIN supplied. Must be numbers only");

            return new EncryptedPin(pin);            
        }

        public bool PinsMatch(string input)
        {
            return _pin == input;
        }
    }
}
