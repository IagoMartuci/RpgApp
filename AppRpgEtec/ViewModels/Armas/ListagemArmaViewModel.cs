using AppRpgEtec.Models;
using AppRpgEtec.Services.Armas;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AppRpgEtec.ViewModels.Armas
{
    public class ListagemArmaViewModel : BaseViewModel
    {
        // Declaração da variável de serviço que consumirá a API
        private ArmaService aService;

        // Declaração da Coleção de Armas como propriedade
        public ObservableCollection<Arma> Armas { get; set; }

        // Construtor pegando token, inicializando o serviço passando o token e inicializando a lista de armas
        public ListagemArmaViewModel()
        {
            string token = Preferences.Get("UsuarioToken", string.Empty);
            // string token = Preferences.Get("UsuarioToken", string.Empty).ToString(); // ?
            // string token = Application.Current.Properties["UsuarioToken"].ToString(); // Obsoleto CS0619
            aService = new ArmaService(token);
            Armas = new ObservableCollection<Arma>();
            _ = ObterArmas(); // O “_” (underline) descarta a operação assíncrona de usar o operador await e armazenar um retorno

            NovaArmaCommand = new Command(async () => { await ExibirCadastroArma(); });
            RemoverArmaCommand = new Command<Arma>(async (Arma a)
                =>
            { await RemoverArma(a); });
        }

        public ICommand NovaArmaCommand { get; }
        public ICommand RemoverArmaCommand { get; }

        public async Task ObterArmas()
        {
            try
            {
                Armas = await aService.GetArmasAsync();
                OnPropertyChanged(nameof(Armas)); // Informara a view que houve carregamento
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }

        public async Task ExibirCadastroArma()
        {
            try
            {
                await Shell.Current.GoToAsync("cadastroArmaView");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }

        // Atributo
        private Arma armaSelecionada;

        // Propriedade
        public Arma ArmaSelecionada
        {
            get { return armaSelecionada; }
            set
            {
                if (value != null)
                {
                    armaSelecionada = value;

                    Shell.Current
                        .GoToAsync($"cadastroArmaView?aId={armaSelecionada.Id}");
                    // cadastroArmaView tem que estar exatamente como está no AppShell
                }
            }
        }

        public async Task RemoverArma(Arma a)
        {
            try
            {
                if (await Application.Current.MainPage
                    .DisplayAlert("Confirmação", $"Confirma a remoção de {a.Nome}?", "Sim", "Não"))
                {
                    await aService.DeleteArmaAsync(a.Id);

                    await Application.Current.MainPage.DisplayAlert("Mensagem",
                        $"Arma {a.Nome} removida com sucesso!", "Ok");

                    _ = ObterArmas();
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
                throw;
            }
        }
    }
}
