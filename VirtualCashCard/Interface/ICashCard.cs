using VirtualCashCardService.Response;

namespace VirtualCashCardService.Interface
{
    public interface ICashCard
    {
        decimal Balance { get; }
        WithdrawalResponse Withdraw(decimal amount, string inputPin);
        DepositResponse Deposit(decimal amount);
    }
}
