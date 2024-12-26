using JarvisWeb.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisWeb.Domain
{
    public class JarvisWebDbContext(DbContextOptions<JarvisWebDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<EndOfDayNote> EndOfDayNotes { get; set; }
    }
}
