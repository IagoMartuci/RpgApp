using AppRpgEtec.ViewModels;
using AppRpgEtec.Views.Armas;
using AppRpgEtec.Views.Personagens;

namespace AppRpgEtec;

public partial class AppShell : Shell
{
	AppShellViewModel viewModel;
	public AppShell()
	{
		InitializeComponent();

		// Criando uma rota para o "botão" que irá direcionar para a view de cadastro
		Routing.RegisterRoute("cadastroPersonagemView", typeof(CadastroPersonagemView));
		Routing.RegisterRoute("cadastroArmaView", typeof(CadastroArmaView));

		viewModel = new AppShellViewModel();
		BindingContext = viewModel;

        // Recuperar dados do usuário da Preference e atribuir ao label que adicionamos no design
        string login = Preferences.Get("UsuarioUserName", string.Empty);
		lblLogin.Text = $"Login: {login}";
	}
}