using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace YandexDzen.Models
{
    public class User
    {
        [Key]
        public int Id_user { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public string Password{ get; set; }
    }

    public enum StateSort
    {
        IdAsc,
        IdDesc,
        EmailAsc,
        EmailDesc
    }
}
