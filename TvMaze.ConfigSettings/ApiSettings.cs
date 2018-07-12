using System.Net;

namespace TvMaze.ConfigSettings
{
    public class ApiSettings
    {
        public string ApiBaseUrl { get; set; }
        public int ApiLimitSleepTimeSeconds { get; set; }
        public HttpStatusCode ApiLimitHttpCode { get; set; }
    }
}
