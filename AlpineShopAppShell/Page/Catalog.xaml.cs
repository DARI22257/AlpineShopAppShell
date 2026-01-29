using AlpineShop.Models;

namespace AlpineShop.Page;

public partial class Catalog : ContentPage
{
    private readonly bool _isAdmin;
    private readonly string _login;
    public bool IsAdmin => AlpineShopAppShell.AppShell.IsAdmin;
    public bool IsNotAdmin => !AlpineShopAppShell.AppShell.IsAdmin;


    public Catalog()
    {
        InitializeComponent();

        _login = AlpineShopAppShell.AppShell.CurrentLogin;
        _isAdmin = AlpineShopAppShell.AppShell.IsAdmin;

        BindingContext = this;
        ProductsView.ItemsSource = DB.Products;
    }
    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        Application.Current.MainPage =
            new NavigationPage(new Login());
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        if (!_isAdmin)
        {
            await DisplayAlert("Доступ запрещён", "Добавлять товары может только админ.", "OK");
            return;
        }

        await Shell.Current.GoToAsync("//addproduct");
    }

    private async void OnProductSelected(object sender, SelectionChangedEventArgs e)
    {
        var product = e.CurrentSelection?.FirstOrDefault() as Product;
        if (product == null) return;

        ProductsView.SelectedItem = null;

        if (!_isAdmin)
            return;

        var action = await DisplayActionSheet(
            "Действия с товаром",
            "Отмена",
            null,
            "Редактировать",
            "Удалить");

        if (action == "Редактировать")
        {
            await Navigation.PushAsync(new Edit(product));
        }
        else if (action == "Удалить")
        {
            var ok = await DisplayAlert("Удаление", $"Удалить товар «{product.Name}»?", "Удалить", "Отмена");
            if (!ok) return;

            try
            {
                if (!string.IsNullOrWhiteSpace(product.ImageFile) && File.Exists(product.ImageFile))
                    File.Delete(product.ImageFile);
            }
            catch {  }

            DB.Products.Remove(product);
            await DB.SaveProductsAsync();
        }
    }
    private async void OnEditClicked(object sender, EventArgs e)
    {
        if (!_isAdmin) return;

        if (sender is Button btn && btn.CommandParameter is Product product)
        {
            var page = new Edit(product);
            page.Disappearing += (s, e) => RefreshProducts();
            await Navigation.PushAsync(page);
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (!_isAdmin) return;

        if (sender is Button btn && btn.CommandParameter is Product product)
        {
            var ok = await DisplayAlert("Удаление", $"Удалить товар «{product.Name}»?", "Удалить", "Отмена");
            if (!ok) return;

            // опционально: удалить файл картинки
            try
            {
                if (!string.IsNullOrWhiteSpace(product.ImageFile) && File.Exists(product.ImageFile))
                    File.Delete(product.ImageFile);
            }
            catch { }

            DB.Products.Remove(product);
            await DB.SaveProductsAsync();
        }
    }
    private async void OnCartClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//cart");
    }

    private async void OnAddToCartClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is Product product)
        {
            await DB.AddToCartAsync(_login, product);
            await DisplayAlert("Корзина", $"Добавлено: {product.Name}", "OK");
        }
    }
    public void RefreshProducts()
    {
        ProductsView.ItemsSource = null;
        ProductsView.ItemsSource = DB.Products;
    }

}