using System.ComponentModel.DataAnnotations;
namespace MyPhamCheilinus.ModelViews
{
    public class Kho
    {
        [Key]
        [Display(Name = "Mã lô hàng")]
        [Required(ErrorMessage = "Vui lòng nhập mã lô hàng")]
        public string MaLoHang { get; set; }
        [Display(Name = "Tên nhà phân phối")]
        [Required(ErrorMessage = "Vui lòng nhập Nhà phân phối")]
        public string TenNhaPP { get; set; }
        [Display(Name = "Ngày nhận")]
        [Required(ErrorMessage = "Vui lòng nhập ngày nhận")]
        public DateTime NgayNhan { get; set; }
        [Display(Name = "Giá lô")]
        [Required(ErrorMessage = "Vui lòng nhập giá lô hàng")]
        public float GiaLo { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Số lượng sản phẩm")]
        public int SoLuong { get; set; }

        [Display(Name = "Tên sản phẩm")]
        [Required(ErrorMessage = "Vui lòng nhập sản phẩm")]
        public string TenSanPham { get; set; }
    }
}
