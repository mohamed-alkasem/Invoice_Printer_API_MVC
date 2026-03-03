using Invoice_printer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Invoice_printer.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<CompanyProfile> CompanyProfiles { get; set; } = default!;
        public DbSet<Party> Parties { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<Receipt> Receipts { get; set; }
        public DbSet<ReceiptItem> ReceiptItems { get; set; }
        public DbSet<ReceiptExport> ReceiptExports { get; set; }
    
       protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CompanyProfile>()
                .HasIndex(x => x.UserId)
                .IsUnique();

            builder.Entity<CompanyProfile>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Party>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Template>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Receipt>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Receipt>()
                .HasOne(x => x.CompanyProfile)
                .WithMany()
                .HasForeignKey(x => x.CompanyProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Receipt>()
                .HasOne(x => x.Party)
                .WithMany(x => x.Receipts)
                .HasForeignKey(x => x.PartyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Receipt>()
                .HasOne(x => x.Template)
                .WithMany(x => x.Receipts)
                .HasForeignKey(x => x.TemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Receipt>()
                .HasIndex(x => new { x.UserId, x.ReceiptNo })
                .IsUnique();


            builder.Entity<ReceiptItem>()
                .HasOne(x => x.Receipt)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.ReceiptId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ReceiptExport>()
                .HasOne(x => x.Receipt)
                .WithMany(x => x.Exports)
                .HasForeignKey(x => x.ReceiptId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
