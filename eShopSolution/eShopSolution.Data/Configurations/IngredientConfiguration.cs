using eShopSolution.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.Data.Configurations
{
    public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
    {
        public void Configure(EntityTypeBuilder<Ingredient> builder)
        {
            builder.ToTable("Ingredients");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired().UseIdentityColumn();
            builder.Property(x => x.Price);
           
            builder.Property(x => x.Stock).IsRequired();
           


        }
    }
}
