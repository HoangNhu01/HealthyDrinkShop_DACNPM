using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using eShopSolution.Application.AppSystem.Users;
using eShopSolution.Data.Entities;
using eShopSolution.ViewModels.AppSystem.ExternalUser;
using eShopSolution.ViewModels.AppSystem.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using NETCore.MailKit.Core;

namespace eShopSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        public UsersController(IUserService userService, IEmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
            
        }

        [HttpPost("authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.Authenticate(request);

            if (string.IsNullOrEmpty(result.Message) && !result.IsSuccessed)    
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.Register(request);
            if (String.IsNullOrEmpty(result.Message))
            {
                return BadRequest(result);
            }
            var rs = await _userService.SendConfirmEmail(result.ResultObj);
            if (!rs.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(rs);
        }
        [HttpGet("send-email/{email}")]
        [AllowAnonymous]
        public async Task<IActionResult> SendConfirmEmail(string email)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var rs = await _userService.SendConfirmEmail(email);

            if (!rs.IsSuccessed)
            {
                return BadRequest();
            }
            //_emailService.Send("hoangnhu300901@gmail.com", "Email Verìfy", "Hello");
            return Ok(rs);
        }
        [HttpPut("verify-email")]
        [AllowAnonymous]
        public async Task<IActionResult> VerìfyEmail(string email, string token)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.VerìfyEmail(email, token);
            if (!result.IsSuccessed)
            {
                return BadRequest();
            }
            return Ok(result);
        }
        [HttpPost("external-fb-authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalFbAuthenticate([FromBody] FaceBookUserInfor request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.ExternalAuthenticate(request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPost("external-gg-authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalGgAuthenticate([FromBody] GoogleUserInfor request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.ExternalAuthenticate(request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        //PUT: http://localhost/api/users/id
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.Update(id, request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("{id}/roles")]
        public async Task<IActionResult> RoleAssign(Guid id, [FromBody] RoleAssignRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.RoleAssign(id, request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        //http://localhost/api/users/paging?pageIndex=1&pageSize=10&keyword=
        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] GetUserPagingRequest request)
        {
            var users = await _userService.GetUsersPaging(request);
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetById(id);
            return Ok(user);
        }
        [HttpGet("searching/{userName}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByName(string userName)
        {
            var user = await _userService.GetByName(userName);
            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _userService.Delete(id);
            return Ok(result);
        }
    }
}