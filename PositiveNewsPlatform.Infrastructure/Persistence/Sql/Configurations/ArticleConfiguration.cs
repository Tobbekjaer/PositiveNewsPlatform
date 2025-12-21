using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PositiveNewsPlatform.Domain.Articles;

namespace PositiveNewsPlatform.Infrastructure.Persistence.Sql.Configurations;

public sealed class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.ToTable("Articles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("ArticleId")
            .HasConversion(
                id => id.Value,
                value => new ArticleId(value)
            )
            .ValueGeneratedNever();

        builder.Property(x => x.Title)
            .HasColumnName("Title")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Content)
            .HasColumnName("Content")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("Status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("CreatedAtUtc")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("UpdatedAtUtc");
    }
}