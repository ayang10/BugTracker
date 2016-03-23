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
    [Authorize(Roles = "Admin, ProjectManager")]
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
       
        // GET: Projects
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Index()
        {
            

            return View(db.Projects.ToList());
        }

        // GET: Projects/Details/5
        [HttpGet]
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Details(int? id)
        {
            UserProjectsHelper helper = new UserProjectsHelper();
            var user = db.Users.Find(User.Identity.GetUserId());

            Project getproject = db.Projects.FirstOrDefault(x => x.Id == id);

            if (User.IsInRole("ProjectManager"))
            {
                if (!helper.IsUserInProject(user.Id, getproject.Id))
                {
                    return RedirectToAction("Unauthorized", "Error");
                }
            }

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
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            

            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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

        
        
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult AssignUserView(int projectId)
        {
            
            AssignProjectUser assignprojectuser = new AssignProjectUser();
            UserProjectsHelper helper = new UserProjectsHelper();

            var users = helper.GetApplicationUsersInRole("ProjectManager");
            var developerUsers = helper.GetApplicationUsersInRole("Developer");

            var select = helper.UsersInProject(projectId).Select(i => i.Id);
            if (User.IsInRole("Admin"))
            {
                
                assignprojectuser.Users = new MultiSelectList(users, "Id", "DisplayName", select);
                assignprojectuser.DeveloperUsers = new MultiSelectList(developerUsers, "Id", "DisplayName", select);

       }
            
            assignprojectuser.Project = db.Projects.Find(projectId);

            return View(assignprojectuser);
        }

        //POST: Edit Project Users
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult AssignUserView(int projectId, AssignProjectUser assignuser)
        {
            UserProjectsHelper helper = new UserProjectsHelper();
           
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
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            UserProjectsHelper helper = new UserProjectsHelper();
            var user = db.Users.Find(User.Identity.GetUserId());

            Project getproject = db.Projects.FirstOrDefault(x => x.Id == id);

            if (User.IsInRole("ProjectManager"))
            {
                if (!helper.IsUserInProject(user.Id, getproject.Id))
                {
                    return RedirectToAction("Unauthorized", "Error");
                }
            }

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
        [Authorize(Roles = "Admin")]
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
        [HttpGet]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
