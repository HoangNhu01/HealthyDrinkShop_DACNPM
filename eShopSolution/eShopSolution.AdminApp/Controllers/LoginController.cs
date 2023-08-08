using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using eShopSolution.AdminApp.Services;
using eShopSolution.Utilities.Constants;
using eShopSolution.ViewModels.AppSystem.ExternalUser;
using eShopSolution.ViewModels.AppSystem.Users;
using Microsoft.AspNet.SignalR.Client.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;
using Microsoft.Data.SqlClient.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using static eShopSolution.Utilities.Constants.SystemConstants;

namespace eShopSolution.AdminApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserApiClient _userApiClient;
        private readonly IConfiguration _configuration;

        public LoginController(IUserApiClient userApiClient,
            IConfiguration configuration)
        {
            _userApiClient = userApiClient;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var Fbsetting = new FaceBookSettings()
            {
                FacebookAppId = _configuration[SystemConstants.FaceBookAuthentication.FacebookAppId],
                FacebookAppSecret = _configuration[SystemConstants.FaceBookAuthentication.FacebookAppSecret],
                FacebookRedirectUri = _configuration[SystemConstants.FaceBookAuthentication.FacebookRedirectUri]
            };
            ViewBag.Fbsetting = Fbsetting;
            var Ggsetting = new GoogleSetting()
            {
                GoogleClientId = _configuration[SystemConstants.GoogleAuthentication.GoogleClientId],
                GoogleRedirectUri = _configuration[SystemConstants.GoogleAuthentication.GoogleRedirectUri],
                Scope = _configuration[SystemConstants.GoogleAuthentication.Scope]
            };
            ViewBag.Ggsetting = Ggsetting;
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> FaceBookExtenalLogin(string code)
        {
            var accessToken = await _userApiClient.GetAccessFBTokenAsync(_configuration[SystemConstants.FaceBookAuthentication.FacebookAppId],
                                                                       _configuration[SystemConstants.FaceBookAuthentication.FacebookAppSecret],
                                                                 _configuration[SystemConstants.FaceBookAuthentication.FacebookRedirectUri], code);
            if(accessToken.Access_token == null)
            {
                return RedirectToAction("PageNotFound");
            }
            var user = await _userApiClient.GetFbUserAsync(accessToken.Access_token);

            var result = await _userApiClient.ExternalAuthenticate(user);
            if (result.ResultObj == null)
            {
                return RedirectToAction("PageNotFound");
            }
            await SettingToken(result.ResultObj, true);
            TempData["img"] = user.Picture.Data.Url;
            return RedirectToAction("Index", "Home");

        }
        [HttpGet]
        public async Task<IActionResult> GoogleExtenalLogin(string code)
        {
            var accessToken = await _userApiClient.GetAccessGGTokenAsync(_configuration[SystemConstants.GoogleAuthentication.GoogleClientId],
                                                                       _configuration[SystemConstants.GoogleAuthentication.ClientSecret],
                                                                 _configuration[SystemConstants.GoogleAuthentication.GoogleRedirectUri], code);
            if (accessToken.Access_token == null)
            {
                return RedirectToAction("PageNotFound");
            }
            var user = await _userApiClient.GetGgUserAsync(accessToken.Access_token);

            var result = await _userApiClient.ExternalAuthenticate(user);
            if (result.ResultObj == null)
            {
                return RedirectToAction("PageNotFound");
            }
            await SettingToken(result.ResultObj, true);
            TempData["img"] = user.Picture;
            return RedirectToAction("Index", "Home");

        }
        public IActionResult PageNotFound()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index");

            var result = await _userApiClient.RegisterUser(request);
            TempData["result"] = result;
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Index(LoginRequest request)
        {
            if (!ModelState.IsValid)
                return View(ModelState);

            var result = await _userApiClient.Authenticate(request);
            if (result.ResultObj == null)
            {
                ModelState.AddModelError("", result.Message);
                return View();
            }
            await SettingToken(result.ResultObj, request.RememberMe);
            TempData["img"] = null;

            return RedirectToAction("Index", "Home");
        }
        private async Task SettingToken(string result, bool isPersistent)
        {
            var userPrincipal = this.ValidateToken(result);
            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
                IsPersistent = isPersistent
            };
            HttpContext.Session.SetString(SystemConstants.AppSettings.DefaultLanguageId, _configuration[SystemConstants.AppSettings.DefaultLanguageId]);
            HttpContext.Session.SetString("Token", result);
            await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        userPrincipal,
                        authProperties);
        }
        private ClaimsPrincipal ValidateToken(string jwtToken)
        {
            IdentityModelEventSource.ShowPII = true;

            SecurityToken validatedToken;
            TokenValidationParameters validationParameters = new TokenValidationParameters();

            validationParameters.ValidateLifetime = true;

            validationParameters.ValidAudience = _configuration["Tokens:Issuer"];
            validationParameters.ValidIssuer = _configuration["Tokens:Issuer"];
            validationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));

            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out validatedToken);

            return principal;
        }
    }
}