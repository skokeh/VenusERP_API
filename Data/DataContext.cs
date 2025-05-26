using System.Data.SqlClient;
using System.Data;
using Microsoft.EntityFrameworkCore;
using VenusERP_API.Models;

namespace VenusERP_API.Data
{
    public class DataContext :  DbContext
    {
        public DbSet<UserModel> sys_Users { get; set; }
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>()
            .HasKey(u => new { u.ID });





            base.OnModelCreating(modelBuilder);
        }
    }

}
