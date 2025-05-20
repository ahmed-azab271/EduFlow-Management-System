using APIs.DTO;
using APIs.Errors;
using AutoMapper;
using Core.Entites;
using Core.IRepository;
using Core.IServices;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Repository;
using System.Diagnostics.CodeAnalysis;

namespace APIs.Controllers
{
    public class CourseController : BaseController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IGetTeacher getTeacher;
        private readonly IMapper mapper;

        public CourseController(IUnitOfWork _unitOfWork , IMapper _mapper , IGetTeacher _getTeacher)
        {
            unitOfWork = _unitOfWork;
            getTeacher = _getTeacher;
            mapper = _mapper;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(CourseDto) , StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce) , StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CourseDto>> AddCourse (CourseDto courseDto)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                if (!await CheckExcistinCourse(courseDto.Code))
                {
                    var Teacher = await getTeacher.GetTeacherAsync(courseDto.TeacherName);
                    var MappedCourse = new Course() 
                    { 
                        Name = courseDto.Name,
                        Code = courseDto.Code,
                        Credits = courseDto.Credits,
                        TeacherId = Teacher.Id 
                    };
                    await unitOfWork.Entity<Course>().AddAsync(MappedCourse);
                    await unitOfWork.SaveAsync();
                    return Ok(courseDto);
                }
                return BadRequest(new ApiResponce(400, "This Course Is Already Exist"));
            }
            catch (Exception ex) { return BadRequest(new ApiResponce(500, $"Internal server error: {ex.Message}")); }
            
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<CourseDto>>> GetCourses()
        {
            try
            {
                var Spec = new ExcistingCourseSpec();
                var Courses = await unitOfWork.Entity<Course>().GetAllIncludedAsync(Spec,true);
                var Mapped = mapper.Map<IReadOnlyList<Course>, IReadOnlyList<CourseDto>>(Courses);
                return Ok(Mapped);
            }
            catch (Exception ex) { return BadRequest(new ApiResponce(500, $"Internal server error: {ex.Message}")); }
        }

        [HttpGet("{Code}")]
        [Authorize]
        [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce) , StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CourseDto>> GetCourseByCode(string Code)
        {
            try
            {
                var Spec = new ExcistingCourseSpec(Code);
                var course = await unitOfWork.Entity<Course>().GetWhereAndIncludedAsync(Spec,true);
                if(course is not null )
                { 
                    var Mapped = mapper.Map<Course, CourseDto>(course);
                    return Ok(Mapped);
                }
                else  { return NotFound(new ApiResponce(404, $"There is no Course With Code : {Code}")); }
            }
            catch (Exception ex) { return BadRequest(new ApiResponce(500, $"Internal server error: {ex.Message}")); }
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CourseDto>> EditCourse(CourseDto courseDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var Spec = new ExcistingCourseSpec(courseDto.Code);
                var Course = await unitOfWork.Entity<Course>().GetWhereAndIncludedAsync(Spec);
                if (Course is not null)
                {
                    var Teacher = await getTeacher.GetTeacherAsync(courseDto.TeacherName);
                    if (Teacher is not null)
                    {
                        Course.Name = courseDto.Name;
                        Course.Code = courseDto.Code;
                        Course.Credits = courseDto.Credits;
                        Course.TeacherId = Teacher.Id;
                        unitOfWork.Entity<Course>().Update(Course);
                        await unitOfWork.SaveAsync();
                        return Ok(courseDto);
                    }
                    return NotFound(new ApiResponce(404, $"There Is no Teacher :{courseDto.TeacherName} in Our Daatabase"));
                }
                return NotFound(new ApiResponce(404, $"There is no Course With Code : {courseDto.Code}"));
            }
            catch(Exception ex) { return BadRequest(new ApiResponce(500, $"Internal server error: {ex.Message}")); }
        }

        [HttpDelete]
        [Authorize(Roles ="Admin")]
        [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CourseDto>> DeleteCourse(string Code)
        {
            try
            {
                var Spec = new ExcistingCourseSpec(Code);
                var DeletedCourse = await unitOfWork.Entity<Course>().GetWhereAndIncludedAsync(Spec);
                if (DeletedCourse is not null)
                {
                    unitOfWork.Entity<Course>().Delete(DeletedCourse);
                    await unitOfWork.SaveAsync();
                    var Mapped = mapper.Map<Course, CourseDto>(DeletedCourse);
                    return Ok(Mapped);
                }
                return NotFound(new ApiResponce(404, $"There is no Course With Code : {Code}"));
            }
            catch(Exception ex) { return BadRequest(new ApiResponce(500, $"Internal server error: {ex.Message}")); }
        }

        [HttpGet("CheckExcistinCourse")]
        public async Task<bool> CheckExcistinCourse(string Code)
        {
            var Spec = new ExcistingCourseSpec(Code);
            var Course = await unitOfWork.Entity<Course>().GetWhereAndIncludedAsync(Spec, true);
            if(Course is null) return false; 
            return true;
        }
    }
}