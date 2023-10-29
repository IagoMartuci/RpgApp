using AppRpgEtec.Models;
using AppRpgEtec.Services;
using AppRpgEtec.Services.Usuarios;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AppRpgEtec.ViewModels.Usuarios
{
    public class ImagemUsuarioViewModel : BaseViewModel
    {
        private UsuarioService uService;

        public ICommand AbrirGaleriaCommand { get; set; }
        public ICommand SalvarImagemCommand { get; set; }
        public ICommand FotografarCommand { get; set; }

        public ImagemUsuarioViewModel()
        {
            string token = Preferences.Get("UsuarioToken", string.Empty);
            uService = new UsuarioService(token);

            InicializarCommands();
            CarregarUsuario();
        }

        public void InicializarCommands()
        {
            AbrirGaleriaCommand = new Command(AbrirGaleria);
            SalvarImagemCommand = new Command(SalvarImagem);
            FotografarCommand = new Command(Fotografar);
        }

        #region AtributosPropriedades

        private ImageSource fonteImagem;
        public ImageSource FonteImagem
        {
            get { return fonteImagem; }
            set
            {
                fonteImagem = value;
                OnPropertyChanged();
            }
        }

        private byte[] foto;
        public byte[] Foto
        {
            get => foto;
            set
            {
                foto = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Metodos

        public async void Fotografar()
        {
            try
            {
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsCameraAvailable ||
                    !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await Application.Current.MainPage.DisplayAlert
                        ("Sem câmera", "A câmera não está disponível.", "Ok");
                    await Task.FromResult(false);
                }

                string fileName = string.Format("{0:ddMMyyyy_HHmmss}", DateTime.Now) + ".jpg";

                var file = await CrossMedia.Current.TakePhotoAsync
                    (new Plugin.Media.Abstractions.StoreCameraMediaOptions
                    {
                        Directory = "Fotos",
                        PhotoSize = Plugin.Media.Abstractions.PhotoSize.Small,
                        Name = fileName
                    });

                if (file is null)
                {
                    await Task.FromResult(false);
                }

                MemoryStream ms = null;
                using (ms = new MemoryStream())
                {
                    var stream = file.GetStream();
                    stream.CopyTo(ms);
                }

                FonteImagem = ImageSource.FromStream(() => file.GetStream());
                Foto = ms.ToArray();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + "Detalhes: " + ex.InnerException, "Ok");
            }
        }

        public async void SalvarImagem()
        {
            try
            {
                var u = new Usuario();
                u.Foto = foto;
                u.Id = Preferences.Get("UsuarioId", 0);

                if (u.Foto != null)
                {
                    if (await uService.PutFotoUsuarioAsync(u) != 0)
                    {
                        await Application.Current.MainPage.DisplayAlert
                            ("Mensagem", "Dados salvos com sucesso!", "Ok");
                    }
                    else
                    {
                        throw new Exception("Erro ao tentar atualizar imagem");
                    }
                }
                else 
                { 
                    throw new Exception("Não existe imagem para ser salva");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }

        public async void AbrirGaleria()
        {
            try
            {
                await CrossMedia.Current.Initialize();
                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    await Application.Current.MainPage.DisplayAlert("Galeria não suportada",
                        "Não existe permissão para acessar a galeria", "Ok");
                    return;
                }

                var file = await CrossMedia.Current.PickPhotoAsync();
                if (file == null)
                {
                    return;
                }

                MemoryStream ms = null;
                using (ms = new MemoryStream())
                {
                    var stream = file.GetStream();
                    stream.CopyTo(ms);
                }

                FonteImagem = ImageSource.FromStream(() => file.GetStream());
                Foto = ms.ToArray();
                return;
            }
            catch (Exception ex)
            {

                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }

        public async void CarregarUsuario()
        {
            try
            {
                int usuarioId = Preferences.Get("UsuarioId", 0);
                Usuario u = await uService.GetUsuarioAsync(usuarioId);

                Foto = u.Foto;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }

        #endregion
    }
}
