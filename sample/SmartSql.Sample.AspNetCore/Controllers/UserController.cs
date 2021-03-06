﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SmartSql.Sample.AspNetCore.DyRepositories;
using SmartSql.Test.DTO;
using SmartSql.Test.Entities;

namespace SmartSql.Sample.AspNetCore.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        // GET api/values
        [HttpPost]
        public long Add([FromBody]User user)
        {
            var id = _userRepository.Insert(user);
            return id;
        }

        // POST api/values
        [HttpGet]
        public User GetById(long id)
        {
            var user = _userRepository.GetById(id);
            var userJson = Newtonsoft.Json.JsonConvert.SerializeObject(user);
            return user;
        }

        [HttpGet]
        public GetByPageResponse<User> GetByPage(int pageIndex = 1)
        {
            return _userRepository.GetByPage<GetByPageResponse<User>>(new
            {
                PageSize = 10,
                PageIndex = pageIndex
            });
        }
        [HttpGet]
        public IEnumerable<User> Query(int taken = 10)
        {
            return _userRepository.Query(taken);
        }
        [HttpGet]
        public async Task<IEnumerable<User>> QueryAsync(int taken = 10)
        {
            return await _userRepository.QueryAsync(taken);
        }
        [HttpGet]
        public async Task Mt(int id)
        {
            try
            {
                _userRepository.SqlMapper.BeginTransaction();
                await _userRepository.InsertAsync(new User
                {
                    Id = id,
                    UserName = "SmartSql"
                });
                var task1 = _userRepository.QueryAsync(10);
                var task2 = _userRepository.QueryAsync(10);
                await Task.WhenAll(task1, task2);
                _userRepository.SqlMapper.CommitTransaction();
            }
            catch (Exception e)
            {
                _userRepository.SqlMapper.RollbackTransaction();
                throw;
            }
            
        }
    }
}
