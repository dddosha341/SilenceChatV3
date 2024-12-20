using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Silence.Web.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; }

        [Column(TypeName = "timestamp with time zone")]
        public DateTime Timestamp { get; set; }
        public User FromUser { get; set; }
        public int ToRoomId { get; set; }
        public Room ToRoom { get; set; }
    }
}
