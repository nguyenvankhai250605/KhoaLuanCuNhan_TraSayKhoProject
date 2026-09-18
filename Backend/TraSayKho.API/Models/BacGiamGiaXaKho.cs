using System;
using System.Collections.Generic;

namespace TraSayKho.API.Models;

public partial class BacGiamGiaXaKho
{
    public int BacGiamGiaId { get; set; }

    public int? DanhMucId { get; set; }

    public string TenBac { get; set; } = null!;

    public decimal PhanTramThoiGianConLaiToiDa { get; set; }

    public decimal MucGiamGiaPhanTram { get; set; }

    public bool TrangThai { get; set; }

    public virtual DanhMuc? DanhMuc { get; set; }
}
