using System;
using System.Collections.Generic;

namespace QuanLyBaiGiuXe.Data.Entities;

public partial class InvoiceLine
{
    public Guid LineId { get; set; }

    public Guid InvoiceId { get; set; }

    public string Description { get; set; } = null!;

    public decimal Qty { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal? LineAmount { get; set; }

    public virtual Invoice Invoice { get; set; } = null!;
}
