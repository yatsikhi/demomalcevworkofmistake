using System;
using System.Collections.Generic;

using System.Linq;
using Avalonia.Controls;
using demomalcevworkofmistake.Context;
using demomalcevworkofmistake.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Avalonia.Interactivity;
using demomalcevworkofmistake.Views;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace demomalcevworkofmistake;

public partial class AdminW : Window
{
    private List<Product> allProduct = new();
    private Product? editingProduct = null;
    private List<Category> allCategories = new();

    public AdminW()
    {
        InitializeComponent();

        SortBox.ItemsSource = new[]
        {
            "Без сортировки",
            "По названию (А-Я)",
            "По названию (Я-А)",
            "Сначала дешевле",
            "Сначала дороже"
        };

        SortBox.SelectedIndex = 0;

        LoadCategories();
        LoadProducts();
        LoadData();
        
        
    }
    // Загружаем товары
    private async void LoadProducts()
    {
        try
        {
            using var db = new DemomalcevContext();

            allProduct = await db.Products
                .Include(p => p.Sales)
                .Include(p => p.CreatorNavigation)
                .ToListAsync();
            

            FilterTovar();
        }
        catch (Exception ex)
        {
            ErrorBlock.Text = ex.Message;
        }
    }

    // Поиск и сортировка
    private void FilterTovar()
    {
        // Получаем поисковый текст
        string search = SearchBox.Text?.Trim() ?? "";

        // Получаем выбранную категорию
        var selectedCategory = CategoryBox.SelectedItem as Category;

        // Фильтруем товары
        var result = allProduct
            .Where(t => (t.Name ?? "")
                .Contains(search, StringComparison.OrdinalIgnoreCase))
            .Where(t => selectedCategory == null ||
                        t.Category == selectedCategory.Id);

        // Получаем сортировку
        string sort = SortBox.SelectedItem?.ToString() ?? "";

        switch (sort)
        {
            case "По названию (А-Я)":
                result = result.OrderBy(t => t.Name);
                break;

            case "По названию (Я-А)":
                result = result.OrderByDescending(t => t.Name);
                break;

            case "Сначала дешевле":
                result = result.OrderBy(t => t.Price);
                break;

            case "Сначала дороже":
                result = result.OrderByDescending(t => t.Price);
                break;
        }

        // Выводим результат
        TovarBox.ItemsSource = result.ToList();
    }

    private void CategoryBox_SelectionChanged(
        object? sender,
        SelectionChangedEventArgs e)
    {
        FilterTovar();
    }
    
    private void SearchBox_OnTextChanging(
        object? sender,
        TextChangingEventArgs e)
    {
        FilterTovar();
    }

    
    private void SortBox_SelectionChanged(
        object? sender,
        SelectionChangedEventArgs e)
    {
        FilterTovar();
    }

    private async Task RefreshProducts()
    {
        using var db = new DemomalcevContext();

        allProduct = await db.Products
            .Include(p => p.Sales)
            .Include(p => p.CreatorNavigation)
            .ToListAsync();

        FilterTovar();
    }

    private async void LoadCategories()
    {
        try
        {
            using var db = new DemomalcevContext();

            allCategories = await db.Categories.ToListAsync();

            CategoryBox.ItemsSource = allCategories;
        }
        catch (Exception ex)
        {
            ErrorBlock.Text = ex.Message;
        }
    }
    
