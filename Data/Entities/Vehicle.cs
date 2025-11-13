using System;
using System.Collections.Generic;

namespace QuanLyBaiGiuXe.Data.Entities;

public partial class Vehicle
{
    public Guid VehicleId { get; set; }

    public Guid VehicleTypeId { get; set; }

    public string? LicensePlate { get; set; }

    public string? RfidTag { get; set; }

    public string? Brand { get; set; }

    public string? Color { get; set; }

    public byte Status { get; set; }

    public virtual CustomerVehicle? CustomerVehicle { get; set; }

    public virtual ICollection<ParkingSession> ParkingSessions { get; set; } = new List<ParkingSession>();

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    public virtual VehicleType VehicleType { get; set; } = null!;
}
