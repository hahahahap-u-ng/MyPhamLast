﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Helpers;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyPhamCheilinus.ChangPassword;
using MyPhamCheilinus.Extension;
using MyPhamCheilinus.Helpper;
using MyPhamCheilinus.Models;
using Newtonsoft.Json.Linq;
using PagedList.Core;
using static System.Net.Mime.MediaTypeNames;

namespace MyPhamCheilinus.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]
    public class AccountsController : Controller
    {
        private readonly _2023MyPhamContext _context;
        public INotyfService _notifyService { get; }

        public AccountsController(_2023MyPhamContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        // GET: Admin/Accounts
        //public async Task<IActionResult> Index()
        //{

        //    ViewData["QuyenTruyCap"] = new SelectList(_context.Roles, "RoleId", "Description");

        //    List<SelectListItem> lsTrangThai = new List<SelectListItem>();
        //    lsTrangThai.Add(new SelectListItem() { Text = "Active", Value = "1" });
        //    lsTrangThai.Add(new SelectListItem() { Text = "Block", Value = "0" });
        //    ViewData["lsTrangThai"] = lsTrangThai;


        //    var _2023MyPhamContext = _context.Accounts.Include(a => a.Role);
        //    return View(await _2023MyPhamContext.ToListAsync());
        //}
        public IActionResult Index(int? page, string? Ten = "", string? Email = "", string? SDT="", int? Quyen= null, Boolean? TrangThai=null)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 10;

            IQueryable<Account> query = _context.Accounts
                .Include(a=>a.Role)
                .AsNoTracking();
            if (!string.IsNullOrEmpty(Ten))
            {
                query = query.Where(x => x.FullName.Contains(Ten));
            }
            if (!string.IsNullOrEmpty(Email))
            {
                query = query.Where(x => x.AccountEmail.Contains(Email));
            }
            if (Quyen != null)
            {
                query = query.Where(x=>x.RoleId == Quyen);
            }
           
            if (!string.IsNullOrEmpty(SDT))
            {
                query = query.Where(x => x.Phone.Contains(SDT));
            }
            if (!string.IsNullOrEmpty(SDT))
            {
                query = query.Where(x => x.Phone.Contains(SDT));
            }
            if (TrangThai != null)
            {
                query = query.Where(x => x.Active == TrangThai);
            }
            var lsProducts = query.OrderByDescending(x => x.RoleId).ToList();

            PagedList<Account> models = new PagedList<Account>(lsProducts.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            ViewBag.CurrentTen = Ten;
            ViewBag.CurrentEmail = Email;
            ViewBag.CurrentSDT = SDT;
            ViewBag.CurrentRole = Quyen;
            ViewBag.CurrentStatus = TrangThai;
            List<SelectListItem> lsTrangThai = new List<SelectListItem>();
            lsTrangThai.Add(new SelectListItem() { Text = "Active", Value = "true" });
            lsTrangThai.Add(new SelectListItem() { Text = "Block", Value = "false" });
            ViewData["lsTrangThai"] = lsTrangThai;
            ViewData["QuyenTruyCap"] = new SelectList(_context.Roles, "RoleId", "Description", Quyen);
            List<SelectListItem> lsGioiTinh = new List<SelectListItem>();
            lsGioiTinh.Add(new SelectListItem() { Text = "Nam", Value = "true" });
            lsGioiTinh.Add(new SelectListItem() { Text = "Nữ", Value = "false" });
            ViewData["lsGioiTinh"] = lsGioiTinh;
            return View(models);
        }


        public IActionResult Filtter(string? Ten, string? Email, string? SDT, int? Quyen, Boolean? TrangThai)
        {
            var url = "/Admin/Accounts?";
            if (!string.IsNullOrEmpty(Ten))
            {
                url += $"Ten={Ten}&";
            }

            if (!string.IsNullOrEmpty(Email))
            {
                url += $"Email={Email}&";
            }
            if (!string.IsNullOrEmpty(SDT))
            {
                url += $"SDT={SDT}&";
            }
            if (Quyen != null)
            {
                url += $"Quyen={Quyen}&";
            }
            if (TrangThai != null)
            {
                url += $"TrangThai={TrangThai}&";
            }
            // Loại bỏ dấu '&' cuối cùng nếu có
            if (url.EndsWith("&"))
            {
                url = url.Substring(0, url.Length - 1);
            }

            return Json(new { status = "success", redirectUrl = url });
        }

        // GET: Admin/Accounts/Details/5
        public async Task<IActionResult> Details(int? id, int? page, string? Ten, string? Email, string? SDT, int? Quyen, Boolean? TrangThai)
        {
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }
            ViewBag.CurrentPage = page;
            ViewBag.CurrentTen = Ten;
            ViewBag.CurrentEmail = Email;
            ViewBag.CurrentSDT = SDT;
            ViewBag.CurrentRole = Quyen;
            ViewBag.CurrentStatus = TrangThai;
            List<SelectListItem> lsTrangThai = new List<SelectListItem>();
            lsTrangThai.Add(new SelectListItem() { Text = "Active", Value = "true" });
            lsTrangThai.Add(new SelectListItem() { Text = "Block", Value = "false" });
            ViewData["lsTrangThai"] = lsTrangThai;
            List<SelectListItem> lsGioiTinh = new List<SelectListItem>();
            lsGioiTinh.Add(new SelectListItem() { Text = "Nam", Value = "true" });
            lsGioiTinh.Add(new SelectListItem() { Text = "Nữ", Value = "false" });
            ViewData["lsGioiTinh"] = lsGioiTinh;
            ViewData["QuyenTruyCap"] = new SelectList(_context.Roles, "RoleId", "Description", Quyen);
            return View(account);
        }
        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DetailsConfirmed(int? id, int? page, string? Ten, string? Email, string? SDT, int? Quyen, Boolean? TrangThai)
        {
            if (_context.Accounts == null)
            {
                return Problem("Entity set '_2023MyPhamContext.SanPhams' is null.");
            }
            // Sau khi xóa thành công, bạn có thể chuyển hướng trở lại trang chứa sản phẩm vừa xóa bằng cách truyền tham số `page`.
            // Nếu `page` không có giá trị, bạn có thể mặc định nó về một trang cụ thể (ví dụ: 1).
            return RedirectToAction("Index", new { page = page, Ten = Ten, Email = Email, SDT = SDT, Quyen = Quyen, TrangThai = TrangThai });

        }

        // GET: Admin/Accounts/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {

            List<SelectListItem> lsTrangThai = new List<SelectListItem>();
            lsTrangThai.Add(new SelectListItem() { Text = "Active", Value = "true" });
            lsTrangThai.Add(new SelectListItem() { Text = "Block", Value = "false" });
            ViewData["lsTrangThai"] = lsTrangThai;
            List<SelectListItem> lsGioiTinh = new List<SelectListItem>();
            lsGioiTinh.Add(new SelectListItem() { Text = "Nam", Value = "true" });
            lsGioiTinh.Add(new SelectListItem() { Text = "Nữ", Value = "false" });
            ViewData["lsGioiTinh"] = lsGioiTinh;
            ViewData["QuyenTruyCap"] = new SelectList(_context.Roles, "RoleId", "Description");
            return View();
        }

        // POST: Admin/Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int page,[Bind("AccountId,Phone,AccountEmail,AccountPassword,Sail,Active,FullName,RoleId,LastLogin,CreateDate")] Account account)
        {
            if (ModelState.IsValid)
            {
                if (_context.Accounts.Any(x => x.AccountEmail == account.AccountEmail))
                {
                    ModelState.AddModelError("AccountEmail", "Tại khoản đã tồn tại. Vui lòng chọn tên khác.");
                 
                    return View(account);
                }
                string salt = Utilities.GetRandomKey();
                account.Salt = salt;
                //Tạo ngẫu nhiên mật khẩu
                account.AccountPassword = (account.Phone + salt.Trim()).ToMD5();
                account.CreateDate = DateTime.Now;

                _context.Add(account);
                await _context.SaveChangesAsync();
                _notifyService.Success("Tạo mới tài khoản quản trị thành công");
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CurrentPage = page;
           
       
            return View(account);
        }
        //ChangePassword
        //public IActionResult ChangePassword1(int? id, int? page, string? Ten, string? Email, string? SDT, int? Quyen, Boolean? TrangThai)
        //{
        //    ViewData["QuyenTruyCap"] = new SelectList(_context.Roles, "RoleId", "Description");
        //    ViewBag.CurrentPage = page;
        //    ViewBag.CurrentTen = Ten;
        //    ViewBag.CurrentEmail = Email;
        //    ViewBag.CurrentSDT = SDT;
        //    ViewBag.CurrentRole = Quyen;
        //    ViewBag.CurrentStatus = TrangThai;
        //    List<SelectListItem> lsTrangThai = new List<SelectListItem>();
        //    lsTrangThai.Add(new SelectListItem() { Text = "Active", Value = "true" });
        //    lsTrangThai.Add(new SelectListItem() { Text = "Block", Value = "false" });
        //    List<SelectListItem> lsGioiTinh = new List<SelectListItem>();
        //    lsGioiTinh.Add(new SelectListItem() { Text = "Nam", Value = "true" });
        //    lsGioiTinh.Add(new SelectListItem() { Text = "Nữ", Value = "false" });
        //    ViewData["lsGioiTinh"] = lsGioiTinh;
        //    ViewData["lsTrangThai"] = lsTrangThai;
        //    ViewData["QuyenTruyCap"] = new SelectList(_context.Roles, "RoleId", "Description", Quyen);
        //    return View();
        //}
        //[HttpPost]
        //public IActionResult ChangePassword1(ChangePasswordViewModel1 model, int? id, int? page, string? Ten, string? Email, string? SDT, int? Quyen, Boolean? TrangThai)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var taikhoan = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.AccountEmail == model.Email);
        //        if (taikhoan == null) return RedirectToAction("Login", "Accounts");

        //        var pass = (model.PasswordNow.Trim() + taikhoan.Salt.Trim()).ToMD5();
        //        if (pass == taikhoan.AccountPassword)
        //        {
        //            string passnew = (model.Password.Trim() + taikhoan.Salt.Trim()).ToMD5();
        //            taikhoan.AccountPassword = passnew;
        //            taikhoan.LastLogin = DateTime.Now;
        //            _context.Update(taikhoan);
        //            _context.SaveChanges();
        //            _notifyService.Success("Thay đổi mật khẩu thành công");
        //            RedirectToAction("Login", "Accounts", new {Area = "Admin"});
        //        }
        //    }

          
        
        //    return View();
        //}

        // GET: Admin/Accounts/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, int? page, string? Ten, string? Email, string? SDT, int? Quyen, Boolean? TrangThai)
        {
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            ViewData["QuyenTruyCap"] = new SelectList(_context.Roles, "RoleId", "Description", account.RoleId);
            ViewBag.CurrentPage = page;
            ViewBag.CurrentTen = Ten;
            ViewBag.CurrentEmail = Email;
            ViewBag.CurrentSDT = SDT;
            ViewBag.CurrentRole = Quyen;
            ViewBag.CurrentStatus = TrangThai;
            List<SelectListItem> lsTrangThai = new List<SelectListItem>();
            lsTrangThai.Add(new SelectListItem() { Text = "Active", Value = "true" });
            lsTrangThai.Add(new SelectListItem() { Text = "Block", Value = "false" });
            ViewData["lsTrangThai"] = lsTrangThai;
            List<SelectListItem> lsGioiTinh = new List<SelectListItem>();
            lsGioiTinh.Add(new SelectListItem() { Text = "Nam", Value = "true" });
            lsGioiTinh.Add(new SelectListItem() { Text = "Nữ", Value = "false" });
            ViewData["lsGioiTinh"] = lsGioiTinh;
            return View(account);
        }
    
        // POST: Admin/Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int? page, string? Ten, string? Email, string? SDT, int? Quyen, Boolean? TrangThai, [Bind("AccountId,Phone,AccountEmail,AccountPassword,Salt,Active,FullName,RoleId,LastLogin,CreateDate,NgaySinh,AnhDaiDien,DiaChi,GioiTinh")] Account account)
        {
            if (id != account.AccountId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    await _context.SaveChangesAsync();

                    // Kiểm tra xem tài khoản đang được chỉnh sửa có phải là tài khoản hiện tại không
                    if (id == int.Parse(HttpContext.User.FindFirst("AccountId")?.Value))
                    {
                        // Đăng xuất
                        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                        // Thêm lại các claim mới tương ứng với quyền truy cập mới của tài khoản
                        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, account.FullName),
        new Claim("AccountId", account.AccountId.ToString())
    };

                        if (account.RoleId == GetRoleIdForAdmin())
                        {
                            claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                        }
                        if (account.RoleId == GetRoleIdForCustomer())
                        {
                            claims.Add(new Claim(ClaimTypes.Role, "Customer"));
                        }
                        if (account.RoleId == GetRoleIdForEmployee())
                        {
                            claims.Add(new Claim(ClaimTypes.Role, "Employee"));
                        }

                        // Đăng nhập lại
                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.AccountId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", new { page = page, Ten = Ten, Email = Email, SDT = SDT, Quyen = Quyen, TrangThai = TrangThai });
            }
            ViewData["QuyenTruyCap"] = new SelectList(_context.Roles, "RoleId", "Description", account.RoleId);
            return View(account);
        }

        private int GetRoleIdForCustomer()
        {
          
            return _context.Roles.Where(r => r.RoleName == "Customer").Select(r => r.RoleId).FirstOrDefault();
        }
        private int GetRoleIdForAdmin()
        {
          
            return _context.Roles.Where(r => r.RoleName == "Admin").Select(r => r.RoleId).FirstOrDefault();
        }
        private int GetRoleIdForEmployee()
        {
          
            return _context.Roles.Where(r => r.RoleName == "Employee").Select(r => r.RoleId).FirstOrDefault();
        }
      
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id, int? page, string? Ten, string? Email, string? SDT, int? Quyen, Boolean? TrangThai)
        {

            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }
            var currentAccountId = int.Parse(HttpContext.User.FindFirst("AccountId")?.Value);
            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }
       
                if (currentAccountId == account.AccountId)
                {
                    _notifyService.Error("Không thể xóa tài khoản này!");
                    return RedirectToAction("Index", new { page = page, Ten = Ten, Email = Email, SDT = SDT, Quyen = Quyen, TrangThai = TrangThai });
                }
            ViewBag.CurrentPage = page;
            ViewBag.CurrentTen = Ten;
            ViewBag.CurrentEmail = Email;
            ViewBag.CurrentSDT = SDT;
            ViewBag.CurrentRole = Quyen;
            ViewBag.CurrentStatus = TrangThai;
            List<SelectListItem> lsTrangThai = new List<SelectListItem>();
            lsTrangThai.Add(new SelectListItem() { Text = "Active", Value = "true" });
            lsTrangThai.Add(new SelectListItem() { Text = "Block", Value = "false" });
            ViewData["lsTrangThai"] = lsTrangThai;
            ViewData["QuyenTruyCap"] = new SelectList(_context.Roles, "RoleId", "Description", Quyen);
            List<SelectListItem> lsGioiTinh = new List<SelectListItem>();
            lsGioiTinh.Add(new SelectListItem() { Text = "Nam", Value = "true" });
            lsGioiTinh.Add(new SelectListItem() { Text = "Nữ", Value = "false" });
            ViewData["lsGioiTinh"] = lsGioiTinh;
            return View(account);
        }

        // POST: Admin/Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id, int? page, string? Ten, string? Email, string? SDT, int? Quyen, Boolean? TrangThai)
        {
            if (_context.Accounts == null)
            {
                return Problem("Entity set '_2023MyPhamContext.Accounts'  is null.");
            }
            var account = await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                _context.Accounts.Remove(account);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { page = page, Ten = Ten, Email = Email, SDT = SDT, Quyen = Quyen, TrangThai = TrangThai });
        }

        private bool AccountExists(int id)
        {
          return (_context.Accounts?.Any(e => e.AccountId == id)).GetValueOrDefault();
        }
        public IActionResult ChangePasswor()
        {
            return PartialView("_ChangePassword");
        }
        [HttpPost]
        public IActionResult ChangePasswor(ChangePasswordViewModel1 model)
        {
            try
            {
                var taikhoanID = HttpContext.Session.GetString("AccountId");
                if (taikhoanID == null)
                {
                    return RedirectToAction("Login", "Accounts");
                }
                if (ModelState.IsValid)
                {
                    var taikhoan = _context.Accounts.Find(Convert.ToInt32(taikhoanID));
                    if (taikhoan == null) return RedirectToAction("Login", "Accounts");
                    var pass = (model.PasswordNow.Trim() + taikhoan.Salt.Trim()).ToMD5();
                    if (pass == taikhoan.AccountPassword)
                    {
                        string passnew = (model.Password.Trim() + taikhoan.Salt.Trim()).ToMD5();
                        taikhoan.AccountPassword = passnew;
                        _context.Update(taikhoan);
                        _context.SaveChanges();
                        _notifyService.Success("Thay đổi mật khẩu thành công!");
                        return RedirectToAction("ChangePasswor", "Accounts");
                    }
                    else
                    {
                        _notifyService.Error("Sai mật khẩu");
                        return RedirectToAction("ChangePasswor", "Accounts");
                    }
                }
            }
            catch
            {
                _notifyService.Success("Thay đổi không mật khẩu thành công");
                return RedirectToAction("ChangePasswor", "Accounts");
            }
            _notifyService.Error("Mật khẩu mới không giống nhau");
            return RedirectToAction("ChangePasswor", "Accounts");

        }
    }
}
