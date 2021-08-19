using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TodoMVCAppAsFastAsICan.Data;
using TodoMVCAppAsFastAsICan.Models;

namespace TodoMVCAppAsFastAsICan.Controllers
{
    [Authorize]
    public class TodoController : Controller
    {
        private readonly ILogger<TodoController> _logger;
        private readonly MongoDbAccessor _db;

        public TodoController(ILogger<TodoController> logger, MongoDbAccessor db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
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
    }
}
