using EventHUB.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static EventHUB.Models.ApplicationDbContext;

namespace EventHUB.Controllers
{
    [Authorize]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class MemberController : Controller
    {
        private readonly ApplicationDbcontext db;
        private readonly IWebHostEnvironment hostingEnvironment;

        public MemberController( ApplicationDbcontext applicationDBContext,  IWebHostEnvironment hostingEnvironment)
        {
            db = applicationDBContext;
            this.hostingEnvironment = hostingEnvironment;
        }

        #region Dashboard
        //------------------Member Dashboard-----------------------------
        public IActionResult MemberDashboard()
        {
            var events = db.Events.ToList();
           

            return View(events);
        }
        #endregion

        #region Events
        //------------------Events---------------------------------------
        public IActionResult EventList()
        {
            var events = db.Events.ToList();
            return View(events);
        }
        #endregion

        #region Payment
        //------------------Payment-------------------------------------
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

        [HttpGet]
        public IActionResult UploadMedia()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UploadMedia(IFormFile file, string eventName,
            string mediaType, string folderName, DateTime eventDate, TimeSpan eventTime, string newFolderName = "")
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    TempData["ErrorMessage"] = "Please select a file to upload.";
                    return RedirectToAction("MediaLibrary");
                }

                string targetFolder = !string.IsNullOrEmpty(newFolderName) ? newFolderName : folderName;

                if (string.IsNullOrWhiteSpace(targetFolder))
                {
                    TempData["ErrorMessage"] = "Folder name is required.";
                    return RedirectToAction("MediaLibrary");
                }

                var uploadPath = Path.Combine(hostingEnvironment.WebRootPath, "uploads", targetFolder);
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                var media = new EventMedia
                {
                    FilePath = $"/uploads/{targetFolder}/{fileName}",
                    EventName = eventName,
                    MediaType = mediaType,
                    FolderName = targetFolder,
                    UploadDate = DateTime.Now,
                    EventDate = eventDate.Date + eventTime
                };

                db.EventMedias.Add(media);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Media uploaded successfully!";
                return RedirectToAction("MediaLibrary", new { folder = targetFolder });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error uploading media: {ex.Message}";
                return RedirectToAction("MediaLibrary");
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

        #region Messaging
        //------------------Messaging----------------------------------
        public IActionResult Message()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Message(MemberMessage membermessage)
        {
            if (ModelState.IsValid)
            {
                db.MemberMessages.Add(membermessage);
                db.SaveChanges();
                return RedirectToAction("Message", "Member");
            }
            return View(membermessage);
        }
        #endregion

        #region Meetings
        //------------------Meetings-----------------------------------
        public IActionResult Meeting()
        {
            var meetings = db.Meetings.ToList();
            return View(meetings);
        }

        [HttpGet]
        public IActionResult AddMeeting()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddMeeting(Meeting meeting)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Meetings.Add(meeting);
                    db.SaveChanges();
                    return RedirectToAction("Meeting");
                }
                catch (FormatException ex)
                {
                    ModelState.AddModelError("", "Invalid date/time format. Please use the correct format.");
                    return View(meeting);
                }
            }
            return View(meeting);
        }

        public IActionResult EditMeeting(int id)
        {
            var meeting = db.Meetings.Find(id);
            if (meeting == null)
            {
                return NotFound();
            }
            return View(meeting);
        }

        [HttpPost]
        public IActionResult EditMeeting(Meeting meeting)
        {
            if (ModelState.IsValid)
            {
                db.Meetings.Update(meeting);
                db.SaveChanges();
                return RedirectToAction("Meeting");
            }
            return View(meeting);
        }

        public IActionResult DeleteMeeting(int id)
        {
            var meeting = db.Meetings.Find(id);
            if (meeting == null)
            {
                return NotFound();
            }

            db.Meetings.Remove(meeting);
            db.SaveChanges();
            return RedirectToAction("Meeting");
        }
        #endregion
    }
}