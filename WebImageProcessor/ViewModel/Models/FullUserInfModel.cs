namespace WebImageProcessor.ViewModel.Models
{
    public class FullUserInfModel
    {
        public string Nickname { get; set; } = null!;

        public string? Name { get; set; }

        public string? Surname { get; set; }

        public string Password { get; set; } = null!;

        public string RoleName { get; set; } = null!;
    }
}
