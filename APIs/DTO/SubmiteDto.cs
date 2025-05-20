namespace APIs.DTO
{
    public class SubmiteDto
    {
        public int Id { get; set; }
        public required string AssignmentTitle { get; set; }
        public required string CourseName { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string? FilePath { get; set; }
        public string? Comments { get; set; }
        public decimal? Grade { get; set; }
        public string? FileUrl { get; set; }
        public string? FeedBack { get; set; }
    }
}
