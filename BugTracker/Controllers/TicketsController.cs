using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System.IO;
using BugTracker.Helper;

namespace BugTracker.Controllers
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets
        public ActionResult Index()
        {
            var tickets = db.Tickets.Include(t => t.Project).Include(t => t.Priority).Include(t => t.Type).Include(t => t.Status).ToList();
            
            
            return View(tickets);
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // GET: Tickets/Create
        public ActionResult Create()
        {
            
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Title");
            ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TypeId = new SelectList(db.TicketTypes, "Id", "Name");
            ViewBag.StatusId = new SelectList(db.TicketStatuses, "Id", "Name");

            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,ProjectId,PriorityId,TypeId,StatusId,Description,Title,CreationDate,MediaUrl")] Ticket ticket, HttpPostedFileBase fileUpload)
        {
            ticket.CreationDate = new DateTimeOffset(DateTime.Now);

            if (ModelState.IsValid)
            {
                // restricting the valid file formats to images only
                if (Ticket.ImageUploadValidator.IsWebFriendlyImage(fileUpload))
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    fileUpload.SaveAs(Path.Combine(Server.MapPath("~/img/"), fileName));
                    ticket.MediaUrl = "~/img/" + fileName;

                }

                var user = db.Users.Find(User.Identity.GetUserId());

                ticket.UserId = user.UserName;
              

                ticket.CreationDate = new DateTimeOffset(DateTime.Now);

                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Title", ticket.ProjectId);
            ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.PriorityId);
            ViewBag.TypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TypeId);
            ViewBag.StatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.StatusId);

            return View(ticket);
        }


        [HttpGet]
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult AssignTicket(int ticketId)
        {

            AssignTicketUser assignticketuser = new AssignTicketUser();
            UserTicketsHelper helper = new UserTicketsHelper();

            var select = helper.UsersInTicket(ticketId).Select(i => i.Id);

            var projectManagerUser = helper.GetApplicationUsersInRole("ProjectManager");

            var developerUsers = helper.GetApplicationUsersInRole("Developer");

            if (User.IsInRole("Admin"))
            {
                
                assignticketuser.ProjectManagerUsers = new MultiSelectList(projectManagerUser, "Id", "DisplayName", select);

                assignticketuser.DeveloperManagerUsers = new MultiSelectList(developerUsers, "Id", "DisplayName", select);

            }
            else if (User.IsInRole("ProjectManager"))
            {
                assignticketuser.DeveloperManagerUsers = new MultiSelectList(developerUsers, "Id", "DisplayName", select);
            }

            assignticketuser.Ticket = db.Tickets.Find(ticketId);

            return View(assignticketuser);
        }

        //POST: Edit Project Users
        [HttpPost]
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult AssignTicket(int ticketId, AssignTicketUser assignuser)
        {
            UserTicketsHelper helper = new UserTicketsHelper();

            if (ModelState.IsValid)
            {
                string[] empty = { };
                assignuser.SelectedUser = assignuser.SelectedUser ?? empty;

                foreach (var user in db.Users)
                {
                    if (assignuser.SelectedUser.Contains(user.Id))
                    {
                        helper.AddUserToTicket(user.Id, ticketId);

                    }
                    else
                    {
                        helper.RemoveUserFromTicket(user.Id, ticketId);

                    }
                }
                db.SaveChanges();
            }

            return RedirectToAction("Index", "Tickets");
        }


        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Title", ticket.ProjectId);
            ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.PriorityId);
            ViewBag.TypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TypeId);
            ViewBag.StatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.StatusId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,ProjectId,PriorityId,TypeId,StatusId,Description,Title,CreationDate,MediaUrl")] Ticket ticket, HttpPostedFileBase fileUpload)
        {
            if (ModelState.IsValid)
            {
                var fetched = db.Tickets.Find(ticket.Id);
                fetched.UserId = ticket.UserId;
                fetched.Priority = ticket.Priority;
                fetched.Type = ticket.Type;
                fetched.Status = ticket.Status;
                fetched.MediaUrl = ticket.MediaUrl;
                fetched.Title = ticket.Title;
                fetched.Description = ticket.Description;

                // restricting the valid file formats to images only
                if (Ticket.ImageUploadValidator.IsWebFriendlyImage(fileUpload))
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    fileUpload.SaveAs(Path.Combine(Server.MapPath("~/img/"), fileName));
                    fetched.MediaUrl = "~/img/" + fileName;

                }



                db.Entry(fetched).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Title", ticket.ProjectId);
            ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.PriorityId);
            ViewBag.TypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TypeId);
            ViewBag.StatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.StatusId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
