using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazorTemplate.Services;

namespace MudBlazorTemplate.Components.Pages
{
    public partial class Log : ComponentBase, IDisposable
    {
        [Inject]
        private IDialogService DialogService { get; set; }

        [Inject]
        private SessionService SessionService { get; set; }

        private bool disposedValue;

        private List<LogModel> logList = new List<LogModel>();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                if (firstRender) await LoadLogGridAsync().ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                await DialogService.ShowMessageBox("Error", ex.Message, yesText: "Ok").ConfigureAwait(true);
                await LogService.InsertLogData(Program.ConnectionString, ex.Message, SessionService.CurrentSession.IPAddress).ConfigureAwait(true);
            }
        }

        async Task LoadLogGridAsync()
        {
            try
            {
                logList = await LogService.GetLog(Program.ConnectionString).ConfigureAwait(true);
                StateHasChanged();
            }
            catch (Exception ex)
            {
                await DialogService.ShowMessageBox("Error", ex.Message, yesText: "Ok").ConfigureAwait(true);
                await LogService.InsertLogData(Program.ConnectionString, ex.Message, SessionService.CurrentSession.IPAddress).ConfigureAwait(true);
            }
        }

        async Task DeleteButtonClick()
        {
            await LogService.DeleteLog(Program.ConnectionString).ConfigureAwait(true);
            await LogService.InsertLogData(Program.ConnectionString, "Deleted All Log Records", SessionService.CurrentSession.IPAddress).ConfigureAwait(true);
            await LoadLogGridAsync().ConfigureAwait(true);
        }

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    logList.Clear();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Log()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    #endregion
    }
}
