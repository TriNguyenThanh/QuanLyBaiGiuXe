using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using QuanLyBaiGiuXe.Data.Entities;

namespace QuanLyBaiGiuXe.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomerVehicle> CustomerVehicles { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<InvoiceLine> InvoiceLines { get; set; }

    public virtual DbSet<ParkingLot> ParkingLots { get; set; }

    public virtual DbSet<ParkingSession> ParkingSessions { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<SendType> SendTypes { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    public virtual DbSet<VehicleType> VehicleTypes { get; set; }

    public virtual DbSet<Zone> Zones { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customer");

            entity.HasIndex(e => new { e.Status, e.FullName }, "IX_Customer_Status_Name");

            entity.Property(e => e.CustomerId)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("customerId");
            entity.Property(e => e.Email)
                .HasMaxLength(200)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(200)
                .HasColumnName("fullName");
            entity.Property(e => e.Phone)
                .HasMaxLength(30)
                .HasColumnName("phone");
            entity.Property(e => e.Status)
                .HasDefaultValue((byte)1)
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.User).WithMany(p => p.Customers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Customer_User");
        });

        modelBuilder.Entity<CustomerVehicle>(entity =>
        {
            entity.HasKey(e => new { e.CustomerId, e.VehicleId });

            entity.ToTable("CustomerVehicle");

            entity.HasIndex(e => e.CustomerId, "IX_CustomerVehicle_Customer");

            entity.HasIndex(e => e.VehicleId, "UQ_CustomerVehicle_UniqueVehicle").IsUnique();

            entity.Property(e => e.CustomerId).HasColumnName("customerId");
            entity.Property(e => e.VehicleId).HasColumnName("vehicleId");
            entity.Property(e => e.AssignedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("assignedAt");
            entity.Property(e => e.Note)
                .HasMaxLength(200)
                .HasColumnName("note");

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerVehicles)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustVeh_Customer");

            entity.HasOne(d => d.Vehicle).WithOne(p => p.CustomerVehicle)
                .HasForeignKey<CustomerVehicle>(d => d.VehicleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustVeh_Vehicle");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.ToTable("Invoice");

            entity.HasIndex(e => new { e.Status, e.IssuedAt }, "IX_Invoice_Status_IssuedAt");

            entity.HasIndex(e => e.InvoiceNo, "UQ_Invoice").IsUnique();

            entity.Property(e => e.InvoiceId)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("invoiceId");
            entity.Property(e => e.CustomerId).HasColumnName("customerId");
            entity.Property(e => e.InvoiceNo)
                .HasMaxLength(50)
                .HasColumnName("invoiceNo");
            entity.Property(e => e.IssuedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("issuedAt");
            entity.Property(e => e.IssuedBy).HasColumnName("issuedBy");
            entity.Property(e => e.Note)
                .HasMaxLength(200)
                .HasColumnName("note");
            entity.Property(e => e.SessionId).HasColumnName("sessionId");
            entity.Property(e => e.Status)
                .HasDefaultValue((byte)1)
                .HasColumnName("status");
            entity.Property(e => e.SubscriptionId).HasColumnName("subscriptionId");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("totalAmount");

            entity.HasOne(d => d.Customer).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_Invoice_Customer");

            entity.HasOne(d => d.IssuedByNavigation).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.IssuedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoice_User");

            entity.HasOne(d => d.Session).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.SessionId)
                .HasConstraintName("FK_Invoice_Session");

            entity.HasOne(d => d.Subscription).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.SubscriptionId)
                .HasConstraintName("FK_Invoice_Sub");
        });

        modelBuilder.Entity<InvoiceLine>(entity =>
        {
            entity.HasKey(e => e.LineId);

            entity.ToTable("InvoiceLine");

            entity.Property(e => e.LineId)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("lineId");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .HasColumnName("description");
            entity.Property(e => e.InvoiceId).HasColumnName("invoiceId");
            entity.Property(e => e.LineAmount)
                .HasComputedColumnSql("([qty]*[unitPrice])", true)
                .HasColumnType("decimal(29, 4)")
                .HasColumnName("lineAmount");
            entity.Property(e => e.Qty)
                .HasDefaultValue(1m)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("qty");
            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("unitPrice");

            entity.HasOne(d => d.Invoice).WithMany(p => p.InvoiceLines)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvoiceLine_Invoice");
        });

        modelBuilder.Entity<ParkingLot>(entity =>
        {
            entity.HasKey(e => e.LotId);

            entity.ToTable("ParkingLot");

            entity.Property(e => e.LotId)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("lotId");
            entity.Property(e => e.Address)
                .HasMaxLength(300)
                .HasColumnName("address");
            entity.Property(e => e.LotName)
                .HasMaxLength(200)
                .HasColumnName("lotName");
        });

        modelBuilder.Entity<ParkingSession>(entity =>
        {
            entity.HasKey(e => e.SessionId);

            entity.ToTable("ParkingSession");

            entity.HasIndex(e => new { e.Status, e.OpenAt }, "IX_Session_Status_OpenAt");

            entity.Property(e => e.SessionId)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("sessionId");
            entity.Property(e => e.CloseAt)
                .HasPrecision(0)
                .HasColumnName("closeAt");
            entity.Property(e => e.ClosedBy).HasColumnName("closedBy");
            entity.Property(e => e.LotId).HasColumnName("lotId");
            entity.Property(e => e.Note)
                .HasMaxLength(200)
                .HasColumnName("note");
            entity.Property(e => e.OpenAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("openAt");
            entity.Property(e => e.OpenedBy).HasColumnName("openedBy");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.SubscriptionId).HasColumnName("subscriptionId");
            entity.Property(e => e.TicketId).HasColumnName("ticketId");
            entity.Property(e => e.VehicleId).HasColumnName("vehicleId");
            entity.Property(e => e.VehicleTypeId).HasColumnName("vehicleTypeId");
            entity.Property(e => e.ZoneId).HasColumnName("zoneId");

            entity.HasOne(d => d.ClosedByNavigation).WithMany(p => p.ParkingSessionClosedByNavigations)
                .HasForeignKey(d => d.ClosedBy)
                .HasConstraintName("FK_Session_UserClose");

            entity.HasOne(d => d.Lot).WithMany(p => p.ParkingSessions)
                .HasForeignKey(d => d.LotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Session_Lot");

            entity.HasOne(d => d.OpenedByNavigation).WithMany(p => p.ParkingSessionOpenedByNavigations)
                .HasForeignKey(d => d.OpenedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Session_UserOpen");

            entity.HasOne(d => d.Subscription).WithMany(p => p.ParkingSessions)
                .HasForeignKey(d => d.SubscriptionId)
                .HasConstraintName("FK_Session_Sub");

            entity.HasOne(d => d.Ticket).WithMany(p => p.ParkingSessions)
                .HasForeignKey(d => d.TicketId)
                .HasConstraintName("FK_Session_Ticket");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.ParkingSessions)
                .HasForeignKey(d => d.VehicleId)
                .HasConstraintName("FK_Session_Vehicle");

            entity.HasOne(d => d.VehicleType).WithMany(p => p.ParkingSessions)
                .HasForeignKey(d => d.VehicleTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Session_VehicleType");

            entity.HasOne(d => d.Zone).WithMany(p => p.ParkingSessions)
                .HasForeignKey(d => d.ZoneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Session_Zone");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("Payment");

            entity.HasIndex(e => e.InvoiceId, "IX_Payment_Invoice");

            entity.Property(e => e.PaymentId)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("paymentId");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.InvoiceId).HasColumnName("invoiceId");
            entity.Property(e => e.MethodId).HasColumnName("methodId");
            entity.Property(e => e.PaidAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("paidAt");
            entity.Property(e => e.Status)
                .HasDefaultValue((byte)1)
                .HasColumnName("status");

            entity.HasOne(d => d.Invoice).WithMany(p => p.Payments)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payment_Invoice");

            entity.HasOne(d => d.Method).WithMany(p => p.Payments)
                .HasForeignKey(d => d.MethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payment_Method");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.MethodId);

            entity.ToTable("PaymentMethod");

            entity.Property(e => e.MethodId)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("methodId");
            entity.Property(e => e.MethodName)
                .HasMaxLength(50)
                .HasColumnName("methodName");
        });

        modelBuilder.Entity<SendType>(entity =>
        {
            entity.ToTable("SendType");

            entity.Property(e => e.SendTypeId)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("sendTypeId");
            entity.Property(e => e.SendName)
                .HasMaxLength(50)
                .HasColumnName("sendName");
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.ToTable("Subscription");

            entity.Property(e => e.SubscriptionId)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("subscriptionId");
            entity.Property(e => e.CustomerId).HasColumnName("customerId");
            entity.Property(e => e.EndDate).HasColumnName("endDate");
            entity.Property(e => e.PackageType)
                .HasMaxLength(10)
                .HasColumnName("packageType");
            entity.Property(e => e.StartDate).HasColumnName("startDate");
            entity.Property(e => e.Status)
                .HasDefaultValue((byte)1)
                .HasColumnName("status");
            entity.Property(e => e.VehicleId).HasColumnName("vehicleId");

            entity.HasOne(d => d.Customer).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Sub_Customer");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Sub_Vehicle");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.ToTable("Ticket");

            entity.HasIndex(e => e.TicketCode, "UQ_Ticket").IsUnique();

            entity.Property(e => e.TicketId)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("ticketId");
            entity.Property(e => e.IssueAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("issueAt");
            entity.Property(e => e.IssuedBy).HasColumnName("issuedBy");
            entity.Property(e => e.LicensePlateAtEntry)
                .HasMaxLength(20)
                .HasColumnName("licensePlateAtEntry");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TicketCode)
                .HasMaxLength(50)
                .HasColumnName("ticketCode");
            entity.Property(e => e.VehicleTypeId).HasColumnName("vehicleTypeId");
            entity.Property(e => e.ZoneId).HasColumnName("zoneId");

            entity.HasOne(d => d.IssuedByNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.IssuedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_User");

            entity.HasOne(d => d.VehicleType).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.VehicleTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_VehicleType");

            entity.HasOne(d => d.Zone).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.ZoneId)
                .HasConstraintName("FK_Ticket_Zone");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.HasIndex(e => e.Username, "UQ_User").IsUnique();

            entity.Property(e => e.UserId)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("userId");
            entity.Property(e => e.FullName)
                .HasMaxLength(200)
                .HasColumnName("fullName");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(256)
                .HasColumnName("passwordHash");
            entity.Property(e => e.RoleName)
                .HasMaxLength(20)
                .HasDefaultValue("STAFF")
                .HasColumnName("roleName");
            entity.Property(e => e.Status)
                .HasDefaultValue((byte)1)
                .HasColumnName("status");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.ToTable("Vehicle");

            entity.HasIndex(e => e.LicensePlate, "UQ_Vehicle_LicensePlate_NotNull")
                .IsUnique()
                .HasFilter("([licensePlate] IS NOT NULL)");

            entity.HasIndex(e => e.RfidTag, "UQ_Vehicle_Rfid_NotNull")
                .IsUnique()
                .HasFilter("([rfidTag] IS NOT NULL)");

            entity.Property(e => e.VehicleId)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("vehicleId");
            entity.Property(e => e.Brand)
                .HasMaxLength(100)
                .HasColumnName("brand");
            entity.Property(e => e.Color)
                .HasMaxLength(50)
                .HasColumnName("color");
            entity.Property(e => e.LicensePlate)
                .HasMaxLength(20)
                .HasColumnName("licensePlate");
            entity.Property(e => e.RfidTag)
                .HasMaxLength(50)
                .HasColumnName("rfidTag");
            entity.Property(e => e.Status)
                .HasDefaultValue((byte)1)
                .HasColumnName("status");
            entity.Property(e => e.VehicleTypeId).HasColumnName("vehicleTypeId");

            entity.HasOne(d => d.VehicleType).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.VehicleTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vehicle_VehicleType");
        });

        modelBuilder.Entity<VehicleType>(entity =>
        {
            entity.ToTable("VehicleType");

            entity.Property(e => e.VehicleTypeId)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("vehicleTypeId");
            entity.Property(e => e.TypeName)
                .HasMaxLength(50)
                .HasColumnName("typeName");
        });

        modelBuilder.Entity<Zone>(entity =>
        {
            entity.ToTable("Zone");

            entity.Property(e => e.ZoneId)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("zoneId");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.LotId).HasColumnName("lotId");
            entity.Property(e => e.VehicleTypeId).HasColumnName("vehicleTypeId");
            entity.Property(e => e.ZoneName)
                .HasMaxLength(200)
                .HasColumnName("zoneName");

            entity.HasOne(d => d.Lot).WithMany(p => p.Zones)
                .HasForeignKey(d => d.LotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Zone_Lot");

            entity.HasOne(d => d.VehicleType).WithMany(p => p.Zones)
                .HasForeignKey(d => d.VehicleTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Zone_VehicleType");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
