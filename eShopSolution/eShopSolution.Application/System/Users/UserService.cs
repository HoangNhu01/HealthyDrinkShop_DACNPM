using eShopSolution.Data.Entities;
using eShopSolution.Utilities.ExternalLoginTool;
using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.AppSystem.ExternalUser;
using eShopSolution.ViewModels.AppSystem.Users;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NETCore.MailKit.Core;
using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using eShopSolution.Utilities.Exceptions;

namespace eShopSolution.Application.AppSystem.Users
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserService(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<AppRole> roleManager,
            IConfiguration config, IEmailService emailService, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _config = config;
            _emailService = emailService;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<ApiResult<string>> Authenticate(LoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null) return new ApiErrorResult<string>("Tài khoản không tồn tại");

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, request.RememberMe);
            if (!result.Succeeded)
            {
                return new ApiErrorResult<string>("Đăng nhập không đúng");
            }
            var token = await this.GetJwtTokenForUser(user);
            return new ApiSuccessResult<string>(token);
        }
        public async Task<ApiResult<string>>ExternalAuthenticate(FaceBookUserInfor user)
        {
            var userIndentity = await _userManager.FindByEmailAsync(user.Email);
            string format = "MM/dd/yyyy";
            if (userIndentity == null)
            {
                userIndentity = new AppUser()
                {
                    FirstName = user.First_name,
                    LastName = user.Last_name,
                    Email = user.Email,
                    Dob = DateTime.ParseExact(user.Birthday, format, CultureInfo.InvariantCulture),
                    UserName = UserNameConvert.ConvertUnicode(user.Name),
                    PhoneNumber = String.Empty,
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(userIndentity);
                if (!result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(userIndentity, "user");
                    return new ApiErrorResult<string>("Có lỗi xảy ra :((");
                }
            }
            else
            {
                userIndentity.FirstName = user.First_name;
                userIndentity.LastName = user.Last_name;
                userIndentity.Email = user.Email;
                userIndentity.Dob = DateTime.ParseExact(user.Birthday, format, CultureInfo.InvariantCulture);
                userIndentity.UserName = user.Name;
            }          
            var token = await this.GetJwtTokenForUser(userIndentity);
            return new ApiSuccessResult<string>(token);

        }
        public async Task<ApiResult<string>> ExternalAuthenticate(GoogleUserInfor user)
        {
            var userIndentity = await _userManager.FindByEmailAsync(user.Email);
            //string format = "MM/dd/yyyy";
            if (userIndentity == null)
            {
                userIndentity = new AppUser()
                {
                    FirstName = user.Given_name,
                    LastName = user.Family_name,
                    Email = user.Email,
                    Dob = DateTime.Now,
                    UserName = UserNameConvert.ConvertUnicode(user.Name),
                    PhoneNumber = String.Empty,
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(userIndentity);
                if (!result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(userIndentity,"user");
                    return new ApiErrorResult<string>("Có lỗi xảy ra :((");
                }
            }
            else
            {
                userIndentity.FirstName = user.Given_name;
                userIndentity.LastName = user.Family_name;
                userIndentity.Email = user.Email;
                userIndentity.Dob = DateTime.Now;
                userIndentity.UserName = user.Name;
            }
            var token = await this.GetJwtTokenForUser(userIndentity);
            return new ApiSuccessResult<string>(token);

        }
        private async Task<string> GetJwtTokenForUser(AppUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new[]
            {
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.GivenName,user.FirstName),
                new Claim(ClaimTypes.Role, string.Join(";",roles)),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("UserId",user.Id.ToString()),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Tokens:Issuer"],
                _config["Tokens:Issuer"],
                claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<ApiResult<bool>> Delete(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return new ApiErrorResult<bool>("User không tồn tại");
            }
            var role = await _userManager.GetRolesAsync(user);
            var remove = await _userManager.RemoveFromRolesAsync(user,role);
            var reult = await _userManager.DeleteAsync(user);
            if (reult.Succeeded)
                return new ApiSuccessResult<bool>();

            return new ApiErrorResult<bool>("Xóa không thành công");
        }

        public async Task<ApiResult<UserVm>> GetById(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return new ApiErrorResult<UserVm>("User không tồn tại");
            }
            var roles = await _userManager.GetRolesAsync(user);
            var userVm = new UserVm()
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                Dob = user.Dob,
                Id = user.Id,
                LastName = user.LastName,
                UserName = user.UserName,
                Roles = roles
            };
            return new ApiSuccessResult<UserVm>(userVm);
        }

        public async Task<ApiResult<UserVm>> GetByName(string name)
        {
            var user = await _userManager.FindByNameAsync(name);
            if (user == null)
            {
                return new ApiErrorResult<UserVm>("User không tồn tại");
            }
            var roles = await _userManager.GetRolesAsync(user);
            var userVm = new UserVm()
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                Dob = user.Dob,
                Id = user.Id,
                LastName = user.LastName,
                UserName = user.UserName,
                Roles = roles
            };
            return new ApiSuccessResult<UserVm>(userVm);
        }
        public async Task<ApiResult<PagedResult<UserVm>>> GetUsersPaging(GetUserPagingRequest request)
        {
            var query = _userManager.Users;
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.UserName.Contains(request.Keyword)
                 || x.PhoneNumber.Contains(request.Keyword));
            }
            var roles = _roleManager.Roles.ToList();
            //3. Paging
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new UserVm()
                {
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                    UserName = x.UserName,
                    FirstName = x.FirstName,
                    Id = x.Id,
                    LastName = x.LastName
                }).ToListAsync();
            foreach (var role in roles)
            {
                var users = _userManager.GetUsersInRoleAsync(role.Name).Result;
                foreach (var user in users)
                {
                    var userRole = data.FirstOrDefault(x => x.UserName == user.UserName);
                    if (userRole != null)
                    {
                        userRole.Roles.Add(role.Name);
                    }
                    else { continue; }
                }
                
            }
            //4. Select and projection
            var pagedResult = new PagedResult<UserVm>()
            {
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = data
            };
            return new ApiSuccessResult<PagedResult<UserVm>>(pagedResult);
        }

        public async Task<ApiResult<string>> Register(RegisterRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user != null)
            {
                return new ApiErrorResult<string>("Tài khoản đã tồn tại");
            }
            if (await _userManager.FindByEmailAsync(request.Email) != null)
            {
                return new ApiErrorResult<string>("Emai đã tồn tại");
            }

            user = new AppUser()
            {
                Dob = request.Dob,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "user");
                return new ApiSuccessResult<string>(user.Email, "Bạn đã đăng kí thành công");
            }
            return new ApiSuccessResult<string>("Đăng ký không thành công");
        }

        public async Task<ApiResult<bool>> RoleAssign(Guid id, RoleAssignRequest request)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return new ApiErrorResult<bool>("Tài khoản không tồn tại");
            }
            var removedRoles = request.Roles.Where(x => x.Selected == false).Select(x => x.Name).ToList();
            foreach (var roleName in removedRoles)
            {
                if (await _userManager.IsInRoleAsync(user, roleName) == true)
                {
                    await _userManager.RemoveFromRoleAsync(user, roleName);
                }
            }
            //await _userManager.RemoveFromRolesAsync(user, removedRoles);

            var addedRoles = request.Roles.Where(x => x.Selected).Select(x => x.Name).ToList();
            foreach (var roleName in addedRoles)
            {
                if (await _userManager.IsInRoleAsync(user, roleName) == false)
                {
                    await _userManager.AddToRoleAsync(user, roleName);
                }
            }

            return new ApiSuccessResult<bool>();
        }

        public async Task<ApiResult<bool>> Update(Guid id, UserUpdateRequest request)
        {
            if (await _userManager.Users.AnyAsync(x => x.Email == request.Email && x.Id != id))
            {
                return new ApiErrorResult<bool>("Emai đã tồn tại");
            }
            var user = await _userManager.FindByIdAsync(id.ToString());
            user.Dob = request.Dob;
            user.Email = request.Email;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return new ApiSuccessResult<bool>();
            }
            return new ApiErrorResult<bool>("Cập nhật không thành công");
        }

        public async Task<ApiResult<string>> SendConfirmEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var content = System.IO.File.ReadAllText(Path.Combine(_webHostEnvironment.WebRootPath, _config["EmailConfig:EmailContent"]));
            content = content.Replace("#Name#", user.FirstName);
            content = content.Replace("#Email#", user.Email);
            content = content.Replace("#Token#", token);

            MailMessage message = new MailMessage(
                from: _config["EmailConfig:SenderEmail"],
                to: user.Email,
                subject: "EShopDrink kính chào quý khách hàng",
                body: content
            );
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;
            message.ReplyToList.Add(new MailAddress(_config["EmailConfig:SenderEmail"]));
            message.Sender = new MailAddress(_config["EmailConfig:SenderEmail"]);


            // Tạo SmtpClient kết nối đến smtp.gmail.com
            using (SmtpClient client = new SmtpClient("smtp.gmail.com"))
            {
                client.Port = int.Parse(_config["EmailConfig:Port"]);
                client.UseDefaultCredentials = true;
                client.Credentials = new NetworkCredential(_config["EmailConfig:Account"], _config["EmailConfig:Password"]);
                client.EnableSsl = true;
                try
                {
                    await client.SendMailAsync(message);
                    return new ApiSuccessResult<string>($"Dã đăng kí thành công. Vui lòng xác thực email: {user.Email}");

                }
                catch (EShopException ex)
                {
                    return new ApiErrorResult<string>(ex.Message);

                }
            }           
        }

        public async Task<ApiResult<string>> VerìfyEmail(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            token = token.Replace(" ", "+");
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return new ApiSuccessResult<string>("Xác thực email thành công");
            }
            return new ApiErrorResult<string>("Xác thực email không thành công");
        }
    }
}