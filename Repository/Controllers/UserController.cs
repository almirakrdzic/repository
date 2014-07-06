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
using Microsoft.Security.Application;

namespace Repository.Controllers
{
   //[Authorize]
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
                user.Username = Sanitizer.GetSafeHtmlFragment(user.Username);
                user.Password = Sanitizer.GetSafeHtmlFragment(user.Password);
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
            username = Sanitizer.GetSafeHtmlFragment(username);
            var database = new Models.digital_libraryEntities();
            var user = database.users.Where(us => us.username == username).FirstOrDefault();
          
            UserProfileModel profile = new UserProfileModel();
            profile.Username = Sanitizer.GetSafeHtmlFragment(username);
            profile.FirstName = Sanitizer.GetSafeHtmlFragment(user.first_name);
            profile.LastName = Sanitizer.GetSafeHtmlFragment(user.last_name);
            profile.Email = Sanitizer.GetSafeHtmlFragment(user.email);
            profile.Year = (int)user.year;
            profile.Department = Sanitizer.GetSafeHtmlFragment(user.department);
            profile.AboutMe = Sanitizer.GetSafeHtmlFragment(user.aboutme);
            profile.Image = Convert.ToBase64String(user.image);

            return profile;
        }

      
        [HttpPost]
        public HttpResponseMessage EditProfile(UserProfileModel profile)
        {
            if (ModelState.IsValid)
            {
                profile.Username = Sanitizer.GetSafeHtmlFragment(profile.Username);
                var database = new Models.digital_libraryEntities();
                var user = database.users.Where(us => us.username == profile.Username).FirstOrDefault();
                user.first_name = Sanitizer.GetSafeHtmlFragment(profile.FirstName);
                user.last_name = Sanitizer.GetSafeHtmlFragment(profile.LastName);
                user.email = Sanitizer.GetSafeHtmlFragment(profile.Email);
                user.year = profile.Year;
                user.department = Sanitizer.GetSafeHtmlFragment(profile.Department);
                user.aboutme = Sanitizer.GetSafeHtmlFragment(profile.AboutMe);

                int comaPoint = profile.Image.IndexOf(",");
                user.image = Convert.FromBase64String(profile.Image.Substring(comaPoint + 1));

                database.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, "success!");
            }

            return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
        }

        [HttpGet]
        public int GetYear()
        {
            return DateTime.Now.Year;
        }
 }    
}
