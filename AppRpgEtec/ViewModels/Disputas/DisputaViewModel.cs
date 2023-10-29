using AppRpgEtec.Models;
using AppRpgEtec.Services.Personagens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AppRpgEtec.ViewModels.Disputas
{
    public class DisputaViewModel : BaseViewModel
    {
        private PersonagemService pService;
        public ObservableCollection<Personagem> PersonagensEncontrados { get; set; }
        public Personagem Atacante { get; set; }
        public Personagem Oponente { get; set; }
        public ICommand PesquisarPersonagensCommand { get; set; }

        public DisputaViewModel()
        {
            string token = Preferences.Get("UsuarioToken", string.Empty);
            pService = new PersonagemService(token);

            // Instanciando as propriedades no construtor para não precisar instanciar nos métodos
            Atacante = new Personagem();
            Oponente = new Personagem();

            PesquisarPersonagensCommand = new Command<string>(async (string pesquisa) =>
                { await PesquisarPersonagens(pesquisa); });
        }

        public async Task PesquisarPersonagens(string nomePersonagem)
        {
            try
            {
                PersonagensEncontrados = await pService.GetPersonagensByNomeAsync(nomePersonagem);
                OnPropertyChanged(nameof(PersonagensEncontrados));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message, "Ok");
            }
        }

        //Adaptação da pesquisa que atualiza a lista em tempo real
        private string textPesquisa;
        public string TextPesquisa
        {
            get => textPesquisa;
            set
            {
                if (value != null)
                {
                    textPesquisa = value;
                    PesquisarPersonagens(textPesquisa);
                    OnPropertyChanged();
                    //PersonagensEncontrados.Clear();
                }
            }
        }

        public string NomePersonagemAtacante
        {
            get => Atacante.Nome;
        }

        public string NomePersonagemOponente
        {
            get => Oponente.Nome;
        }

        private Personagem personagemSelecionado;
        public Personagem PersonagemSelecionado
        {
            set
            {
                if(value != null)
                {
                    personagemSelecionado = value;
                    SelecionarPersonagem(personagemSelecionado);
                    OnPropertyChanged();
                    PersonagensEncontrados.Clear();
                }
            }
        }

        public async void SelecionarPersonagem(Personagem p)
        {
            try
            {
                string tipoCombatente = await Application.Current.MainPage
                    .DisplayActionSheet("Atacante ou Oponente?", "Cancelar", "", "Atacante", "Oponente");

                if (tipoCombatente.Equals("Atacante"))
                {
                    Atacante = p;
                    OnPropertyChanged(nameof(NomePersonagemAtacante));
                }
                else
                {
                    Oponente = p;
                    OnPropertyChanged(nameof(NomePersonagemOponente));
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message, "Ok");
            }
        }
    }
}
