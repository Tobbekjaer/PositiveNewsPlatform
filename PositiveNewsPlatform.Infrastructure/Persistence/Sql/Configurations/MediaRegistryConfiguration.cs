using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PositiveNewsPlatform.Infrastructure.Persistence.Sql.Models;

namespace PositiveNewsPlatform.Infrastructure.Persistence.Sql.Configurations;

public sealed class MediaRegistryConfiguration : IEntityTypeConfiguration<MediaRegistryWriteModel>
{
    public void Configure(EntityTypeBuilder<MediaRegistryWriteModel> builder)
    {
        builder.ToTable("MediaRegistry");

        builder.HasKey(x => x.MediaId);

        builder.Property(x => x.MediaId)
            .HasColumnName("MediaId")
            .ValueGeneratedNever();

        builder.Property(x => x.ArticleId)
            .HasColumnName("ArticleId")
            .IsRequired();

        builder.Property(x => x.ObjectKey)
            .HasColumnName("ObjectKey")
            .HasMaxLength(300)
            .IsRequired();

        builder.HasIndex(x => x.ObjectKey).IsUnique();

        builder.Property(x => x.MimeType)
            .HasColumnName("MimeType")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.SizeBytes)
            .HasColumnName("SizeBytes")
            .IsRequired();

        builder.Property(x => x.UploadedAtUtc)
            .HasColumnName("UploadedAtUtc")
            .IsRequired();
    }
}