    //загрузка данных 
    private async Task LoadData()
    {
        using var db = new DemomalcevContext();

        UnitBox.ItemsSource = await db.Units.ToListAsync();
        CreatorBox.ItemsSource = await db.Creators.ToListAsync();
        DeliverBox.ItemsSource = await db.Delivers.ToListAsync();
        CategoryAddBox.ItemsSource = await db.Categories.ToListAsync();
    }
    
    
    private async void AddProduct_Click(
        object? sender,
        RoutedEventArgs e)
    {
        // Получаем данные из полей
        string name = ProductNameBox.Text?.Trim() ?? "";
        string price = PriceBox.Text?.Trim() ?? "";

        // Проверяем название и цену
        if (name == "" || price == "")
        {
            AddErrorBlock.Text = "Введите название и цену!";
            return;
        }

        if (!decimal.TryParse(price, out decimal priceValue) || priceValue < 0)
        {
            AddErrorBlock.Text = "Неправильная цена!";
            return;
        }

        // Проверяем скидку
        decimal discount = 0;

        if (!string.IsNullOrWhiteSpace(DiscountBox.Text) &&
            (!decimal.TryParse(DiscountBox.Text, out discount) ||
             discount < 0 || discount > 100))
        {
            AddErrorBlock.Text = "Скидка должна быть от 0 до 100!";
            return;
        }

        // Подтверждение добавления
        var result = await MessageBoxManager
            .GetMessageBoxStandard(
                "Подтверждение",
                "Добавить товар?",
                ButtonEnum.YesNo)
            .ShowAsync();

        if (result != ButtonResult.Yes)
            return;

        try
        {
            using var db = new DemomalcevContext();

            Product product = new Product
            {
                Name = name,
                Price = price,
                Discount = "0",
                About = AboutBox.Text?.Trim() ?? "",
                Image = ImageBox.Text?.Trim(),

                // Получаем ID из выпадающих списков
                Units = (UnitBox.SelectedItem as Unit)?.Id,
                Creator = (CreatorBox.SelectedItem as Creator)?.Id,
                Deliver = (DeliverBox.SelectedItem as Deliver)?.Id,
                Category = (CategoryAddBox.SelectedItem as Category)?.Id
            };

            db.Products.Add(product);

            // Если есть скидка, добавляем её отдельно
            if (discount > 0)
            {
                db.Sales.Add(new Sale
                {
                    Product = product,
                    Discount = discount
                });
            }

            await db.SaveChangesAsync();

            AddErrorBlock.Text = "Товар успешно добавлен!";

            ClearForm();
            await RefreshProducts();
        }
        catch (Exception ex)
        {
            AddErrorBlock.Text = ex.InnerException?.Message ?? ex.Message;
        }
    }
    
    private void ClearForm()
    {
        ProductNameBox.Text = "";
        PriceBox.Text = "";
        DiscountBox.Text = "";
        AboutBox.Text = "";
        ImageBox.Text = "";

        UnitBox.SelectedItem = null;
        CreatorBox.SelectedItem = null;
        DeliverBox.SelectedItem = null;
        CategoryAddBox.SelectedItem = null;
    }

