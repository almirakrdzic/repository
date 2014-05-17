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

namespace Repository.Controllers
{
    public class UserController : ApiController
    {
        [HttpPost]
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
        public User UserDetails(string username)
        {
            User u = new User();
            var database = new Models.digital_libraryEntities();
            var user = database.users.Where(us => us.username == username).FirstOrDefault();
            u = user.ToContract();
            return u;
        }
 }    
}
