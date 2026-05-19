using System;
using System.Collections.Generic;
using System.Text;

namespace Game.BLL.DTOs
{
    public class LibraryViewDTO
    {
        public int Id { get; set; }
        public int? UserId { get; set; }  
        public int? GameId { get; set; }
        public string? GameTitle { get; set; } 
        public string? Genre { get; set; }
        public byte[]? Cover { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Cart_Type { get; set; }
        public DateOnly? OrderDate { get; set; }
    }
}
