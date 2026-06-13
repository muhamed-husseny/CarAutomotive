namespace CarAutomotive.API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService,IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _emailService = emailService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AppUserDto>> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null) return Unauthorized(new ApiResponse(401));

            if (!user.EmailConfirmed)
                return Unauthorized("Please verify your email first.");
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
                FirstName = model.FirstName,
                LastName = model.LastName,
                DisplayName = model.DisplayName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                UserName = model.Email.Split("@")[0]
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            //if (result.Succeeded is false) return BadRequest(new ApiResponse(400));
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            Console.WriteLine($"RAW TOKEN: {token}");

            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            Console.WriteLine($"ENCODED TOKEN: {encodedToken}");

            var verificationLink =
                        $"https://grad-project-lemon.vercel.app/verify-email" +
                        $"?email={user.Email}&token={encodedToken}";
            Console.WriteLine("Before Email");
            await _emailService.SendEmailAsync(user.Email!,
                "Verify Your Email",
                $@"
                    <h2>Welcome To CarAutomotive</h2>
                    <p>Please verify your email by clicking the link below:</p>
                    <a href='{verificationLink}'>
                        Verify Email
                    </a>
                ");
            Console.WriteLine("After Email");
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

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return Ok("If the email exists, a reset link has been sent.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            Console.WriteLine($"RESET TOKEN: {token}");
            Console.WriteLine($"ENCODED RESET TOKEN: {encodedToken}");
            var resetLink =
                    $"https://grad-project-lemon.vercel.app/reset-password" +
                    $"?email={user.Email}&token={encodedToken}";

            await _emailService.SendEmailAsync(
                user.Email!,
                "Reset Your Password",
                $@"<h2>CarAutomotive Password Reset</h2>
                   <p>Click the link below to reset your password:</p>
                   <a href='{resetLink}'>Reset Password</a>");

            return Ok("Password reset email sent.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return BadRequest("User not found");

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));

            var result = await _userManager.ResetPasswordAsync(
                user,
                decodedToken,
                model.NewPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Password reset successfully");
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

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string email,string token)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return BadRequest("User not found");

            Console.WriteLine($"TOKEN FROM URL: {token}");

            token = WebUtility.UrlDecode(token);

            Console.WriteLine($"DECODED TOKEN: {token}");

            var decodedToken = Encoding.UTF8.GetString( WebEncoders.Base64UrlDecode(token));

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Email verified successfully");
        }

    }
}
