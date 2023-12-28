using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyPhamCheilinus.Models;
using System.Diagnostics;
using System.Web;
using X.PagedList;
using static MyPhamCheilinus.Controllers.HomeController;

namespace MyPhamCheilinus.Controllers
{
  

    public class HomeController : Controller
    {
        _2023MyPhamContext db = new _2023MyPhamContext();
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {

            var listDanhMucSanPham = db.DanhMucSanPhams.ToList();
            return View(listDanhMucSanPham);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //public async Task<IActionResult> DanhMucSanPham(int? page, string sortOrder)
        //{
        //    // Số sản phẩm trên mỗi trang
        //    int pageSize = 6;

        //    var danhMucSanPhams = db.DanhMucSanPhams.ToList();
        //    // Sắp xếp danh sách nếu sortOrder được chỉ định
        //    if (!string.IsNullOrEmpty(sortOrder))
        //    {
        //        switch (sortOrder)
        //        {
        //            case "average_rating":
        //                danhMucSanPhams = danhMucSanPhams.OrderBy(s => s.DanhGia).ToList();
        //                break;
        //            ;
        //            case "price_low_high":
        //                danhMucSanPhams = danhMucSanPhams.OrderBy(s => s.Gia).ToList();
        //                break;
        //            case "price_high_low":
        //                danhMucSanPhams = danhMucSanPhams.OrderByDescending(s => s.Gia).ToList();
        //                break;
        //            case "name_z":
        //                danhMucSanPhams = danhMucSanPhams.OrderByDescending(s => s.TenDanhMuc).ToList();
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //    // Sử dụng thư viện PagedList để phân trang
        //    IPagedList<DanhMucSanPham> pagedList = await danhMucSanPhams.ToPagedListAsync(page ?? 1, pageSize);

        //    return View(pagedList);
        //}

        //[HttpGet]
        //public IActionResult FilterByPriceAndTag(double minPrice, double maxPrice, string tag)
        //{
        //    // Bắt đầu với tất cả sản phẩm
        //    IQueryable<DanhMucSanPham> filteredSanPhams = db.DanhMucSanPhams;

        //    // Lọc sản phẩm dựa trên khoảng giá
        //    filteredSanPhams = filteredSanPhams.Where(p => p.Gia >= minPrice && p.Gia <= maxPrice);

        //    // Lọc sản phẩm dựa trên thẻ (tag)
        //    if (!string.IsNullOrEmpty(tag))
        //    {
        //        filteredSanPhams = filteredSanPhams.Where(p => p.TenDanhMuc.Contains(tag));
        //    }

        //    // Lấy danh sách sản phẩm đã lọc
        //    var filteredProducts = filteredSanPhams.ToList();

        //    // Trả về kết quả dưới dạng partial view
        //    return PartialView("_ReturnHangs", filteredProducts);
        //}





        public IActionResult DanhMucSanPham(int? page, List<string> selectedMaHangs = null, List<string> selectedMaCTLoais = null, string search = "", double? minPrice = null, double? maxPrice = null, string sortBy = "")
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 6;

            IQueryable<DanhMucSanPham> query = db.DanhMucSanPhams.AsQueryable();

            if (selectedMaCTLoais != null && selectedMaCTLoais.Any())
            {
                query = query.Where(x => selectedMaCTLoais.Contains(x.MaCtloai));
            }

            if (selectedMaHangs != null && selectedMaHangs.Any())
            {
                query = query.Where(x => selectedMaHangs.Contains(x.MaHang));
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.TenDanhMuc.Contains(search));
            }

            if (minPrice != null)
            {
                query = query.Where(x => x.Gia >= minPrice);
            }

            if (maxPrice != null)
            {
                query = query.Where(x => x.Gia <= maxPrice);
            }

            switch (sortBy)
            {
                case "price_asc":
                    query = query.OrderBy(x => x.Gia);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(x => x.Gia);
                    break;
                case "name_asc":
                    query = query.OrderBy(x => x.TenDanhMuc);
                    break;
                case "name_desc":
                    query = query.OrderByDescending(x => x.TenDanhMuc);
                    break;
                default:
                    query = query.OrderBy(x => x.MaDanhMuc);
                    break;
            }

