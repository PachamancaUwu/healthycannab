using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using healthycannab.Models;

namespace healthycannab.Controllers;

public class MainController : BaseController
{
    private readonly ILogger<MainController> _logger;

    public MainController(ILogger<MainController> logger)
    {
        _logger = logger;
    }

    public IActionResult Inicio()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
