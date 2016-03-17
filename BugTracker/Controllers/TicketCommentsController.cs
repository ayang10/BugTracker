using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;

namespace BugTracker.Controllers
{
    public class TicketCommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TicketComments
        public ActionResult Index()
        {
            if (User.IsInRole("Admin") || User.IsInRole("ProjectManager") || User.IsInRole("Developer") || User.IsInRole("Submitter"))
            {
                var ticketComments = db.TicketComments.Include(t => t.Ticket).Include(t => t.User);
                return View(ticketComments.ToList());
            }

            return RedirectToAction("Error", "Shared");
        }

        // GET: TicketComments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }
            if (User.Identity.Name != ticketComment.User.UserName)
            {
                return RedirectToAction("Index");
            }
            return View(ticketComment);
        }

        // GET: TicketComments/Create
        public ActionResult Create(int id)
        {
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "UserId");
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName");

            TicketComment model = new TicketComment { TicketId = id };

            return View(model);
        }

        // POST: TicketComments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TicketId,UserId,CreationDate,Body")] TicketComment ticketComment)
        {
            ticketComment.CreationDate = new DateTimeOffset(DateTime.Now);

            if (ModelState.IsValid)
            {
                ticketComment.CreationDate = new DateTimeOffset(DateTime.Now);
                ticketComment.UserId = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name).Id;

                db.TicketComments.Add(ticketComment);
                db.SaveChanges();
                return RedirectToAction("Details", "Tickets", new { id = ticketComment.TicketId});


            }

            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "UserId", ticketComment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketComment.UserId);
            return View(ticketComment);
        }

        // GET: TicketComments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }
            if (User.Identity.Name != ticketComment.User.UserName)
            {
                return RedirectToAction("Index");
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "UserId", ticketComment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketComment.UserId);
            return View(ticketComment);
        }

        // POST: TicketComments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TicketId,UserId,CreationDate,Body")] TicketComment ticketComment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketComment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "UserId", ticketComment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketComment.UserId);
            return View(ticketComment);
        }

        // GET: TicketComments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }
            if (User.Identity.Name != ticketComment.User.UserName)
            {
                return RedirectToAction("Index");
            }
            return View(ticketComment);
        }

        // POST: TicketComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketComment ticketComment = db.TicketComments.Find(id);
            db.TicketComments.Remove(ticketComment);
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index");
            }

            db.SaveChanges();
            return RedirectToAction("Details", "TicketComments", new { id = ticketComment.TicketId});
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
