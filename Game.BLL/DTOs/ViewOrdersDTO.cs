using System;
using System.Collections.Generic;

namespace Game.BLL.DTOs
{
    public class ViewOrdersDTO
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public DateOnly? OrderDate { get; set; }
        public decimal? TotalPrice { get; set; }
        public List<string> PurchasedGameTitles { get; set; } = new List<string>();
    }
}