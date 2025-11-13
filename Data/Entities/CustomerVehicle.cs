using System;
using System.Collections.Generic;

namespace QuanLyBaiGiuXe.Data.Entities;

public partial class CustomerVehicle
{
    public Guid CustomerId { get; set; }

    public Guid VehicleId { get; set; }

    public DateTime AssignedAt { get; set; }

    public string? Note { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Vehicle Vehicle { get; set; } = null!;
}
