namespace Onyx.Models.ViewModels
{
    public class ExternalLoginConfirmationViewModel
    {
        public string FullName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
