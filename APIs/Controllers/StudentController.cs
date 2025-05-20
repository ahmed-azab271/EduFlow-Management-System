using APIs.DTO;
using APIs.Errors;
using AutoMapper;
using Core.Entites;
using Core.Entites.Identity;
using Core.IRepository;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Repository;
using System.Security.Claims;

namespace APIs.Controllers
{
    public class StudentController : BaseController
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<ApplicationUser> userManager;

        public StudentController(SignInManager<ApplicationUser> _signInManager,IMapper _mapper,IUnitOfWork _unitOfWork , UserManager<ApplicationUser> _userManager)
        {
            signInManager = _signInManager;
            mapper = _mapper;
            unitOfWork = _unitOfWork;
            userManager = _userManager;
        }
        [HttpDelete("DeleteStudentEmail")]
        [Authorize(Roles ="Admin")]
        [ProducesResponseType(typeof(StudentDto),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce),StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponce),StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StudentDto>> DeleteStudent (string Email)
        {
            var Spec = new StudentSpec(Email);
            var Student = await unitOfWork.Entity<Student>().GetWhereAndIncludedAsync(Spec);
            if (Student is not null)
            {
                var user = await userManager.FindByIdAsync(Student.ApplicationUserId);
                var result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    unitOfWork.Entity<Student>().Delete(Student);
                    await unitOfWork.SaveAsync();
                    var Mapped = mapper.Map<Student,StudentDto>(Student);
                    return Ok(Mapped);
                }
                return BadRequest(new ApiResponce(500));
            }
            return NotFound(new ApiResponce(404 , $"There is no Student with Email : {Email}"));
        }
        [HttpPost]
        [Authorize(Roles = "Student,Admin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(AddCoursesDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AddCoursesDto>> AddCourse(string Code , string Action = "Add")
        {
            var Returned = await GetStudentAndCourseInfoWithEnrollmentIfExicst(Code, Action);
            if (Returned.Result is OkObjectResult okResult)
            {
                var result = (StudentCourseDto)okResult.Value;
                var enrolment = new Enrollment()
                {
                    StudentId = result.StudentId,
                    CourseId = result.CourseId
                };
                await unitOfWork.Entity<Enrollment>().AddAsync(enrolment);
                await unitOfWork.SaveAsync();
                var mappedAddCourse = mapper.Map<Enrollment, AddCoursesDto>(enrolment);
                return Ok(mappedAddCourse);
            }
            if (Returned.Result is NotFoundObjectResult notFound) return NotFound(notFound.Value);
            if (Returned.Result is UnauthorizedObjectResult unauthorized) return Unauthorized(unauthorized.Value);
            if (Returned.Result is BadRequestObjectResult badRequest) return BadRequest(badRequest.Value);
            return BadRequest(new ApiResponce(400, "Unknown error occurred."));
        }

        [HttpDelete]
        [Authorize(Roles ="Student,Admin")]
        [ProducesResponseType(typeof(AddCoursesDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AddCoursesDto>> DeleteCourse(string Code, string Action = "Delete")
        {
            var Returned = await GetStudentAndCourseInfoWithEnrollmentIfExicst(Code,Action);
            if(Returned.Result is OkObjectResult ok)
            {
                var result = (StudentCourseDto)ok.Value;
                var EnrollmentSpec = new ExcistingEnrollmentSpec(result.Id);
                var enrolment = await unitOfWork.Entity<Enrollment>().GetWhereAndIncludedAsync(EnrollmentSpec);
                unitOfWork.Entity<Enrollment>().Delete(enrolment);
                await unitOfWork.SaveAsync();
                var mappedAddCourse = mapper.Map<Enrollment, AddCoursesDto>(enrolment);
                return Ok(mappedAddCourse);
            }
            if(Returned.Result is NotFoundObjectResult notfound) return BadRequest(notfound.Value);
            if (Returned.Result is UnauthorizedObjectResult unauthorized) return BadRequest(unauthorized.Value);
            if (Returned.Result is BadRequestObjectResult badRequest) return BadRequest(badRequest.Value);
            return BadRequest(new ApiResponce(400, "Unknown error occurred."));
        }

        [HttpGet("GetStudentAndCourseInfoWithEnrollmentIfExicst")]
        [ProducesResponseType(typeof(StudentCourseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponce),StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<StudentCourseDto>> GetStudentAndCourseInfoWithEnrollmentIfExicst(string Code , string Action)
        {
            var Spec = new ExcistingCourseSpec(Code);
            var Course = await unitOfWork.Entity<Course>().GetWhereAndIncludedAsync(Spec);

            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (userEmail == null) return Unauthorized(new ApiResponce(401,"Email not found"));

            var StudentSpec = new StudentSpec(userEmail);
            var student = await unitOfWork.Entity<Student>().GetWhereAndIncludedAsync(StudentSpec);
            if (student == null || Course == null)
                return NotFound(new ApiResponce(404, "Student or course not found"));

            var result = await GetEnrollment(student.Id, Course.Id, Action);
            if(result.Result is BadRequestObjectResult badRequest ) return BadRequest(badRequest.Value);
            if(result.Result is NotFoundObjectResult notFound )
            {
                var dto = new StudentCourseDto
                {
                    Id = 0,
                    StudentId = student.Id,
                    CourseId = Course.Id,
                    StudentName = student.FullName,
                    CourseName = Course.Name
                };
                return Ok(dto);
            }
            if (result.Result is OkObjectResult okResult && okResult.Value is Enrollment enrollment)
            {
                var dto = new StudentCourseDto
                {
                    Id = enrollment.Id,
                    StudentId = student.Id,
                    CourseId = Course.Id,
                    StudentName = student.FullName,
                    CourseName = Course.Name
                };
                return Ok(dto);
            }
            return BadRequest();
        }

        private async Task<ActionResult<Enrollment?>> GetEnrollment(int studentId, int CourseId, string Action)
        {
            var EnrollmentSpec = new ExcistingEnrollmentSpec(studentId, CourseId);
            var ifExicst = await unitOfWork.Entity<Enrollment>().GetWhereAndIncludedAsync(EnrollmentSpec);
            if (Action == "Add" && ifExicst != null)
                    return BadRequest(new ApiResponce(400, $"The Student is already enrolled in Course"));

            if (Action == "Delete" && ifExicst == null)
                    return BadRequest(new ApiResponce(400, $"The Student is already not enrolled in Course"));

            if (ifExicst == null)
                return NotFound(new ApiResponce(404, $"The Enrollment Dosen't Excist"));

            return Ok(ifExicst);
        }

    }
        
}
