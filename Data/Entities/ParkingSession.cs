using System;
using System.Collections.Generic;

namespace QuanLyBaiGiuXe.Data.Entities;

public partial class ParkingSession
{
    public Guid SessionId { get; set; }

    public DateTime OpenAt { get; set; }

    public DateTime? CloseAt { get; set; }

    public Guid LotId { get; set; }

    public Guid ZoneId { get; set; }

    public Guid VehicleTypeId { get; set; }

    public Guid? TicketId { get; set; }

    public Guid? SubscriptionId { get; set; }

    public Guid? VehicleId { get; set; }

    public Guid OpenedBy { get; set; }

    public Guid? ClosedBy { get; set; }

    public byte Status { get; set; }

    public string? Note { get; set; }

    public virtual User? ClosedByNavigation { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ParkingLot Lot { get; set; } = null!;

    public virtual User OpenedByNavigation { get; set; } = null!;

    public virtual Subscription? Subscription { get; set; }

    public virtual Ticket? Ticket { get; set; }

    public virtual Vehicle? Vehicle { get; set; }

    public virtual VehicleType VehicleType { get; set; } = null!;

    public virtual Zone Zone { get; set; } = null!;
}
