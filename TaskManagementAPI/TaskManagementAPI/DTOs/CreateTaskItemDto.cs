namespace TaskManagementAPI.DTOs
{
    public class CreateTaskItemDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }

    }
}
