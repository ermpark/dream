using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class MainDBContext : DbContext
    {
        public MainDBContext(DbContextOptions<MainDBContext> options) : base(options)
        {

        }
        private string connection;
        public MainDBContext(string connection) => this.connection = connection;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!string.IsNullOrWhiteSpace(connection))
                optionsBuilder.UseMySql(connection);
        }
        public DbSet<Dream> Dream { get; set; }
        public DbSet<DreamInfo> DreamInfo { get; set; }
    }
}
