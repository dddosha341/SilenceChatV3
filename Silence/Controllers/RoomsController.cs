using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Silence.Web.Data;
using Silence.Web.Entities;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Silence.Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using Silence.Infrastructure.ViewModels;
using System.Security.Claims;
using Silence.Web.Services;


namespace Silence.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly DbService _db;
        private readonly IMapper _mapper;
        private readonly IHubContext<ChatHub> _hubContext;

        public RoomsController(DbService db,
            IMapper mapper,
            IHubContext<ChatHub> hubContext)
        {
            _db = db;
            _mapper = mapper;
            _hubContext = hubContext;
        }

        [HttpGet]
        public IActionResult  Get()
        {
            User? user = null;

            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                var username = identity.FindFirst(ClaimTypes.Name)?.Value;
                user = _db.GetUser(username);
            }

            if (user is null)
            {
                return Unauthorized();
            }

            var rooms = _db.GetRooms();

            var roomsViewModel = _mapper.Map<IEnumerable<Room>, IEnumerable<RoomViewModel>>(rooms);

            //return Ok(roomsViewModel);

            return Ok(roomsViewModel);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> Get(int id)
        {
            User? user = null;

            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                var username = identity.FindFirst(ClaimTypes.Name)?.Value;
                user = _db.GetUser(username);
            }

            if (user is null)
            {
                return Unauthorized();
            }

            var room = _db.GetRoom(id);
            if (room == null)
                return NotFound();

            var roomViewModel = _mapper.Map<Room, RoomViewModel>(room);
            return Ok(roomViewModel);
        }

        [HttpPost]
        public async Task<ActionResult<Room>> Create(RoomViewModel viewModel)
        {

            if (_db.IsExistsRoom(viewModel.Name))
                return BadRequest("Invalid room name or room already exists");

            var user = _db.GetUser(User.Identity.Name);
            var room = new Room()
            {
                Name = viewModel.Name,
                Admin = user
            };

            _db.AddRoom(room);
            _db.SaveChanges();

            var createdRoom = _mapper.Map<Room, RoomViewModel>(room);
            await _hubContext.Clients.All.SendAsync("addChatRoom", createdRoom);

            return CreatedAtAction(nameof(Get), new { id = room.Id }, createdRoom);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, RoomViewModel viewModel)
        {
            if (_db.IsExistsRoom(viewModel.Name))
                return BadRequest("Invalid room name or room already exists");

            var room = _db.GetRoomByAdmin(id, User.Identity.Name).Result;

            if (room == null)
                return NotFound();

            room.Name = viewModel.Name;
            _db.SaveChanges();

            var updatedRoom = _mapper.Map<Room, RoomViewModel>(room);
            await _hubContext.Clients.All.SendAsync("updateChatRoom", updatedRoom);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var room = _db.GetRoomByAdmin(id, User.Identity.Name).Result;

            if (room == null)
                return NotFound();

            _db.RemoveRoom(room);
            _db.SaveChanges();

            await _hubContext.Clients.All.SendAsync("removeChatRoom", room.Id);
            await _hubContext.Clients.Group(room.Name).SendAsync("onRoomDeleted");

            return Ok();
        }
    }
}
