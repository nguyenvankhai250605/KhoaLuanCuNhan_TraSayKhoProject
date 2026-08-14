using System;
using System.Collections.Generic;

namespace TraSayKho.API.Models;

public partial class VaiTro
{
    public int VaiTroId { get; set; }

    public string TenVaiTro { get; set; } = null!;

    public virtual ICollection<TaiKhoan> TaiKhoans { get; set; } = new List<TaiKhoan>();
}
