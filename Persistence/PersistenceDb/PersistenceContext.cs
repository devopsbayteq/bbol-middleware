using PersistenceDb.Models.Authentication;
using PersistenceDb.Models.Core;
using PersistenceDb.Models.Enums;
using Microsoft.EntityFrameworkCore;
using PersistenceDb.Utils.Extension;
using PersistenceDb.Models.Configuration;
using Microsoft.Extensions.Configuration;

namespace PersistenceDb;
public class PersistenceContext(
    DbContextOptions<PersistenceContext> options,
    IConfiguration configuration) : DbContext(options)
{
    #region Constructor

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var databaseConfiguration = configuration.GetSection("CustomConnectionStrings").Get<List<DatabaseConfiguration>>().FirstOrDefault()
            ?? throw new InvalidOperationException("No se encontró la configuración de la base de datos en el appsettings.json");
        modelBuilder.UseEncryption(databaseConfiguration.AesSecret);

        base.OnModelCreating(modelBuilder);
    }

    #endregion

    #region Core DbSet

    public DbSet<Device> MobileDevice { get; set; }
    public DbSet<User> UserApp { get; set; }
    public DbSet<AccountUser> AccountUser { get; set; }
    public DbSet<UserDeviceChallenge> UserDeviceChallenge { get; set; }
    public DbSet<Beneficiary> Beneficiary { get; set; }
    public DbSet<Transaction> Transaction { get; set; }

    #endregion

}