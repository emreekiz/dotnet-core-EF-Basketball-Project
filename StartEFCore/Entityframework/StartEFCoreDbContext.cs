using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StartEFCore.Models;

namespace StartEFCore.Entityframework
{
    public class StartEFCoreDbContext:DbContext
    {
        //yapıcı metot
        public StartEFCoreDbContext(DbContextOptions<StartEFCoreDbContext> options):base(options)
        {

        }
        //model siniflari al ve ef ile tanistir yani contexte ekle
        public  DbSet<Team> Teams { get; set; }
        public DbSet<Player> Players { get; set; }
    }
}
