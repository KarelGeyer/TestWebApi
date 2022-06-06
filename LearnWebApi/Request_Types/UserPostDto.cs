using System.ComponentModel.DataAnnotations;

namespace LearnWebApi.Request_Types
{
    #pragma warning disable CS8618

    public class UserPost
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [EmailAddress(ErrorMessage = "Email must be in a correct format")]
        public string Email { get; set; }

        [Required]
        [StringLength(16, MinimumLength = 8, ErrorMessage = "Password must include at lest 8 character and maximum of 16 characters.")]
        [RegularExpression(@"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z]).{8,16}$",
            ErrorMessage = "Password must inlude at least one lowercase and one uppercase character, number and")]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