    private async void EditProduct_Click(
        object? sender,
        RoutedEventArgs e)
    {
        if (TovarBox.SelectedItem is not Product product)
        {
            ErrorBlock.Text = "Выберите товар!";
            return;
        }

        editingProduct = product;

        // Загружаем справочники для выпадающих списков
        await LoadData();

        // Заполняем поля
        ProductNameBox.Text = product.Name;
        PriceBox.Text = product.Price;
        AboutBox.Text = product.About;
        ImageBox.Text = product.Image;

        // Получаем скидку товара
        DiscountBox.Text = product.Sales
            .FirstOrDefault()?.Discount.ToString() ?? "0";

        // Выбираем связанные значения
        UnitBox.SelectedItem = UnitBox.Items
            .Cast<Unit>()
            .FirstOrDefault(x => x.Id == product.Units);

        CreatorBox.SelectedItem = CreatorBox.Items
            .Cast<Creator>()
            .FirstOrDefault(x => x.Id == product.Creator);

        DeliverBox.SelectedItem = DeliverBox.Items
            .Cast<Deliver>()
            .FirstOrDefault(x => x.Id == product.Deliver);

        CategoryAddBox.SelectedItem = CategoryAddBox.Items
            .Cast<Category>()
            .FirstOrDefault(x => x.Id == product.Category);

        AddErrorBlock.Text = "Измените данные и нажмите «Сохранить изменения»!";
    }

    
    private async void SaveEdit_Click(
        object? sender,
        RoutedEventArgs e)
    {
        if (editingProduct == null)
        {
            AddErrorBlock.Text = "Сначала выберите товар для редактирования!";
            return;
        }

        string name = ProductNameBox.Text?.Trim() ?? "";
        string price = PriceBox.Text?.Trim() ?? "";

        if (name == "" || price == "")
        {
            AddErrorBlock.Text = "Введите название и цену!";
            return;
        }

        if (!decimal.TryParse(price, out decimal priceValue) || priceValue < 0)
        {
            AddErrorBlock.Text = "Неправильная цена!";
            return;
        }

        decimal discount = 0;

        if (!string.IsNullOrWhiteSpace(DiscountBox.Text) &&
            (!decimal.TryParse(DiscountBox.Text, out discount) ||
             discount < 0 || discount > 100))
        {
            AddErrorBlock.Text = "Скидка должна быть от 0 до 100!";
            return;
        }

        try
        {
            using var db = new DemomalcevContext();

            var product = await db.Products
                .FirstOrDefaultAsync(x => x.Id == editingProduct.Id);

            if (product == null)
            {
                AddErrorBlock.Text = "Товар не найден!";
                return;
            }

            // Изменяем данные товара
            product.Name = name;
            product.Price = price;
            product.About = AboutBox.Text?.Trim() ?? "";
            product.Image = ImageBox.Text?.Trim();

            product.Units = (UnitBox.SelectedItem as Unit)?.Id;
            product.Creator = (CreatorBox.SelectedItem as Creator)?.Id;
            product.Deliver = (DeliverBox.SelectedItem as Deliver)?.Id;
            product.Category = (CategoryAddBox.SelectedItem as Category)?.Id;

            // Обновляем скидку
            var sale = await db.Sales
                .FirstOrDefaultAsync(x => x.ProductId == product.Id);

            if (discount > 0)
            {
                if (sale == null)
                {
                    db.Sales.Add(new Sale
                    {
                        ProductId = product.Id,
                        Discount = discount
                    });
                }
                else
                {
                    sale.Discount = discount;
                }
            }
            else if (sale != null)
            {
                db.Sales.Remove(sale);
            }

            await db.SaveChangesAsync();

            AddErrorBlock.Text = "Товар успешно изменён!";

            editingProduct = null;

            ClearForm();
            await RefreshProducts();
        }
        catch (Exception ex)
        {
            AddErrorBlock.Text = ex.InnerException?.Message ?? ex.Message;
        }
    }

    
    private async void DeleteProduct_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (TovarBox.SelectedItem is not Product selectedProduct)
        {
            ErrorBlock.Text = "Выберите товар!";
            return;
        }
        //метод для бокса да нет 
        var result = await MessageBoxManager
            .GetMessageBoxStandard(
                "Подтверждение",
                "Вы действительно хотите удалить товар?",
                ButtonEnum.YesNo)
            .ShowAsync();

        if (result != ButtonResult.Yes)
            return;

        try
        {
            using var db = new DemomalcevContext();

            var product = await db.Products
                .FirstOrDefaultAsync(p => p.Id == selectedProduct.Id);

            if (product == null)
            {
                ErrorBlock.Text = "Товар не найден!";
                return;
            }

            // Удаляем скидки
            var sales = await db.Sales
                .Where(s => s.ProductId == product.Id)
                .ToListAsync();

            db.Sales.RemoveRange(sales);

            // Удаляем количества
            var amounts = await db.Amounts
                .Where(a => a.ProductId == product.Id)
                .ToListAsync();

            db.Amounts.RemoveRange(amounts);

            // Удаляем товар
            db.Products.Remove(product);

            await db.SaveChangesAsync();

            ErrorBlock.Text = "Товар успешно удалён!";
            await RefreshProducts();
        }
        catch (Exception ex)
        {
            ErrorBlock.Text = ex.InnerException?.Message ?? ex.Message;
        }
    }

    private void Back_Button(object? sender, RoutedEventArgs e)
    {
        var back = new MainWindow();
        back.Show();
        this.Close();
    }
}