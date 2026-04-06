using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Domain;
using VgcCollege.Web.Data;

namespace VgcCollege.Web.Controllers
{
    [Authorize(Roles = "Admin,Faculty")]
    public class CourseEnrolmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CourseEnrolmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CourseEnrolments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Enrolments.Include(c => c.Course).Include(c => c.Student);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: CourseEnrolments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseEnrolment = await _context.Enrolments
                .Include(c => c.Course)
                .Include(c => c.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (courseEnrolment == null)
            {
                return NotFound();
            }

            return View(courseEnrolment);
        }

        // GET: CourseEnrolments/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name");
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Name");
            return View();
        }

        // POST: CourseEnrolments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentProfileId,CourseId,EnrolDate,Status")] CourseEnrolment courseEnrolment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(courseEnrolment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id", courseEnrolment.CourseId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Id", courseEnrolment.StudentProfileId);
            return View(courseEnrolment);
        }

        // GET: CourseEnrolments/Edit/5
        
        [Authorize(Roles = "Admin,Faculty")] 
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

           
            var courseEnrolment = await _context.Enrolments
                .Include(c => c.Student)
                .Include(c => c.Course)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (courseEnrolment == null)
            {
                return NotFound();
            }

            
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", courseEnrolment.CourseId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Name", courseEnrolment.StudentProfileId);

            return View(courseEnrolment);
        }

        // POST: CourseEnrolments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Faculty")] 
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentProfileId,CourseId,EnrolDate,Status,Grade")] CourseEnrolment courseEnrolment)
        {
            if (id != courseEnrolment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                   
                    _context.Update(courseEnrolment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseEnrolmentExists(courseEnrolment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

             
                if (User.IsInRole("Faculty"))
                {
                    return RedirectToAction("Gradebook", "FacultyProfiles", new { id = courseEnrolment.CourseId });
                }

               
                return RedirectToAction(nameof(Index));
            }

            
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", courseEnrolment.CourseId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Name", courseEnrolment.StudentProfileId);

            return View(courseEnrolment);
        }

        // GET: CourseEnrolments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseEnrolment = await _context.Enrolments
                .Include(c => c.Course)
                .Include(c => c.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (courseEnrolment == null)
            {
                return NotFound();
            }

            return View(courseEnrolment);
        }

        // POST: CourseEnrolments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var courseEnrolment = await _context.Enrolments.FindAsync(id);
            if (courseEnrolment != null)
            {
                _context.Enrolments.Remove(courseEnrolment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseEnrolmentExists(int id)
        {
            return _context.Enrolments.Any(e => e.Id == id);
        }
    }
}
