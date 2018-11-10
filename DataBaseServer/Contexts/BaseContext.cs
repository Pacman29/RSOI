using DataBaseServer.DBO;
using DataBaseServer.Exceptions.DBExceptions;
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
                .Property(e => e.Md5)
                .IsRequired(true)
                .HasMaxLength(32);

            modelBuilder.Entity<FileInfo>()
                .Property(e => e.changed);

            modelBuilder.Entity<FileInfo>()
                .Property(e => e.FileLength)
                .IsRequired(true);
            
            modelBuilder.Entity<FileInfo>()
                .Property(e => e.FileType);
            
            modelBuilder.Entity<FileInfo>()
                .Property(e => e.Path)
                .IsRequired(true)
                .HasMaxLength(250);

            modelBuilder.Entity<FileInfo>()
                .HasOne(e => e.Job)
                .WithMany(j => j.fileInfos)
                .HasForeignKey(e => e.JobGuidFk);

            modelBuilder.Entity<FileInfo>()
                .Property(e => e.PageNo);

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

            modelBuilder.Entity<Job>()
                .HasMany(e => e.fileInfos)
                .WithOne(fi => fi.Job)
                .HasForeignKey(fi => fi.JobGuidFk)
                .OnDelete(DeleteBehavior.Cascade);
        }
        
    }
}