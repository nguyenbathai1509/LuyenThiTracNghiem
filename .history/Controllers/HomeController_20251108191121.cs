using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LuyenThiTracNghiem.Models;

namespace LuyenThiTracNghiem.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly DataContext _context;
    public HomeController(ILogger<HomeController> logger, DataContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [Route("/{slug}-{id:long}.html", Name = "ExamInfor")]
    public IActionResult Details(long? id)
    {
        if (id == null) return NotFound();

        var exam = _context.Exams
            .FirstOrDefault(e => (e.ExamId == id) && (e.Status == true));

        if (exam == null) return NotFound();

        return View(exam);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
