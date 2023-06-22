﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace ChatApi.Controllers
{
    [ApiController]
    [Route("chat")]
    public class ChatController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly ConnectionMultiplexer _redis;


        public ChatController(IConfiguration configuration)
        {
            _configuration = configuration;
            _redis = StackExchange.Redis.ConnectionMultiplexer.Connect(_configuration.GetSection("Redis")["ConnectionString"]);
        }


        [HttpGet]
        public IActionResult Get()
        {
            var db = _redis.GetDatabase();
            var messages = db.ListRange("messages");
            return Ok(messages);
        }

        [HttpPost]
        public IActionResult Post([FromBody] string message)
        {
            var db = _redis.GetDatabase();
            db.ListRightPush("messages", message);
            return Ok();
        }

    }
}
