namespace MudBlazorTemplate.Services
{
    public class LogModel
    {
        public long Id { get; set; }
        public string Ip { get; set; }
        public string Message { get; set; }
        public DateTime MessageDate { get; set; }
        public LogModel() { }
    }
}
