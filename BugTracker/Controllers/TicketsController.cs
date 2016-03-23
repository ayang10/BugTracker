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
    [Authorize]
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets
        [Authorize]
        [HttpGet]
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
            else if (User.IsInRole("ProjectManager"))
            {
                var tickets = db.Tickets.Where(t => t.UserId == user.UserName).Include(t => t.Project).Include(t => t.Priority).Include(t => t.Type).Include(t => t.Status).ToList();
                return View(tickets);
            }

            else if(User.IsInRole("Admin")){

                var tickets = db.Tickets.Include(t => t.Project).Include(t => t.Priority).Include(t => t.Type).Include(t => t.Status).ToList();
                return View(tickets);

            }
            else
            {
                return View();
            }

            
            
        }

        // GET: Tickets/Details/5
        [Authorize(Roles = "Admin, ProjectManager, Developer, Submitter")]
        [HttpGet]
        public ActionResult Details(int? id)
        {
          
            UserTicketsHelper helperticket = new UserTicketsHelper();
            var user = db.Users.Find(User.Identity.GetUserId());



            Ticket userTickets = db.Tickets.FirstOrDefault(u => u.Id == id);

            

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

            if (User.IsInRole("Admin"))
            {
                return View(ticket);
            }

            if (User.IsInRole("ProjectManager"))
            {
             
                var tickets = db.Tickets.ToList();
               
                List<Ticket> ticketsProject = (from project in user.Projects.ToList()
                                               from t in (tickets.Where(t => t.ProjectId == project.Id))
                                               where t.AssignedToUserId != user.Id
                                               select t).ToList();
                
                if(!ticketsProject.Contains(userTickets))
                {
                    return RedirectToAction("Unauthorized", "Error");
                }
            }
            if (User.IsInRole("Developer"))
            {
                if (!helperticket.IsUserInTicket(user.Id, userTickets.Id))
                {
                    return RedirectToAction("Unauthorized", "Error");
                }
            }
            if (User.IsInRole("Submitter"))
            {
                if(user.UserName != userTickets.UserId)
                {
                    return RedirectToAction("Unauthorized", "Error");
                }
            }


            return View(ticket);
        }

        // GET: Tickets/Create
        [Authorize(Roles = "Admin, ProjectManager, Developer, Submitter")]
        [HttpGet]
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
        [Authorize(Roles = "Admin, ProjectManager, Developer, Submitter")]
        public ActionResult Create([Bind(Include = "Id,UserId,ProjectId,PriorityId,TypeId,StatusId,Description,Title,CreationDate,Attachment")] Ticket ticket, HttpPostedFileBase fileUpload)
        {
            ticket.CreationDate = new DateTimeOffset(DateTime.Now);

            if (ModelState.IsValid)
            {


                //// restricting the valid file formats to images only
                //if (Ticket.ImageUploadValidator.IsWebFriendlyImage(fileUpload))
                //{
                //    var fileName = Path.GetFileName(fileUpload.FileName);
                //    fileUpload.SaveAs(Path.Combine(Server.MapPath("~/img/"), fileName));
                //    ticket.Attachment = "~/img/" + fileName;

                //}

                if(fileUpload != null && fileUpload.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                        fileUpload.SaveAs(Path.Combine(Server.MapPath("~/documents/"), fileName));
                        ticket.Attachment = "~/documents/" + fileName;
                }

                var user = db.Users.Find(User.Identity.GetUserId());

                ticket.UserId = user.UserName;

                ticket.StatusId = 1;

                ticket.CreationDate = new DateTimeOffset(DateTime.Now);

                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Dashboard", "Home");
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
        [Authorize(Roles = "Admin, ProjectManager, Developer")]
        [HttpGet]
        public ActionResult Edit(int? id)
        {

            UserTicketsHelper helperticket = new UserTicketsHelper();
            var user = db.Users.Find(User.Identity.GetUserId());

            Ticket userTickets = db.Tickets.FirstOrDefault(u => u.Id == id);

           

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            

            if (User.IsInRole("ProjectManager"))
            {

                var tickets = db.Tickets.ToList();

                List<Ticket> ticketsProject = (from project in user.Projects.ToList()
                                               from t in (tickets.Where(t => t.ProjectId == project.Id))
                                               where t.AssignedToUserId != user.Id
                                               select t).ToList();

                if (!ticketsProject.Contains(userTickets))
                {
                    return RedirectToAction("Unauthorized", "Error");
                }
            }
            if (User.IsInRole("Developer"))
            {
                if (!helperticket.IsUserInTicket(user.Id, userTickets.Id))
                {
                    return RedirectToAction("Unauthorized", "Error");
                }
            }
            if (User.IsInRole("Submitter"))
            {
                if (user.UserName != userTickets.UserId)
                {
                    return RedirectToAction("Unauthorized", "Error");
                }
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
        [Authorize(Roles = "Admin, ProjectManager, Developer, Submitter")]
        public ActionResult Edit([Bind(Include = "Id,UserId,ProjectId,PriorityId,TypeId,StatusId,Description,Title,CreationDate,Attachment")] Ticket ticket, HttpPostedFileBase fileUpload)
        {
            

            if (ModelState.IsValid)
            {
                UserTicketsHelper helper = new UserTicketsHelper();
                var user = db.Users.Find(User.Identity.GetUserId());
                var OldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);
               
                var changedDate = DateTimeOffset.Now;
        

                ticket.AssignTicketUsers = helper.UsersInTicket(ticket.Id);
                Notification notification = new Notification();
                TicketHistory ticketHistorys = new TicketHistory();

                if (OldTicket.Title != ticket.Title)
                {

                    ticketHistorys.TicketId = ticket.Id;
                    ticketHistorys.Property = "Title";
                      ticketHistorys.Old = OldTicket.Title;
                      ticketHistorys.OldValue = OldTicket.Title;
                      ticketHistorys.New = ticket.Title;
                     ticketHistorys.NewValue = ticket.Title;
                   ticketHistorys.ChangedDate = changedDate;
                     ticketHistorys.UserId = user.Id;
                   
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

                    ticketHistorys.TicketId = ticket.Id;
                    ticketHistorys.Property = "Description";
                    ticketHistorys.Old = OldTicket.Description;
                    ticketHistorys.OldValue = OldTicket.Description;
                    ticketHistorys.New = ticket.Description;
                    ticketHistorys.NewValue = ticket.Description;
                    ticketHistorys.ChangedDate = changedDate;
                       ticketHistorys.UserId = user.Id;
                    
                    db.TicketHistories.Add(ticketHistorys);


                    foreach (var item in ticket.AssignTicketUsers.ToList())
                    {
                        notification.TicketId = ticket.Id;
                        notification.CreatorUserId = user.UserName;
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
                    ticket.Priority = db.TicketPriorities.Find(ticket.PriorityId);


                    ticketHistorys.TicketId = ticket.Id;
                     ticketHistorys.Property = "Priority";
                      ticketHistorys.Old = OldTicket.Priority.Name;
                      ticketHistorys.OldValue = OldTicket.Priority.Name;
                     ticketHistorys.New = ticket.Priority.Name;
                     ticketHistorys.NewValue = ticket.Priority.Name;
                     ticketHistorys.ChangedDate = changedDate;
                      ticketHistorys.UserId = user.Id;
                
                    
                    db.TicketHistories.Add(ticketHistorys);
                    db.SaveChanges();

                    foreach (var item in ticket.AssignTicketUsers.ToList())
                    {
                        notification.TicketId = ticket.Id;
                        notification.CreatorUserId = user.UserName;
                        notification.Creator = user;
                        notification.RecipientUserId = item.Id;
                        notification.Recipient = db.Users.Find(item.UserName);
                        notification.Change = "Changed Priority";
                        notification.DateNotified = changedDate;
                        
                        db.Notifications.Add(notification);
                    
                    }

                }

                if (OldTicket.StatusId != ticket.StatusId)
                {
                    ticket.Status = db.TicketStatuses.Find(ticket.StatusId);


                    ticketHistorys.TicketId = ticket.Id;
                     ticketHistorys.Property = "Status";
                      ticketHistorys.Old = OldTicket.Status.Name;
                      ticketHistorys.OldValue = OldTicket.Status.Name;
                      ticketHistorys.New = ticket.Status.Name;
                      ticketHistorys.NewValue = ticket.Status.Name;
                      ticketHistorys.ChangedDate = changedDate;
                      ticketHistorys.UserId = user.Id;
                  
                    db.TicketHistories.Add(ticketHistorys);


                    foreach (var item in ticket.AssignTicketUsers.ToList())
                    {
                        notification.TicketId = ticket.Id;
                        notification.CreatorUserId = user.UserName;
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
                    ticket.Type = db.TicketTypes.Find(ticket.TypeId);

                    ticketHistorys.TicketId = ticket.Id;
                    ticketHistorys.Property = "Type";
                    ticketHistorys.Old = OldTicket.Type.Name;
                    ticketHistorys.OldValue = OldTicket.Type.Name;
                    ticketHistorys.New = ticket.Type.Name;
                    ticketHistorys.NewValue = ticket.Type.Name;
                    ticketHistorys.ChangedDate = changedDate;
                      ticketHistorys.UserId = user.Id;
                  
                    db.TicketHistories.Add(ticketHistorys);


                    foreach (var item in ticket.AssignTicketUsers.ToList())
                    {
                        notification.TicketId = ticket.Id;
                        notification.CreatorUserId = user.UserName;
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

                    ticketHistorys.TicketId = ticket.Id;
                    ticketHistorys.Property = "Attachment";
                    ticketHistorys.Old = OldTicket.Attachment;
                    ticketHistorys.OldValue = OldTicket.Attachment;
                    ticketHistorys.New = ticket.Attachment;
                    ticketHistorys.NewValue = ticket.Attachment;
                    ticketHistorys.ChangedDate = changedDate;
                    ticketHistorys.UserId = user.Id;
                   
                    db.TicketHistories.Add(ticketHistorys);


                    foreach (var item in ticket.AssignTicketUsers.ToList())
                    {
                        notification.TicketId = ticket.Id;
                        notification.CreatorUserId = user.UserName;
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
                //if (Ticket.ImageUploadValidator.IsWebFriendlyImage(fileUpload))
                //{
                //    var fileName = Path.GetFileName(fileUpload.FileName);
                //    fileUpload.SaveAs(Path.Combine(Server.MapPath("~/img/"), fileName));
                //    fetched.Attachment = "~/img/" + fileName;

                //}
                if (fileUpload != null && fileUpload.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    fileUpload.SaveAs(Path.Combine(Server.MapPath("~/documents/"), fileName));
                    ticket.Attachment = "~/documents/" + fileName;
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
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpGet]
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
        [Authorize(Roles = "Admin, ProjectManager")]
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
