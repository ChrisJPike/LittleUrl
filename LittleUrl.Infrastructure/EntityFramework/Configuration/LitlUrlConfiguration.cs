using LittleUrl.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LittleUrl.Infrastructure.EntityFramework.Configuration
{
    class LitlUrlConfiguration : IEntityTypeConfiguration<LitlUrl>
    {
        public void Configure(EntityTypeBuilder<LitlUrl> builder)
        {
            builder.ToTable("LitlUrl");

            builder.Property(l => l.Code)
                .HasMaxLength(9); // max length of a li.tl code

            builder.Property(l => l.LongUrl)
                .HasMaxLength(2083); // max length that Edge supports in a Url

            builder.HasIndex(l => l.Code)
                .IsUnique(); // codes should always be unique

            builder.HasKey(s => s.Id);
        }
    }
}
