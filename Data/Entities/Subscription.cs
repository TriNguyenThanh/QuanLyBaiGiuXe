using System;
using System.Collections.Generic;

namespace QuanLyBaiGiuXe.Data.Entities;

public partial class Subscription
{
    public Guid SubscriptionId { get; set; }

    public Guid CustomerId { get; set; }

    public Guid VehicleId { get; set; }

    public string PackageType { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public byte Status { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<ParkingSession> ParkingSessions { get; set; } = new List<ParkingSession>();

    public virtual Vehicle Vehicle { get; set; } = null!;
}
