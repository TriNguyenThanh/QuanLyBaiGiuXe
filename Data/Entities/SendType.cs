using System;
using System.Collections.Generic;

namespace QuanLyBaiGiuXe.Data.Entities;

public partial class SendType
{
    public Guid SendTypeId { get; set; }

    public string SendName { get; set; } = null!;
}
