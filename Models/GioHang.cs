namespace MyPhamCheilinus.Models
{
    public class GioHang
    {
        public List<GioHangLine> Lines { get; set; }= new List<GioHangLine>();
        public void AddItem(SanPham sanpham, int soluong)
        {
            GioHangLine? line = Lines.Where(p=>p.SanPham.MaSanPham == sanpham.MaSanPham).FirstOrDefault();
            if (line == null)
            {
                Lines.Add(new GioHangLine
                {
                    SanPham = sanpham,
                    SoLuong = soluong
                });
            }
            else
            {
                line.SoLuong += soluong;
            }
        }

        public void RemoveLine(SanPham sanpham) =>
            Lines.RemoveAll(l => l.SanPham.MaSanPham == sanpham.MaSanPham);

        public double ComputeTotalValues() =>
            (double)Lines.Sum(e => e.SanPham?.Gia * e.SoLuong);
        public void Clear() => Lines.Clear();
    }


    public class GioHangLine
    {
        public int MaGioHangLine { get; set; }
        public SanPham SanPham { get; set; } = new();
        public int SoLuong { get; set; }
    }
}
