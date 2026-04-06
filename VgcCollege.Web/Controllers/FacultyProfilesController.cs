using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Domain;
using VgcCollege.Web.Data;

namespace VgcCollege.Web.Controllers
{
    [Authorize(Roles = "Admin,Faculty")]
    public class FacultyProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public FacultyProfilesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.FacultyProfiles.ToListAsync());
        }

        [Authorize(Roles = "Faculty")]
        public async Task<IActionResult> MyDashboard()
        {
            var userId = _userManager.GetUserId(User);
            var profile = await _context.FacultyProfiles
                .FirstOrDefaultAsync(p => p.IdentityUserId == userId);

            if (profile == null)
            {
                return NotFound("Faculty profile not found for the current user.");
            }

            var myCourses = await _context.Courses
                .Include(c => c.Branch)
                .Where(c => c.FacultyProfileId == profile.Id)
                .ToListAsync();

            return View(myCourses);
        }

        //  Gradebook
        [Authorize(Roles = "Faculty")]
        public async Task<IActionResult> Gradebook(int? id)
        {
            if (id == null) return NotFound();

            
            var course = await _context.Courses
                .Include(c => c.Branch)
                .Include(c => c.Enrolments)
                    .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (course == null) return NotFound();

            
            var userId = _userManager.GetUserId(User);
            var profile = await _context.FacultyProfiles
                .FirstOrDefaultAsync(p => p.IdentityUserId == userId);

            if (profile == null || course.FacultyProfileId != profile.Id)
            {
                return Forbid(); 
            }

            return View(course);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var facultyProfile = await _context.FacultyProfiles
                .FirstOrDefaultAsync(m => m.Id == id);

            if (facultyProfile == null) return NotFound();

            if (User.IsInRole("Faculty"))
            {
                var userId = _userManager.GetUserId(User);
                if (facultyProfile.IdentityUserId != userId)
                {
                    return Forbid();
                }
            }

            return View(facultyProfile);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,IdentityUserId,Name,Email,Phone")] FacultyProfile facultyProfile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(facultyProfile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(facultyProfile);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var facultyProfile = await _context.FacultyProfiles.FindAsync(id);
            if (facultyProfile == null) return NotFound();

            return View(facultyProfile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdentityUserId,Name,Email,Phone")] FacultyProfile facultyProfile)
        {
            if (id != facultyProfile.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(facultyProfile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacultyProfileExists(facultyProfile.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(facultyProfile);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var facultyProfile = await _context.FacultyProfiles
                .FirstOrDefaultAsync(m => m.Id == id);

            if (facultyProfile == null) return NotFound();

            return View(facultyProfile);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var facultyProfile = await _context.FacultyProfiles.FindAsync(id);
            if (facultyProfile != null)
            {
                _context.FacultyProfiles.Remove(facultyProfile);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FacultyProfileExists(int id)
        {
            return _context.FacultyProfiles.Any(e => e.Id == id);
        }
    }
}