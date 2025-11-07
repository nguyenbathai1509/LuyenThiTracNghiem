using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LuyenThiTracNghiem.Models;
using LuyenThiTracNghiem.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using LuyenThiTracNghiem.Filters;

namespace LuyenThiTracNghiem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminOnly]
    public class UserController : Controller
    {
        private readonly DataContext _context;

        public UserController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 8)
        {
            var query = _context.Users.AsQueryable();

            int totalItems = await query.CountAsync();

            var users = await query
                .OrderBy(u => u.UserId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new PagedResult<tblUser>
            {
                Items = users,
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };

            return View(viewModel);
        }

        public async Task<IActionResult> LoadData(string searchTerm, string role, int page = 1, int pageSize = 8)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(u => u.FullName.Contains(searchTerm) || u.Username.Contains(searchTerm));

            if (!string.IsNullOrEmpty(role))
            {
                int r = int.Parse(role);
                query = query.Where(u => u.Role == r);
            }

            int totalItems = await query.CountAsync();

            var users = await query
                .OrderBy(u => u.UserId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new PagedResult<tblUser>
            {
                Items = users,
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };

            return PartialView("_UserListPartial", model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(tblUser model, IFormFile? AvatarFile)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Vui lòng kiểm tra lại thông tin nhập vào.";
                return View(model);
            }

            bool usernameExists = await _context.Users.AnyAsync(u => u.Username == model.Username);
            if (usernameExists)
            {
                ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại.");
                return View(model);
            }

            if (!string.IsNullOrEmpty(model.Email))
            {
                bool emailExists = await _context.Users.AnyAsync(u => u.Email == model.Email);
                if (emailExists)
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại.");
                    return View(model);
                }
            }

            if (AvatarFile != null && AvatarFile.Length > 0)
            {
                var fileName = Path.GetFileName(AvatarFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/admin/images/img-avatar", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    AvatarFile.CopyTo(stream);
                }
                model.Avatar = fileName;
            }

            model.UserCode = Guid.NewGuid().ToString();
            model.CreatedAt = DateTime.Now;
            model.CreatedBy = HttpContext.Session.GetString("Username") ?? "Admin";
            model.Status = true;

            _context.Users.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Tạo user thành công!";
            return RedirectToAction(nameof(Create));
        }

        public IActionResult Edit(int? userId)
        {
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng !";
                return RedirectToAction("NotFoundPage", "Error", new { area = "Admin" });
            }

            var user = _context.Users.Find(userId);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Người dùng không tồn tại !";
                return RedirectToAction("NotFoundPage", "Error", new { area = "Admin" });
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(tblUser model, IFormFile? AvatarFile)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Vui lòng kiểm tra lại thông tin nhập vào.";
                return View(model);
            }

            var oldUser = await _context.Users.FirstOrDefaultAsync(s => s.UserId == model.UserId);

            if (oldUser == null)
            {
                TempData["ErrorMessage"] = "Người dùng không tồn tại !";
                return RedirectToAction("NotFoundPage", "Error", new { area = "Admin" });
            }

            bool usernameExists = await _context.Users.AnyAsync(u => u.Username == model.Username && u.UserId != model.UserId);
            if (usernameExists)
            {
                ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại.");
                return View(model);
            }

            if (!string.IsNullOrEmpty(model.Email))
            {
                bool emailExists = await _context.Users.AnyAsync(u => u.Email == model.Email && u.UserId != model.UserId);
                if (emailExists)
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại.");
                    return View(model);
                }
            }

            if (!string.IsNullOrEmpty(model.PhoneNumber))
            {
                bool phonenumberExists = await _context.Users.AnyAsync(u => u.PhoneNumber == model.PhoneNumber && u.UserId != model.UserId);
                if (phonenumberExists)
                {
                    ModelState.AddModelError("PhoneNumber", "Số điện thoại đã tồn tại.");
                    return View(model);
                }
            }

            oldUser.FullName = model.FullName;
            oldUser.BirthDate = model.BirthDate;
            oldUser.Gender = model.Gender;
            oldUser.PasswordHash = model.PasswordHash;
            oldUser.PhoneNumber = model.PhoneNumber;
            oldUser.Email = model.Email;
            oldUser.Role = model.Role;
            oldUser.Status = model.Status;

            if (AvatarFile != null && AvatarFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(oldUser.Avatar))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/admin/images/img-avatar", oldUser.Avatar);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                var fileName = Path.GetFileName(AvatarFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/admin/images/img-avatar", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    AvatarFile.CopyTo(stream);
                }
                oldUser.Avatar = fileName;
            }

            oldUser.UpdatedAt = DateTime.Now;
            oldUser.UpdatedBy = HttpContext.Session.GetString("Username") ?? "Admin";

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật thông tin người dùng thành công!";

            return RedirectToAction("Edit", new { userId = model.UserId });
        }

        public IActionResult Delete(int? userId)
        {
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng !";
                return RedirectToAction("NotFoundPage", "Error", new { area = "Admin" });
            }

            var user = _context.Users.Find(userId);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Người dùng không tồn tại !";
                return RedirectToAction("NotFoundPage", "Error", new { area = "Admin" });
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete([FromBody] DeleteAccountRequest request)
        {
            if (request == null || request.Id <= 0)
                return BadRequest();

            var user = _context.Users.FirstOrDefault(u => u.UserId == request.Id);
            if (user == null)
                return RedirectToAction("NotFoundPage", "Error", new { area = "Admin" });

            _context.Users.Remove(user);
            _context.SaveChanges();

            return Ok();
        }
    }

    public class DeleteAccountRequest
    {
        public int Id { get; set; }
    }
}