using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using News.DbModels;
using News.Models;
using System.Collections.Generic;

namespace News.Controllers;
public class NewsController : Controller
{
    private readonly NewsDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    public NewsController(NewsDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }
    private List<NewsModel> MapDbNewsToNews(List<DbNewsModel> dbNewsList)
    {
        return dbNewsList.Select(dbNews => new NewsModel
        {
            Id = dbNews.Id,
            Title = dbNews.Title,
            FullText = dbNews.FullText,
            ShortText = dbNews.ShortText,
            Date = dbNews.Date,
            ImageUrl = dbNews.ImageFile?.Url()
        }).ToList();
    }

    public IActionResult Index()
    {
        var dbNewsItems = _context.News.Include(x=> x.ImageFile).OrderByDescending(x=> x.Date).ToList();

        var newsItems = MapDbNewsToNews(dbNewsItems);

        return View(newsItems);
    }
    [HttpPost("/News/Uploads/{id}")]
    public async Task<IActionResult> Upload(IFormFile file, int id)
    {
        var current = _context.News.First(x => x.Id == id);
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
    public IActionResult NewsDetail(int id)
    {
        var news = MapDbNewsToNews(_context.News.Include(x=> x.ImageFile).ToList());
        var newsItem = news[id - 1];
        if (newsItem == null)
        {
            return NotFound(); 
        }

        return View(newsItem);
    }
    [Authorize]
    public IActionResult Edit()
    {
        // Retrieve the news model by ID from the database
        var newsModels = MapDbNewsToNews(_context.News.ToList());

        if (newsModels == null)
        {
            return NotFound();
        }

        return View(newsModels);
    }
    [HttpPost]
    public IActionResult SaveNewsModels([FromBody] List<NewsModel> newsModels)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var temp = _context.News.Include(x=> x.ImageFile).ToList();
                foreach (var news in _context.News)
                {
                    _context.News.Remove(news);
                }
                _context.News.AddRange(newsModels.Select(x=> new DbNewsModel()
                {
                    Date = x.Date,
                    FullText = x.FullText,
                    ShortText = x.ShortText,
                    Title = x.Title,
                    Id = x.Id,
                    ImageFile = temp.FirstOrDefault(y=> y.Id == x.Id)?.ImageFile
                }));
                _context.SaveChanges();
                return Json(new { success = true });
               
            }
            catch (DbUpdateException)
            {
                // Handle database update errors if needed
                ModelState.AddModelError("", "An error occurred while saving changes to the database.");
            }
        }

        // If ModelState is not valid or if an error occurred, return an error response
        return Json(new { success = false });
    }

}
