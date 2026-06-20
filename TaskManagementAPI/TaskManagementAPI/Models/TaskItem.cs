namespace TaskManagementAPI.Models
{
    public class TaskItem
    {
        public int Taskid { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public DateTime CreatedDate { get; set; }

        public int UserId { get; set; }
    }
}
