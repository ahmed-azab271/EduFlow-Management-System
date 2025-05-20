using APIs.DTO;
using AutoMapper;
using Core.Entites;
using Core.Entites.SignalR;

namespace APIs.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Student, StudentDto>().ReverseMap();

            CreateMap<Teacher, TeacherDto>().ReverseMap();

            CreateMap<Course, CourseDto>()
                .ForMember(D=>D.TeacherName , O=>O.MapFrom(T=>T.Teacher.FullName))
                .ReverseMap();

            CreateMap<Enrollment,AddCoursesDto>()
                .ForMember(D=>D.CourseName , O=>O.MapFrom(E=>E.Course.Name))
                .ForMember(D => D.StudentName, O => O.MapFrom(E => E.Student.FullName))
                .ReverseMap();

            CreateMap<Assignment, AssignmentDto>()
                .ForMember(D => D.CourseName, O => O.MapFrom(S => S.Course.Name))
                .ReverseMap();

            CreateMap<Submission, SubmiteDto>()
                .ForMember(D => D.AssignmentTitle, O => O.MapFrom(I => I.Assignment.Title))
                .ForMember(D => D.CourseName, O => O.MapFrom(I => I.Assignment.Course.Name))
                .ReverseMap();

           
        }
    }
}
