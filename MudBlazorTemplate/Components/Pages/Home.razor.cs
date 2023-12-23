using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazorTemplate.Components.Custom;
using MudBlazorTemplate.Components.Layout;
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
                        string password = await ShowPasswordDialog();
                        await ShowConfirmationDialogAsync("Password Test", $"The password is {password} for IP {SessionService.CurrentSession.IPAddress}");
                        SessionService.CurrentSession.IsAuthenticated = true;
                        await LogService.InsertLogData(Program.ConnectionString, "Logon", SessionService?.CurrentSession?.IPAddress);
                    }
                }
            }
            catch (Exception ex)
            {
                await LogService.InsertLogData(Program.ConnectionString, ex.Message, SessionService?.CurrentSession?.IPAddress);
                await ShowConfirmationDialogAsync("Error", $"{ex.Message}{Environment.NewLine}{ex.InnerException.Message}");
            }
        }

        private async Task InitSession()
        {
            try
            {
                if (SessionService == null)
                {
                    SessionService = new SessionService();
                }

                if (SessionService.CurrentSession == null)
                {
                    SessionService.CurrentSession = new SessionModel
                    {
                        IsAuthenticated = false,
                        EmailAddress = "",
                        IPAddress = HttpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString(),
                        UserAgent = HttpContextAccessor?.HttpContext?.Request?.Headers?.UserAgent
                    };
                }
            }
            catch (Exception ex)
            {
                await LogService.InsertLogData(Program.ConnectionString, ex.Message, SessionService?.CurrentSession?.IPAddress);
                await ShowConfirmationDialogAsync("Error", $"{ex.Message}{Environment.NewLine}{ex.InnerException.Message}");
            }
        }

        public async Task<string> ShowPasswordDialog()
        {
            try
            {
                DialogParameters passwordDialogParameters = new DialogParameters { { "DialogData", new InputDialogModel("Password:") } };
                IDialogReference dialogresult = DialogService.Show<InputDialog>("Enter Password", passwordDialogParameters);
                await dialogresult.Result.ConfigureAwait(true);
                return ((InputDialog)((DialogReference)dialogresult).Dialog).DialogData.InputValue;
            }
            catch (Exception ex)
            {
                await LogService.InsertLogData(Program.ConnectionString, ex.Message, SessionService?.CurrentSession?.IPAddress);
                await ShowConfirmationDialogAsync("Error", $"{ex.Message}{Environment.NewLine}{ex.InnerException.Message}");
                return "";
            }
        }

        public async Task ShowConfirmationDialogAsync(string title, string prompt)
        {
            try
            {
                var parameters = new DialogParameters { { "DialogPrompt", prompt } };
                var dialog = DialogService.Show<ConfirmationDialog>(title, parameters);
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
