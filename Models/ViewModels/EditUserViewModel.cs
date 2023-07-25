namespace Onyx.Models.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Password { get; set; }
        public List<string> RolesName { get; set; }
        public bool IsExternalLogin { get; set; }
    }
}
