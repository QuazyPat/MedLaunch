namespace MedLaunch.Classes.MednaNet.db
{
    /*
    public class MednaLogDbContext : DbContext
    {
        public DbSet<MedLaunch.Classes.MednaNet.db.VersionInfo> VersionInfo { get; set; }
        public DbSet<MedLaunch.Classes.MednaNet.db.DiscordMessages> DiscordMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<VersionInfo>()
                .HasKey(a => a.Id);

            mb.Entity<DiscordMessages>()
                .HasKey(a => a.MessageId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder ob)
        {
            var sq1 = new SqliteConnection(@"DataSource = Data\Settings\MednaNetLogging.db");
            ob.UseSqlite(sq1);
        }
    }
    */
}
