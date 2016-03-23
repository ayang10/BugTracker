using BugTracker.Helper;
using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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


       [Authorize]
        public ActionResult Dashboard()
        {
            DashboardViewModels model = new DashboardViewModels();

            var user = db.Users.Find(User.Identity.GetUserId()); //get user Id
            
            if (User.IsInRole("Admin"))
            {
                
                model.Projects = db.Projects.ToList();
                model.Tickets = db.Tickets.ToList();
                model.Notifications = db.Notifications.ToList();
               
                
                return View(model);

            }
            else if (User.IsInRole("ProjectManager"))
            {
                

                //var userProjects = new List<Project>();
                //var projects = db.Projects.ToList();
                
                List<Project> userProjects = (from project in db.Projects.ToList()
                                              from u in project.AssignProjectUsers
                                              where u.Id == user.Id
                                              select project).ToList();

                //foreach (var project in projects)
                //{
                //    foreach (var u in project.AssignProjectUsers)
                //    {
                //        if (u.Id == user.Id)
                //        {
                //            userProjects.Add(project);
                //        }
                //    }
                //}
                var tickets = db.Tickets.ToList();
               

                var ticketList = new List<Ticket>();
                

                List<Ticket> ticketsProject = (from project in user.Projects.ToList()
                                               from ticket in (tickets.Where(t => t.ProjectId == project.Id))
                                               where ticket.AssignedToUserId != user.Id
                                               select ticket).ToList();



                model.Notifications = db.Notifications.Where(u => u.RecipientUserId == user.Id).ToList();
                model.Projects = userProjects;
                model.Tickets = ticketsProject.ToList();
                //model.Tickets = db.Tickets.ToList();
                return View(model);

            }
            else if (User.IsInRole("Developer"))
            {
                //var userTickets = new List<Ticket>();
                //var tickets = db.Tickets.ToList();

                //var userProjects = new List<Project>();
                //var projects = db.Projects.ToList();

                List<Project> userProjects = (from project in db.Projects.ToList()
                                           from u in project.AssignProjectUsers
                                           where u.Id == user.Id
                                           select project).ToList();

                //foreach (var project in projects)
                //{
                //    foreach (var u in project.AssignProjectUsers)
                //    {
                //        if (u.Id == user.Id)
                //        {
                //            userProjects.Add(project);
                //        }
                //    }
                //}
                List<Ticket> userTickets = (from ticket in db.Tickets.ToList()
                                           from u in ticket.AssignTicketUsers
                                           where u.Id == user.Id
                                           select ticket).ToList();

                //foreach (var ticket in tickets)
                //{
                //    foreach (var u in ticket.AssignTicketUsers)
                //    {
                //        if (u.Id == user.Id)
                //        {
                //            userTickets.Add(ticket);
                //        }
                //    }
                //}
                model.Notifications = db.Notifications.Where(u => u.RecipientUserId == user.Id).ToList();
                model.Projects = userProjects;
                model.Tickets = userTickets;

                return View(model);
            }
            else if (User.IsInRole("Submitter"))
            {
                var tickets = db.Tickets.Where(t => t.UserId == user.UserName);

                model.Tickets = tickets.ToList();

                model.Notifications = db.Notifications.Where(u => u.RecipientUserId == user.Id).ToList();

                return View(model);
            }
            else
            {
                return View();
            }
        }
        [Authorize]
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
               string[] empty = { };

                editrole.SelectedRoles = editrole.SelectedRoles ?? empty;

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

