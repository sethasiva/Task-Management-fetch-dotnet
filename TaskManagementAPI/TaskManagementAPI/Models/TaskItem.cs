namespace TaskManagementAPI.Models
{
    public class TaskItem
    {
        public int Taskid { get; set; }
        public required string Title { get; set; }

        public required string Description { get; set; }

        public required string Status { get; set; }

        public DateTime CreatedDate { get; set; }

        public int UserId { get; set; }
    }
}
