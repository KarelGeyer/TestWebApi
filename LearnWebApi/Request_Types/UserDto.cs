namespace LearnWebApi.Request_Types
{
    #pragma warning disable CS8618

    public class UserFindDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserChangeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
    }

    public class UserAuth
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}