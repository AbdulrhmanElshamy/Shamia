using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shamia.DataAccessLayer.Entities.Config
{
    public class ProductsOptionsConfig : IEntityTypeConfiguration<ProductOptions>
    {
        public void Configure(EntityTypeBuilder<ProductOptions> builder)
        {
            builder.Property(x => x.Quantity_In_Unit).HasColumnType("float");
        }
    }
}
