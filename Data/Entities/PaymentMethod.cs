using System;
using System.Collections.Generic;

namespace QuanLyBaiGiuXe.Data.Entities;

public partial class PaymentMethod
{
    public Guid MethodId { get; set; }

    public string MethodName { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
