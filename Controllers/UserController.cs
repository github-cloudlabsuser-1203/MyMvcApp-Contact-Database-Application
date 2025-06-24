using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyMvcApp.Models;
using MyMvcApp.Data;
using Microsoft.EntityFrameworkCore;

namespace MyMvcApp.Controllers;

public class UserController : Controller
{
    private readonly UserDbContext _context;

    public UserController(UserDbContext context)
    {
        _context = context;
    }    // GET: User
    public async Task<ActionResult> Index(string searchString)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            searchString = searchString.ToLower();
            query = query.Where(u => 
                u.Name.ToLower().Contains(searchString) ||
                u.Email.ToLower().Contains(searchString) ||
                u.Phone.Contains(searchString));
        }

        var users = await query.ToListAsync();
        ViewBag.CurrentSearch = searchString;
        return View(users);
    }

    // GET: User/Details/5
    public async Task<ActionResult> Details(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return View(user);
    }

    // GET: User/Create
    public ActionResult Create()
    {
        return View();
    }

    // POST: User/Create
    [HttpPost]
    public async Task<ActionResult> Create(User user)
    {
        if (ModelState.IsValid)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        return View(user);
    }

    // GET: User/Edit/5
    public async Task<ActionResult> Edit(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return View(user);
    }

    // POST: User/Edit/5
    [HttpPost]
    public async Task<ActionResult> Edit(int id, User user)
    {
        if (id != user.Id)
        {
            return BadRequest();
        }
        if (ModelState.IsValid)
        {
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        return View(user);
    }

    // GET: User/Delete/5
    public async Task<ActionResult> Delete(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return View(user);
    }

    // POST: User/Delete/5
    [HttpPost]
    public async Task<ActionResult> Delete(int id, IFormCollection collection)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
}
