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

            builder.HasKey(s => s.Id);
        }
    }
}
