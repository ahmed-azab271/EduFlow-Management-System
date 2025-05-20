using APIs.DTO;
using APIs.Errors;
using AutoMapper;
using Core.Entites;
using Core.Entites.Identity;
using Core.IRepository;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Net;
using System.Security.Claims;

namespace APIs.Controllers
{
    public class SubmissionController : BaseController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;

        public SubmissionController(IUnitOfWork _unitOfWork , IMapper _mapper , UserManager<ApplicationUser> _userManager)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            userManager = _userManager;
        }

        [HttpGet]
        [Authorize(Roles = "Student,Admin")]
        [ProducesResponseType(typeof(SubmiteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<SubmiteDto>>> GetSubmissions()
        {
            var users = await userManager.GetUserAsync(User);
            var roles = await userManager.GetRolesAsync(users);
            SubmissionSpec? submissionSpec;
            if (roles.Contains("Student"))
            {
                var studentEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                submissionSpec = new SubmissionSpec(studentEmail);
            }
            else  { submissionSpec = new SubmissionSpec(); }
            var submissions = await unitOfWork.Entity<Submission>().GetAllIncludedAsync(submissionSpec, true);
            if(!submissions.Any())
                return NotFound(new ApiResponce(404 , "There is No Submissions"));
            var mapped = mapper.Map<IReadOnlyList<Submission>, IReadOnlyList<SubmiteDto>>(submissions);
            return Ok(mapped);
        }

        [HttpPost]
        [Authorize(Roles ="Student,Admin")]
        [ProducesResponseType(typeof(SubmiteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SubmiteDto>> SubmiteAssignment(SubmiteDto submiteDto)
        {
            var studentEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var studentSpec = new StudentSpec(studentEmail);
            var student = await unitOfWork.Entity<Student>().GetWhereAndIncludedAsync(studentSpec, true);
            var result = student.Enrollments.Any(i => i.Course.Name == submiteDto.CourseName);
            if (!result)
                return BadRequest(new ApiResponce(400, "Your are not Enrolled in This Course"));
            var assignmentSpec = new AssignmentSpec(submiteDto.AssignmentTitle);
            var assignment = await unitOfWork.Entity<Assignment>().GetWhereAndIncludedAsync(assignmentSpec);
            if(assignment == null)
                return NotFound(new ApiResponce(404, $"There is no Assignment With Title : {submiteDto.AssignmentTitle}"));
            var submission = new Submission()
            {
                FileUrl = submiteDto.FileUrl,
                AssignmentId = assignment.Id,
                StudentId = student.Id,
            };
            await unitOfWork.Entity<Submission>().AddAsync(submission);
            var count = await unitOfWork.SaveAsync();
            if (count == 0)
                return BadRequest(new ApiResponce(400, "Try To Submite Later"));
            var mapped = mapper.Map<Submission, SubmiteDto>(submission);
            return Ok(mapped);
        }

        [HttpPut("UpdateMySubmission")]
        [Authorize(Roles = "Student,Admin")]
        [ProducesResponseType(typeof(SubmiteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SubmiteDto>> UpdateSubmission(SubmiteDto submiteDto)
        {
            var studentEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var studentSpec = new StudentSpec(studentEmail);
            var student = await unitOfWork.Entity<Student>().GetWhereAndIncludedAsync(studentSpec);
            var submission = student.Submissions.FirstOrDefault(I=>I.Id == submiteDto.Id);
            if (submission == null)
                return NotFound(new ApiResponce(404, $"There Is No Submission For You With ID : {submiteDto.Id}"));
            submission.FileUrl = submiteDto.FileUrl;
            unitOfWork.Entity<Submission>().Update(submission);
            var count = await unitOfWork.SaveAsync();
            if (count == 0)
                return BadRequest(new ApiResponce(400, "Try To Update You Submission Later"));
            var mapped = mapper.Map<Submission, SubmiteDto>(submission);
            return Ok(mapped);
        }

        [HttpPut("Grading")]
        [Authorize(Roles = "Teacher,Admin")]
        [ProducesResponseType(typeof(SubmiteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SubmiteDto>> GradeSubmission (GradeDto gradeDto)
        {
            var submissionSpec = new SubmissionSpec(gradeDto.SubmissionId);
            var submission = await unitOfWork.Entity<Submission>().GetWhereAndIncludedAsync(submissionSpec);
            if(submission == null)
                return NotFound(new ApiResponce(404, $"There Is No Submission With ID : {gradeDto.SubmissionId}"));
            submission.Grade = gradeDto.Grade;
            submission.FeedBack = gradeDto.FeedBack;
            unitOfWork.Entity<Submission>().Update(submission);
            var count = await unitOfWork.SaveAsync();
            if (count == 0)
                return BadRequest(new ApiResponce(400, "Try To Grade Later"));
            var mapped = mapper.Map<Submission, SubmiteDto>(submission);
            return Ok(mapped);
        }

        [HttpDelete]
        [Authorize]
        [ProducesResponseType(typeof(SubmiteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SubmiteDto>> DeleteSubmission(int SubmissionId)
        {
            var submissionSpec = new SubmissionSpec(SubmissionId);
            var submission = await unitOfWork.Entity<Submission>().GetWhereAndIncludedAsync(submissionSpec);
            if (submission == null)
                return NotFound(new ApiResponce(404, $"There Is No Submission With ID : {SubmissionId}"));
            unitOfWork.Entity<Submission>().Delete(submission);
            var count = await unitOfWork.SaveAsync();
            if (count == 0)
                return BadRequest(new ApiResponce(400, "Try To Delete Later"));
            var mapped = mapper.Map<Submission, SubmiteDto>(submission);
            return Ok(mapped);
        }
    }
}