using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models
{

    public class Ticket
    {
        public Ticket()
        {
            this.TicketComments = new HashSet<TicketComment>();
            this.TicketChanges = new HashSet<TicketChange>();
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public int PriorityId { get; set; }
        public int TypeId { get; set; }
        public int StatusId { get; set;}

        [AllowHtml]
        public string Description { get; set; }
        public string Title { get; set; }

        public DateTimeOffset CreationDate { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public string MediaUrl { get; set; }

        public virtual ApplicationUser EntryUser { get; set; }
        public virtual TicketPriority TicketPriorities { get; set; }
        public virtual TicketType TicketTypes { get; set; }
        public virtual TicketStatus TicketStatuses { get; set; }
        public virtual Project Projects { get; set; }

        public virtual ICollection<TicketComment> TicketComments { get; set; }
        public virtual ICollection<TicketChange> TicketChanges { get; set; }


    }


    public class Project
    {
     
        public int Id { get; set; }
        public string Title { get; set; }

        [AllowHtml]
        public string Description { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public string UserId { get; set; }

        public virtual ApplicationUser ProjectUsers { get; set; }


    }


    public class TicketChange
    {
        public TicketChange()
        {
           
            this.Projects = new HashSet<Project>();
        }
        
        public int Id { get; set; }
        public string Title { get; set; }

        [AllowHtml]
        public string Description { get; set; }
        public int TicketId { get; set; }
        public int PriorityId { get; set; }
        public int TypeId { get; set; }
        public int StatusId { get; set; }
        public int NewDeveloperId { get; set; }
        public int ChangeUserId { get; set; }
        public DateTimeOffset ChangedDate { get; set; }

        public string MediaUrl { get; set; }

        public virtual ApplicationUser ChangeUser { get; set; } 
        public virtual Ticket Tickets { get; set; }
        public virtual TicketPriority TicketPriorities { get; set; }
        public virtual TicketType TicketTypes { get; set; }
        public virtual TicketStatus TicketStatuses { get; set; }
        public virtual ICollection<Project> Projects { get; set; }

    }

    public class TicketComment
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string UserId { get; set; }
        public DateTimeOffset CreationDate { get; set; }

        [AllowHtml]
        public string Body { get; set; }

        public virtual Ticket Tickets { get; set; }
        public virtual ApplicationUser User { get; set; }

    }
     
    public class TicketPriority
    {
        public TicketPriority()
        {
            this.Tickets = new HashSet<Ticket>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
    }
    public class TicketType
    {
        public TicketType()
        {
            this.Tickets = new HashSet<Ticket>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
    }
    public class TicketStatus
    {
        public TicketStatus()
        {
            this.Tickets = new HashSet<Ticket>();
        }
        public int Id { get; set; }
        public string Name { get; set; }


        public virtual ICollection<Ticket> Tickets { get; set; }
    }

    public class Notification
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public DateTimeOffset DateNotified { get; set; }
        public string CreatorUserId { get; set; }
        public string RecipientUserId { get; set; }
        public string Change { get; set; }

        [AllowHtml]
        public string Details { get; set; }

        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser Creator { get; set; }
        public virtual ApplicationUser Recipient { get; set; }
    }

    public class SendGridCredential
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }


    //public class UserAndRole
    //{
    //    public int Id { get; set; }
    //    public string Users { get; set; }
    //    public string Roles { get; set; }
    
    //}

    public class DashboardViewModels
    {
        public ICollection<Project> Projects { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }
}