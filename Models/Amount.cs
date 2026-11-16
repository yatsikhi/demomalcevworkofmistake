using System;
using System.Collections.Generic;

namespace demomalcevworkofmistake.Models;

public partial class Amount
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public string Quantity { get; set; } = null!;

    public virtual Product? Product { get; set; }
}
