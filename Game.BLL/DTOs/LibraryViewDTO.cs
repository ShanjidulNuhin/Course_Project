using System;
using System.Collections.Generic;
using System.Text;

namespace Game.BLL.DTOs
{
    public class LibraryViewDTO
    {
        public int Id { get; set; }
        public int? UserId { get; set; }  
        public string? GameTitle { get; set; } 
        public string? Cart_Type { get; set; }
        public DateOnly? OrderDate { get; set; }
    }
}
