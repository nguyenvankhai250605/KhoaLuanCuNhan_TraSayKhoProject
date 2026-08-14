using System;
using System.Collections.Generic;

namespace TraSayKho.API.Models;

public partial class CuocHoiThoai
{
    public int CuocHoiThoaiId { get; set; }

    public int KhachHangId { get; set; }

    public DateTime NgayBatDau { get; set; }

    public string TrangThai { get; set; } = null!;

    public virtual KhachHang KhachHang { get; set; } = null!;

    public virtual ICollection<TinNhan> TinNhans { get; set; } = new List<TinNhan>();
}
