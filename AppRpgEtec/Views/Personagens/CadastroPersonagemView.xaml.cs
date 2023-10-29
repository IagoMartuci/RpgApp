using AppRpgEtec.ViewModels.Personagens;

namespace AppRpgEtec.Views.Personagens;

public partial class CadastroPersonagemView : ContentPage
{
	private CadastroPersonagemViewModel cadastroViewModel;
	public CadastroPersonagemView()
	{
		InitializeComponent();

		cadastroViewModel = new CadastroPersonagemViewModel();
		BindingContext = cadastroViewModel;
		Title = "Cadastro Personagem";
	}
}