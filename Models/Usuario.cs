using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VizinhApp.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Senha { get; set; }
        public string? TipoUsuario { get; set; }
    }
}