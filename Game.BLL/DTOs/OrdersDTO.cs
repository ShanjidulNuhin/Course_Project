using System;
using System.Collections.Generic;

namespace Game.BLL.DTOs
{
    public class OrderDTO
    {
        public int UserId { get; set; }

        public List<int> GameIds { get; set; } = new List<int>();
    }
}