using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
            this.AssignTicketUsers = new HashSet<ApplicationUser>();
        }

        public int Id { get; set; }
        public string UserId { get; set; }
        public int ProjectId { get; set; }
        public int PriorityId { get; set; }
        public int TypeId { get; set; }
        public int StatusId { get; set;}

        [AllowHtml]
        public string Description { get; set; }
        public string Title { get; set; }

        public DateTimeOffset CreationDate { get; set; }
        public string MediaUrl { get; set; }

        //public virtual ApplicationUser EntryUser { get; set; }
        public virtual TicketPriority Priority { get; set; }
        public virtual TicketType Type { get; set; }
        public virtual TicketStatus Status { get; set; } 
        public virtual Project Project { get; set; }

        public virtual ICollection<TicketComment> TicketComments { get; set; }
        public virtual ICollection<TicketChange> TicketChanges { get; set; }

        public virtual ICollection<ApplicationUser> AssignTicketUsers { get; set; }

        private int BodyTextLimit = 900;

        public string BodyTextTrimmed
        {
            get
            {
                if (this.Description.Length > this.BodyTextLimit)
                    return this.Description.Substring(0, this.BodyTextLimit) + "...";
                else
                    return this.Description;
            }
        }


        public static class ImageUploadValidator
        {
            public static bool IsWebFriendlyImage(HttpPostedFileBase fileUpload)
            {
                //check for actual object
                if (fileUpload == null)
                    return false;

                //check size - file must be  less than 2 MB and greater than 1 KB
                if (fileUpload.ContentLength > 2 * 1024 * 1024)
                    return false;

                try
                {
                    using (var img = Image.FromStream(fileUpload.InputStream))
                    {
                        return ImageFormat.Jpeg.Equals(img.RawFormat) ||
                               ImageFormat.Png.Equals(img.RawFormat) ||
                               ImageFormat.Gif.Equals(img.RawFormat);
                    }
                }

                catch
                {

                    return false;
                }
            }
        }



    }


    public class Project
    {

        public Project()
        {
            this.AssignProjectUsers = new HashSet<ApplicationUser>();
            this.Tickets = new HashSet<Ticket>();
        }
        public int Id { get; set; }
        public string Title { get; set; }

        [AllowHtml]
        public string Description { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public string UserId { get; set; }
        
        public virtual ICollection<ApplicationUser> AssignProjectUsers { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
        //public virtual ICollection<ApplicationUser> ProjectUsers { get; set; }
      

    }

    public class AssignProjectUser
    {
        public Project Project { get; set; } 
        public MultiSelectList Users { get; set; }
        public string[] SelectedUser { get; set; }

    }

    public class AssignTicketUser
    {
        public Ticket Ticket { get; set; }
        public MultiSelectList ProjectManagerUsers { get; set; }
        public MultiSelectList DeveloperManagerUsers { get; set; }
        public string[] SelectedUser { get; set; }
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
        public string NewDeveloperId { get; set; }
        public string ChangeUserId { get; set; }
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
        public TicketComment()
        {
            this.TicketComments = new HashSet<TicketComment>();
        }

        public int Id { get; set; }
        public int TicketId { get; set; }
        public string UserId { get; set; }
        public DateTimeOffset CreationDate { get; set; }

        [AllowHtml]
        public string Body { get; set; }

        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<TicketComment> TicketComments { get; set; }

    }
     
    public class TicketPriority
    {
        public TicketPriority()
        {
            this.Tickets = new HashSet<Ticket>();
            this.Projects = new HashSet<Project>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
    }
    public class TicketType
    {
        public TicketType()
        {
            this.Tickets = new HashSet<Ticket>();
            this.Projects = new HashSet<Project>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
    }
    public class TicketStatus
    {
        public TicketStatus()
        {
            this.Tickets = new HashSet<Ticket>();
            this.Projects = new HashSet<Project>();
        }
        public int Id { get; set; }
        public string Name { get; set; }


        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
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


    public class DashboardViewModels
    {
        public ICollection<Project> Projects { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public AssignProjectUser AssignProjectUsers { get; set; }
    }

    public class AdminEditRole
    {
        public ApplicationUser Users { get; set; }
        public MultiSelectList Roles { get; set; }
        public string[] SelectedRoles { get; set; }
    }
}