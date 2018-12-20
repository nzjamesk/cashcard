namespace VirtualCashCardService.Response
{
    public class DepositResponse
    {
        public DepositResponse(DepositStatus status, string errorMessage = "")
        {
            Status = status;
            ErrorMessage = errorMessage;
        }

        public DepositStatus Status { get; }
        public string ErrorMessage { get; }
    }
}
