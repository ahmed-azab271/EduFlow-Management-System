using APIs.DTO;
using APIs.Errors;
using AutoMapper;
using Core.Entites;
using Core.Entites.Identity;
using Core.IRepository;
using Core.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Repository;

namespace APIs.Controllers
{
    public class TeacherController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ITokenService tokenService;
        private readonly IGetTeacher getTeacher;

        public TeacherController(UserManager<ApplicationUser> _userManager 
            , IUnitOfWork _unitOfWork , IMapper _mapper, ITokenService _tokenService , IGetTeacher _getTeacher) 
        {
            userManager = _userManager;
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            tokenService = _tokenService;
            getTeacher = _getTeacher;
        }
        [HttpPost("RegisterFoTeachers")]
        [ProducesResponseType(typeof(TeacherDto) , StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce) , StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponce) , StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TeacherDto>> AddTeacher(RegisterDto registerDto)
        {
            var user = await userManager.FindByEmailAsync(registerDto.Email);
            if (user is null)
            {
                var AppUser = new ApplicationUser
                {
                    Email = registerDto.Email,
                    UserName = registerDto.Email.Split('@')[0],
                    FullName = registerDto.FullName,
                    PhoneNumber = registerDto.PhoneNumber,
                    Address = registerDto.Address,
                };
                var result = await userManager.CreateAsync(AppUser, registerDto.Password);
                if (result.Succeeded)
                {
                    var Teacher = new Teacher()
                    {
                        FullName = registerDto.FullName,
                        PhoneNumber = registerDto.PhoneNumber,
                        Address = registerDto.Address,
                        Email = registerDto.Email,
                        ApplicationUserId = AppUser.Id,
                    };
                    await unitOfWork.Entity<Teacher>().AddAsync(Teacher);
                    if (userManager.Users.Count() == 1)
                        await userManager.AddToRoleAsync(AppUser, "Admin");
                    else
                        await userManager.AddToRoleAsync(AppUser, "Teacher");
                    await unitOfWork.SaveAsync();
                    var TeacherDto = mapper.Map<Teacher, TeacherDto>(Teacher);
                    TeacherDto.Token = await tokenService.CreateTokenAsync(AppUser, userManager);
                    return Ok(TeacherDto);
                }
                else { return BadRequest(new ApiResponce(500)); }
            }
            else { return BadRequest(new ApiResponce(400 , "The Email Is Already Taken")); }
        }
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(TeacherDto),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce),StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponce),StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TeacherDto>> DeleteTeacher(string TeacherEmail)
        {
            var Teacher = await getTeacher.GetTeacherAsync(TeacherEmail);
            if (Teacher is not null)
            {
                var user = await userManager.FindByIdAsync(Teacher.ApplicationUserId);
                var result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    unitOfWork.Entity<Teacher>().Delete(Teacher);
                    await unitOfWork.SaveAsync();
                    var teacherDto = mapper.Map<Teacher, TeacherDto>(Teacher);
                    return Ok(teacherDto);
                }
                return BadRequest(new ApiResponce(500)); 
            }
            return NotFound(new ApiResponce(404));
        }
    }
}
