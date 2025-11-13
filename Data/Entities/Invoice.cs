using System;
using System.Collections.Generic;

namespace QuanLyBaiGiuXe.Data.Entities;

public partial class Invoice
{
    public Guid InvoiceId { get; set; }

    public string InvoiceNo { get; set; } = null!;

    public Guid? CustomerId { get; set; }

    public Guid? SessionId { get; set; }

    public Guid? SubscriptionId { get; set; }

    public decimal TotalAmount { get; set; }

    public byte Status { get; set; }

    public DateTime IssuedAt { get; set; }

    public Guid IssuedBy { get; set; }

    public string? Note { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();

    public virtual User IssuedByNavigation { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ParkingSession? Session { get; set; }

    public virtual Subscription? Subscription { get; set; }
}
