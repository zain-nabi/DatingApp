using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationUserController : ControllerBase
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        public ApplicationUserController(IUserRepository repository, IMapper mapper){
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await _repository.GetMembersAsync();
            return Ok(users);
        }

        //api/Users/3
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _repository.GetMemberAsync(username);

        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto MemberUpdateDto)
        {
            var claimsIdentity = User.Identity as System.Security.Claims.ClaimsIdentity;
            var username = claimsIdentity.Name;
            var user = await _repository.GetUserByUsername(username);

            _mapper.Map(MemberUpdateDto, user);

            _repository.Update(user);
            if(await _repository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update user");
        }
    }
}