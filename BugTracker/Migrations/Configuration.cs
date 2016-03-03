namespace BugTracker.Migrations
{
    using Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BugTracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(BugTracker.Models.ApplicationDbContext context)
        {
           // Assign values to TicketStatus

            if (!context.TicketStatuses.Any(s => s.Name == "Unassigned"))
            {
                TicketStatus unassigned = new TicketStatus();
                unassigned.Name = "Unassigned";
                context.TicketStatuses.Add(unassigned);
            }

            if (!context.TicketStatuses.Any(s => s.Name == "Assigned"))
            {
                TicketStatus assigned = new TicketStatus();
                assigned.Name = "Assigned";
                context.TicketStatuses.Add(assigned);
            }

            if (!context.TicketStatuses.Any(s => s.Name == "in Progress"))
            {
                TicketStatus progress = new TicketStatus();
                progress.Name = "in Progress";
                context.TicketStatuses.Add(progress);
            }

            if (!context.TicketStatuses.Any(s => s.Name == "Resolved"))
            {
                TicketStatus resolved = new TicketStatus();
                resolved.Name = "Resolved";
                context.TicketStatuses.Add(resolved);
            }
            if (!context.TicketStatuses.Any(s => s.Name == "Unresolved"))
            {
                TicketStatus unresolved = new TicketStatus();
                unresolved.Name = "Unresolved";
                context.TicketStatuses.Add(unresolved);
            }

            // Assign values to TicketPriorities

            if (!context.TicketPriorities.Any(p => p.Name == "Low"))
            {
                TicketPriority low = new TicketPriority();
                low.Name = "Low";
                context.TicketPriorities.Add(low);
            }
            if (!context.TicketPriorities.Any(p => p.Name == "Medium"))
            {
                TicketPriority medium = new TicketPriority();
                medium.Name = "Medium";
                context.TicketPriorities.Add(medium);
            }
            if (!context.TicketPriorities.Any(p => p.Name == "High"))
            {
                TicketPriority high = new TicketPriority();
                high.Name = "High";
                context.TicketPriorities.Add(high);
            }
            if (!context.TicketPriorities.Any(p => p.Name == "Urgent"))
            {
                TicketPriority urgent = new TicketPriority();
                urgent.Name = "Urgent";
                context.TicketPriorities.Add(urgent);
            }
          
            
            // Assign values to TicketTypes

            if (!context.TicketTypes.Any(t => t.Name == "Bug"))
            {
                TicketType bug = new TicketType();
                bug.Name = "Bug";
                context.TicketTypes.Add(bug);
            }
            if (!context.TicketTypes.Any(t => t.Name == "Enhancement"))
            {
                TicketType enhancement = new TicketType();
                enhancement.Name = "Enhancement";
                context.TicketTypes.Add(enhancement);
            }
            if (!context.TicketTypes.Any(t => t.Name == "Design Feature"))
            {
                TicketType designfeature = new TicketType();
                designfeature.Name = "Design Feature";
                context.TicketTypes.Add(designfeature);
            }


            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            //lamda expression, check to see if context, roles, look for any roles in the table name is "Admin"
            //if(!roleManager.RoleExists("Admin")) this if functions does the same thing
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }
            if (!context.Roles.Any(r => r.Name == "ProjectManager"))
            {
                roleManager.Create(new IdentityRole { Name = "ProjectManager" });
            }
            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }
            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }
            
            //assign me to the Admin role, if not already in it
            var uStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(uStore);

            if (userManager.FindByEmail("buggytracker1@gmail.com") == null)
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "buggytracker1@gmail.com",
                    Email = "buggytracker1@gmail.com",
                    FirstName = "A",
                    LastName = "Yang"
                }, "Password-1");
            }
            //assign me to the Admin role, if not already in it
            var userId = userManager.FindByEmail("buggytracker1@gmail.com").Id;
            if (!userManager.IsInRole(userId, "Admin"))
            {
                userManager.AddToRole(userId, "Admin");
            }

            // add bobby to the database
            if (userManager.FindByEmail("bdavis@coderfoundry") == null)
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "bdavis@coderfoundry",
                    Email = "bdavis@coderfoundry.com",
                    FirstName = "Bobby",
                    LastName = "Davis"
                }, "Password-1");
            }
            //assign me to the Admin role, if not already in it
            userId = userManager.FindByEmail("bdavis@coderfoundry.com").Id;
            if (!userManager.IsInRole(userId, "ProjectManager"))
            {
                userManager.AddToRole(userId, "ProjectManager");
            }


            // add Andrew to the database
            if (userManager.FindByEmail("ajensen@coderfoundry.com") == null)
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "ajensen@coderfoundry.com",
                    Email = "ajensen@coderfoundry.com",
                    FirstName = "Andrew",
                    LastName = "Jensen"
                }, "Password-1");
            }

            // assign Andrew to the ProjectManager role, if not already in it
            userId = userManager.FindByEmail("ajensen@coderfoundry.com").Id;
            if (!userManager.IsInRole(userId, "ProjectManager"))
            {
                userManager.AddToRole(userId, "ProjectManager");
            }


            // add Rai to the database
            if (userManager.FindByEmail("ramanglani@coderfoundry.com") == null)
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "ramanglani@coderfoundry.com",
                    Email = "ramanglani@coderfoundry.com",
                    FirstName = "Ria",
                    LastName = "Manglani"
                }, "Password-1");
            }

            // assign Ria to the ProjectManager role, if not already in it
            userId = userManager.FindByEmail("ramanglani@coderfoundry.com").Id;
            if (!userManager.IsInRole(userId, "ProjectManager"))
            {
                userManager.AddToRole(userId, "ProjectManager");
            }


            // add TJ to the database
            if (userManager.FindByEmail("tjones@coderfoundry.com") == null)
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "tjones@coderfoundry.com",
                    Email = "tjones@coderfoundry.com",
                    FirstName = "TJ",
                    LastName = "Jones"
                }, "Password-1");
            }


            // assign TJ to the ProjectManager role, if not already in it
            userId = userManager.FindByEmail("tjones@coderfoundry.com").Id;
            if (!userManager.IsInRole(userId, "ProjectManager"))
            {
                userManager.AddToRole(userId, "ProjectManager");
            }




        }
    }
}

