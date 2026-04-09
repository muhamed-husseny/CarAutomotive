namespace CarAutomotive.API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AppUserDto>> Login(LoginDto model)
        {

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null)
                return Unauthorized(new ApiResponse(401));

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if(result.Succeeded is false)
                return Unauthorized(new ApiResponse(401));

            return Ok(new AppUserDto()
            {
                Email = user.Email!,
                DisplayName = user.DisplayName,
                Token = "This is a token"
            });

        }

        [HttpPost("register")]
        public async Task<ActionResult<AppUserDto>> Register(RegisterDto model)
        {
            var user = new AppUser()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                DisplayName = model.DisplayName,
                Email = model.Email,
                UserName = model.Email.Split("@")[0],
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if(result.Succeeded is false)
                return BadRequest(new ApiResponse(400));

            return Ok(new AppUserDto()
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Token = "This is a token"
            });
        }


    }
}
