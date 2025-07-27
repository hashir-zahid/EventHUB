using EventHUB.Models.Entities;
using HelpPage;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static EventHUB.Models.ApplicationDbContext;

namespace EventHUB.Controllers
{
    public class GuestController : Controller
    {
        private readonly ApplicationDbcontext db;
        private readonly IWebHostEnvironment hostingEnvironment;

        public GuestController(ApplicationDbcontext applicationDBContext, IWebHostEnvironment hostingEnvironment)
        {
            db = applicationDBContext;
            this.hostingEnvironment = hostingEnvironment;
        }


        

        #region Dashboard
        //------------------Guest Dashboard-----------------------------
        public IActionResult GuestDashboard()
        {
            var events = db.Events.ToList();
            return View(events);
        }
        #endregion

        #region Tickets
        //------------------My Tickets----------------------------------
        [HttpGet]
        public IActionResult MyTickets(string studentId = null)
        {
            IQueryable<Register> query = db.Registers;

            if (!string.IsNullOrEmpty(studentId))
            {
                query = query.Where(r => r.StudentId.Contains(studentId));
            }

            var tickets = query.ToList();
            return View(tickets);
        }
        #endregion

        #region Events
        //------------------Events--------------------------------------
        [HttpGet]
        public IActionResult Events()
        {
            var events = db.Events.ToList();
            return View(events);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [Route("Guest/Register/{eventId}")]
        [HttpGet]
        public IActionResult Register(int eventId)
        {
            var evt = db.Events.Find(eventId);
            if (evt == null)
            {
                return NotFound();
            }

            var model = new Register
            {
                EventId = evt.Id,
                EventName = evt.Name,
                EventDate = evt.Date.ToString("yyyy-MM-dd"),
                EventTime = $"{evt.Start} - {evt.End}",
                EventSpot = evt.Spot,
                EventPrice = evt.Price
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Register model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var evt = await db.Events.FindAsync(model.EventId);
                    if (evt == null)
                    {
                        ModelState.AddModelError("", "The event no longer exists");
                        return View(model);
                    }

                    var registration = new Register
                    {
                        FullName = model.FullName,
                        StudentId = model.StudentId,
                        Email = model.Email,
                        Department = model.Department,
                        Semester = model.Semester,
                        EventId = model.EventId,
                        EventName = evt.Name,
                        EventDate = evt.Date.ToString("yyyy-MM-dd"),
                        EventTime = $"{evt.Start} - {evt.End}",
                        EventSpot = evt.Spot,
                        EventPrice = evt.Price,
                    };

                    db.Registers.Add(registration);
                    await db.SaveChangesAsync();

                    return RedirectToAction("Events", "Guest");
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Error saving to database. Please try again.");
                    Console.WriteLine($"Database error: {ex.InnerException?.Message}");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An unexpected error occurred.");
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            return View(model);
        }
        #endregion

        #region Feedback
        //------------------Feedback-----------------------------------
        public IActionResult Feedback()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Feedback(UserFeedback userFeedback)
        {
            if (ModelState.IsValid)
            {
                db.Feedbacks.Add(userFeedback);
                db.SaveChanges();
                return RedirectToAction("Feedback", "Guest");
            }
            return View(userFeedback);
        }
        #endregion

        #region Authentication
        //------------------Login/Signup-------------------------------
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Signup(User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();

                var notification = new AdminNotification
                {
                    UserId = user.Id,
                    IsRead = false
                };
                db.AdminNotifications.Add(notification);
                db.SaveChanges();

                return RedirectToAction("GuestDashboard");
            }
            return View(user);
        }
        #endregion

        #region Media Library
        //------------------Media Library------------------------------
        public IActionResult MediaLibrary(string folder = "")
        {
            try
            {
                var mediaItems = db.EventMedias.AsQueryable();

                if (!string.IsNullOrEmpty(folder))
                {
                    mediaItems = mediaItems.Where(m => m.FolderName == folder);
                }

                var allFolders = db.EventMedias
                                   .Select(m => m.FolderName)
                                   .Distinct()
                                   .ToList();

                ViewBag.CurrentFolder = folder;
                ViewBag.FolderList = allFolders;

                return View(mediaItems.ToList());
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while loading the media library.";
                return View(new List<EventMedia>());
            }
        }

        public IActionResult ViewMedia(int id)
        {
            var media = db.EventMedias.FirstOrDefault(m => m.Id == id);
            if (media == null)
            {
                TempData["ErrorMessage"] = "Media not found.";
                return RedirectToAction("MediaLibrary");
            }

            return View(media);
        }
        #endregion

        #region Payment
        //------------------Payment------------------------------------
        public IActionResult Payment()
        {
            var events = db.Events.ToList();
            var registrations = db.Registers.ToList();

            var eventRegisters = from e in events
                                 join r in registrations on e.Id equals r.EventId
                                 select new EventRegister
                                 {
                                     Event = e,
                                     Register = r
                                 };

            ViewBag.TotalEvents = events.Count;
            return View(eventRegisters);
        }
        #endregion

        #region Help
        //------------------Help---------------------------------------
        public ActionResult Help()
        {
            var faqs = FaqProvider.GetFaqs();
            return View(faqs);
        }
        #endregion

        #region Logout
        //------------------Logout---------------------------------------
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("GuestDashboard", "Guest");
        }
        #endregion

        #region RedirectToMap
        //------------------RedirectToMap -----------------------------

        public async Task<IActionResult> RedirectToMap(int id)
        {
            // Fetch the event asynchronously
            var evt = await db.Events
                              .Where(e => e.Id == id && !string.IsNullOrWhiteSpace(e.Spot))
                              .FirstOrDefaultAsync();

            // If event not found or Spot is missing
            if (evt == null)
                return NotFound("Event not found or spot is missing.");

            // Safely extract the last word of the spot
            string lastWord = evt.Spot.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();

            if (string.IsNullOrWhiteSpace(lastWord))
                return BadRequest("Invalid spot value.");

            // Build the Google Maps search URL
            string location = $"NUML H9 {lastWord}";
            string encodedLocation = Uri.EscapeDataString(location);
            string googleMapsUrl = $"https://www.google.com/maps/search/?api=1&query={encodedLocation}";

            // Redirect to Google Maps
            return Redirect(googleMapsUrl);
        }

        #endregion
    }
}