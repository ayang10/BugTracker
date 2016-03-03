using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BugTracker.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
           

            return View();
        }

        public ActionResult LoginNewUsername()
        {
            //ViewBag.RoleName = new SelectList(Roles.GetAllRoles(), "RoleName");

            return View();
        }

     
        public async Task <ActionResult> EditUserRole()
        {

            //ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");


            return View();
        }
        
    }
}