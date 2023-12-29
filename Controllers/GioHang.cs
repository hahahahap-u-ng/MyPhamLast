using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyPhamCheilinus.Controllers;
using MyPhamCheilinus.Infrastructure;
using MyPhamCheilinus.Models;
using System.Data;
using System.Net;


namespace MyPhamCpuheilinus.Controllers
{
    public class GioHangController : Controller
    {

        public GioHang? GioHang { get; set; }
        _2023MyPhamContext db = new _2023MyPhamContext();
        public INotyfService _notifyService { get; }
        private readonly ILogger<GioHangController> _logger;

        public GioHangController(ILogger<GioHangController> logger, INotyfService notifyService)
        {
            _logger = logger;
            _notifyService = notifyService;
        }
        [Authorize(Roles = "Customer")]
        public IActionResult AddGioHang(string maSanPham)
        {
            SanPham? sanpham = db.SanPhams.FirstOrDefault(p => p.MaSanPham == maSanPham);
            if (sanpham.Slkho == 0)
            {
                _notifyService.Error("Sản phẩm đã hết hàng");
                return RedirectToAction("SanPhamTheoDanhMuc");
            }

            if (sanpham != null)
            {
                GioHang = HttpContext.Session.GetJson<GioHang>("giohang") ?? new GioHang();
                GioHang.AddItem(sanpham, 1);
                HttpContext.Session.SetJson("giohang", GioHang);
                TempData["ThongBao"] = "Sản phẩm đã được thêm vào giỏ hàng";
            }
            return View("GioHang", GioHang);
        }
        public IActionResult Remove_1_FromGioHang(string maSanPham)
        {
            SanPham? sanpham = db.SanPhams.FirstOrDefault(p => p.MaSanPham == maSanPham);
            if (sanpham != null)
            {
                GioHang = HttpContext.Session.GetJson<GioHang>("giohang") ?? new GioHang();
                GioHang.AddItem(sanpham, -1);
                HttpContext.Session.SetJson("giohang", GioHang);
            }
            return View("GioHang", GioHang);
        }

        public IActionResult RemoveFromGioHang(string maSanPham)
        {
            SanPham? sanpham = db.SanPhams.FirstOrDefault(p => p.MaSanPham == maSanPham);
            if (sanpham != null)
            {
                GioHang = HttpContext.Session.GetJson<GioHang>("giohang");
                GioHang.RemoveLine(sanpham);
                HttpContext.Session.SetJson("giohang", GioHang);
            }
            return View("GioHang", GioHang);
        }

        public IActionResult CheckOut()
        {
            // Lấy AccountId từ session
            var taikhoanID = HttpContext.Session.GetString("AccountId");

            // Kiểm tra đăng nhập
            if (string.IsNullOrEmpty(taikhoanID))
            {
                // Nếu chưa đăng nhập, có thể chuyển hướng người dùng đến trang đăng nhập
                return RedirectToAction("Login", "Accounts");
            }

            if (int.TryParse(taikhoanID, out var accountId))
            {
                // Truy vấn danh sách KhachHang thuộc Account có AccountId tương ứng
                var danhSachKhachHang = db.KhachHangs
                    .Where(kh => kh.AccountId == accountId)
                    .ToList();

                // Lấy thông tin giỏ hàng từ session
                var gioHang = HttpContext.Session.GetJson<MyPhamCheilinus.Models.GioHang>("giohang") ?? new MyPhamCheilinus.Models.GioHang();

                // Kết hợp danh sách KhachHang và thông tin giỏ hàng
                var model = new Tuple<List<MyPhamCheilinus.Models.KhachHang>, MyPhamCheilinus.Models.GioHang>(danhSachKhachHang, gioHang);

                // Trả về View với model chứa cả danh sách KhachHang và thông tin giỏ hàng
                return View(model);
            }

            // Nếu có lỗi xử lý, có thể chuyển hướng người dùng đến trang lỗi hoặc trang mặc định
            return RedirectToAction("Login", "Accounts");
        }
        public IActionResult ViewGioHang()
        {
            var gioHang = HttpContext.Session.GetJson<GioHang>("giohang") ?? new GioHang();
            return View(gioHang);
        }
        private int GenerateUniqueCustomerCode()
        {
            Random random = new Random();
            int randomNumber = random.Next(10000, 99999); // Sinh ra số ngẫu nhiên từ 10,000 đến 99,999

            return randomNumber;
        }
        private string GenerateUniqueOrderCode()
        {
            Random random = new Random();
            int randomNumber = random.Next(10000, 99999); // Sinh ra số ngẫu nhiên từ 10,000 đến 99,999
            string customerCode = "DH" + randomNumber.ToString();
            return customerCode;
        }

        //   public ActionResult ThanhToan(string maKhachHang, string hoTen, string soDienThoai, string diaChi, string email)
        //   {
        //       var taikhoanID = HttpContext.Session.GetString("AccountId");
        //       var khachhang = new KhachHang
        //       {
        //           //MaKhachHang = GenerateUniqueCustomerCode(),
        //           TenKhachHang = hoTen,
        //           DiaChi = diaChi,
        //           SoDienThoai = soDienThoai,
        //           AccountId= Convert.ToInt32(taikhoanID)
        //       };
        //       db.KhachHangs.Add(khachhang);
        //       db.SaveChanges();

