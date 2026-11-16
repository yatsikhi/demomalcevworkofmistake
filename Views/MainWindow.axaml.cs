using System;
using Avalonia.Controls;    
using Avalonia.Interactivity;

using demomalcevworkofmistake.Context;
using demomalcevworkofmistake.Models;
using Microsoft.EntityFrameworkCore;

namespace demomalcevworkofmistake.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void Sign_in(object? sender, RoutedEventArgs e)
    {
        string login = LoginBox.Text ?? "";
        string password = PasswordBox.Text ?? "";

        if (string.IsNullOrWhiteSpace(login) ||
            string.IsNullOrWhiteSpace(password))
        {
            ErrorBlock.Text = "Введите логин и пароль!";
            return;
        }

        try
        {
            using var db = new DemomalcevContext();

            var user = await db.Users
                .FirstOrDefaultAsync(x => x.Login == login && x.Password == password);

            if (user == null)
            {
                ErrorBlock.Text = "Неверный логин или пароль!";
                return;
            }

            if (user.Role == 1)
            {
                var window = new AdminW();
                window.Show();
                this.Hide();
            }
            else if (user.Role == 2)
            {
                var window = new AdminW();
                window.Show();
                this.Hide();
            }
            else if (user.Role == 3)
            {
                var window = new AdminW();
                window.Show();
                this.Hide();
            }
            else
            {
                ErrorBlock.Text = "Неизвестная роль!";
            }
        }
        catch (Exception ex)
        {
            ErrorBlock.Text = ex.Message;
        }

    }
}