﻿using System;
using System.Collections.Generic;

namespace MyPhamCheilinus.Models;

public partial class LoHang
{
    public string MaLoHang { get; set; } = null!;

    public DateTime? NgayNhan { get; set; }

    public string? MaNhaPp { get; set; }

    public double? GiaLo { get; set; }

    public virtual ICollection<ChiTietLoHang> ChiTietLoHangs { get; set; } = new List<ChiTietLoHang>();

    public virtual NhaPhanPhoi? MaNhaPpNavigation { get; set; }
}
