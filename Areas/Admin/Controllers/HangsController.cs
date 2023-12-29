﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using MyPhamCheilinus.Helpper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyPhamCheilinus.Models;
using PagedList.Core;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Runtime.InteropServices;

namespace MyPhamCheilinus.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Employee")]
    public class HangsController : Controller
    {
        private readonly _2023MyPhamContext _context;
        public INotyfService _notifyService { get; }

        public HangsController(_2023MyPhamContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        // GET: Admin/Hangs
        public IActionResult Index(int? page, string? TenHang = "")
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 10;

            IQueryable<Hang> query = _context.Hangs
                .AsNoTracking();

            if (!string.IsNullOrEmpty(TenHang))
            {
                query = query.Where(x => x.TenHang.Contains(TenHang));
            }

            var lsProducts = query.OrderByDescending(x => x.MaHang).ToList();
            PagedList<Hang> models = new PagedList<Hang>(lsProducts.AsQueryable(), pageNumber, pageSize);


           ViewBag.CurrentPage = pageNumber;
           ViewBag.CurrentSearch = TenHang;


            return View(models);
        }
        // GET: Admin/Hangs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Hangs == null)
            {
                return NotFound();
            }

            var hang = await _context.Hangs
                .FirstOrDefaultAsync(m => m.MaHang == id);
            if (hang == null)
            {
                return NotFound();
            }

            return View(hang);
        }

        public IActionResult Filtter( string? search)
        {
            var url = "/Admin/Hangs?";

            if (!string.IsNullOrEmpty(search))
            {
                url += $"TenHang={search}&";
            }

            // Loại bỏ dấu '&' cuối cùng nếu có
            if (url.EndsWith("&"))
            {
                url = url.Substring(0, url.Length - 1);
            }

            return Json(new { status = "success", redirectUrl = url });
        }

        // GET: Admin/Hangs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Hangs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaHang,TenHang,XuatXu")] Hang hang)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hang);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hang);
        }

        // GET: Admin/Hangs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Hangs == null)
            {
                return NotFound();
            }

            var hang = await _context.Hangs.FindAsync(id);
            if (hang == null)
            {
                return NotFound();
            }
            return View(hang);
        }

        // POST: Admin/Hangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaHang,TenHang,XuatXu")] Hang hang)
        {
            if (id != hang.MaHang)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HangExists(hang.MaHang))
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
            return View(hang);
        }

        // GET: Admin/Hangs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Hangs == null)
            {
                return NotFound();
            }

            var hang = await _context.Hangs
                .FirstOrDefaultAsync(m => m.MaHang == id);
            if (hang == null)
            {
                return NotFound();
            }

            return View(hang);
        }

        // POST: Admin/Hangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Hangs == null)
            {
                return Problem("Entity set '_2023MyPhamContext.Hangs' is null.");
            }

            var hang = await _context.Hangs.FindAsync(id);

            if (hang == null)
            {
                return NotFound();
            }

            // Xác nhận không có danh mục sản phẩm nào liên quan
            var danhMuc = await _context.DanhMucSanPhams.Where(p => p.MaHang == id).ToListAsync();

            // Xác nhận không có sản phẩm nào liên quan
            var products = _context.SanPhams
.AsEnumerable()
.Where(ct => danhMuc.Any(p => p.MaDanhMuc == ct.MaDanhMuc))
.ToList();
            // Xác nhận không có chi tiết đơn hàng nào liên quan
            var orderDetails = _context.ChiTietDonHangs
    .AsEnumerable()
    .Where(ct => products.Any(p => p.MaSanPham == ct.MaSanPham))
    .ToList();

            // Xóa chi tiết đơn hàng
            _context.RemoveRange(orderDetails);

            // Xóa sản phẩm
            _context.SanPhams.RemoveRange(products);
            _context.DanhMucSanPhams.RemoveRange(danhMuc);

            // Xóa Hãng sản phẩm
            _context.Hangs.Remove(hang);

            await _context.SaveChangesAsync();
            _notifyService.Success("Xóa hãng sản phẩm thành công");
            return RedirectToAction(nameof(System.Index));
        }


        private bool HangExists(string id)
    {
          return (_context.Hangs?.Any(e => e.MaHang == id)).GetValueOrDefault();
        }
    }
}
