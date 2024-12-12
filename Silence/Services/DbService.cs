using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Elfie.Serialization;
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

    public User GetUser(string username)
    {
        return _dbContext.Users.FirstOrDefault(u => u.UserName == username);
    }

    public void SaveChanges()
    {
        _dbContext.SaveChanges();
    }

    public void AddUser(User user)
    {
        _dbContext.Users.Add(user);
        SaveChanges();
    }

    public void DeleteMessage(Message message)
    {
        _dbContext.Messages.Remove(message);
        SaveChanges();
    }

    public Message GetMessage(int id)
    {
        return _dbContext.Messages.FirstOrDefault(m => m.Id == id);
    }

    public void AddMessage(Message message)
    {
        _dbContext.Messages.Add(message);
        SaveChanges();
    }

    public Room GetRoom(string roomName)
    {
        return _dbContext.Rooms.FirstOrDefault(r => r.Name == roomName);
    }

    public IEnumerable<Message> GetMessages(int roomId)
    {
        return _dbContext.Messages.Where(m => m.ToRoomId == roomId);
    }

    public IEnumerable<Room> GetRooms()
    {
        return _dbContext.Rooms;
    }

    public Room GetRoom(int roomId)
    {
        return _dbContext.Rooms.FirstOrDefault(r => r.Id == roomId);
    }

    public bool IsExistsRoom(string roomName)
    {
        return _dbContext.Rooms.Any(r => r.Name == roomName);
    }

    public void AddRoom(Room room)
    {
        _dbContext.Rooms.Add(room);
        SaveChanges();
    }

    public Task<Room> GetRoomByAdmin(int id, string username)
    {
        return _dbContext.Rooms.Include(r => r.Admin).Where(r => r.Id == id && r.Admin.UserName == username).FirstOrDefaultAsync();
    }

    public void RemoveRoom(Room room)
    {
        _dbContext.Rooms.Remove(room);
        SaveChanges();
    }
}