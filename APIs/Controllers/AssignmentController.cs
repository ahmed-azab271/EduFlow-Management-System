using APIs.DTO;
using APIs.Errors;
using AutoMapper;
using Core.Entites;
using Core.IRepository;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace APIs.Controllers
{
    public class AssignmentController : BaseController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public AssignmentController(IUnitOfWork _unitOfWork , IMapper _mapper)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IReadOnlyList<AssignmentDto>), StatusCodes.Status200OK)] 
        public async Task<ActionResult<IReadOnlyList<AssignmentDto>>> GetAssignments()
        {
            var studentEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var studentSpec = new StudentSpec(studentEmail);
            var student = await unitOfWork.Entity<Student>().GetWhereAndIncludedAsync(studentSpec,true);
            var courses = student.Enrollments.Select(S => S.Course.Code).ToList();
            var assignmentSpec = new AssignmentSpec(courses);
            var assignments = await unitOfWork.Entity<Assignment>().GetAllIncludedAsync(assignmentSpec,true);
            if (!assignments.Any()) 
                return NotFound(new ApiResponce(404,"There is no Assignments For Courses You Enrroled in"));
            var mapped = mapper.Map<IReadOnlyList<Assignment>,IReadOnlyList<AssignmentDto>>(assignments);
            return Ok(mapped);
        }

        [HttpPost]
        [Authorize(Roles = "Student,Admin")]
        [ProducesResponseType(typeof(AssignmentDto) , StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce) , StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponce) , StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AssignmentDto>> AddAssignment(AssignmentDto assignmentDto)
        {
            var Spec = new AssignmentSpec(assignmentDto.Title, assignmentDto.DueDate, assignmentDto.CourseName);
            var assignmentIfExcist = await unitOfWork.Entity<Assignment>().GetWhereAndIncludedAsync(Spec, true);
            if(assignmentIfExcist == null)
            {
                var courseSpec = new ExcistingCourseSpec(assignmentDto.CourseName , string.Empty);
                var course = await unitOfWork.Entity<Course>().GetWhereAndIncludedAsync(courseSpec);
                if(course == null)
                        return NotFound(new ApiResponce(400, $"The Course : {assignmentDto.CourseName} Doesn't Excist"));
                var assignment = new Assignment()
                {
                    Title = assignmentDto.Title,
                    DueDate = assignmentDto.DueDate,
                    CourseId = course.Id,
                };
                await unitOfWork.Entity<Assignment>().AddAsync(assignment);
                var count = await unitOfWork.SaveAsync();
                if (count == 0)
                    return BadRequest(new ApiResponce(400, "The Assignment Doesn't Added"));
                return Ok(assignmentDto);
            }
            return BadRequest(new ApiResponce(400, "The Assignment Already Excist"));
        }

        [HttpPut]
        [Authorize(Roles = "Student,Admin")]
        [ProducesResponseType(typeof(AssignmentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AssignmentDto>> UpdateAssignment(AssignmentDto assignmentDto)
        {
            var Spec = new AssignmentSpec(assignmentDto.Id);
            var assignmentIfExcist = await unitOfWork.Entity<Assignment>().GetWhereAndIncludedAsync(Spec);
            if (assignmentIfExcist != null)
            {
                var courseSpec = new ExcistingCourseSpec(assignmentDto.CourseName, string.Empty);
                var course = await unitOfWork.Entity<Course>().GetWhereAndIncludedAsync(courseSpec);
                if (course == null)
                    return NotFound(new ApiResponce(400, $"The Course : {assignmentDto.CourseName} Doesn't Excist"));
                assignmentIfExcist.Title = assignmentDto.Title;
                assignmentIfExcist.DueDate = assignmentDto.DueDate;
                assignmentIfExcist.CourseId = course.Id;
                unitOfWork.Entity<Assignment>().Update(assignmentIfExcist);
                var count = await unitOfWork.SaveAsync();
                if (count == 0)
                    return BadRequest(new ApiResponce(400, "The Assignment Doesn't Updated"));
                return Ok(assignmentDto);
            }
            return BadRequest(new ApiResponce(400, "The Assignment Dosen't Excist"));
        }

        [HttpDelete]
        [Authorize(Roles = "Student,Admin")]
        [ProducesResponseType(typeof(AssignmentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AssignmentDto>> DeleteAssignment (int AssignmentId)
        {
            var Spec = new AssignmentSpec(AssignmentId);
            var assignmentIfExcist = await unitOfWork.Entity<Assignment>().GetWhereAndIncludedAsync(Spec);
            if (assignmentIfExcist != null)
            {
                unitOfWork.Entity<Assignment>().Delete(assignmentIfExcist);
                var count = await unitOfWork.SaveAsync();
                if (count == 0)
                    return BadRequest(new ApiResponce(400, "The Assignment Doesn't Updated"));
                var mapped = mapper.Map<Assignment, AssignmentDto>(assignmentIfExcist);
                return Ok(mapped);
            }
            return NotFound(new ApiResponce(400, "The Assignment Already Deleted"));
        }
    }
}