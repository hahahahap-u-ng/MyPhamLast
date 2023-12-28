﻿using System;
using System.Collections.Generic;

namespace MyPhamCheilinus.Models;

public partial class ChiTietLoHang
{
    public string MaLoHang { get; set; } = null!;

    public string MaSanPham { get; set; } = null!;

    public int? SoLuong { get; set; }

    public double? GiaNhap { get; set; }

    public virtual LoHang MaLoHangNavigation { get; set; } = null!;

    public virtual SanPham MaSanPhamNavigation { get; set; } = null!;
}
