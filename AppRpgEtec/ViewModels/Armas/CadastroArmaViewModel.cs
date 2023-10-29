using AppRpgEtec.Models;
using AppRpgEtec.Services.Armas;
using AppRpgEtec.Services.Personagens;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AppRpgEtec.ViewModels.Armas
{
    [QueryProperty("ArmaSelecionadaId", "aId")]
    public class CadastroArmaViewModel : BaseViewModel
    {
        private ArmaService aService;
        private PersonagemService pService;

        public CadastroArmaViewModel()
        {
            string token = Preferences.Get("UsuarioToken", string.Empty);
            aService = new ArmaService(token);
            pService = new PersonagemService(token);

            ObterPersonagens();

            //SalvarCommand = new Command(async () => await SalvarArma());
            SalvarCommand = new Command(SalvarArma);
            CancelarCommand = new Command(async => CancelarCadastro());

        }

        public ICommand SalvarCommand { get; set; }
        public ICommand CancelarCommand { get; set; }

        #region Atributos_Propriedades

        private int id;
        private string nome;
        private int dano;
        private int personagemId;

        public int Id
        {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged();
            }
        }

        public string Nome
        {
            get => nome;
            set
            {
                nome = value;
                OnPropertyChanged();
            }
        }
        public int Dano
        {
            get => dano;
            set
            {
                dano = value;
                OnPropertyChanged();

            }
        }
        public int PersonagemId
        {
            get => personagemId;
            set
            {
                personagemId = value;
                OnPropertyChanged();
            }
        }

        private Personagem personagemSelecionado;
        public Personagem PersonagemSelecionado
        {
            get { return personagemSelecionado; }
            set
            {
                if (value != null)
                {
                    personagemSelecionado = value;
                    OnPropertyChanged();
                }
            }
        }

        /*private Arma armaSelecionada;
        public Arma ArmaSelecionada
        {
            get { return armaSelecionada; }
            set
            {
                if (value != null)
                {
                    armaSelecionada = value;
                    OnPropertyChanged();
                }
            }
        }*/

        public ObservableCollection<Personagem> Personagens { get; set; }
        // public ObservableCollection<Arma> Armas { get; set; }

        // Atributo
        private string armaSelecionadaId;

        // Propriedade
        public string ArmaSelecionadaId
        {
            get => armaSelecionadaId;
            set
            {
                if (value != null)
                {
                    armaSelecionadaId = Uri.UnescapeDataString(value);
                    CarregarArma();
                }
            }
        }

        #endregion

        #region Metodos
        public async void ObterPersonagens()
        {
            try
            {
                Personagens = await pService.GetPersonagensAsync();
                OnPropertyChanged(nameof(Personagens));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ops", ex.Message, "Ok");
            }
        }

        public async void CarregarArma()
        {
            try
            {
                Arma a = await
                    aService.GetArmaAsync(int.Parse(armaSelecionadaId));

                this.Nome = a.Nome;
                this.Dano = a.Dano;
                this.Id = a.Id;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
                throw;
            }
        }

        public async void SalvarArma()
        {
            try
            {
                Arma model = new Arma()
                {
                    Id = this.id,
                    Nome = this.nome,
                    Dano = this.dano,
                    PersonagemId = this.personagemSelecionado.Id
                };

                // Se o Id for 0 é porque a arma não existe, então ele cria uma nova
                if (model.Id == 0)
                {
                    await aService.PostArmaAsync(model);
                }
                else // Senão ele atualiza a arma do Id selecionado
                {
                    await aService.PutArmaAsync(model);
                }

                await Application.Current.MainPage.DisplayAlert("Mensagem", "Dados salvo com sucesso", "Ok");

                await Shell.Current.GoToAsync("..");
            }
            catch (System.Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ops!", ex.Message, "Ok");
            }
        }

        private async void CancelarCadastro()
        {
            await Shell.Current.GoToAsync("..");
        }
        #endregion
    }
}