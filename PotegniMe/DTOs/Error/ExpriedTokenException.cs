namespace PotegniMe.DTOs.Error
{
    public class ExpiredTokenException : Exception
    {
        public ExpiredTokenException(string message = "Token has expired") : base(message)
        {
        }
    }
}
