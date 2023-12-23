namespace MudBlazorTemplate.Services
{
    public interface ISessionService
    {
        SessionModel CurrentSession { get; set; }

        event EventHandler SessionDataChanged;

        void OnSessionDataChanged(SessionDataChangedEventArgs e);
    }
}