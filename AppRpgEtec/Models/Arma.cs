﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AppRpgEtec.Models
{
    public class Arma
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int Dano { get; set; }
        public Personagem Personagem { get; set; }
        public int PersonagemId { get; set; }
    }

}
