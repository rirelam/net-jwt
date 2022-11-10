using Microsoft.EntityFrameworkCore;
using ServerJwt.Models;

namespace ServerJwt.Data
{
public class UserContext : DbContext
  {
    public UserContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
    {
    }

    public DbSet<LoginModel>? LoginModels { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<LoginModel>().HasData(new LoginModel
      {
        Id = 1,
        UserName = "johndoe",
        Password = "def@123"
      });
    }
  }
}