using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LuyenThiTracNghiem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ErrorController : Controller
    {
        public IActionResult NotFoundPage()
        {
            if (TempData["ErrorMessage"] == null)
            {
                TempData["ErrorMessage"] = "Trang quản trị bạn yêu cầu không tồn tại hoặc đã bị xóa.";
            }

            return View("~/Areas/Admin/Views/Shared/AdminNotFound.cshtml");
        }
    }
}