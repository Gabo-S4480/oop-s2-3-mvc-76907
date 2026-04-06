using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Domain;
using VgcCollege.Web.Data;

namespace VgcCollege.Web.Controllers
{
    [Authorize]
    public class StudentProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentProfilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StudentProfiles
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.StudentProfiles.ToListAsync());
        }

        // GET: StudentProfiles/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentProfile = await _context.StudentProfiles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentProfile == null)
            {
                return NotFound();
            }

            return View(studentProfile);
        }

        // GET: StudentProfiles/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }

        // POST: StudentProfiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdentityUserId,Name,Email,Phone,Address,StudentNumber")] StudentProfile studentProfile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(studentProfile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(studentProfile);
        }

        // GET: StudentProfiles/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentProfile = await _context.StudentProfiles.FindAsync(id);
            if (studentProfile == null)
            {
                return NotFound();
            }
            return View(studentProfile);
        }

        // POST: StudentProfiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdentityUserId,Name,Email,Phone,Address,StudentNumber")] StudentProfile studentProfile)
        {
            if (id != studentProfile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studentProfile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentProfileExists(studentProfile.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(studentProfile);
        }

        // GET: StudentProfiles/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentProfile = await _context.StudentProfiles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentProfile == null)
            {
                return NotFound();
            }

            return View(studentProfile);
        }

        // POST: StudentProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studentProfile = await _context.StudentProfiles.FindAsync(id);
            if (studentProfile != null)
            {
                _context.StudentProfiles.Remove(studentProfile);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentProfileExists(int id)
        {
            return _context.StudentProfiles.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MyGrades()
        {
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

           
            var studentProfile = await _context.StudentProfiles
                .FirstOrDefaultAsync(s => s.IdentityUserId == userId);

            if (studentProfile == null) return NotFound("Student profile not found.");

            
            var myEnrolments = await _context.Enrolments
                .Include(e => e.Course)
                .Where(e => e.StudentProfileId == studentProfile.Id)
                .ToListAsync();

            return View(myEnrolments);
        }
    }
}
