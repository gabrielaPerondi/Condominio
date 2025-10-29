using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VizinhApp.Models;

namespace VizinhApp.Context
{
    public class CondominioContext: DbContext
    {
        public CondominioContext(DbContextOptions<CondominioContext> options)
        : base(options) { }
        
        public DbSet<Usuario> Usuarios {get; set;}
    }
}