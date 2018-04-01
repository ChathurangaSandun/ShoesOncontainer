namespace ProductCatelogApi.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using ProductCatelogApi.Domain;

    public class CatalogContext : DbContext
    {
        public DbSet<CatalogItem> CatalogItems { get; set; }
        public DbSet<CatalogBrand> CatalogBrands{ get; set; }
        public DbSet<CatalogType> CatalogTypes { get; set; }

        public CatalogContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CatalogBrand>(ConfigureCatalogBrand);
            modelBuilder.Entity<CatalogType>(ConfigureCatalogType);
            modelBuilder.Entity<CatalogItem>(ConfigureCatalogItem);
        }

        private void ConfigureCatalogItem(EntityTypeBuilder<CatalogItem> builder)
        {
            builder.ToTable("Catalog");
            builder.Property(c => c.Id)
                .ForSqlServerUseSequenceHiLo("Catalog_hilo")
                .IsRequired(true);
            builder.Property(c => c.Name).IsRequired(true).HasMaxLength(50);
            builder.Property(c => c.Price).IsRequired(true);
            builder.Property(c => c.PictureUrl).IsRequired(false);

            builder.HasOne(c => c.CatalogBrand).WithMany()
                .HasForeignKey(c => c.CatalogBrandId);
            builder.HasOne(c => c.CatalogType).WithMany()
                .HasForeignKey(c => c.CatalogTypeId);
        }

        private void ConfigureCatalogType(EntityTypeBuilder<CatalogType> builder)
        {
            builder.ToTable("Brand");
            builder.Property(c => c.Id)
                .ForSqlServerUseSequenceHiLo("Catalog_Type_hilo")
                .IsRequired(true);
            builder.Property(c => c.Type).IsRequired(true).HasMaxLength(100);
        }

        private void ConfigureCatalogBrand(EntityTypeBuilder<CatalogBrand> builder)
        {
            builder.ToTable("Brand");
            builder.Property(c => c.Id)
                .ForSqlServerUseSequenceHiLo("Catalog_brand_hilo")
                .IsRequired(true);
            builder.Property(c => c.Brand).IsRequired(true).HasMaxLength(100);
        }
    }
}
