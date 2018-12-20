namespace VirtualCashCardService.Response
{
    public class WithdrawalResponse
    {
        public WithdrawalResponse(WithdrawalStatus status, string errorMessage = "")
        {
            Status = status;
            ErrorMessage = errorMessage;
        }

        public WithdrawalStatus Status { get; }
        public string ErrorMessage { get; }
    }
}
