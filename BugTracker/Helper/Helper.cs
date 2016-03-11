using BugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BugTracker.Helper
{
    //Userroles
    public class UserRolesHelper
    {
        private UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

        public bool IsUserInRole(string userId, string roleName)
        {
            return manager.IsInRole(userId, roleName);

        }

        public IList<string> ListUserRoles(string userId)
        {
            return manager.GetRoles(userId);
        }

        public bool AddUserToRole(string userId, string roleName)
        {
            var result = manager.AddToRole(userId, roleName);
            return result.Succeeded;
        }

        public bool RemoveUserFromRole(string userId, string roleName)
        {
            var result = manager.RemoveFromRole(userId, roleName);
            return result.Succeeded;
        }

        public IList<ApplicationUser> UsersInRole(string roleName)
        {
            var db = new ApplicationDbContext();
            var resultList = new List<ApplicationUser>();

            foreach (var user in db.Users)
            {
                if (IsUserInRole(user.Id, roleName))
                {
                    resultList.Add(user);
                }
            }
            return resultList;
        }

        public IList<ApplicationUser> UsersNotInRole(string roleName)
        {
            var resultList = new List<ApplicationUser>();

            foreach (var user in manager.Users)
            {
                if (!IsUserInRole(user.Id, roleName))
                {
                    resultList.Add(user);
                }
            }
            return resultList;
        }
    }

    //Projects
    public class UserProjectsHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public bool IsUserInProject(string userId, int projectId)
        {
            return db.Users.Find(userId).Projects.Any(p => p.Id == projectId);
        }

        public IList<Project> ListUserProjects(string userId)
        {
            var user = db.Users.Find(userId);
            return user.Projects.ToList();
        }

        public bool AddUserToProject(string userId, int projectId)
        {
            var user = db.Users.Find(userId);
            var project = db.Projects.Find(projectId);
            if (!IsUserInProject(userId, projectId))
            {
                project.AssignProjectUsers.Add(user);
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RemoveUserFromProject(string userId, int projectId)
        {
            var user = db.Users.Find(userId);
            var project = db.Projects.Find(projectId);
            if (IsUserInProject(userId, projectId))
            {
                project.AssignProjectUsers.Remove(user);
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

      

        public IList<ApplicationUser> UsersInProject(int projectId)
        {
            var resultList = new List<ApplicationUser>();
            
               
            foreach (var user in db.Users.ToList())
            {
                if (IsUserInProject(user.Id, projectId))
                {
                    resultList.Add(user);
                }
            }
            return resultList;
        }

        public IList<ApplicationUser> UsersNotInProject(int projectId)
        {
            var resultList = new List<ApplicationUser>();

            foreach (var user in db.Users.ToList())
            {
                if (!IsUserInProject(user.Id, projectId))
                {
                    resultList.Add(user);
                }
            }
            return resultList;
        }

        public IEnumerable<ApplicationUser> GetApplicationUsersInRole(string roleName)
        {
            return from role in db.Roles
                   where role.Name == roleName
                   from userRoles in role.Users
                   join user in db.Users
                   on userRoles.UserId equals user.Id
                  
                   select user;
        }
    }

    //Tickets
    public class UserTicketsHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public bool IsUserInTicket(string userId, int ticketId)
        {
            return db.Users.Find(userId).Tickets.Any(p => p.Id == ticketId);
        }

        public IList<Ticket> ListUserTickets(string userId)
        {
            var user = db.Users.Find(userId);
            return user.Tickets.ToList();
        }

        public bool AddUserToTicket(string userId, int ticketId)
        {
            var user = db.Users.Find(userId);
            var ticket = db.Tickets.Find(ticketId);
            if (!IsUserInTicket(userId, ticketId))
            {
                ticket.AssignTicketUsers.Add(user);
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RemoveUserFromTicket(string userId, int ticketId)
        {
            var user = db.Users.Find(userId);
            var ticket = db.Tickets.Find(ticketId);
            if (IsUserInTicket(userId, ticketId))
            {
                ticket.AssignTicketUsers.Remove(user);
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }



        public IList<ApplicationUser> UsersInTicket(int ticketId)
        {
            var resultList = new List<ApplicationUser>();


            foreach (var user in db.Users.ToList())
            {
                if (IsUserInTicket(user.Id, ticketId))
                {
                    resultList.Add(user);
                }
            }
            return resultList;
        }

        public IList<ApplicationUser> UsersNotInTicket(int ticketId)
        {
            var resultList = new List<ApplicationUser>();

            foreach (var user in db.Users.ToList())
            {
                if (!IsUserInTicket(user.Id, ticketId))
                {
                    resultList.Add(user);
                }
            }
            return resultList;
        }

        public IEnumerable<ApplicationUser> GetApplicationUsersInRole(string roleName)
        {
            return from role in db.Roles
                   where role.Name == roleName
                   from userRoles in role.Users
                   join user in db.Users
                   on userRoles.UserId equals user.Id

                   select user;
        }
    }
 
}