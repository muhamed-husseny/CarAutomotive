using CarAutomotive.Core.Entities;
using CarAutomotive.Core.Entities.Mechanic;

namespace CarAutomotive.API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IGenericRepository<Merchants> _merchantRepo;
        private readonly IGenericRepository<MechanicProfile> _mechanicRepo;
        private readonly IUnitOfWork _unitOfWork;
        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ITokenService tokenService,
            RoleManager<IdentityRole<Guid>> roleManager,
            IEmailService emailService, IGenericRepository<Merchants> merchantRepo,
            IGenericRepository<MechanicProfile> mechanicRepo,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _roleManager = roleManager;
        }

        [EnableRateLimiting("StrictPolicy")]
        [HttpPost("login")]
        public async Task<ActionResult<AppUserDto>> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null)
                return Unauthorized("Invalid email or password");

            var result = await _signInManager.CheckPasswordSignInAsync(
                user,
                model.Password,
                false);

            if (!result.Succeeded)
                return Unauthorized("Invalid email or password");


            var newAccessToken = await _tokenService.CreateToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _userManager.UpdateAsync(user);

            return Ok(new AppUserDto
            {
                Email = user.Email!,
                DisplayName = user.DisplayName,
                Token = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        [EnableRateLimiting("StrictPolicy")]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            var user = new AppUser()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                DisplayName = model.DisplayName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                UserName = model.Email.Split("@")[0],

                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

         
            var roleResult = await _userManager.AddToRoleAsync(user, model.Role);
            if (!roleResult.Succeeded) return BadRequest("Failed to assign role.");

            if (model.Role == "Merchant")
            {
                var merchant = new Merchants
                {
                    AppUserId = user.Id,
                    ShopName = model.BusinessName, 
                    Status = "PENDING VETTING"    
                };
                _merchantRepo.Add(merchant);
                await _unitOfWork.CompleteAsync();
            }
            else if (model.Role == "Mechanic")
            {
                var mechanic = new MechanicProfile
                {
                    UserId = user.Id,
                    Name = model.BusinessName,
                    IsAvailable = false 
                };
                _mechanicRepo.Add(mechanic);
                await _unitOfWork.CompleteAsync();
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var verificationLink = $"https://grad-project-lemon.vercel.app/verify-email?email={user.Email}&token={encodedToken}";

            await _emailService.SendEmailAsync(user.Email!, "Verify Your Email", $"<a href='{verificationLink}'>Verify</a>");

            return Ok(new { Message = "Registration successful. Please verify your email." });
            var verificationLink =
                        $"https://grad-project-lemon.vercel.app/verify-email" +
                        $"?email={user.Email}&token={encodedToken}";

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

            return Ok(new
            {
                Message = "Registration successful. Please verify your email."
            });
        }
        [EnableRateLimiting("StrictPolicy")]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return Ok("If the email exists, a reset link has been generated.");



            var token =
                await _userManager.GeneratePasswordResetTokenAsync(user);


            var encodedToken =
                WebEncoders.Base64UrlEncode(
                    Encoding.UTF8.GetBytes(token));


            return Ok(new
            {
                Email = user.Email,
                Token = encodedToken
            });
        }
        [EnableRateLimiting("StrictPolicy")]
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

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return NotFound();

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { Message = "Logged out successfully" });
        }

        [EnableRateLimiting("StrictPolicy")]
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

            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return BadRequest("Invalid refresh token");

            var newAccessToken = await _tokenService.CreateToken(user);
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

        [EnableRateLimiting("StrictPolicy")]
        [Authorize(Roles = "Admin")]
        [HttpPost("register-admin")]
        public async Task<ActionResult<AppUserDto>> RegisterAdmin(RegisterDto model)
        {
            var user = new AppUser()
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                UserName = model.Email.Split("@")[0],
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded is false) return BadRequest(new ApiResponse(400));

            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
            }

            await _userManager.AddToRoleAsync(user, "Admin");

            var newAccessToken = await _tokenService.CreateToken(user);
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

        //[HttpGet("confirm-email")]
        //public async Task<IActionResult> ConfirmEmail(string email, string token)
        //{
        //    var user = await _userManager.FindByEmailAsync(email);
        //
        //    if (user == null)
        //        return BadRequest("User not found");
        //
        //    Console.WriteLine($"TOKEN FROM URL: {token}");
        //
        //    token = WebUtility.UrlDecode(token);
        //
        //    Console.WriteLine($"DECODED TOKEN: {token}");
        //
        //    var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        //
        //    var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
        //
        //    if (!result.Succeeded)
        //        return BadRequest(result.Errors);
        //
        //    return Ok("Email verified successfully");
        //}
    }
}