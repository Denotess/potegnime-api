namespace PotegniMe.DTOs.Error
{
    public class SendGridLimitException : Exception
    {
        public SendGridLimitException(string message = "SendGrid request limit exceeded") : base(message)
        {
        }
    }
}
