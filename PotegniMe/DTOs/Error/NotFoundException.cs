namespace PotegniMe.DTOs.Error
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message = "Not found") : base(message)
        {
        }
    }
}
