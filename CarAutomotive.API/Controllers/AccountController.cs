namespace CarAutomotive.API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AppUserDto>> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null) return Unauthorized(new ApiResponse(401));

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (result.Succeeded is false) return Unauthorized(new ApiResponse(401));

            
            var newAccessToken = _tokenService.CreateToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return Ok(new AppUserDto()
            {
                Email = user.Email!,
                DisplayName = user.DisplayName,
                Token = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        [HttpPost("register")]
        public async Task<ActionResult<AppUserDto>> Register(RegisterDto model)
        {
            var user = new AppUser()
            {
                
                DisplayName = model.DisplayName,
                Email = model.Email,
                UserName = model.Email.Split("@")[0]
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded is false) return BadRequest(new ApiResponse(400));

            
            var newAccessToken = _tokenService.CreateToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return Ok(new AppUserDto()
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Token = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<AppUserDto>> RefreshToken(TokenRequestDto tokenRequest)
        {
            if (tokenRequest is null) return BadRequest("Invalid client request");

            string accessToken = tokenRequest.AccessToken;
            string refreshToken = tokenRequest.RefreshToken;

            
            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            var email = principal.FindFirstValue(ClaimTypes.Email);

            if (email is null) return BadRequest("Invalid token client");

            
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return BadRequest("Invalid refresh token");

           
            var newAccessToken = _tokenService.CreateToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            
            return Ok(new AppUserDto()
            {
                Email = user.Email!,
                DisplayName = user.DisplayName,
                Token = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        [Authorize]
        [HttpGet("secret-room")]
        public ActionResult<string> GetSecretData()
        {
            return Ok("This is a secret room only for authorized users!");
        }
    }
}