        //       var gioHang = HttpContext.Session.GetJson<GioHang>("giohang");
        //       var donHang = new DonHang
        //       {
        //           MaDonHang = GenerateUniqueOrderCode(),
        //           NgayDatHang = DateTime.Now,
        //           TongTien = gioHang.ComputeTotalValues(),
        //           TrangThaiDonHang = 1,
        //           MaKhachHang = khachhang.MaKhachHang,
        //           ThanhToan = true,
        //           VanChuyen = 1,
        //           PhiVanChuyen = 10000
        //       };
        //       db.DonHangs.Add(donHang);
        //       db.SaveChanges();
        //       foreach (var line in gioHang.Lines)
        //       {
        //           var chiTietDonHang = new ChiTietDonHang
        //           {
        //               MaDonHang = donHang.MaDonHang,
        //               MaSanPham = line.SanPham.MaSanPham,
        //               SoLuong = line.SoLuong,
        //               GiaBan = line.SanPham.Gia
        //           };
        //           db.ChiTietDonHangs.Add(chiTietDonHang);
        //           RemoveFromGioHang(line.SanPham.MaSanPham);
        //       }

        //       db.SaveChanges();

        //       foreach (var line in gioHang.Lines)
        //       {
        //           ;
        //           RemoveFromGioHang(line.SanPham.MaSanPham);
        //       }
        //;

        //       gioHang.Clear();


        //       return View("Success");
        //   }






        [HttpPost]
        public IActionResult SaveAddress(string hoTen, string diaChi, string soDienThoai, string email)
        {
            try
            {
                var taikhoanID = HttpContext.Session.GetString("AccountId");

                // Kiểm tra và chuyển đổi taikhoanID thành int
                if (!int.TryParse(taikhoanID, out int accountId))
                {
                    return Json(new { success = false, message = "AccountId không hợp lệ" });
                }

                // Tạo đối tượng KhachHang
                var khachhang = new KhachHang
                {
                    //MaKhachHang = GenerateUniqueCustomerCode(), // Cần triển khai hàm này
                    TenKhachHang = hoTen,
                    DiaChi = diaChi,
                    SoDienThoai = soDienThoai,
                    AccountId = accountId
                };

                // Lưu đối tượng vào cơ sở dữ liệu
                db.KhachHangs.Add(khachhang);
                db.SaveChanges();

                return RedirectToAction("CheckOut");
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
        [HttpPost]
        public ActionResult ThanhToan(int selectedKhachHang)
        {
            try
            {
                // Lấy thông tin của KhachHang đã chọn
                var selectedKhachHangObject = db.KhachHangs.Find(selectedKhachHang);

                if (selectedKhachHangObject != null)
                {
                    // Thực hiện các bước thanh toán với selectedKhachHangObject
                    var gioHang = HttpContext.Session.GetJson<GioHang>("giohang");
                    var donHang = new DonHang
                    {
                        MaDonHang = GenerateUniqueOrderCode(),
                        NgayDatHang = DateTime.Now,
                        TongTien = gioHang.ComputeTotalValues(),
                        TrangThaiDonHang = 1,
                        MaKhachHang = selectedKhachHangObject.MaKhachHang,
                        ThanhToan = true,
                        VanChuyen = 1,
                        PhiVanChuyen = 10000
                    };

                    db.DonHangs.Add(donHang);
                    db.SaveChanges();
                    foreach (var line in gioHang.Lines)
                    {
                        // Lấy thông tin sản phẩm từ cơ sở dữ liệu
                        var sanPham = db.SanPhams.Find(line.SanPham.MaSanPham);

                        if (sanPham != null)
                        {
                            // Trừ số lượng đã mua từ số lượng tồn kho
                            sanPham.Slkho -= line.SoLuong;


                            // Cập nhật giá trị mới của số lượng tồn kho trong cơ sở dữ liệu
                            db.Update(sanPham);

                        }
                    }
                    foreach (var line in gioHang.Lines)
                    {
                        var chiTietDonHang = new ChiTietDonHang
                        {
                            MaDonHang = donHang.MaDonHang,
                            MaSanPham = line.SanPham.MaSanPham,
                            SoLuong = line.SoLuong,
                            GiaBan = line.SanPham.Gia
                        };
                        db.ChiTietDonHangs.Add(chiTietDonHang);
                        RemoveFromGioHang(line.SanPham.MaSanPham);
                    }


                    db.SaveChanges();
                    foreach (var line in gioHang.Lines)
                    {

                        RemoveFromGioHang(line.SanPham.MaSanPham);
                    }
     ;
                    gioHang.Clear();

                    // Có thể thêm các xử lý khác nếu cần
                    // ...

                    TempData["SuccessMessage"] = "Đã thanh toán thành công.";

                    return View("Success");
                }
                else
                {
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin Khách Hàng đã chọn.";
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                TempData["ErrorMessage"] = $"Đã xảy ra lỗi trong quá trình thanh toán: {ex.Message}";
            }

            // Chuyển hướng về CheckOut hoặc trang khác tùy thuộc vào yêu cầu của bạn
            return RedirectToAction("CheckOut");
        }

    }
}
