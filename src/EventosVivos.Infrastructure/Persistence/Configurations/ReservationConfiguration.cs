using EventosVivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventosVivos.Infrastructure.Persistence.Configurations;

internal sealed class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("reservations");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.BuyerName).HasMaxLength(120).IsRequired();
        builder.Property(r => r.BuyerEmail).HasMaxLength(200).IsRequired();
        builder.Property(r => r.ConfirmationCode).HasMaxLength(12);
        builder.Property(r => r.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(r => r.RowVersion).IsRowVersion();

        builder.HasOne(r => r.Event)
            .WithMany()
            .HasForeignKey(r => r.EventId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => r.EventId);
        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => r.ConfirmationCode).IsUnique();
    }
}
