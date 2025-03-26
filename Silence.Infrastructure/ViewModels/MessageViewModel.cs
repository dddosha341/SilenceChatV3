using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Silence.Infrastructure.ViewModels
{
    public class MessageViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }
        //Создан атрибут для понимания, что время международное
        [Column(TypeName = "timestamp with time zone")]
        public DateTime Timestamp { get; set; }
        public string FromUserName { get; set; }
        public string FromFullName { get; set; }
        [Required]
        public string Room { get; set; }
        public string Avatar { get; set; }

        public bool IsCurrentUser { get; set; }
    }
}
