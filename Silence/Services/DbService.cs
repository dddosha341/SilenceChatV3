using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Silence.Web.Data;
using Silence.Web.Entities;

namespace Silence.Web.Services;

public class DbService
{
    private readonly AppDbContext _dbContext;

    public DbService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    async public Task<User> GetUser(string username)
    {
        return _dbContext.Users.FirstOrDefault(u => u.UserName == username);
    }

    async public Task SaveChanges()
    {
        _dbContext.SaveChanges();
    }

    async public Task AddUser(User user)
    {
        _dbContext.Users.Add(user);
        await this.SaveChanges();
    }

    async public Task DeleteMessage(Message message)
    {
        _dbContext.Messages.Remove(message);
        await this.SaveChanges();
    }

    async public Task<Message> GetMessage(int id)
    {
        return _dbContext.Messages.FirstOrDefault(m => m.Id == id);
    }

    async public Task AddMessage(Message message)
    {
        _dbContext.Messages.Add(message);
        await this.SaveChanges();
    }

    async public Task<Room> GetRoom(string roomName)
    {
        return _dbContext.Rooms.FirstOrDefault(r => r.Name == roomName);
    }

    async public Task<IEnumerable<Message>> GetMessages(int roomId)
    {
        return _dbContext.Messages.Where(m => m.ToRoomId == roomId).Include(m => m.FromUser)
                        .Include(m => m.ToRoom)
                        .OrderByDescending(m => m.Timestamp)
                        .Take(20)
                        .AsEnumerable()
                        .Reverse()
                        .ToList();
    }

    async public Task<IEnumerable<Room>> GetRooms()
    {
        return _dbContext.Rooms;
    }

    async public Task<Room> GetRoom(int roomId)
    {
        return _dbContext.Rooms.FirstOrDefault(r => r.Id == roomId);
    }

    async public Task<bool> IsExistsRoom(string roomName)
    {
        return _dbContext.Rooms.Any(r => r.Name == roomName);
    }

    async public Task AddRoom(Room room)
    {
        _dbContext.Rooms.Add(room);
        await SaveChanges();
    }

    async public Task<Room> GetRoomByAdmin(int id, string username)
    {
        return _dbContext.Rooms.Include(r => r.Admin).Where(r => r.Id == id && r.Admin.UserName == username).FirstOrDefaultAsync().Result;
    }

    async public Task RemoveRoom(Room room)
    {
        _dbContext.Rooms.Remove(room);
        await this.SaveChanges();
    }
}