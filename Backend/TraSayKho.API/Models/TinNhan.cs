using System;
using System.Collections.Generic;

namespace TraSayKho.API.Models;

public partial class TinNhan
{
    public int TinNhanId { get; set; }

    public int CuocHoiThoaiId { get; set; }

    public string NguoiGui { get; set; } = null!;

    public string NoiDung { get; set; } = null!;

    public DateTime ThoiGianGui { get; set; }

    public virtual CuocHoiThoai CuocHoiThoai { get; set; } = null!;
}
