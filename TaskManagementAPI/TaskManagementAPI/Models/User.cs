namespace TaskManagementAPI.Models
{
    public class User
    {
        public int UserId { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }

    }
}
