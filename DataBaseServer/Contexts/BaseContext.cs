using DataBaseServer.DBO;
using Microsoft.EntityFrameworkCore;

namespace DataBaseServer.Contexts
{
    public sealed class BaseContext :  DbContext
    {
        public DbSet<FileInfo> FileInfos { get; set; }
        public DbSet<Job> Jobs { get; set; }

        public BaseContext()
        {
            
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            DataBaseConnection.GetDatabaseConnection(optionsBuilder);
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FileInfo>()
                .ToTable("FileInfos");
            
            modelBuilder.Entity<FileInfo>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<FileInfo>()
                .HasIndex(e => e.Md5)
                .IsUnique(true);

            modelBuilder.Entity<FileInfo>()
                .Property(e => e.Md5)
                .IsRequired(true)
                .HasMaxLength(32);

            modelBuilder.Entity<FileInfo>()
                .Property(e => e.changed);

            modelBuilder.Entity<FileInfo>()
                .Property(e => e.fileLength)
                .IsRequired(true);
            
            modelBuilder.Entity<FileInfo>()
                .Property(e => e.Version);
            
            modelBuilder.Entity<FileInfo>()
                .Property(e => e.Path)
                .IsRequired(true)
                .HasMaxLength(250);

            modelBuilder.Entity<FileInfo>()
                .HasOne(e => e.Job)
                .WithMany(j => j.fileInfos)
                .HasForeignKey(e => e.JobGuidFk);

            modelBuilder.Entity<Job>()
                .ToTable("Jobs");
            
            modelBuilder.Entity<Job>()
                .HasKey(e => e.GUID);
            
            modelBuilder.Entity<Job>()
                .Property(e => e.GUID)
                .IsRequired(true)
                .HasMaxLength(36);
            
            modelBuilder.Entity<Job>()
                .Property(e => e.changed);

            modelBuilder.Entity<Job>()
                .Property(e => e.status);
        }
        
    }
}