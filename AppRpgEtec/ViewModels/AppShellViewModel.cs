﻿using AppRpgEtec.Models;
using AppRpgEtec.Services;
using AppRpgEtec.Services.Usuarios;
using AppRpgEtec.ViewModels.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRpgEtec.ViewModels
{
    public class AppShellViewModel : BaseViewModel
    {
        private UsuarioService uService;

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

        public AppShellViewModel()
        {
            string token = Preferences.Get("UsuarioToken", string.Empty);
            uService = new UsuarioService(token);

            CarregarUsuario();
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
    }
}
