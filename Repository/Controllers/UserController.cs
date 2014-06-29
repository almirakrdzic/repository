using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Transactions;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using Repository.Filters;
using Repository.Models;
using Repository.Helpers;
using System.DirectoryServices;
using System.DirectoryServices.Protocols;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace Repository.Controllers
{
   [Authorize]
    public class UserController : ApiController
    {

        [HttpPost]        
        [AllowAnonymous]
        public HttpResponseMessage Login(LoginModel user)
        {            
           
        if (this.ModelState.IsValid)
        {
            bool authenticated = false;
            try
            {
                Validation validation = new Validation();
                authenticated = validation.ValidateUser(user.Username, user.Password);               
            }
            catch
            {
            }           
            if (authenticated)
            {
                var response = this.Request.CreateResponse(HttpStatusCode.Created, true);
                FormsAuthentication.SetAuthCookie(user.Username, false);               
                return response;
            }
            return this.Request.CreateErrorResponse(HttpStatusCode.Forbidden,"");
        }
        return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
    }
        [HttpPost]
        public HttpResponseMessage SignOut()
        {           
                var response = this.Request.CreateResponse(HttpStatusCode.Created, true);              
                FormsAuthentication.SignOut();               
                return response;           
        }

        [HttpGet]
        public HttpResponseMessage IsLogged()
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated) {
                return this.Request.CreateResponse(HttpStatusCode.Created, System.Web.HttpContext.Current.User.Identity.Name);
            }
            else return this.Request.CreateResponse(HttpStatusCode.Forbidden, "");

        }

        [HttpGet]
        public UserProfileModel UserDetails(string username)
        {
            var database = new Models.digital_libraryEntities();
            var user = database.users.Where(us => us.username == username).FirstOrDefault();
          
            UserProfileModel profile = new UserProfileModel();
            profile.Username = username;
            profile.FirstName = user.first_name;
            profile.LastName = user.last_name;
            profile.Email = user.email;
            profile.Year = (int)user.year;
            profile.Department = user.department;
            profile.AboutMe = user.aboutme;
            profile.Image = Convert.ToBase64String(user.image);

            return profile;
        }

      
        [HttpPost]
        public HttpResponseMessage EditProfile(UserProfileModel profile)
        {
            if (ModelState.IsValid)
            {
                var database = new Models.digital_libraryEntities();
                var user = database.users.Where(us => us.username == profile.Username).FirstOrDefault();
                user.first_name = profile.FirstName;
                user.last_name = profile.LastName;
                user.email = profile.Email;
                user.year = profile.Year;
                user.department = profile.Department;
                user.aboutme = profile.AboutMe;

                int comaPoint = profile.Image.IndexOf(",");
                user.image = Convert.FromBase64String(profile.Image.Substring(comaPoint + 1));

                database.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, "success!");
            }

            return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
        }

       
 }    
}
