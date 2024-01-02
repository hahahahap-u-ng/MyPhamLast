using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyPhamCheilinus.Helpper;
using MyPhamCheilinus.Models;
using MyPhamCheilinus.ModelViews;
using PagedList.Core;


namespace MyPhamCheilinus.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoHangsController : Controller
    {
        private readonly _2023MyPhamContext _context;

        public INotyfService _notifyService { get; }

        public LoHangsController(_2023MyPhamContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        // GET: Admin/LoHangs

        public IActionResult Index(int? page, string? MaID = null, string search = "", double? minPrice = null, double? maxPrice = null)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 10;

            IQueryable<LoHang> query = _context.LoHangs
                .AsNoTracking().Include(dh => dh.ChiTietLoHangs).ThenInclude(c => c.MaSanPhamNavigation)
                  .Include(x => x.MaNhaPpNavigation);


            if (!string.IsNullOrEmpty(MaID))
            {
                query = query.Where(x => x.MaLoHang.Contains(MaID));
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.MaNhaPpNavigation.TenNhaPp.Contains(search));
            }

            //if (minPrice != null)
            //{
            //    query = query.Where(x => x.NgayNhan >= minPrice);
            //}

            //if (maxPrice != null)
            //{
            //    query = query.Where(x => x.TongTien <= maxPrice);
            //}



            var lsDonHangs = query.OrderByDescending(x => x.MaLoHang).ToList();

            PagedList<LoHang> models = new PagedList<LoHang>(lsDonHangs.AsQueryable(), pageNumber, pageSize);

            ViewBag.CurrentMaID = MaID;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentMinPrice = minPrice;
            ViewBag.CurrentMaxPrice = maxPrice;

            //ViewData["KhachHang"] = new SelectList(_context.DanhMucSanPhams, "MaKhachHang", "TenKhachHang", MaKH);

            return View(models);
        }

        // GET: Admin/LoHangs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.LoHangs == null)
            {
                return NotFound();
            }

            var loHang = await _context.LoHangs
                .AsNoTracking().Include(dh => dh.ChiTietLoHangs).ThenInclude(c => c.MaSanPhamNavigation)
                  .Include(x => x.MaNhaPpNavigation)
                .FirstOrDefaultAsync(m => m.MaLoHang == id);
            if (loHang == null)
            {
                return NotFound();
            }
            var lohang = _context.LoHangs
               .AsNoTracking().Include(dh => dh.ChiTietLoHangs).ThenInclude(c => c.MaSanPhamNavigation)
                 .Include(x => x.MaNhaPpNavigation)
               .Where(x => x.MaLoHang == loHang.MaLoHang)
                .OrderBy(x => x.MaLoHang).ToList();
            ViewBag.ChiTiet = lohang;

            return View(loHang);
        }
        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DetailsConfirmed(string id, int? page, string? MaID, string? search, double? minPrice, double? maxPrice)
        {
            if (_context.LoHangs == null)
            {
                return Problem("Entity set '_2023MyPhamContext.LoHangs' is null.");
            }
            // Sau khi xóa thành công, bạn có thể chuyển hướng trở lại trang chứa sản phẩm vừa xóa bằng cách truyền tham số `page`.
            // Nếu `page` không có giá trị, bạn có thể mặc định nó về một trang cụ thể (ví dụ: 1).
            return RedirectToAction("Index", new { page = page, MaID = MaID, search = search, minPrice = minPrice, maxPrice = maxPrice });

        }
        // GET: Admin/LoHangs/Create
        public IActionResult NhapKho(string id, float? Gia)
        {
            ViewBag.CurrentGia = Gia;
            ViewBag.CurrentMaSP = id;
            ViewData["MaNhaPp"] = new SelectList(_context.NhaPhanPhois, "MaNhaPp", "TenNhaPp");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> NhapKho(Kho nhapKho)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string salt = Utilities.GetRandomKey();
                    LoHang khachhang = new LoHang
                    {
                        MaLoHang = nhapKho.MaLoHang,
                        MaNhaPp = nhapKho.TenNhaPP,
                        NgayNhan = DateTime.Now,
                        GiaLo = nhapKho.GiaLo * nhapKho.SoLuong,
                        SoLuong = nhapKho.SoLuong

                    };
                    ChiTietLoHang loHang = new ChiTietLoHang
                    {
                        MaSanPham = nhapKho.TenSanPham,
                        MaLoHang = nhapKho.MaLoHang,

                    };

                    _context.Add(loHang);
                    var sanPham = _context.SanPhams.Find(nhapKho.TenSanPham);
                    if (sanPham != null)
                    {
                        // Trừ số lượng đã mua từ số lượng tồn kho
                        sanPham.Slkho += nhapKho.SoLuong;


                        // Cập nhật giá trị mới của số lượng tồn kho trong cơ sở dữ liệu
                        _context.Update(sanPham);

                    }
                    try
                    {

                        _context.Add(khachhang);
                        await _context.SaveChangesAsync();
                        _notifyService.Success("Thêm lô hàng thành công!");
                        return RedirectToAction("Index", "SanPhams");
                    }
                    catch (Exception ex)
                    {
                        return RedirectToAction("NhapKho", "SanPhams");
                    }
                }
                else
                {
                    return View(nhapKho);
                }
            }
            catch
            {
                return View(nhapKho);
            }
        }
        public IActionResult Create()
        {
            ViewData["MaNhaPp"] = new SelectList(_context.NhaPhanPhois, "MaNhaPp", "TenNhaPp");
            return View();
        }

        // POST: Admin/LoHangs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaLoHang,NgayNhan,MaNhaPp,GiaLo")] LoHang loHang)
        {
            if (ModelState.IsValid)
            {
                _context.Add(loHang);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaNhaPp"] = new SelectList(_context.NhaPhanPhois, "MaNhaPp", "MaNhaPp", loHang.MaNhaPp);
            return View(loHang);
        }

        // GET: Admin/LoHangs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.LoHangs == null)
            {
                return NotFound();
            }

            var loHang = await _context.LoHangs.FindAsync(id);
            if (loHang == null)
            {
                return NotFound();
            }
            ViewData["MaNhaPp"] = new SelectList(_context.NhaPhanPhois, "MaNhaPp", "MaNhaPp", loHang.MaNhaPp);
            return View(loHang);
        }

        // POST: Admin/LoHangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaLoHang,NgayNhan,MaNhaPp,GiaLo")] LoHang loHang)
        {
            if (id != loHang.MaLoHang)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loHang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoHangExists(loHang.MaLoHang))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaNhaPp"] = new SelectList(_context.NhaPhanPhois, "MaNhaPp", "MaNhaPp", loHang.MaNhaPp);
            return View(loHang);
        }

        // GET: Admin/LoHangs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.LoHangs == null)
            {
                return NotFound();
            }

            var loHang = await _context.LoHangs
                .Include(l => l.MaNhaPpNavigation)
                .FirstOrDefaultAsync(m => m.MaLoHang == id);
            if (loHang == null)
            {
                return NotFound();
            }

            return View(loHang);
        }

        // POST: Admin/LoHangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.LoHangs == null)
            {
                return Problem("Entity set '_2023MyPhamContext.LoHangs'  is null.");
            }
            var loHang = await _context.LoHangs.FindAsync(id);
            if (loHang != null)
            {
                _context.LoHangs.Remove(loHang);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoHangExists(string id)
        {
            return (_context.LoHangs?.Any(e => e.MaLoHang == id)).GetValueOrDefault();
        }
    }
}
