using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LuyenThiTracNghiem.Filters;
using Microsoft.AspNetCore.Mvc;

namespace LuyenThiTracNghiem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminOnly]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}