namespace AppRpgEtec;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		// Alterando a página inicial para ser a página de login
		MainPage = new NavigationPage(new Views.Usuarios.LoginView());
		// MainPage = new NavigationPage(new Views.Usuarios.LocalizacaoView());
    }
}
