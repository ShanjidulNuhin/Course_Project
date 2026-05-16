using System;
using System.Collections.Generic;

namespace Game.DAL.EF.Tables;

public partial class Game
{
    public int Id { get; set; }

    public byte[]? Title { get; set; }

    public byte[]? Genre { get; set; }

    public decimal Price { get; set; }

    public byte[]? Description { get; set; }
}
