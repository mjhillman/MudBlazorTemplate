namespace MudBlazorTemplate.Services
{
    public class SessionModel
    {
        public bool IsAuthenticated {  get; set; }
        public string IPAddress { get; set; }
        public string UserAgent { get; set; }
        public string EmailAddress { get; set; }

        public SessionModel()
        {
            IsAuthenticated = false;
        }
    }
}
