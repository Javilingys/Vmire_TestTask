using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parser.Models;

namespace Parser.Database.Configurations
{
    public class NumberCountConfiguration : IEntityTypeConfiguration<NumberCount>
    {
        public void Configure(EntityTypeBuilder<NumberCount> builder)
        {
            builder.HasIndex(x => x.Name).IsUnique();
        }
    }
}
