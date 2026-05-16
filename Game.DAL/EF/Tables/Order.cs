using System;
using System.Collections.Generic;

namespace Game.DAL.EF.Tables;

public partial class Order
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public DateOnly? OrderDate { get; set; }

    public decimal? TotalPrice { get; set; }

    public virtual User? User { get; set; }
}
