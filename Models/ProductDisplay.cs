using System.Linq;
using Avalonia.Media;

namespace demomalcevworkofmistake.Models;

public partial class Product
{
    public decimal? ProductDiscount =>
        Sales.Select(s => s.Discount)
            .FirstOrDefault(d => d.HasValue);

    public bool IsLowDiscount =>
        ProductDiscount.HasValue && ProductDiscount.Value < 10;

    public IBrush RowBackground =>
        IsLowDiscount ? Brushes.LightGreen : Brushes.White;

    public string DiscountText =>
        ProductDiscount.HasValue
            ? $"{ProductDiscount.Value}%"
            : "Без скидки";
}