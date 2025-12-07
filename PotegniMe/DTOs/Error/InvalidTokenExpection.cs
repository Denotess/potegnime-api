namespace PotegniMe.DTOs.Error
{
    public class InvalidTokenException : Exception
    {
        public InvalidTokenException(string message = "Token is invalid") : base(message)
        {
        }
    }
}
