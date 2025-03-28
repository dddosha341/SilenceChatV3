﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Silence.Web.Entities;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Silence.Web.Hubs;
using Silence.Infrastructure.ViewModels;
using System.Text.RegularExpressions;
using Silence.Web.Services;

namespace Silence.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly DbService _db;
        private readonly IMapper _mapper;
        private readonly IHubContext<ChatHub> _hubContext;

        public MessagesController(DbService db,
            IMapper mapper,
            IHubContext<ChatHub> hubContext
            )
        {
            _db = db;
            _mapper = mapper;
            _hubContext = hubContext;
            //_authController = authController;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> Get(int id)
        {
            //Взаимодействие с СУБД должно быть асинхронным, для 100% результата
            var message = await _db.GetMessage(id);
            if (message == null)
                return NotFound();

            var messageViewModel = _mapper.Map<Message, MessageViewModel>(message);
            return Ok(messageViewModel);
        }

        //Сделали ручку асинхронной
        [HttpGet("Room/{roomName}")]
        public async Task<IActionResult> GetMessages(string roomName)
        {
            //Взаимодействие с СУБД должно быть асинхронным, для 100% результата
            var room = await _db.GetRoom(roomName);
            if (room == null)
                return BadRequest();

            //Взаимодействие с СУБД должно быть асинхронным, для 100% результата
            var messages = await _db.GetMessages(room.Id);

            var messagesViewModel = _mapper.Map<IEnumerable<Message>, IEnumerable<MessageViewModel>>(messages);

            return Ok(messagesViewModel);
        }

        [HttpPost]
        public async Task<ActionResult<Message>> Create(MessageViewModel viewModel)
        {
            //Взаимодействие с СУБД должно быть асинхронным, для 100% результата
            var user = await _db.GetUser(User.Identity.Name);
            var room = await _db.GetRoom(viewModel.Room);
            if (room == null)
                return BadRequest();

            var msg = new Message()
            {
                //ToUniversalTime() для PostgreSQL, потому что он не воспринимает обычное время, только международное
                Content = Regex.Replace(viewModel.Content, @"<.*?>", string.Empty),
                FromUser = user,
                ToRoom = room,
                Timestamp = DateTime.Now.ToUniversalTime()
            };

            //Взаимодействие с СУБД должно быть асинхронным, для 100% результата
            await _db.AddMessage(msg);
            await _db.SaveChanges();

            // Broadcast the message
            var createdMessage = _mapper.Map<Message, MessageViewModel>(msg);
            await _hubContext.Clients.Group(room.Name).SendAsync("newMessage", createdMessage);

            return CreatedAtAction(nameof(Get), new { id = msg.Id }, createdMessage);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            //Взаимодействие с СУБД должно быть асинхронным, для 100% результата
            var message = await _db.GetMessage(id);

            if (message == null)
                return NotFound();

            //Взаимодействие с СУБД должно быть асинхронным, для 100% результата
            await _db.DeleteMessage(message); 
            await _db.SaveChanges();

            await _hubContext.Clients.All.SendAsync("removeChatMessage", message.Id);

            return Ok();
        }
    }
}
