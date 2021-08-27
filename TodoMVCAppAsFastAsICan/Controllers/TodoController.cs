using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TodoMVCAppAsFastAsICan.Data;
using TodoMVCAppAsFastAsICan.Models;

namespace TodoMVCAppAsFastAsICan.Controllers
{
    [Authorize("Auth_Policy")]
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
            var user = getLoggedInUserByEmail();
            if (user.Todos == null)
            {
                user.Todos = new List<TodoModel>();
            }
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(TodoModel newTodo)
        {
            var user = getLoggedInUserByEmail();

            if (ModelState.IsValid)
            {
                newTodo.DateCreated = DateTime.UtcNow;
                newTodo.LastEdited = DateTime.UtcNow;

                if (user.Todos == null)
                {
                    user.Todos = new List<TodoModel>();
                }
                user.Todos.Add(newTodo);
                _db.UpsertRecord(user.Id, user); 
            }

            return View(user);
        }

        public IActionResult Edit(int todoIndex)
        {
            var user = getLoggedInUserByEmail();

            var dbTodo = user.Todos[todoIndex];

            var todo = new EditTodoModel
            {
                Index = todoIndex,
                Message = dbTodo.Message,
                DateCreated = dbTodo.DateCreated,
                LastEdited = dbTodo.LastEdited
            };

            return View(todo);
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EditTodoModel todo)
        {
            var user = getLoggedInUserByEmail();

            if (ModelState.IsValid)
            {
                // TODO: Edit inputs validation
                user.Todos[todo.Index] = new TodoModel { 
                    Message = todo.Message,
                    DateCreated = todo.DateCreated,
                    LastEdited = DateTime.UtcNow
                };

                _db.UpsertRecord(user.Id, user); 
            }

            return RedirectToAction(nameof(Index));
            
        }

        // todo: maybe make this delete POST and have a get version that has a confirm delete view.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int todoIndex)
        {
            var user = getLoggedInUserByEmail();

            user.Todos.RemoveAt(todoIndex);

            _db.UpsertRecord(user.Id, user);

            return RedirectToAction(nameof(Index));
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

        private UserModel getLoggedInUserByEmail()
        {
            string email = HttpContext.User.Claims.Where(x => x.Type == ClaimTypes.Email).First().Value;
            return _db.LoadRecords<UserModel>().Where(x => x.EmailAddress == email).First();
        }
    }
}
