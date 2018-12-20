using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using VirtualCashCardService.Interface;
using VirtualCashCardService.Response;

namespace VirtualCashCardService.Test
{
    [TestFixture]
    public class CashCardTests
    {
        private IPinValidationService _pinValidationService;

        private const string TestPin = "1234";

        [OneTimeSetUp]
        public void Initialise()
        {
            _pinValidationService = Substitute.For<IPinValidationService>();
            _pinValidationService.RequiredPinDigits.Returns(4);
            _pinValidationService.GetEncryptedPin(TestPin).Returns(EncryptedPin.EncryptPin(TestPin));
            _pinValidationService.IsPinCorrect(Arg.Any<EncryptedPin>(), TestPin).Returns(true);
        }

        [Test]
        public void Can_create_new_card()
        {
            var card = new CashCard(TestPin, _pinValidationService);
            Assert.That(card.Balance, Is.EqualTo(0));
            Assert.That(card.Pin.PinsMatch(TestPin));
        }

        [Test]
        [TestCase(0, 50, 50, DepositStatus.Successful, "")]
        [TestCase(25, 30, 55, DepositStatus.Successful, "")]
        [TestCase(25, -5, 25, DepositStatus.Error, ErrorCodes.DepositInvalidAmount)]
        [TestCase(0, 0, 0, DepositStatus.Error, ErrorCodes.DepositInvalidAmount)]
        public void Can_deposit_amount(decimal balance, decimal depositAmount, decimal finalBalance, DepositStatus expectedStatus, string expectedErrorMessage)
        {
            var card = new CashCard(TestPin, _pinValidationService);
            card.Deposit(balance);

            var response = card.Deposit(depositAmount);

            Assert.That(response.Status, Is.EqualTo(expectedStatus));
            Assert.That(response.ErrorMessage, Is.EqualTo(expectedErrorMessage));
            Assert.That(card.Balance, Is.EqualTo(finalBalance));
        }

        [Test]
        [TestCase(100, 50, 50, WithdrawalStatus.Successful, "")]
        [TestCase(50, 50, 0, WithdrawalStatus.Successful, "")]
        [TestCase(0.5, 0.49, 0.01, WithdrawalStatus.Successful, "")]
        [TestCase(49.5, 50, 49.5, WithdrawalStatus.InsufficientFunds, "")]
        [TestCase(50, -25, 50, WithdrawalStatus.Error, ErrorCodes.WithdrawalInvalidAmount)]
        [TestCase(50, 0, 50, WithdrawalStatus.Error, ErrorCodes.WithdrawalInvalidAmount)]
        public void Can_withdraw_amount(decimal balance, decimal withdrawalAmount, decimal finalBalance, WithdrawalStatus expectedStatus, string expectedErrorMessage)
        {
            var card = new CashCard(TestPin, _pinValidationService);
            card.Deposit(balance);

            var response = card.Withdraw(withdrawalAmount, TestPin);

            Assert.That(response.Status, Is.EqualTo(expectedStatus));
            Assert.That(response.ErrorMessage, Is.EqualTo(expectedErrorMessage));
            Assert.That(card.Balance, Is.EqualTo(finalBalance));
        }

        [Test]
        public void Cannot_withdraw_with_incorrect_pin()
        {
            var card = new CashCard(TestPin, _pinValidationService);
            card.Deposit(50);

            var response = card.Withdraw(25, "4321");

            Assert.That(response.Status, Is.EqualTo(WithdrawalStatus.Error));
            Assert.That(response.ErrorMessage, Is.EqualTo(ErrorCodes.InvalidPin));
            Assert.That(card.Balance, Is.EqualTo(50));
        }

        [Test]
        public void Can_withdraw_multiple_amounts_simultaneously()
        {
            var pinValidationService = Substitute.For<IPinValidationService>();
            pinValidationService.IsPinCorrect(Arg.Any<EncryptedPin>(), Arg.Any<string>()).ReturnsForAnyArgs(true)
                .AndDoes(async _ => await Task.Delay(1000));

            var card = new CashCard(TestPin, pinValidationService);
            card.Deposit(100);

            var tf = new TaskFactory();
            var tasks = new List<Task>();
            for (var i = 1; i <= 4; i++)
            {
                tasks.Add(tf.StartNew(() =>
                {
                    var resp = card.Withdraw(10, TestPin);
                    Assert.That(resp.Status, Is.EqualTo(WithdrawalStatus.Successful));
                }));
            }

            Task.WaitAll(tasks.ToArray());

            // One final one to ensure we end up with the right value
            var response = card.Withdraw(10, TestPin);
            Assert.That(response.Status, Is.EqualTo(WithdrawalStatus.Successful));
            Assert.That(response.ErrorMessage, Is.EqualTo(""));
            Assert.That(card.Balance, Is.EqualTo(50));
        }

        [Test]
        public void Cannot_withdraw_more_than_balance_with_multiple_simultaneous_requests()
        {
            var pinValidationService = Substitute.For<IPinValidationService>();
            pinValidationService.IsPinCorrect(Arg.Any<EncryptedPin>(), Arg.Any<string>()).ReturnsForAnyArgs(true)
                .AndDoes(async _ => await Task.Delay(1500));

            var card = new CashCard(TestPin, pinValidationService);
            card.Deposit(25);

            var tf = new TaskFactory();
            var tasks = new List<Task>();
            var responses = new List<WithdrawalStatus>();

            for (var i = 1; i <= 4; i++)
            {
                tasks.Add(tf.StartNew(() =>
                {
                    var resp = card.Withdraw(10, TestPin);
                    responses.Add(resp.Status);
                }));
            }

            Task.WaitAll(tasks.ToArray());
            Assert.That(responses.Count(r => r == WithdrawalStatus.Successful), Is.EqualTo(2));
            Assert.That(responses.Count(r => r == WithdrawalStatus.InsufficientFunds), Is.EqualTo(2));

            Assert.That(card.Balance, Is.EqualTo(5));
        }


    }
}
