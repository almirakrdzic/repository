using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Repository.Models;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Security;
using Microsoft.Security.Application;
using Repository.Filters;

namespace Repository.Controllers
{
    [Authorize]
    public class ResourceController : ApiController
    {
        [HttpPost]
        [AntiForgeryValidate]
        public HttpResponseMessage AddKomentar(Comment comment)
        {      
            
            
            if (ModelState.IsValid)
            {
                var db = new Models.digital_libraryEntities();
                var user = db.users.Where(us => us.username == User.Identity.Name).FirstOrDefault();
                var _comment = new comments();
                _comment.active = true;
                _comment.idBook = comment.IdBook;
                _comment.idUser = user.id;
                _comment.text = Sanitizer.GetSafeHtmlFragment(comment.Text);
                db.comments.Add(_comment);
                db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, "success!");
            }
            return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
        }

        [HttpGet]
        [AllowAnonymous]
        [AntiForgeryValidate]
        public IEnumerable<Book> Get()
        {
            var db = new Models.digital_libraryEntities();
            var data = db.books.ToList().Where(book => DateTime.Now.Subtract((DateTime)book.date).TotalDays < 7).ToList();
            return data.Any() ? data.Select(book => book.ToContract()).ToList() : new List<Book>();
        }



        [HttpGet]
        [AntiForgeryValidate]
        public IEnumerable<Comment> GetCommentsForBook(int id)
        {
            var db = new Models.digital_libraryEntities();

            books book = db.books.Where(boo => boo.id == id).FirstOrDefault();

            return (book != null && book.comments != null && book.comments.Any())
                ? book.comments.Select(cooment => cooment.ToContract()).ToList()
                : new List<Comment>();
        }


        [HttpGet]
        [AntiForgeryValidate]
        public IEnumerable<Book> GetUploads(int userId)
        {
            List<Book> resources = new List<Book>();

            var db = new Models.digital_libraryEntities();
            users user = db.users.Where(u => u.id == userId).FirstOrDefault();

            if (user.books1 != null)
            {
                resources = user.books1.Select(book => book.ToContract()).ToList();
            }
           
            return resources;
        }

        [HttpGet]
        [AntiForgeryValidate]
        public IEnumerable<Book> GetDownloads(int userId)
        {
            List<Book> resources = new List<Book>();

            var db = new Models.digital_libraryEntities();
            users user = db.users.Where(u => u.id == userId).FirstOrDefault();

            if (user.books != null)
            {
                resources = user.books.Select(book => book.ToContract()).ToList();
            }
           
            return resources;
        }


        [HttpGet]
        [AntiForgeryValidate]
        public Double Rate(int rate, int id)
        {
            var db = new Models.digital_libraryEntities();
            var book = db.books.Where(boo => boo.id == id).FirstOrDefault();
            book.ratingpeople = book.ratingpeople + 1;
            book.ratingscore = book.ratingscore + rate;
            book.rating = book.ratingscore / book.ratingpeople;
            db.SaveChanges();
            return (double)(book.ratingscore / book.ratingpeople);
        }

        [HttpGet]
        [AntiForgeryValidate]
        public Book Get(int id)
        {
            var db = new Models.digital_libraryEntities();
            var book = db.books.Where(boo => boo.id == id).FirstOrDefault();

            return book != null ? book.ToContract() : new Book();
        }

        [HttpGet]
        [AntiForgeryValidate]
        public User GetU(int id)
        {
            var db = new Models.digital_libraryEntities();
            var book = db.books.Where(boo => boo.id == id).FirstOrDefault();
            var idAB = book.added_by;
            var user1 = db.users.Where(us => us.id == idAB).FirstOrDefault();
            var newUser = user1.ToContract();
            return newUser;
        }

        [HttpGet]
        [AntiForgeryValidate]
        public List<Book> SearchBooks(string query, string field)
        {
            query = Sanitizer.GetSafeHtmlFragment(query);

            var db = new Models.digital_libraryEntities();
            List<Book> results = new List<Book>();
 
            switch (field)
            {
                
                case "Title": case "Naslov":
                    {
                        results = db.books.Where(boo => boo.title.Contains(query)).ToList().Select(book => book.ToContract()).ToList();
                        break;
                    }
                case "Description": case "Opis":
                    {
                        results = db.books.Where(boo => boo.description.Contains(query)).ToList().Select(book => book.ToContract()).ToList();
                        break;
                    }
            }

            return results;
        }
       
        [HttpGet]
        [AntiForgeryValidate]
        public List<string> translateText(string text)
        {
            text = Sanitizer.GetSafeHtmlFragment(text);
            List<string> response = new List<string>();
            using (var wb = new WebClient())
            {
                string res = wb.DownloadString("https://translate.yandex.net/api/v1.5/tr.json/translate?key=trnsl.1.1.20140611T230811Z.9f391bd1f7c3d5dd.b57e755a9f330e12a3243f2900b496cbd1d3a2e1&lang=bs-en&text=" + System.Web.HttpUtility.UrlEncode(text));
                string translation = "";
                System.Web.Script.Serialization.JavaScriptSerializer converter = new System.Web.Script.Serialization.JavaScriptSerializer();
                Translation result = (Translation)converter.Deserialize(res, typeof(Translation));
                foreach (string s in result.text)
                    translation = translation + s;
                response.Add(translation);
            }
            return response;
        }

        [HttpPost]
        [AntiForgeryValidate]
        public async Task<HttpResponseMessage> AddBook()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);

                var db = new Models.digital_libraryEntities();
                books book = new books();

                book.date = DateTime.Now;
                book.title = provider.FormData.GetValues("Title").FirstOrDefault();
                book.description = provider.FormData.GetValues("Description").FirstOrDefault();
                book.edition = provider.FormData.GetValues("Edition").FirstOrDefault();
                book.isbn = provider.FormData.GetValues("ISBN").FirstOrDefault();
                book.path = provider.FileData != null && provider.FileData.Any()
                    ? provider.FileData.First().LocalFileName
                    : null;
                book.active = true;
                book.rating = 0;
                book.ratingpeople = 0;
                book.ratingscore = 0;
                book.added_by = db.users.Where(us => us.username == User.Identity.Name).FirstOrDefault().id;

                db.books.Add(book);
                db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [HttpGet]
        [AntiForgeryValidate]
        public HttpResponseMessage DownloadBook(int id)
        {
            var db = new Models.digital_libraryEntities();
            books book = db.books.Where(boo => boo.id == id).FirstOrDefault();

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(File.ReadAllBytes(book.path));
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = book.title;
            return result;
           

        }
    }
}