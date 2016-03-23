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

namespace BugTracker.Controllers
{
    [Authorize(Roles = "Admin, ProjectManager, Developer, Submitter")]
    public class NotificationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Notifications
        [HttpGet]
        [Authorize(Roles = "Admin, ProjectManager, Developer, Submitter")]
        public ActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId()); //get user Id

            if (User.IsInRole("Admin")) {
                var notifications = db.Notifications.Include(n => n.Ticket);

                return View(notifications.ToList());
            }
            if (User.IsInRole("ProjectManager"))
            {
                var projectNot = db.Notifications.Where(u => u.RecipientUserId == user.Id);

                return View(projectNot.ToList());
            }
            if (User.IsInRole("Developer"))
            {
                var developerNot = db.Notifications.Where(u => u.RecipientUserId == user.Id);

                return View(developerNot.ToList());
            }
            if (User.IsInRole("Submitter"))
            {
                var submitterNot = db.Notifications.Where(u => u.RecipientUserId == user.Id);

                return View(submitterNot.ToList());
            }

            return RedirectToAction("Dashboard", "Home");
            //var notifications = db.Notifications.Include(n => n.Ticket);
            //return View(notifications.ToList());
        }

        //// GET: Notifications/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Notification notification = db.Notifications.Find(id);
        //    if (notification == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(notification);
        //}

        //// GET: Notifications/Create
        //public ActionResult Create()
        //{
        //    ViewBag.TicketId = new SelectList(db.Tickets, "Id", "UserId");
        //    return View();
        //}

        //// POST: Notifications/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,TicketId,DateNotified,CreatorUserId,RecipientUserId,Change,Details")] Notification notification)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Notifications.Add(notification);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.TicketId = new SelectList(db.Tickets, "Id", "UserId", notification.TicketId);
        //    return View(notification);
        //}

        //// GET: Notifications/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Notification notification = db.Notifications.Find(id);
        //    if (notification == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.TicketId = new SelectList(db.Tickets, "Id", "UserId", notification.TicketId);
        //    return View(notification);
        //}

        //// POST: Notifications/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,TicketId,DateNotified,CreatorUserId,RecipientUserId,Change,Details")] Notification notification)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(notification).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.TicketId = new SelectList(db.Tickets, "Id", "UserId", notification.TicketId);
        //    return View(notification);
        //}

        // GET: Notifications/Delete/5
        [HttpGet]
        [Authorize(Roles = "Admin, ProjectManager, Developer, Submitter")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // POST: Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, ProjectManager, Developer, Submitter")]
        public ActionResult DeleteConfirmed(int id)
        {
            Notification notification = db.Notifications.Find(id);
            db.Notifications.Remove(notification);
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
