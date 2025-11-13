using System;
using System.Collections.Generic;

namespace QuanLyBaiGiuXe.Data.Entities;

public partial class Payment
{
    public Guid PaymentId { get; set; }

    public Guid InvoiceId { get; set; }

    public Guid MethodId { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaidAt { get; set; }

    public byte Status { get; set; }

    public virtual Invoice Invoice { get; set; } = null!;

    public virtual PaymentMethod Method { get; set; } = null!;
}
