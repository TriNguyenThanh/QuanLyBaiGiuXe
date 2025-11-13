using System;
using System.Collections.Generic;

namespace QuanLyBaiGiuXe.Data.Entities;

public partial class Ticket
{
    public Guid TicketId { get; set; }

    public string TicketCode { get; set; } = null!;

    public DateTime IssueAt { get; set; }

    public Guid IssuedBy { get; set; }

    public Guid VehicleTypeId { get; set; }

    public string? LicensePlateAtEntry { get; set; }

    public Guid? ZoneId { get; set; }

    public byte Status { get; set; }

    public virtual User IssuedByNavigation { get; set; } = null!;

    public virtual ICollection<ParkingSession> ParkingSessions { get; set; } = new List<ParkingSession>();

    public virtual VehicleType VehicleType { get; set; } = null!;

    public virtual Zone? Zone { get; set; }
}
