using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PositiveNewsPlatform.Infrastructure.Persistence.Sql.Models;

namespace PositiveNewsPlatform.Infrastructure.Persistence.Sql.Configurations;

public sealed class MediaRegistryConfiguration : IEntityTypeConfiguration<MediaRegistryWriteModel>
{
    public void Configure(EntityTypeBuilder<MediaRegistryWriteModel> builder)
    {
        builder.ToTable("media_registry");

        builder.HasKey(x => x.MediaId);

        builder.Property(x => x.MediaId)
            .HasColumnName("media_id")
            .ValueGeneratedNever();

        builder.Property(x => x.ArticleId)
            .HasColumnName("article_id")
            .IsRequired();

        builder.Property(x => x.ObjectKey)
            .HasColumnName("object_key")
            .HasMaxLength(300)
            .IsRequired();

        builder.HasIndex(x => x.ObjectKey).IsUnique();

        builder.Property(x => x.MimeType)
            .HasColumnName("mime_type")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.SizeBytes)
            .HasColumnName("size_bytes")
            .IsRequired();

        builder.Property(x => x.UploadedAtUtc)
            .HasColumnName("uploaded_at_utc")
            .IsRequired();
    }
}