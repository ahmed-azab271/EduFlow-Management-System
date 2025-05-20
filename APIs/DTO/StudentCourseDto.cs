namespace APIs.DTO
{
    public class StudentCourseDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string StudentName { get; set; }
    }
}
