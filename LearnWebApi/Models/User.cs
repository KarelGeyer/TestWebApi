using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LearnWebApi.Models
{
    #pragma warning disable CS8618

    public class User
    {
        public int Id { get; set; } = 0;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Surname { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage = "Email must be in a correct format")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(16, MinimumLength = 8, ErrorMessage = "Password must include at lest 8 character and maximum of 16 characters.")]
        [RegularExpression(@"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z]).{8,16}$", 
            ErrorMessage = "Password must inlude at least one lowercase and one uppercase character, number and")]
        public string Password { get; set; } = string.Empty;

        // Navigation Properties
        [JsonIgnore]
        public ICollection<Transaction> Transactions { get; set; }
    }
}
