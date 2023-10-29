using AppRpgEtec.Helpers.Message;
using AppRpgEtec.Models;
using AppRpgEtec.Services;
using AppRpgEtec.Services.Usuarios;
using AppRpgEtec.Views.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AppRpgEtec.ViewModels.Usuarios
{

    public class UsuarioViewModel : BaseViewModel
    {
        private UsuarioService uService;

        public ICommand RegistrarCommand { get; set; }
        public ICommand AutenticarCommand { get; set; }
        public ICommand DirecionarCadastroCommand { get; set; }

        //ctor + TAB + TAB: Atalho para criar o método construtor
        public UsuarioViewModel()
        {
            string token = Preferences.Get("UsuarioToken", string.Empty);
            uService = new UsuarioService(token);
            InicializarCommands();
        }

        public void InicializarCommands()
        {
            RegistrarCommand = new Command(async () => await RegistrarUsuario());
            AutenticarCommand = new Command(async () => await AutenticarUsuario());
            DirecionarCadastroCommand = new Command(async () => await DirecionarParaCadastro());
        }

        #region AtributosPropriedades
        // As propriedades serão chamadas na View futuramente

        // Atributo (Pode ser alterado)
        private string login = string.Empty;

        //Propriedade (Não pode ser alterada)
        public string Login
        {
            get { return login; }
            set
            {
                login = value;
                OnPropertyChanged();
            }
        }

        private string senha = string.Empty;
        public string Senha
        {
            get { return senha; }
            set
            {
                senha = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Métodos

        public async Task RegistrarUsuario()
        {
            try
            {
                Usuario u = new Usuario();
                u.UserName = Login;
                u.PasswordString = Senha;

                /*if (String.IsNullOrEmpty(u.UserName) ||
                    String.IsNullOrEmpty(u.PasswordString))
                {
                    throw new Exception("Favor informar um nome de usuario e senha");
                }*/

                // Usuario uRegistrado = await uService.PostRegistrarUsuarioAsync(u);

                Usuario uRegistrado = null;

                if (!String.IsNullOrEmpty(u.UserName) ||
                    !String.IsNullOrEmpty(u.PasswordString))
                {
                    uRegistrado = await uService.PostRegistrarUsuarioAsync(u);
                }
                else
                {
                    throw new Exception("Informar nome de usuário e senha para cadastrar");
                }

                // Se o Id retornado for diferente de 0 é porque o cadastro deu certo
                if (uRegistrado.Id != 0)
                {
                    string mensagem = $"Usuário Id {uRegistrado.Id} registrado com sucesso!";
                    await Application.Current.MainPage.DisplayAlert("Informação", mensagem, "Ok");

                    await Application.Current.MainPage
                        .Navigation.PopAsync(); // PopAsync remove a página da pilha de visualização (Não acumula histórico das páginas para voltar <-)
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Informação", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }

        // Variáveis de controle
        private CancellationTokenSource _cancelTokenSource;
        private bool _isCheckingLocation;
        public async Task AutenticarUsuario()
        {
            try
            {
                Usuario u = new Usuario();
                u.UserName = Login;
                u.PasswordString = Senha;

                Usuario uAutenticado = await uService.PostAutenticarUsuarioAsync(u);

                // Na classe UsuarioService estamos passando o token como string.Empty para o PostAsync
                // Sendo assim, essa validação não estaria "sempre" com o uAutenticado.Token como Empty?
                if (!string.IsNullOrEmpty(uAutenticado.Token))
                {
                    string mensagem = $"Bem-vindo(a) {uAutenticado.UserName}";

                    // Guardando os dados nas Preferences para uso futuro
                    Preferences.Set("UsuarioId", uAutenticado.Id);
                    Preferences.Set("UsuarioUserName", uAutenticado.UserName);
                    Preferences.Set("UsuarioPerfil", uAutenticado.Perfil);
                    Preferences.Set("UsuarioToken", uAutenticado.Token);

                    Models.Email email = new Models.Email();
                    email.Remetente = "apprpgetec@gmail.com";
                    email.RemetentePassword = "ibouqpzdwymuphja";
                    email.Destinatario = "apprpgetec@gmail.com";
                    email.DestinatarioCopia = string.Empty;
                    email.DominioPrimario = "smtp.gmail.com";
                    email.PortaPrimaria = 587;
                    email.Assunto = "Notificação de Acesso";
                    email.Mensagem = $" Usuário {u.UserName} acessou o aplicativo RPG Etec" +
                        $" em {DateTime.Now:dd/MM/yyyy HH:mm:ss}";

                    // Comentei pois estava dando erro de SMTP quando ia fazer o login
                    /*EmailHelper emailHelper = new EmailHelper();
                    await emailHelper.EnviarEmail(email);*/

                    //Inicio da coleta de Geolocalização atual para Atualização da API:
                    _isCheckingLocation = true;
                    _cancelTokenSource = new CancellationTokenSource();
                    GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromMicroseconds(10));

                    Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

                    Usuario uLocation = new Usuario();
                    uLocation.Id = uAutenticado.Id;
                    uLocation.Latitude = location.Latitude;
                    uLocation.Longitude = location.Longitude;

                    UsuarioService uServiceLoc = new UsuarioService(uAutenticado.Token);
                    await uServiceLoc.PutAtualizarLocalizacaoAsync(uLocation);
                    //Fim da coleta de Geolocalização atual para a atualização da API

                    await Application.Current.MainPage
                        .DisplayAlert("Informação", mensagem, "Ok");

                    // Dúvida: O que essa linha faz?
                    // Application.Current.MainPage = new MainPage();
                    // Definindo que a página após o login seja a página que contém o menu:
                    Application.Current.MainPage = new AppShell();
                }
                else
                {
                    await Application.Current.MainPage
                        .DisplayAlert("Informação", "Dados incorretos!", "Ok");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Informação", ex.Message + " Detalhes: " + ex.InnerException.Message, "Ok");
            }
        }

        public async Task DirecionarParaCadastro()
        {
            try
            {
                await Application.Current.MainPage
                    .Navigation.PushAsync(new CadastroView()); // PushAsync empurra para a view escolhida
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Informação", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }

        #endregion

    }
}