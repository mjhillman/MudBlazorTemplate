using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazorTemplate.Components.Custom;
using MudBlazorTemplate.Services;

namespace MudBlazorTemplate.Components.Pages
{
    public partial class Home : ComponentBase, IDisposable
    {
        [Inject]
        private IDialogService DialogService { get; set; }

        [Inject]
        private SessionService SessionService { get; set; }

        [Inject]
        private IHttpContextAccessor HttpContextAccessor { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                if (firstRender)
                {
                    if (SessionService == null || SessionService.CurrentSession == null || !SessionService.CurrentSession.IsAuthenticated)
                    {
                        await InitSession();
                        await ShowConfirmationDialogAsync("TEST", "Confirmation Dialog Test");
                        SessionService.CurrentSession.IsAuthenticated = true;
                        await LogService.InsertLogData("Logon", SessionService?.CurrentSession?.IPAddress);
                    }
                }
            }
            catch (Exception ex)
            {
                await LogService.InsertLogData(ex.Message, SessionService?.CurrentSession?.IPAddress);
                await ShowConfirmationDialogAsync("Error", $"{ex.Message}{Environment.NewLine}{ex.InnerException.Message}");
            }
        }

        private async Task InitSession()
        {
            try
            {
                if (SessionService == null) SessionService = new SessionService();

                if (SessionService.CurrentSession == null)
                {
                    SessionService.CurrentSession = new SessionModel
                    {
                        IsAuthenticated = true,
                        EmailAddress = "you@you.com",
                        IPAddress = HttpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString(),
                        UserAgent = HttpContextAccessor?.HttpContext?.Request?.Headers?.UserAgent
                    };
                }
            }
            catch (Exception ex)
            {
                await LogService.InsertLogData(ex.Message, SessionService?.CurrentSession?.IPAddress);
                await ShowConfirmationDialogAsync("Error", $"{ex.Message}{Environment.NewLine}{ex.InnerException.Message}");
            }
        }

        public async Task ShowConfirmationDialogAsync(string title, string prompt)
        {
            try
            {
                var dialog = DialogService.Show<ConfirmationDialog>(title, new DialogParameters { { "DialogPrompt", prompt } });
                var result = await dialog.Result.ConfigureAwait(true);
            }
            catch (Exception)
            {
            }
        }

#region dispose

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            try
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: dispose managed state (managed objects)
                    }
                    disposedValue = true;
                }
            }
            catch (Exception)
            {
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

#endregion

}
