namespace MudBlazorTemplate.Components.Layout
{
    public class InputDialogModel
    {
        public string InputPrompt { get; set; }
        public string InputValue { get; set; } = "";
        public string CancelButtonLabel { get; set; } = "Cancel";
        public string SubmitButtonLabel { get; set; } = "Submit";

        public InputDialogModel() { }
        public InputDialogModel(string inputPrompt) 
        { 
            InputPrompt = inputPrompt;
        }
    }
}
