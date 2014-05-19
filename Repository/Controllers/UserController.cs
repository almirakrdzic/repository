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
        public UserProfileModel UserDetails(string username)
        {
            var database = new Models.digital_libraryEntities();
            var user = database.users.Where(us => us.username == username).FirstOrDefault();
          
            UserProfileModel profile = new UserProfileModel();
            profile.Username = username;
            profile.FirstName = user.first_name;
            profile.LastName = user.last_name;
            profile.Email = user.email;
            profile.AboutMe = "";


            return profile;
        }

      
        [HttpPost]
        public async Task<HttpResponseMessage> PostPicture()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var root = HttpContext.Current.Server.MapPath("~/Content");
            var provider = new MultipartFormDataStreamProvider(root);
            var result = await Request.Content.ReadAsMultipartAsync(provider);
            if (result.FormData["profile"] == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var data = result.FormData["profile"];
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
            UserProfileModel profile = js.Deserialize<UserProfileModel>(data);           
            //TODO: Do something with the json model which is currently a string

            var file = result.FileData[0];           

            var database = new Models.digital_libraryEntities();
            var user = database.users.Where(us => us.username == profile.Username).FirstOrDefault();
            MemoryStream stream = new MemoryStream();
            Image im = Image.FromFile(file.LocalFileName);
            im.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            user.image = stream.ToArray();          
            database.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, "success!");
        }

        [HttpPost]
        public HttpResponseMessage EditProfile(UserProfileModel profile)
        {          
           
            //TODO: Do something with the json model which is currently a string                 

            var database = new Models.digital_libraryEntities();
            var user = database.users.Where(us => us.username == profile.Username).FirstOrDefault();
            MemoryStream stream = new MemoryStream();           
            user.first_name = profile.FirstName;
            user.last_name = profile.LastName;
            user.email = profile.Email;
            database.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, "success!");
        }

       
 }    
}
