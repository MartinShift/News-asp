using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using News.DbModels;

namespace News.Controllers
{
    public class MapController : Controller
    {
        private readonly NewsDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public MapController(NewsDbContext context , IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var waypoints = await _context.Waypoints.Include(x=> x.ImageFile).ToListAsync();
            return View(waypoints);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Name,Lat,Lon")] Waypoint waypoint)
        {
            if (ModelState.IsValid)
            {
                _context.Add(waypoint);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(waypoint);
        }
        [HttpPost("/Waypoint/Add")]
        public IActionResult Add([FromBody] Waypoint waypoint)
        {
            try
            {
                _context.Waypoints.Add(waypoint);
                _context.SaveChanges();
                return Json(new { success = true, id = _context.Waypoints.ToList().Last().Id });
            }
            catch (Exception ex)
            {
                // Handle errors and return failure status
                return Json(new { success = false, error = ex.Message });
            }
        }
        [HttpPost("/Waypoint/Uploads/{id}")]
        public async Task<IActionResult> Upload(IFormFile file, int id)
        {
            var current = _context.Waypoints.First(x => x.Id == id);
            var filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var dbFile = new DbImageFile()
            {
                Filename = filename,
                RootDirectory = "Uploads",
            };
            var localFilename =
                Path.Combine(_webHostEnvironment.WebRootPath, dbFile.RootDirectory, dbFile.Filename);
            using (var localFile = System.IO.File.Open(localFilename, FileMode.OpenOrCreate))
            {
                await file.CopyToAsync(localFile);
            }
            _context.Images.Add(dbFile);
            current.ImageFile = dbFile;
            await _context.SaveChangesAsync();
            return Ok(dbFile);
        }
        [HttpPost("/Waypoint/Edit")]
        public IActionResult Edit([FromBody] Waypoint waypoint)
        {
            try
            {
                var wp = _context.Waypoints.FirstOrDefault(x => x.Id == waypoint.Id);
                wp.Name = waypoint.Name;
                wp.Lat = waypoint.Lat;
                wp.Lon = waypoint.Lon;
                wp.Description = waypoint.Description;
                _context.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Handle errors and return failure status
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost("/Waypoint/Delete")]
        public IActionResult Delete([FromBody]  int id)
        {
            try
            {
                _context.Waypoints.Remove(_context.Waypoints.First(x=> x.Id == id));
                _context.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Handle errors and return failure status
                return Json(new { success = false, error = ex.Message });
            }
        }


    }
}
