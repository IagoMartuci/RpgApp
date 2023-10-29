using AppRpgEtec.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRpgEtec.Services.Usuarios
{
    // Classe de serviços do Usuário herda a classe de serviços Request
    public class UsuarioService : Request
    {
        private readonly Request _request;
        private const string apiUrlBase = "http://iagomartuci.somee.com/RpgApi/Usuarios";
        //private const string apiUrlBase = "https://bsite.net/luizfernando987/Usuarios";
        private string _token;
        // Método construtor
        public UsuarioService(string token)
        {
            // Instanciando na memória a classe Request
            _request = new Request();
            _token = token;
        }

        // Método que retorna o u.Id do objeto Usuário (Apenas o Id do Usuario)
        public async Task<Usuario> PostRegistrarUsuarioAsync(Usuario u)
        {
            string urlComplementar = "/Registrar";
            // Dentro do PostReturnIntAsync passamos os parametros que foram definidos na classe Request
            u.Id = await _request.PostReturnIntAsync(apiUrlBase + urlComplementar, u);
            return u;
        }

        // Método que retorna o u (Objeto Usuario Inteiro)
        public async Task<Usuario> PostAutenticarUsuarioAsync(Usuario u)
        {
            string urlComplementar = "/Autenticar";
            // Dentro do PostAsync passamos os parametros que foram definidos na classe Request
            u = await _request.PostAsync(apiUrlBase + urlComplementar, u, string.Empty);
            return u;
        }

        public async Task<int> PutFotoUsuarioAsync(Usuario u)
        {
            string urlComplementar = "/AtualizarFoto";
            var result = await _request.PutAsync(apiUrlBase + urlComplementar, u, _token);
            return result;
        }

        public async Task<Usuario> GetUsuarioAsync(int usuarioId)
        {
            string urlComplementar = string.Format("/{0}", usuarioId);
            var usuario = await
                _request.GetAsync<Usuario>(apiUrlBase+ urlComplementar, _token);
            return usuario;
        }

        public async Task<int> PutAtualizarLocalizacaoAsync(Usuario u)
        {
            string urlComplementar = "/AtualizarLocalizacao";
            var result = await _request.PutAsync(apiUrlBase + urlComplementar, u, _token);
            return result;
        }

        //using System.Collections.ObjectModel
        public async Task<ObservableCollection<Usuario>> GetUsuariosAsync()
        {
            string urlComplementar = string.Format("{0}", "/GetAll");
            ObservableCollection<Models.Usuario> listaUsuarios = await
            _request.GetAsync<ObservableCollection<Models.Usuario>>(apiUrlBase + urlComplementar, _token);
            return listaUsuarios;
        }
    }
}
