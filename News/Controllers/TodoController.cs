using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using News.DbModels;
using News.Models;

namespace News.Controllers
{
    [Authorize]
    public class TodoController : Controller
    {
        private readonly NewsDbContext _context;

        public TodoController(NewsDbContext context)
        {
            _context = context;
        }

        // GET: /TodoIndex/{date}
        [HttpGet("/Todo/Index/{date}")]
        public IActionResult Index(string date)
        {
            var truedate = DateTime.Parse(date);
            // Load tasks for the selected date
            var tasks = _context.Todos
                .Where(x => x.UserId == User.Identity.Name)
                .Where(task => task.Date.Value.Date == truedate.Date)
                .ToList();

            ViewData["SelectedDate"] = date;
            return View(tasks);
        }

        [HttpGet]
        public IActionResult Index()
        {
            var currentDate = DateTime.Now.Date;
            var tasks = _context.Todos
                .Where(x => x.UserId == User.Identity.Name)
                .Where(task => task.Date.Value.Date == currentDate)
                .ToList();

            ViewData["SelectedDate"] = currentDate.ToString("yyyy-MM-dd");
            return View(tasks);
        }

        [HttpPost("/Todo/AddTask")]
        public IActionResult AddTask([FromBody] Todo model)
        {
            ModelState.Values
       .SelectMany(v => v.Errors)
       .ToList().ForEach(x => Console.WriteLine(x.ErrorMessage));
            if (ModelState.IsValid)
            {
                model.Date += TimeSpan.FromHours(3);
                model.UserId = User.Identity.Name;
                _context.Todos.Add(model);
                _context.SaveChanges();

                // Return the new task's ID
                return Json(new { id = model.Id });
            }

            return BadRequest(new { Message = "Invalid model state" });
        }

        // POST: /Todo/DeleteTask
        [HttpPost("/Todo/DeleteTask")]
        public IActionResult DeleteTask([FromBody] int taskId)
        {
            var task = _context.Todos.Find(taskId);
            if (task != null)
            {
                _context.Todos.Remove(task);
                _context.SaveChanges();
                return Ok();
            }

            return NotFound(new { Message = "Task not found" });
        }
        [HttpPost("/Todo/UpdateTask")]
        public IActionResult UpdateTask([FromBody] Todo model)
        {
            var task = _context.Todos.Find(model.Id);
            if (task != null)
            {
                if (model.Date != null)
                {
                    task.Date = model.Date;
                }
                task.IsDone = model.IsDone;
                task.Name = model.Name;
                task.UserId = User.Identity.Name;
                _context.SaveChanges();
                return Ok();
            }

            return NotFound(new { Message = "Task not found" });
        }
    }
}