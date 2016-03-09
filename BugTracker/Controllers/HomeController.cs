using BugTracker.Helper;
using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BugTracker.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }

        //[Authorize(Roles = "Admin, Project Manager, Developer")]
        public ActionResult Dashboard()
        {
            DashboardViewModels model = new DashboardViewModels();

            var user = db.Users.Find(User.Identity.GetUserId()); //get user Id

            if (User.IsInRole("Admin") || (User.IsInRole("ProjectManager")))
            {
                model.Projects = db.Projects.ToList();

                return View(model);

            }


            return View();
        }

        public ActionResult LoginNewUsername()
        {
          
            return View();
        }


        [Authorize(Roles = "Admin")]
        public ActionResult ListUserRole()
        {
            

            return View(db.Users.ToList()); 
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult EditUserRoles(string Id)
        {
           
            AdminEditRole AdminModel = new AdminEditRole();
            UserRolesHelper helper = new UserRolesHelper();
            var selected = helper.ListUserRoles(Id);
            AdminModel.Roles = new MultiSelectList(db.Roles, "Name", "Name", selected);
            AdminModel.Users = db.Users.Find(Id);

            return View(AdminModel);
        }

        
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult EditUserRoles(string id, AdminEditRole editrole)
        {
            UserRolesHelper helper = new UserRolesHelper();
            if (ModelState.IsValid)
            {
               
                editrole.SelectedRoles = editrole.SelectedRoles;

                foreach (var role in db.Roles)
                {
                    if (editrole.SelectedRoles.Contains(role.Name))
                    {
                        helper.AddUserToRole(id, role.Name);
                    }
                    else
                    {
                        helper.RemoveUserFromRole(id, role.Name);
                    }
                }
                db.SaveChanges();
            }

            return RedirectToAction("ListUserRole", "Home");

        }
    }
}