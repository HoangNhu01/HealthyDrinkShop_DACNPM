using eShopSolution.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.Data.Configurations
{
    public class IngredientInProductConfiguration : IEntityTypeConfiguration<IngredientInProduct>
    {
        public void Configure(EntityTypeBuilder<IngredientInProduct> builder)
        {
            builder.HasKey(t => new {t.IngredientId, t.ProductId });

            builder.ToTable("IngredientInProducts");

            builder.HasOne(t => t.Product).WithMany(pc => pc.IngredientInProducts)
                .HasForeignKey(pc=>pc.ProductId);

            builder.HasOne(t => t.Ingredient).WithMany(pc => pc.IngredientInProducts)
              .HasForeignKey(pc => pc.IngredientId);
        }
    }
}
