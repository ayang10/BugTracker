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
            var user = db.Users.Find(User.Identity.GetUserId());

            if (User.IsInRole("Submitter"))
            {
                var tickets = db.Tickets.Where(t => t.UserId == user.UserName).Include(t => t.Project).Include(t => t.Priority).Include(t => t.Type).Include(t => t.Status).ToList();
                return View(tickets);
            }
            else if (User.IsInRole("Developer"))
            {
                var tickets = db.Tickets.Where(t => t.UserId == user.UserName).Include(t => t.Project).Include(t => t.Priority).Include(t => t.Type).Include(t => t.Status).ToList();
                return View(tickets);
            }
            
            else if(User.IsInRole("Admin") || User.IsInRole("ProjectManager")){

                var tickets = db.Tickets.Include(t => t.Project).Include(t => t.Priority).Include(t => t.Type).Include(t => t.Status).ToList();
                return View(tickets);

            }
            else
            {
                return View();
            }

            
            
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
        public ActionResult Create([Bind(Include = "Id,UserId,ProjectId,PriorityId,TypeId,StatusId,Description,Title,CreationDate,Attachment")] Ticket ticket, HttpPostedFileBase fileUpload)
        {
            ticket.CreationDate = new DateTimeOffset(DateTime.Now);

            if (ModelState.IsValid)
            {
                

                // restricting the valid file formats to images only
                if (Ticket.ImageUploadValidator.IsWebFriendlyImage(fileUpload))
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    fileUpload.SaveAs(Path.Combine(Server.MapPath("~/img/"), fileName));
                    ticket.Attachment = "~/img/" + fileName;

                }

                var user = db.Users.Find(User.Identity.GetUserId());

                ticket.UserId = user.UserName;

                ticket.StatusId = 1;

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
            var userId = User.Identity.GetUserId();
            var changed = DateTimeOffset.Now;
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
            Ticket ticket = new Ticket();
            var userId = User.Identity.GetUserId();
            TicketHistoryHelper thHelper = new TicketHistoryHelper();
            var changed = DateTimeOffset.Now;
            var select = helper.UsersInTicket(ticketId).Select(i => i.Id);
            
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
        public ActionResult Edit([Bind(Include = "Id,UserId,ProjectId,PriorityId,TypeId,StatusId,Description,Title,CreationDate,Attachment")] Ticket ticket, HttpPostedFileBase fileUpload)
        {
            

            if (ModelState.IsValid)
            {
                TicketHistoryHelper thHelper = new TicketHistoryHelper();
                UserTicketsHelper helper = new UserTicketsHelper();
                var user = db.Users.Find(User.Identity.GetUserId());
                var OldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);
               
                var changedDate = DateTimeOffset.Now;
        

                ticket.AssignTicketUsers = helper.UsersInTicket(ticket.Id);
                Notification notification = new Notification();
                

                if (OldTicket.Title != ticket.Title)
                {
                    TicketHistory ticketHistorys = new TicketHistory
                    {
                        TicketId = ticket.Id,
                        Property = "Title",
                        Old = OldTicket.Title,
                        OldValue = OldTicket.Title,
                        New = ticket.Title,
                        NewValue = ticket.Title,
                        ChangedDate = changedDate,
                        UserId = user.Id
                    };
                    db.TicketHistories.Add(ticketHistorys);


                    foreach (var item in ticket.AssignTicketUsers.ToList())
                    {
                        notification.TicketId = ticket.Id;
                        notification.CreatorUserId = user.UserName;
                        notification.Creator = user;
                        notification.RecipientUserId = item.Id;
                        notification.Recipient = db.Users.Find(item.UserName);
                        notification.Change = "Changed Title";
                        notification.DateNotified = changedDate;


                        db.Notifications.Add(notification);
                        db.SaveChanges();
                    }

                }

                if(OldTicket.Description != ticket.Description)
                {
                    TicketHistory ticketHistorys = new TicketHistory
                    {
                        TicketId = ticket.Id,
                        Property = "Description",
                        Old = OldTicket.Description,
                        OldValue = OldTicket.Description,
                        New = ticket.Description,
                        NewValue = ticket.Description,
                        ChangedDate = changedDate,
                        UserId = user.Id
                    };
                    db.TicketHistories.Add(ticketHistorys);


                    foreach (var item in ticket.AssignTicketUsers.ToList())
                    {
                        notification.TicketId = ticket.Id;
                        notification.CreatorUserId = user.Id;
                        notification.Creator = user;
                        notification.RecipientUserId = item.Id;
                        notification.Recipient = db.Users.Find(item.UserName);
                        notification.Change = "Edit Description";
                        notification.DateNotified = changedDate;


                        db.Notifications.Add(notification);
                        db.SaveChanges();
                    }
                    
                }

                if (OldTicket.PriorityId != ticket.PriorityId)
                {
                    TicketHistory ticketHistorys = new TicketHistory
                    {
                        TicketId = ticket.Id,
                        Property = "Priority",
                        Old = OldTicket.Priority.Name,
                        OldValue = OldTicket.Priority.Name,
                        New = ticket.Priority.Name,
                        NewValue = ticket.Priority.Name,
                        ChangedDate = changedDate,
                        UserId = user.Id
                    };
                    db.TicketHistories.Add(ticketHistorys);


                    foreach (var item in ticket.AssignTicketUsers.ToList())
                    {
                        notification.TicketId = ticket.Id;
                        notification.CreatorUserId = user.Id;
                        notification.Creator = user;
                        notification.RecipientUserId = item.Id;
                        notification.Recipient = db.Users.Find(item.UserName);
                        notification.Change = "Changed Priority";
                        notification.DateNotified = changedDate;


                        db.Notifications.Add(notification);
                        db.SaveChanges();
                    }

                }

                if (OldTicket.StatusId != ticket.StatusId)
                {
                    TicketHistory ticketHistorys = new TicketHistory
                    {
                        TicketId = ticket.Id,
                        Property = "Status",
                        Old = OldTicket.Status.Name,
                        OldValue = OldTicket.Status.Name,
                        New = ticket.Status.Name,
                        NewValue = ticket.Status.Name,
                        ChangedDate = changedDate,
                        UserId = user.Id
                    };
                    db.TicketHistories.Add(ticketHistorys);


                    foreach (var item in ticket.AssignTicketUsers.ToList())
                    {
                        notification.TicketId = ticket.Id;
                        notification.CreatorUserId = user.Id;
                        notification.Creator = user;
                        notification.RecipientUserId = item.Id;
                        notification.Recipient = db.Users.Find(item.UserName);
                        notification.Change = "Changed Status";
                        notification.DateNotified = changedDate;


                        db.Notifications.Add(notification);
                        db.SaveChanges();
                    }

                }

                if (OldTicket.TypeId != ticket.TypeId)
                {
                    TicketHistory ticketHistorys = new TicketHistory
                    {
                        TicketId = ticket.Id,
                        Property = "Status",
                        Old = OldTicket.Type.Name,
                        OldValue = OldTicket.Type.Name,
                        New = ticket.Type.Name,
                        NewValue = ticket.Type.Name,
                        ChangedDate = changedDate,
                        UserId = user.Id
                    };
                    db.TicketHistories.Add(ticketHistorys);


                    foreach (var item in ticket.AssignTicketUsers.ToList())
                    {
                        notification.TicketId = ticket.Id;
                        notification.CreatorUserId = user.Id;
                        notification.Creator = user;
                        notification.RecipientUserId = item.Id;
                        notification.Recipient = db.Users.Find(item.UserName);
                        notification.Change = "Changed Type";
                        notification.DateNotified = changedDate;


                        db.Notifications.Add(notification);
                        db.SaveChanges();
                    }

                }

                if (OldTicket.Attachment != ticket.Attachment)
                {
                    TicketHistory ticketHistorys = new TicketHistory
                    {
                        TicketId = ticket.Id,
                        Property = "Status",
                        Old = OldTicket.Attachment,
                        OldValue = OldTicket.Attachment,
                        New = ticket.Attachment,
                        NewValue = ticket.Attachment,
                        ChangedDate = changedDate,
                        UserId = user.Id
                    };
                    db.TicketHistories.Add(ticketHistorys);


                    foreach (var item in ticket.AssignTicketUsers.ToList())
                    {
                        notification.TicketId = ticket.Id;
                        notification.CreatorUserId = user.Id;
                        notification.Creator = user;
                        notification.RecipientUserId = item.Id;
                        notification.Recipient = db.Users.Find(item.UserName);
                        notification.Change = "Changed Attachment";
                        notification.DateNotified = changedDate;


                        db.Notifications.Add(notification);
                        db.SaveChanges();
                    }

                }

                var fetched = db.Tickets.Find(ticket.Id);
                fetched.UserId = ticket.UserId;
                fetched.PriorityId = ticket.PriorityId;
                fetched.TypeId = ticket.TypeId;
                fetched.StatusId = ticket.StatusId;
                fetched.Attachment = ticket.Attachment;
                fetched.Title = ticket.Title;
                fetched.Description = ticket.Description;

                
                // restricting the valid file formats to images only
                if (Ticket.ImageUploadValidator.IsWebFriendlyImage(fileUpload))
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    fileUpload.SaveAs(Path.Combine(Server.MapPath("~/img/"), fileName));
                    fetched.Attachment = "~/img/" + fileName;

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
