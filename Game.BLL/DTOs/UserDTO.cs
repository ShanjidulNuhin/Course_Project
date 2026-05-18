using System;
using System.Collections.Generic;
using System.Text;

namespace Game.BLL.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public decimal? Blance { get; set; }
    }
}
