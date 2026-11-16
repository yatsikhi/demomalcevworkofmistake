using System;
using System.Collections.Generic;

namespace demomalcevworkofmistake.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? Units { get; set; }

    public string Discount { get; set; } = null!;

    public string Price { get; set; } = null!;

    public int? Creator { get; set; }

    public int? Deliver { get; set; }

    public int? Category { get; set; }

    public string About { get; set; } = null!;

    public string? Image { get; set; }

    public virtual ICollection<Amount> Amounts { get; set; } = new List<Amount>();

    public virtual Category? CategoryNavigation { get; set; }

    public virtual Creator? CreatorNavigation { get; set; }

    public virtual Deliver? DeliverNavigation { get; set; }

    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

    public virtual Unit? UnitsNavigation { get; set; }
}
