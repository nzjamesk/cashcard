using System;
using VirtualCashCardService.Interface;
using VirtualCashCardService.Response;

namespace VirtualCashCardService
{
    // I envisage this being created through a DI factory like Windsor typed factory. The validation service would be injected from there and
    // the pin would be passed to the factories Create method at run-time by the caller
    public class CashCard : ICashCard
    {
        private readonly IPinValidationService _pinValidationService;
        private readonly object _cardLock = new object();
        public decimal Balance { get; private set; }

        public Guid CardId { get; }
        public EncryptedPin Pin { get; }

        public CashCard(string pin, IPinValidationService pinValidationService)
        {
            // This would probably in production come from a data store somewhere that tracked unique card ids but for this exercise this works almost as well as I don't have this store available
            CardId = Guid.NewGuid();

            Balance = 0;

            _pinValidationService = pinValidationService;
            Pin = _pinValidationService.GetEncryptedPin(pin);
        }

        public WithdrawalResponse Withdraw(decimal amount, string inputPin)
        {
            if (amount <= 0)
            {
                return new WithdrawalResponse(WithdrawalStatus.Error, ErrorCodes.WithdrawalInvalidAmount);
            }

            if (!_pinValidationService.IsPinCorrect(Pin, inputPin))
            {
                // On a more fully fledged implementation I would keep track of pin attempts and block the card if exceeded
                return new WithdrawalResponse(WithdrawalStatus.Error, ErrorCodes.InvalidPin);
            }
                
            if (amount <= Balance)
            {
                lock (_cardLock)
                {
                    if (amount <= Balance)
                    {
                        Balance -= amount;
                        return new WithdrawalResponse(WithdrawalStatus.Successful);
                    }
                }
            }

            return new WithdrawalResponse(WithdrawalStatus.InsufficientFunds);
        }

        public DepositResponse Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                return new DepositResponse(DepositStatus.Error, ErrorCodes.DepositInvalidAmount);
            }  

            lock (_cardLock)
            {
                Balance += amount;
                return new DepositResponse(DepositStatus.Successful);                    
            }
        }
    }
}
