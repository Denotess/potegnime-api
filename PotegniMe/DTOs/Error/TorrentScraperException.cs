namespace PotegniMe.DTOs.Error
{
    public class TorrentScraperException : Exception
    {
        public TorrentScraperException(string message = "External service unavailable") : base(message)
        {
        }
    }
}
