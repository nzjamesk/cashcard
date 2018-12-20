namespace VirtualCashCardService
{
    public static class ErrorCodes
    {
        public const string WithdrawalInvalidAmount = "Withdrawal amount must be more than zero";
        public const string DepositInvalidAmount = "Deposit amount must be more than zero";
        public const string InvalidPin = "Invalid PIN";
    }
}
