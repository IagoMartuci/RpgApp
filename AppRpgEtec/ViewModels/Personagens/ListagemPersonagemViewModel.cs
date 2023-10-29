using AppRpgEtec.Models;
using AppRpgEtec.Services.Personagens;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AppRpgEtec.ViewModels.Personagens
{
    public class ListagemPersonagemViewModel : BaseViewModel
    {
        // Declaração da variável de serviço que consumirá a API
        private PersonagemService pService;

        // Declaração da Coleção de Personagens como propriedade
        public ObservableCollection<Personagem> Personagens { get; set; }

        // Construtor pegando token, inicializando o serviço passando o token e inicializando a lista de personagens
        public ListagemPersonagemViewModel()
        {
            string token = Preferences.Get("UsuarioToken", string.Empty);
            // string token = Preferences.Get("UsuarioToken", string.Empty).ToString(); // ?
            // string token = Application.Current.Properties["UsuarioToken"].ToString(); // Obsoleto CS0619
            pService = new PersonagemService(token);
            Personagens = new ObservableCollection<Personagem>();
            _ = ObterPersonagens(); // O “_” (underline) descarta a operação assíncrona de usar o operador await e armazenar um retorno

            NovoPersonagem = new Command(async () => { await ExibirCadastroPersonagem(); });
            RemoverPersonagemCommand = new Command<Personagem>(async (Personagem p)
                =>
            { await RemoverPersonagem(p); });
        }

        public ICommand NovoPersonagem { get; }
        public ICommand RemoverPersonagemCommand { get; }

        public async Task ObterPersonagens()
        {
            try
            {
                Personagens = await pService.GetPersonagensAsync();
                OnPropertyChanged(nameof(Personagens)); // Informara a view que houve carregamento
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }

        public async Task ExibirCadastroPersonagem()
        {
            try
            {
                await Shell.Current.GoToAsync("cadastroPersonagemView");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }

        // Atributo
        private Personagem personagemSelecionado;

        // Propriedade
        public Personagem PersonagemSelecionado
        {
            get { return personagemSelecionado;}
            set 
            {
                if (value != null)
                {
                    personagemSelecionado = value;

                    Shell.Current
                        .GoToAsync($"cadastroPersonagemView?pId={personagemSelecionado.Id}");
                        // cadastroPersonagemView tem que estar exatamente como está no AppShell
                }
            }
        }

        public async Task RemoverPersonagem(Personagem p)
        {
            try
            {
                if(await Application.Current.MainPage
                    .DisplayAlert("Confirmação", $"Confirma a remoção de {p.Nome}?", "Sim", "Não"))
                {
                    await pService.DeletePersonagemAsync(p.Id);

                    await Application.Current.MainPage.DisplayAlert("Mensagem",
                        $"Personagem {p.Nome} removido com sucesso!", "Ok");

                    _ = ObterPersonagens();
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
