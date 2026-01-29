namespace AlpineShopAppShell
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new AlpineShop.Page.Login());
        }


    }
}