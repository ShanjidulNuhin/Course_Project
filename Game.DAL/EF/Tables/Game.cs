using System;
using System.Collections.Generic;

namespace Game.DAL.EF.Tables;

public partial class Game
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Genre { get; set; }

    public decimal Price { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Library> Libraries { get; set; } = new List<Library>();
}