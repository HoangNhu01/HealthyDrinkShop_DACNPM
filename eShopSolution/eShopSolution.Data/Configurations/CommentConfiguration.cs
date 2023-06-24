using eShopSolution.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.Data.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable("Comments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId).IsRequired();/*.UseIdentityColumn();*/
            builder.Property(x => x.ProductId).IsRequired();/*.UseIdentityColumn();*/


            builder.Property(x => x.CreatedDate);

            builder.Property(x => x.CommentText).IsRequired().IsUnicode(false).HasMaxLength(100);

            builder.Property(x => x.ParentId);

           

            builder.HasOne(x => x.User).WithMany(x => x.Comments).HasForeignKey(x => x.UserId);
            builder.HasOne(x => x.Product).WithMany(x => x.Comments).HasForeignKey(x => x.ProductId);


        }
    }
}
