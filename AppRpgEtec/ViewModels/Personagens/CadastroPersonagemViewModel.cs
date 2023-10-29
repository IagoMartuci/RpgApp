using AppRpgEtec.Models;
using AppRpgEtec.Models.Enuns;
using AppRpgEtec.Services.Personagens;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AppRpgEtec.ViewModels.Personagens
{
    // Diretiva que serve como uma pesquisa para relacionar o id do personagem selecionado...
    // ...na view de listagem para que esse dado seja recuperado na view de cadastro:
    [QueryProperty("PersonagemSelecionadoId", "pId")]
    public class CadastroPersonagemViewModel : BaseViewModel
    {
        private PersonagemService pService;
        public CadastroPersonagemViewModel()
        {
            string token = Preferences.Get("UsuarioToken", string.Empty);
            pService = new PersonagemService(token);
            // _ descarta o retorno do método, então o método apenas executará a ação
            _ = ObterClasses();

            SalvarCommand = new Command(async () => { await SalvarPersonagem(); }, () => ValidarCampos());
            // SalvarCommand = new Command(SalvarPersonagem);
            CancelarCommand = new Command(async => CancelarCadastro());
        }

        public ICommand SalvarCommand { get; set; }
        public ICommand CancelarCommand { get; set; }

        // Atributos
        private int id;
        private string nome;
        private int pontosVida;
        private int forca;
        private int defesa;
        private int inteligencia;
        private int disputas;
        private int vitorias;
        private int derrotas;

        // Propriedades padrão criadas automaticamente com CTRL+R+E
        /*
        public int Id { get => id; set => id = value; }
        public string Nome { get => nome; set => nome = value; }
        public int PontosVida { get => pontosVida; set => pontosVida = value; }
        public int Forca { get => forca; set => forca = value; }
        public int Defesa { get => defesa; set => defesa = value; }
        public int Inteligencia { get => inteligencia; set => inteligencia = value; }
        public int Disputas { get => disputas; set => disputas = value; }
        public int Vitorias { get => vitorias; set => vitorias = value; }
        public int Derrotas { get => derrotas; set => derrotas = value; }
        */

        // Propriedades com OnPropertyChanged
        public int Id
        {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged();//Informa mundaça de estado para a View
            }
        }

        public string Nome
        {
            get => nome;
            set
            {
                nome = value;
                OnPropertyChanged();
                ((Command)SalvarCommand).ChangeCanExecute();
            }
        }
        public int PontosVida
        {
            get => pontosVida;
            set
            {
                pontosVida = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CadastroHabilitado));
                ((Command)SalvarCommand).ChangeCanExecute();

            }
        }

        public int Forca
        {
            get => forca;
            set
            {
                forca = value;
                OnPropertyChanged();
                ((Command)SalvarCommand).ChangeCanExecute();

            }
        }

        public int Defesa
        {
            get => defesa;
            set
            {
                defesa = value;
                OnPropertyChanged();
                ((Command)SalvarCommand).ChangeCanExecute();
            }
        }

        public int Inteligencia
        {
            get => inteligencia;
            set
            {
                inteligencia = value;
                OnPropertyChanged();
                ((Command)SalvarCommand).ChangeCanExecute();
            }
        }

        public int Disputas
        {
            get => disputas;
            set
            {
                disputas = value;
                OnPropertyChanged();
            }
        }

        public int Vitorias
        {
            get => vitorias;
            set
            {
                vitorias = value;
                OnPropertyChanged();
            }
        }

        public int Derrotas
        {
            get => derrotas;
            set
            {
                derrotas = value;
                OnPropertyChanged();
            }
        }

        // Atributo
        private ObservableCollection<TipoClasse> listaTipoClasse;

        // Propriedade
        public ObservableCollection<TipoClasse> ListaTipoClasse
        {
            get => listaTipoClasse;
            set
            {
                listaTipoClasse = value;
                OnPropertyChanged();
            }
        }

        public async Task ObterClasses()
        {
            try
            {
                ListaTipoClasse = new ObservableCollection<TipoClasse>();
                ListaTipoClasse.Add(new TipoClasse() { Id = 1, Descricao = "Cavaleiro" });
                ListaTipoClasse.Add(new TipoClasse() { Id = 2, Descricao = "Mago" });
                ListaTipoClasse.Add(new TipoClasse() { Id = 3, Descricao = "Clerigo" });
                OnPropertyChanged(nameof(ListaTipoClasse));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");    
            }
        }

        // Atributo
        private TipoClasse tipoClasseSelecionado;

        // Propriedade
        public TipoClasse TipoClasseSelecionado
        {
            get => tipoClasseSelecionado;
            set
            {
                if (value != null)
                {
                    tipoClasseSelecionado = value;
                    OnPropertyChanged();
                }
            }
        }

        public async Task SalvarPersonagem()
        //public async void SalvarPersonagem()
        {
            try
            {
                // Atribuindo valores para os personagens, "this" significa o que temos dentro da classe em questão
                Personagem model = new Personagem()
                {
                    Nome = this.nome,
                    PontosVida = this.pontosVida,
                    Defesa = this.defesa,
                    Derrotas = this.derrotas,
                    Disputas = this.disputas,
                    Forca = this.forca,
                    Inteligencia = this.inteligencia,
                    Vitorias = this.vitorias,
                    Id = this.id,
                    // Convertendo o Id do tipoClasseSelecionado para Enum
                    Classe = (ClasseEnum)tipoClasseSelecionado.Id
                };

                // Se o Id for 0 é porque o personagem não existe, então ele cria um novo
                if (model.Id == 0)
                {
                    await pService.PostPersonagemAsync(model);
                }
                else // Senão ele atualiza o personagem do Id selecionado
                {
                    await pService.PutPersonagemAsync(model);
                }

                await Application.Current.MainPage
                    .DisplayAlert("Mensagem", "Dados salvos com sucesso!", "Ok");

                await Shell.Current.GoToAsync(".."); // Remove a página atual da pilha de páginas
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }

        private async void CancelarCadastro()
        {
            await Shell.Current.GoToAsync("..");
        }

        // Atributo
        private string personagemSelecionadoId;

        // Propriedade
        public string PersonagemSelecionadoId
        {
            get => personagemSelecionadoId;
            set
            {
                if(value != null)
                {
                    personagemSelecionadoId = Uri.UnescapeDataString(value);
                    CarregarPersonagem();
                }
            }
        }

        public async void CarregarPersonagem()
        {
            try
            {
                Personagem p = await
                    pService.GetPersonagemAsync(int.Parse(personagemSelecionadoId));

                this.Nome = p.Nome;
                this.PontosVida = p.PontosVida;
                this.Defesa = p.Defesa;
                this.Derrotas = p.Derrotas;
                this.Disputas = p.Disputas;
                this.Forca = p.Forca;
                this.Inteligencia = p.Inteligencia;
                this.Vitorias = p.Vitorias;
                this.Id = p.Id;

                TipoClasseSelecionado = this.ListaTipoClasse
                    .FirstOrDefault(tClasse => tClasse.Id == (int)p.Classe);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
                throw;
            }
        }

        public bool cadastroHabilitado;

        // Retorna apenas true ou false, então não conseguimos atribuir valores ela, por isso temos apenas o get
        public bool CadastroHabilitado
        {
            get
            {
                return (PontosVida > 0); 
            }
        }

        public bool ValidarCampos()
        {
            return !string.IsNullOrEmpty(Nome)
                && !string.IsNullOrWhiteSpace(Nome)
                && CadastroHabilitado
                && Forca != 0
                && Defesa != 0
                && Inteligencia != 0;
        }
        
    }
}
