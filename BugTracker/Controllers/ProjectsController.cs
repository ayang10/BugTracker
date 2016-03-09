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
using BugTracker.Helper;

namespace BugTracker.Controllers
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects
        public ActionResult Index()
        {


            return View(db.Projects.ToList());
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            

            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,CreationDate,UserId")] Project project)
        {
            project.CreationDate = new DateTimeOffset(DateTime.Now);
            
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());

                project.CreationDate = new DateTimeOffset(DateTime.Now);

                project.UserId = user.UserName;

                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }

        
        //[Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult AssignUserView(int projectId)
        {
            
            AssignProjectUser assignprojectuser = new AssignProjectUser();
            UserProjectsHelper helper = new UserProjectsHelper();

            var selected = helper.UsersInProject(projectId).Select(i => i.Id);
            
                assignprojectuser.Users = new MultiSelectList(db.Users, "Id", "FirstName", selected);
       
            assignprojectuser.Project = db.Projects.Find(projectId);

            return View(assignprojectuser);
        }

        //POST: Edit Project Users
        [HttpPost]
        //[Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult AssignUserView(int projectId, AssignProjectUser assignuser)
        {
            UserProjectsHelper helper = new UserProjectsHelper();
            var project = db.Projects.Find(projectId);

            if (ModelState.IsValid)
            {
                string[] empty = { };
                assignuser.SelectedUser = assignuser.SelectedUser ?? empty;

                foreach (var user in db.Users)
                {
                    if (assignuser.SelectedUser.Contains(user.Id))
                    {
                        helper.AddUserToProject(user.Id, projectId);
                      
                    }
                    else
                    {
                        helper.RemoveUserFromProject(user.Id, projectId);
                      
                    }
                }
                db.SaveChanges();
            }

            return RedirectToAction("Index", "Projects");
        }

        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }
        
        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,CreationDate,UpdatedDate,UserId")] Project project)
        {
            

            if (ModelState.IsValid)
            {
                
                
                var fetched = db.Projects.Find(project.Id);

                fetched.Title = project.Title;
                fetched.Description = project.Description;
                fetched.CreationDate = project.CreationDate;
                fetched.UserId = project.UserId;
              

                db.Entry(fetched).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }
       

       

        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
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
