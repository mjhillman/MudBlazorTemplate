using HITS.LIB.Utilities;
using Microsoft.AspNetCore.Components;
using MudBlazorTemplate.Services;

namespace MudBlazorTemplate.Components.Pages
{
    public partial class SessionViewer : ComponentBase
    {
        [Inject]
        private SessionService SessionService { get; set; }

        private string session { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                session = SessionService.CurrentSession.Serialize();
                StateHasChanged();
            }
        }
    }
}
