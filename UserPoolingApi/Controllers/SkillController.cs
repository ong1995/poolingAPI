using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserPoolingApi.Ef;
using UserPoolingApi.Models;
using UserPoolingApi.ViewModels;

namespace UserPoolingApi.Controllers
{
    [Produces("application/json")]
    [Route("Skill")]
    public class SkillController : Controller
    {
        private readonly DataContext _context;
        private readonly IMapper _map;

        public SkillController(DataContext context, IMapper map)
        {
            _context = context;
            this._map = map;
        }

        [HttpGet]
        public IActionResult getAllSkills()
        {
            return Ok(_map.Map<IEnumerable<SkillViewModel>>(_context.Skills));
        }

        [HttpGet("{id}")]
        public IActionResult getById(int id)
        {
            try
            {
                var skill = _context.Skills.FirstOrDefault(s => s.SkillId == id);
                if (skill == null)
                {
                    return NotFound("Skill ID with " + id + " is not found.");
                }

                return Ok(_map.Map<SkillViewModel>(skill));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create([FromBody] SkillViewModel skillVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var is_skill_exist = _context.Skills.Any(s => s.SkillName == skillVM.SkillName);

                if (is_skill_exist)
                {
                    return StatusCode(409);
                }

                _context.Add(_map.Map<Skill>(skillVM));
                _context.SaveChanges();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }            
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] SkillViewModel skillVM)
        {
            try
            {
                var skill = _context.Skills.FirstOrDefault(s => s.SkillId == id);
                if (skill == null)
                {
                    return NotFound("Skill ID with " + id + " is not found");
                }

                var is_skill_exist = _context.Skills.Any(s => s.SkillName == skillVM.SkillName);

                if (is_skill_exist)
                {
                    return StatusCode(409);
                }

                skillVM.SkillId = id;
                _map.Map(skillVM, skill);
                _context.SaveChanges();

                return new NoContentResult();
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
                var skill = _context.Skills.FirstOrDefault(s => s.SkillId == id);

                if (skill == null)
                {
                    return NotFound("SKill ID with "+ id +" is not found");
                }

                _context.Skills.Remove(skill);
                _context.SaveChanges();

                return new NoContentResult();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}