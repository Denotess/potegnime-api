namespace PotegniMe.DTOs.Error
{
    public class ConflictExceptionDto : Exception
    {
        public ConflictExceptionDto(string message = "Conflict") : base(message)
        {
        }
    }
}
