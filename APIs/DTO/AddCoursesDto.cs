using Core.Entites;

namespace APIs.DTO
{
    public class AddCoursesDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public required string CourseName { get; set; }
        public required string StudentName { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }
}
