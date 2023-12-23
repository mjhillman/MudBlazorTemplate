namespace MudBlazorTemplate.Services
{
    public class SessionService : ISessionService
    {
        public SessionModel CurrentSession { get; set; }

        public event EventHandler SessionDataChanged;

        public SessionService()
        {
        }

        public void OnSessionDataChanged(SessionDataChangedEventArgs e)
        {
            EventHandler handler = SessionDataChanged;
            handler?.Invoke(this, new SessionDataChangedEventArgs() { SessionModel = CurrentSession });
        }
    }

    public class SessionDataChangedEventArgs : EventArgs
    {
        public SessionModel SessionModel { get; set; }
    }
}
