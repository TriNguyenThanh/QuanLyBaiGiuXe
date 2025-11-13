using System;
using System.Collections.Generic;

namespace QuanLyBaiGiuXe.Data.Entities;

public partial class User
{
    public Guid UserId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string RoleName { get; set; } = null!;

    public byte Status { get; set; }

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<ParkingSession> ParkingSessionClosedByNavigations { get; set; } = new List<ParkingSession>();

    public virtual ICollection<ParkingSession> ParkingSessionOpenedByNavigations { get; set; } = new List<ParkingSession>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
