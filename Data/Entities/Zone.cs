using System;
using System.Collections.Generic;

namespace QuanLyBaiGiuXe.Data.Entities;

public partial class Zone
{
    public Guid ZoneId { get; set; }

    public Guid LotId { get; set; }

    public string ZoneName { get; set; } = null!;

    public Guid VehicleTypeId { get; set; }

    public int Capacity { get; set; }

    public virtual ParkingLot Lot { get; set; } = null!;

    public virtual ICollection<ParkingSession> ParkingSessions { get; set; } = new List<ParkingSession>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual VehicleType VehicleType { get; set; } = null!;
}