            var lsDanhMuc = query.ToList();

            PagedList<DanhMucSanPham> models = new PagedList<DanhMucSanPham>(lsDanhMuc.AsQueryable(), pageNumber, pageSize);

            ViewBag.PageNumber = pageNumber;
            ViewBag.SelectedMaHangs = selectedMaHangs;
           
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentMinPrice = minPrice;
            ViewBag.CurrentMaxPrice = maxPrice;
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.ListHangs = db.Hangs.Select(x => x.MaHang).ToList();
            ViewBag.ListCTLoais = db.Ctloais.Select(x => x.MaCtloai).ToList();


            return View(models);
        }

        public IActionResult Filtter(string selectedMaHangs = "", string selectedMaCTLoai = "", string search = "", double? minPrice = null, double? maxPrice = null, string sortBy = "")
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            if (!string.IsNullOrEmpty(selectedMaHangs))
            {
                queryString["selectedMaHangs"] = selectedMaHangs;
            }

            if (!string.IsNullOrEmpty(selectedMaCTLoai))
            {
                queryString["selectedMaCTLoai"] = selectedMaCTLoai;
            }

            if (!string.IsNullOrEmpty(search))
            {
                queryString["search"] = search;
            }

            if (minPrice != null)
            {
                queryString["minPrice"] = minPrice.ToString();
            }

            if (maxPrice != null)
            {
                queryString["maxPrice"] = maxPrice.ToString();
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                queryString["sortBy"] = sortBy;
            }

            // Add page parameter to maintain pagination
            queryString["page"] = "1";

            var url = $"/DanhMucSanPham?{queryString}";

            return Json(new { status = "success", redirectUrl = url });
        }


        public async Task<IActionResult> SanPhamTheoHang(string mahang, int? page, string sortOrder)
        {
            // Số sản phẩm trên mỗi trang
            int pageSize = 9;

            // Lấy danh sách sản phẩm theo mã hàng
            var danhMucSanPhams = db.DanhMucSanPhams
                .Include(x => x.MaHangNavigation) // Include để nạp thông tin về hãng
                .Where(x => x.MaHang == mahang);

            // Lấy tên hãng tương ứng với mã hàng
            var tenHang = danhMucSanPhams.FirstOrDefault()?.MaHangNavigation?.TenHang;

            // Sử dụng thư viện PagedList để phân trang
            IPagedList<DanhMucSanPham> pagedList = await danhMucSanPhams.ToPagedListAsync(page ?? 1, pageSize);

            // Đặt giá trị "mahang" vào ViewBag để truyền sang view
            ViewBag.Mahang = mahang;

            // Đặt giá trị tên hãng vào ViewBag
            ViewBag.TenHang = tenHang;

            return View(pagedList);
        }


        public IActionResult SanPhamTheoDanhMuc(string maDanhMuc)
        {
            var danhMuc = db.DanhMucSanPhams.FirstOrDefault(d => d.MaDanhMuc == maDanhMuc);
            var danhMucList = db.DanhMucSanPhams.Where(d => d.MaCtloai == danhMuc.MaCtloai).ToList();

            if (danhMuc == null)
            {
                return NotFound(); // Xử lý trường hợp danh mục không tồn tại
            }

            var sanPhamList = db.SanPhams.Where(s => s.MaDanhMuc == maDanhMuc).ToList();

            // Lấy danh sách màu sắc duy nhất từ danh sách sản phẩm
            var mauSanPhamList = sanPhamList.Select(s => s.Mau).Distinct().ToList();

            ViewData["DanhMuc"] = danhMuc;
            ViewData["DanhMucList"] = danhMucList;
            ViewData["MauSanPhamList"] = mauSanPhamList; // Truyền danh sách màu vào view
            return View(sanPhamList);
        }

        




    }


}
