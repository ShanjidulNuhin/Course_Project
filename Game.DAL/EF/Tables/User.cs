using System;
using System.Collections.Generic;

namespace Game.DAL.EF.Tables;

public partial class User
{
    public int Id { get; set; }

    public string[]? Name { get; set; }

    public string[]? Email { get; set; }

    public string[]? Password { get; set; }

    public string[]? Role { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<Library> Libraries { get; set; } = new List<Library>();
}
