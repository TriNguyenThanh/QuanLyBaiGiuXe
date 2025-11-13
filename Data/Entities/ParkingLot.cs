using System;
using System.Collections.Generic;

namespace QuanLyBaiGiuXe.Data.Entities;

public partial class ParkingLot
{
    public Guid LotId { get; set; }

    public string LotName { get; set; } = null!;

    public string? Address { get; set; }

    public virtual ICollection<ParkingSession> ParkingSessions { get; set; } = new List<ParkingSession>();

    public virtual ICollection<Zone> Zones { get; set; } = new List<Zone>();
}
