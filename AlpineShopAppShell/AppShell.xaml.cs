namespace AlpineShopAppShell
{
    public partial class AppShell : Shell
    {
        public static string CurrentLogin { get; private set; } = "";
        public static bool IsAdmin { get; private set; }

        public AppShell(string login, bool isAdmin)
        {
            InitializeComponent();

            CurrentLogin = login;
            IsAdmin = isAdmin;

            AdminAddItem.IsVisible = isAdmin;

            _ = GoToAsync("//catalog");
        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new AlpineShop.Page.Login());
        }
    }
}
