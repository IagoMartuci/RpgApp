using AppRpgEtec.ViewModels.Armas;

namespace AppRpgEtec.Views.Armas;

public partial class ListagemView : ContentPage
{
	ListagemArmaViewModel viewModel;
	public ListagemView()
	{
		InitializeComponent();

		viewModel = new ListagemArmaViewModel();
		BindingContext = viewModel;
        Title = "Armas - App Rpg Etec";
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = viewModel.ObterArmas();
    }
}