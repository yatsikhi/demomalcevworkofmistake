using System;
using System.Collections.Generic;

namespace demomalcevworkofmistake.Models;

public partial class Sale
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public decimal? Discount { get; set; }

    public virtual Product? Product { get; set; }
}
