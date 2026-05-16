using System;
using System.Collections.Generic;

namespace Game.DAL.EF.Tables;

public partial class User
{
    public int Id { get; set; }

    public byte[]? Name { get; set; }

    public byte[]? Email { get; set; }

    public byte[]? Password { get; set; }

    public byte[]? Role { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
