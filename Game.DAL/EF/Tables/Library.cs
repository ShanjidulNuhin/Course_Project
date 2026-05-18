using System;
using System.Collections.Generic;

namespace Game.DAL.EF.Tables;

public partial class Library
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? GameId { get; set; }

    public string? Cart_Type { get; set; } 

    public DateOnly? OrderDate { get; set; }

    public virtual User? User { get; set; }
    public virtual Game? Game { get; set; }
}