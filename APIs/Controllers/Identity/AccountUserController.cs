using APIs.DTO;
using APIs.Errors;
using AutoMapper;
using Core.Entites;
using Core.Entites.Identity;
using Core.IRepository;
using Core.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIs.Controllers.Identity
{
    public class AccountUserController : BaseController
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ITokenService tokenService;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public AccountUserController(SignInManager<ApplicationUser> _signInManager,UserManager<ApplicationUser> _userManager ,
            IMapper _mapper , IUnitOfWork _unitOfWork , ITokenService _tokenService)
        {
            signInManager = _signInManager;
            tokenService = _tokenService;
            userManager = _userManager;
            unitOfWork = _unitOfWork;
            mapper = _mapper;
        }
        [HttpPost("Register")]
        [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StudentDto>> Register (RegisterDto model)
        {
            var Appuser = await userManager.FindByEmailAsync(model.Email);
            if (Appuser is null)
            {
                var user = new ApplicationUser() 
                {
                    FullName =  model.FullName,
                    Email = model.Email,
                    UserName = model.Email.Split('@')[0],
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var student = new Student()
                    {
                        FullName = model.FullName,
                        Email = model.Email,
                        Address = model.Address,
                        PhoneNumber = model.PhoneNumber,
                        ApplicationUserId = user.Id,
                    };
                    await unitOfWork.Entity<Student>().AddAsync(student);
                    if (userManager.Users.Count() == 1)
                        await userManager.AddToRoleAsync(user, "Admin");
                    else
                        await userManager.AddToRoleAsync(user, "Student");
                    await unitOfWork.SaveAsync();
                    var MappedStudent  = mapper.Map<Student,StudentDto>(student);
                    MappedStudent.Token = await tokenService.CreateTokenAsync(user, userManager);
                    return Ok(MappedStudent);
                }
            }
            return BadRequest(new ApiResponce(500, "The Email Already Taken"));
        }

        [HttpPost("Login")]
        [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StudentDto>> Login (LoginDto login)
        {
            var user = await userManager.FindByEmailAsync(login.Email);
            if (user == null) 
                return NotFound(new ApiResponce(404, "Incorrect Email"));
            var result = await signInManager.CheckPasswordSignInAsync(user, login.Password, false);
            if (result.Succeeded)
            {
                var ReturnDto = new StudentDto()
                {
                    Email = login.Email,
                    Token = await tokenService.CreateTokenAsync(user,userManager)
                };
                return Ok(ReturnDto);
            }
            return BadRequest(new ApiResponce(500 , "Incorrect Password"));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<UserDto>>> GetUsers()
        {
            var users = await userManager.Users.ToListAsync();
            if (!users.Any())
                return BadRequest(new ApiResponce(400, "There Is no Users Yet"));
            List<UserDto> UsersDto = new List<UserDto>();
            foreach (var user in users)
            {
                var userDto = new UserDto()
                {
                    Id = user.Id,
                    Email = user.Email,
                    Roles = new List<string>(await userManager.GetRolesAsync(user))
                };
                UsersDto.Add(userDto);
            }
            return Ok(UsersDto);
        }
    }
}
