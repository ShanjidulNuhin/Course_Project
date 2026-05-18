using System;
using System.Collections.Generic;
using System.Text;

namespace Game.BLL.DTOs
{
    public class GameDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Genre { get; set; } = "";
        public byte[] Cover { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }

    }
}
