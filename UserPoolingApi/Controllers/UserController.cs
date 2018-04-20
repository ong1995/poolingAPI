using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UserPoolingApi.Ef;
using UserPoolingApi.Enums;
using UserPoolingApi.Helper;
using UserPoolingApi.Models;
using UserPoolingApi.Models.Enums;
using UserPoolingApi.ViewModels;

namespace UserPoolingApi.Controllers
{
    [Produces("application/json")]
    [Route("User")]
    [Authorize]
    public class UserController : Controller
    {
        private DataContext _context;
        private readonly SmtpHelper _send;
        private readonly IMapper _map;
        public static IConfiguration _configuration { get; set; }


        public UserController(IMapper map, DataContext context, SmtpHelper send, IConfiguration configuration)
        {
            _context = context;
            _send = send;
            _map = map;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            return Ok(_map.Map<IEnumerable<DisplayUserViewModel>>(_context.Users.Include("UserSkills").Include("UserSkills.Skill")));
        }

        [HttpGet]
        [Route("PaginateUser")]
        public IActionResult GetPage(int pageNumber = 1, int pageSize = 1, string searchFirstName = "")
        {
            PagedParamModel param = new PagedParamModel()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                searchValue = searchFirstName
            };
            Expression<Func<User, bool>> searchFunc = (s => s.FirstName.ToLower().Contains(searchFirstName.ToLower()));

            var users = _context.Users.Include("UserSkills").Include("UserSkills.Skill").ToPagedList(param, filter: !string.IsNullOrEmpty(searchFirstName) ? searchFunc : null, orderBy: null);
            var results = _map.Map<PageResultViewModel>(users);
            return Ok(results);
        }

        [HttpGet]
        [Route("DownloadFile")]
        public async Task<IActionResult> DLFile(int userId)
        {
            var user = _context.Users.Find(userId);
            string fileName = user.Filename;
            string path = Path.Combine("C:\\UploadFiles", userId.ToString());
            if (!Directory.Exists(path))
            {
                return NotFound("No files found.");
            }

            string file = Path.Combine(path, fileName);
            string contentType = FileHelper.GetContentType(file);
            var memory = await FileHelper.CreateMemoryStream(file);

            return File(memory, contentType, Path.GetFileName(file));
        }

        [HttpGet("Status")]
        public IActionResult GetStatus()
        {
            //List<StatusEnum> status = Enum.GetValues(typeof(StatusEnum)).Cast<StatusEnum>().ToList();
            List<UserStatusModel> status = ((StatusEnum[])Enum.GetValues(typeof(StatusEnum))).Select(c => new UserStatusModel() { Value = (int)c, Name = c.ToString().ToSentenceCase() }).ToList();
            return Ok(status);
        }

        [HttpPut("Status/{id}")]
        public IActionResult UpdateStatus(int id, string status)
        {
            status = status.ToPascalCase();
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            if (user == null)
            {
                return BadRequest("User with ID " + id + " is not found");
            }

            user.Status = (StatusEnum)Enum.Parse(typeof(StatusEnum), status);
            user.StatusDate = DateTime.Now.AddHours(8);
            _context.Users.Update(user);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            var user = _context.Users.Include("UserSkills").Include("UserSkills.Skill").FirstOrDefault(u => u.UserId == id);

            if (user == null)
            {
                return NotFound("User with Id " + id + " is not found");
            }

            return Ok(_map.Map<DisplayUserViewModel>(user));
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAsync(UserViewModel userVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // email validation
                var is_email_exist = _context.Users.Any(u => u.Email == userVM.Email);

                if (is_email_exist)
                {
                    return StatusCode(409);
                }
                userVM.SubmittedDate = DateTime.Now.AddHours(8);
                var user = _map.Map<User>(userVM);
                _context.Add(user);
                _context.SaveChanges();

                // save skill
                if (userVM.Skills != null)
                {
                    string[] skillId = null;
                    skillId = userVM.Skills.Split(",");
                    for (int i = 0; i < skillId.Length; i++)
                    {
                        UserSkillViewModel userskillVM = new UserSkillViewModel
                        {
                            UserId = user.UserId,
                            SkillId = int.Parse(skillId[i])
                        };
                        _context.Add(_map.Map<UserSkills>(userskillVM));
                    }
                    _context.SaveChanges();
                }

                // will create folder where it stored the uploaded file.
                if (userVM.Filename == null || userVM.Filename.Length == 0)
                    return Content("file not selected");

                var path = Path.Combine("C:\\UploadFiles",
                            user.UserId.ToString(), userVM.Filename.FileName);
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    userVM.Filename.CopyTo(stream);
                }

                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                // email the user_config.GetSection("AppSettings:Token").Value
                await _send.SendEmailAsync(userVM.FirstName, _configuration.GetSection("AdminEmailCredentials:SenderEmail").Value, userVM.Email,
                    "Thank you for submitting your profile to Dev Partners. We will review your CV and contact you for current openings.", "Application received.");
                // email the admin
                await _send.SendEmailAsync("Admin", _configuration.GetSection("AdminEmailCredentials:SenderEmail").Value, _configuration["AdminEmailCredentials:ReceiverEmail"], 
                    "A new CV/profile has been uploaded to the Pooling System on " + userVM.SubmittedDate, "New application received on the Pooling System.");

                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId == id);

                if (user == null)
                {
                    return NotFound("User with ID " + id + " is not found.");
                }

                string path = Path.Combine("C:\\UploadFiles", id.ToString());
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }

                _context.Users.Remove(user);
                _context.SaveChanges();
                return new NoContentResult();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}