using EventHUB.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static EventHUB.Models.ApplicationDbContext;

namespace EventHUB.Controllers
{
    [Authorize(Roles = "Admin")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class AdminController : Controller
    {
        private readonly ApplicationDbcontext db;
        private readonly IWebHostEnvironment hostingEnvironment;

        public AdminController(ApplicationDbcontext applicationDBContext, IWebHostEnvironment hostingEnvironment)
        {
            db = applicationDBContext;
            this.hostingEnvironment = hostingEnvironment;
        }

        #region Admin Dashboard
        //----------AdminDashboard----------------
        public IActionResult AdminDashboard()
        {
            var events = db.Events.ToList();
            return View(events);
        }
        #endregion

        #region Media Library
        //----------MediaLibrary----------------
        public IActionResult MediaLibrary(string folder)
        {
            var query = db.EventMedias.AsQueryable();

            if (!string.IsNullOrEmpty(folder))
            {
                query = query.Where(m => m.FolderName == folder);
            }

            var mediaList = query.OrderByDescending(m => m.UploadDate).ToList();

            // Get distinct folder names for the sidebar
            var folderList = db.EventMedias
                .Select(m => m.FolderName)
                .Distinct()
                .ToList();

            ViewBag.CurrentFolder = folder;
            ViewBag.FolderList = folderList;

            return View(mediaList);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteMedia(int id)
        {
            try
            {
                // Use AsNoTracking if you're only deleting
                var media = db.EventMedias.AsNoTracking().FirstOrDefault(m => m.Id == id);

                if (media == null)
                {
                    TempData["ErrorMessage"] = "Media not found.";
                    return RedirectToAction("MediaLibrary");
                }

                // Delete physical file
                var filePath = Path.Combine(hostingEnvironment.WebRootPath, media.FilePath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Create a new entity with just the ID for deletion
                var mediaToDelete = new EventMedia { Id = id };
                db.EventMedias.Attach(mediaToDelete);
                db.EventMedias.Remove(mediaToDelete);

                // Explicitly save changes
                int affectedRows = db.SaveChanges();

                if (affectedRows > 0)
                {
                    TempData["SuccessMessage"] = "Media deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete media from database.";
                }

                return RedirectToAction("MediaLibrary", new { folder = media.FolderName });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting media: {ex.Message}";
                return RedirectToAction("MediaLibrary");
            }
        }

        public IActionResult EditMedia(int id)
        {
            var media = db.EventMedias.Find(id);
            if (media == null)
            {
                TempData["ErrorMessage"] = "Media not found.";
                return RedirectToAction("MediaLibrary");
            }

            var folderList = db.EventMedias
                .Select(m => m.FolderName)
                .Distinct()
                .ToList();

            ViewBag.FolderList = folderList;
            return View(media);
        }

        [HttpPost]
        public IActionResult EditMedia(int id, EventMedia model)
        {
            try
            {
                var media = db.EventMedias.Find(id);
                if (media == null)
                {
                    TempData["ErrorMessage"] = "Media not found.";
                    return RedirectToAction("MediaLibrary");
                }

                media.EventName = model.EventName;
                media.FolderName = model.FolderName;
                media.EventDate = model.EventDate;

                db.SaveChanges();

                TempData["SuccessMessage"] = "Media updated successfully!";
                return RedirectToAction("MediaLibrary", new { folder = media.FolderName });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating media: {ex.Message}";
                return RedirectToAction("MediaLibrary");
            }
        }

        public IActionResult ViewMedia(int id)
        {
            var media = db.EventMedias.Find(id);
            if (media == null)
            {
                TempData["ErrorMessage"] = "Media not found.";
                return RedirectToAction("MediaLibrary");
            }

            return View(media);
        }
        #endregion

        #region Events
        //----------Events----------------
        [HttpGet]
        public IActionResult AddEvent()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddEvent(Event model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Events.Add(model);
                    db.SaveChanges();
                    return RedirectToAction("EventList");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return View(model);
                }
            }
            return View(model);
        }

        public IActionResult EventList()
        {
            var events = db.Events.ToList();
            return View(events);
        }

        public ActionResult DeleteEvents(int id)
        {
            try
            {
                var dbEvents = db.Events.SingleOrDefault(p => p.Id == id);
                if (dbEvents == null)
                {
                    return NotFound();
                }

                db.Events.Remove(dbEvents);
                db.SaveChanges();

                return RedirectToAction("EventList");
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult EditEvent(int id)
        {
            var eventToEdit = db.Events.AsNoTracking().FirstOrDefault(e => e.Id == id);
            if (eventToEdit == null)
            {
                return NotFound();
            }
            return View("UpdateEvent", eventToEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditEvent(int id, Event updatedEvent)
        {
            if (id != updatedEvent.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View("UpdateEvent", updatedEvent);
            }

            try
            {
                var existingEvent = db.Events.Find(id);
                if (existingEvent == null)
                {
                    return NotFound();
                }

                existingEvent.Name = updatedEvent.Name;
                existingEvent.Date = updatedEvent.Date;
                existingEvent.Start = updatedEvent.Start;
                existingEvent.End = updatedEvent.End;
                existingEvent.Spot = updatedEvent.Spot;
                existingEvent.Description = updatedEvent.Description;
                existingEvent.Price = updatedEvent.Price;
                existingEvent.Max_Attendees = updatedEvent.Max_Attendees;

                db.SaveChanges();

                TempData["SuccessMessage"] = "Event updated successfully!";
                return RedirectToAction(nameof(EventList));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the event. Please try again.");
                return View("UpdateEvent", updatedEvent);
            }
        }
        #endregion

        #region Members
        //----------Members----------------
        public IActionResult MemberList()
        {
            var approvedMembers = db.Users
                .Where(u => u.IsApproved)
                .ToList();

            var pendingCount = db.AdminNotifications.Count(n => !n.IsRead);
            ViewBag.PendingCount = pendingCount;

            return View(approvedMembers);
        }

        [HttpGet("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                var dbUser = db.Users.SingleOrDefault(p => p.Id == id);
                if (dbUser == null)
                {
                    return NotFound();
                }

                db.Users.Remove(dbUser);
                db.SaveChanges();

                return RedirectToAction("MemberList");
            }
            catch
            {
                return View();
            }
        }

        public IActionResult ApproveUser(int id)
        {
            var user = db.Users.Find(id);
            if (user != null)
            {
                user.IsApproved = true;

                var notification = db.AdminNotifications.FirstOrDefault(n => n.UserId == id);
                if (notification != null)
                {
                    db.AdminNotifications.Remove(notification);
                }

                db.SaveChanges();
            }

            return RedirectToAction("Notification");
        }

        public IActionResult RejectUser(int id)
        {
            var user = db.Users.Find(id);
            if (user != null)
            {
                var notification = db.AdminNotifications.FirstOrDefault(n => n.UserId == id);
                if (notification != null)
                {
                    db.AdminNotifications.Remove(notification);
                }

                db.Users.Remove(user);
                db.SaveChanges();
            }

            return RedirectToAction("Notification");
        }

        public IActionResult UpdateMember(int id)
        {
            var user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateMember(int id, User updatedUser)

        {
            if (id != updatedUser.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction("MemberList");
            }

            var user = db.Users.FirstOrDefault(p => p.Id == id);
            if (user == null )
            {
                return NotFound();
            }


            //var dbEntity = db.Users.FirstOrDefault(p => p.Id == id);

            // Update allowed fields only
            user.Username = updatedUser.Username;
            user.Email = updatedUser.Email;
            user.Phone = updatedUser.Phone;
            user.Department = updatedUser.Department;
            user.Semester = updatedUser.Semester;
            user.Team = updatedUser.Team;
            user.Address = updatedUser.Address;
            user.StudentID = updatedUser.StudentID;
            user.IsApproved = updatedUser.IsApproved;

           // db.Users.Update(user);
            db.SaveChanges();


            return RedirectToAction("MemberList"); 
        }
        #endregion

        #region Finance
        //----------Finance----------------
        public IActionResult Finance()
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

        #region Meetings
        //----------Meetings----------------
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

        #region Feedback
        //----------Feedback----------------
    
        public IActionResult Feedback()
        {
            var feed = db.Feedbacks.ToList();
            return View(feed);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkFeedbackAsRead(int id)
        {
            var feedback = db.Feedbacks.Find(id);
            if (feedback != null)
            {
                feedback.IsRead = true;
                db.SaveChanges();
                TempData["Success"] = "Feedback marked as read";
            }
            return RedirectToAction("Feedback");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteFeedback(int id)
        {
            var feedback = db.Feedbacks.FirstOrDefault(n => n.Id == id);
            if (feedback != null)
            {
                db.Feedbacks.Remove(feedback);
                db.SaveChanges();
                TempData["Success"] = "Feedback deleted successfully";
            }
            return RedirectToAction("Feedback");
        }
        #endregion

        #region Message

        //----------Message----------------
        public IActionResult Message()
        {
            var messages = db.MemberMessages.ToList();
            return View(messages);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkMessageAsRead(int id)
        {
            var message = db.MemberMessages.Find(id);
            if (message != null)
            {
                message.IsRead = true;
                db.SaveChanges();
                TempData["Success"] = "Message marked as read";
            }
            return RedirectToAction("Message");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteMessage(int id)
        {
            var message = db.MemberMessages.Find(id);
            if (message != null)
            {
                db.MemberMessages.Remove(message);
                db.SaveChanges();
                TempData["Success"] = "Message deleted successfully";
            }
            return RedirectToAction("Message");
        }
        #endregion

        #region Notification
        //----------Notification----------------
        public IActionResult Notification()
        {
            var notifications = db.AdminNotifications
                .Include(n => n.User)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();

            // Mark all as read when viewed
            foreach (var notification in notifications.Where(n => !n.IsRead))
            {
                notification.IsRead = true;
            }
            db.SaveChanges();

            return View(notifications);
        }

        public IActionResult GetUnreadNotificationCount()
        {
            var count = db.AdminNotifications.Count(n => !n.IsRead);
            return Json(new { count });
        }
        #endregion

    }
}