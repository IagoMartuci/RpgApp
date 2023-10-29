using AppRpgEtec.ViewModels.Disputas;

namespace AppRpgEtec.Views.Disputas;

public partial class ListagemView : ContentPage
{
	DisputaViewModel disputaViewModel;

	public ListagemView()
	{
		InitializeComponent();

		disputaViewModel= new DisputaViewModel();
		BindingContext = disputaViewModel;
	}
}