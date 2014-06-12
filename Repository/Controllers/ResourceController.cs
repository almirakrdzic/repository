using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Repository.Models;
using System.IO;


namespace Repository.Controllers
{
    public class ResourceController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage AddKomentar(string text, int idBook)
        {
            var db = new Models.digital_libraryEntities();
            var user = db.users.Where(us => us.username == User.Identity.Name).FirstOrDefault();
            var comment = new comments();
            comment.active = true;
            comment.idBook = idBook;
            comment.idUser = user.id;
            comment.text = text;
            db.comments.Add(comment);
            db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, "success!");

        }

        [HttpGet]
        public IEnumerable<EResource> Get()
        {
            DateTime today = DateTime.Now;
            List<EResource> resources = new List<EResource>();
            var db = new Models.digital_libraryEntities();
            var _data = db.books.ToList().Where(book => today.Subtract((DateTime)book.date).TotalDays < 7).ToList();
            if (_data == null)
            {
                throw new Exception("There are no books present!");
            }
            List<string> elasticIds = (from e in _data
                                       select e.elastic_id).ToList();

            foreach (string id in elasticIds)
            {
                using (var wb = new WebClient())
                {
                    string response = wb.DownloadString("http://10.102.216.70/elasticservice.php?request=search&query=*");
                    //string response = "{\"id\":\"ARV60YFERbmL-QgWnGVemg\",\"download\":\"http://10.102.216.70/elasticservice.php?request=getbook&id=ARV60YFERbmL-QgWnGVemg\",\"image\":\"http://localhost:4416/Account/getprofilepic/?username=akrdzic1\",\"score\":0.14958034,\"isbn\":\"04432234\",\"highlight\":\"I E E E TRANSACTIONS ON\r\n\r\n<b>WIRELESS</b> C O M M U N I C AT I O N S\r\nA PUBLICATION OF THE IEEE COMMUNICATIONS\",\"details\":{\"title\":\"Naslov knjige\",\"subtitle\":\"Pod naslov knjige\",\"publicationDate\":\"22.2.1998\",\"publisher\":\"izdavac\",\"description\":\"Opis knjige\",\"institution\":\"IEEE\"}}";
                    EResource result = new EResource();
                    System.Web.Script.Serialization.JavaScriptSerializer converter = new System.Web.Script.Serialization.JavaScriptSerializer();
                    result = (EResource)converter.Deserialize(response, typeof(EResource));
                    resources.Add(result);
                }
            }

            return resources;
        }



        [HttpGet]
        public IEnumerable<Comment> GetCommentsForBook(string id)
        {
            List<Comment> comments = new List<Comment>();
            var db = new Models.digital_libraryEntities();

            var book = db.books.Where(boo => boo.elastic_id == id).FirstOrDefault();
            if (book == null)
            {
                throw new Exception("Book with selected ID does not exist!");
            }
            comments = book.comments.Select(comment => comment.ToContract()).ToList();
            return comments;
        }


        [HttpGet]
        public IEnumerable<Book> GetUploads(string username)
        {
            var db = new Models.digital_libraryEntities();
            var _user = db.users.Where(user => user.username == username).FirstOrDefault();
            var id = _user.id;
            List<Book> books = new List<Book>();

            var _books = db.books.ToList().Where(book => book.added_by == id).ToList();
            if (_books == null)
            {
                throw new Exception("There are no books present!");
            }
            books = _books.ToList().Select(boo => boo.ToContract()).ToList();

            return books;
        }

        [HttpGet]
        public IEnumerable<Book> GetDownloads(string username)
        {
            List<Book> downloadedBooks = new List<Book>();
            var db = new Models.digital_libraryEntities();

            var _user = db.users.Where(user => user.username == username).FirstOrDefault();
            var id = _user.id;
            if (_user == null)
            {
                throw new Exception("User with selected username does not exist!");
            }
            downloadedBooks = _user.books1.Select(book => book.ToContract()).ToList();
            return downloadedBooks;
        }


        [HttpGet]
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

        // GET api/resource/5
        [HttpGet]
        public Resource Get(string id)
        {
            Resource resource = new Resource();
            string response;
            using (var wb = new WebClient())
            {
                response = wb.DownloadString("http://192.168.0.108/nwt/elasticservice/elasticservice.php?request=getdetail&id=" + id);
                //response = "{\"id\":\"ARV60YFERbmL-QgWnGVemg\",\"download\":\"http://10.102.216.70/elasticservice.php?request=getbook&id=ARV60YFERbmL-QgWnGVemg\",\"image\":\"http://localhost:4416/Account/getprofilepic/?username=akrdzic1\",\"score\":0.14958034,\"isbn\":\"04432234\",\"highlight\":\"I E E E TRANSACTIONS ON\r\n\r\n<b>WIRELESS</b> C O M M U N I C AT I O N S\r\nA PUBLICATION OF THE IEEE COMMUNICATIONS\",\"details\":{\"title\":\"Naslov knjige\",\"subtitle\":\"Pod naslov knjige\",\"publicationDate\":\"22.2.1998\",\"publisher\":\"izdavac\",\"description\":\"Opis knjige\",\"institution\":\"IEEE\"}}";
                Details result = new Details();
                System.Web.Script.Serialization.JavaScriptSerializer converter = new System.Web.Script.Serialization.JavaScriptSerializer();
                result = (Details)converter.Deserialize(response, typeof(Details));
                resource.details = result;
            }
            var db = new Models.digital_libraryEntities();
            var book = db.books.Where(boo => boo.elastic_id == id).FirstOrDefault();
            resource.addedby = book.users.first_name + " " + book.users.last_name;
            resource.id = book.id;
            resource.people = (int)book.ratingpeople;
            resource.rating = (double)book.rating;
            return resource;
        }

        [HttpGet]
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
        public EResult SearchBooks(string query)
        {
            EResult result = new EResult();
            string response;
            using (var wb = new WebClient())
            {
                response = wb.DownloadString("http://10.102.216.70/elasticservice.php?request=search&query=*");
                //response = "{\"took\":69,\"timed_out\":false,\"num_results\":103,\"results\":[{\"id\":\"ARV60YFERbmL-QgWnGVemg\",\"download\":\"http://10.102.216.70/elasticservice.php?request=getbook&id=ARV60YFERbmL-QgWnGVemg\",\"image\":\"http://localhost:4416/Account/getprofilepic/?username=akrdzic1\",\"score\":0.14958034,\"isbn\":\"04432234\",\"highlight\":\"I E E E TRANSACTIONS ON\r\n\r\n<b>WIRELESS</b> C O M M U N I C AT I O N S\r\nA PUBLICATION OF THE IEEE COMMUNICATIONS\",\"details\":{\"title\":\"Naslov knjige\",\"subtitle\":\"Pod naslov knjige\",\"publicationDate\":\"22.2.1998\",\"publisher\":\"izdavac\",\"description\":\"Opis knjige\",\"institution\":\"IEEE\"}},{\"id\":\"IMhdL94wT6akFvl-RU6g6Q\",\"download\":\"http://10.102.216.70/elasticservice.php?request=getbook&id=IMhdL94wT6akFvl-RU6g6Q\",\"image\":\"http://localhost:4416/Account/getprofilepic/?username=akrdzic1\",\"score\":0.10807292,\"isbn\":\"04432241\",\"highlight\":\"IEEE TRANSACTIONS ON <em>WIRELESS</em> COMMUNICATIONS, VOL. 7, NO. 1, JANUARY 2008\r\n\r\n37\r\n\r\nCross-Layer Congestion\",\"details\":{\"title\":\"Naslov knjige\",\"subtitle\":\"Pod naslov knjige\",\"publicationDate\":\"22.2.1998\",\"publisher\":\"izdavac\",\"description\":\"Opis knjige\",\"institution\":\"IEEE\"}},{\"id\":\"SjSKx3UwSzeOR7Hwo-bFlQ\",\"download\":\"http://10.102.216.70/elasticservice.php?request=getbook&id=SjSKx3UwSzeOR7Hwo-bFlQ\",\"image\":\"http://localhost:4416/Account/getprofilepic/?username=akrdzic1\",\"score\":0.10807292,\"isbn\":\"04432241\",\"highlight\":\"IEEE TRANSACTIONS ON <em>WIRELESS</em> COMMUNICATIONS, VOL. 7, NO. 1, JANUARY 2008\r\n\r\n37\r\n\r\nCross-Layer Congestion\",\"details\":{\"title\":\"Naslov knjige\",\"subtitle\":\"Pod naslov knjige\",\"publicationDate\":\"22.2.1998\",\"publisher\":\"izdavac\",\"description\":\"Opis knjige\",\"institution\":\"IEEE\"}},{\"id\":\"w9hRCYrQTOOWfYfTym5g4g\",\"download\":\"http://10.102.216.70/elasticservice.php?request=getbook&id=w9hRCYrQTOOWfYfTym5g4g\",\"image\":\"http://localhost:4416/Account/getprofilepic/?username=akrdzic1\",\"score\":0.10733522,\"isbn\":\"04432261\",\"highlight\":\"IEEE TRANSACTIONS ON <em>WIRELESS</em> COMMUNICATIONS, VOL. 7, NO. 1, JANUARY 2008\r\n\r\n193\r\n\r\nThroughput Analysis\",\"details\":{\"title\":\"Naslov knjige\",\"subtitle\":\"Pod naslov knjige\",\"publicationDate\":\"22.2.1998\",\"publisher\":\"izdavac\",\"description\":\"Opis knjige\",\"institution\":\"IEEE\"}},{\"id\":\"inOKGJ7wQeSgXuFfOOCrqg\",\"download\":\"http://10.102.216.70/elasticservice.php?request=getbook&id=inOKGJ7wQeSgXuFfOOCrqg\",\"image\":\"http://localhost:4416/Account/getprofilepic/?username=akrdzic1\",\"score\":0.10733522,\"isbn\":\"04432261\",\"highlight\":\"IEEE TRANSACTIONS ON <em>WIRELESS</em> COMMUNICATIONS, VOL. 7, NO. 1, JANUARY 2008\r\n\r\n193\r\n\r\nThroughput Analysis\",\"details\":{\"title\":\"Naslov knjige\",\"subtitle\":\"Pod naslov knjige\",\"publicationDate\":\"22.2.1998\",\"publisher\":\"izdavac\",\"description\":\"Opis knjige\",\"institution\":\"IEEE\"}},{\"id\":\"w316-6foSf2P5IdaF3oHWg\",\"download\":\"http://10.102.216.70/elasticservice.php?request=getbook&id=w316-6foSf2P5IdaF3oHWg\",\"image\":\"http://localhost:4416/Account/getprofilepic/?username=akrdzic1\",\"score\":0.10640588,\"isbn\":\"04432267\",\"highlight\":\"IEEE TRANSACTIONS ON <em>WIRELESS</em> COMMUNICATIONS, VOL. 7, NO. 1, JANUARY 2008\r\n\r\n251\r\n\r\nAsymptotic Bounds\",\"details\":{\"title\":\"Naslov knjige\",\"subtitle\":\"Pod naslov knjige\",\"publicationDate\":\"22.2.1998\",\"publisher\":\"izdavac\",\"description\":\"Opis knjige\",\"institution\":\"IEEE\"}}]}";
                System.Web.Script.Serialization.JavaScriptSerializer converter = new System.Web.Script.Serialization.JavaScriptSerializer();
                result = (EResult)converter.Deserialize(response, typeof(EResult));
            }

            return result;
        }

        [HttpGet]
        public List<string> translateText(string text)
        {
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

    }
}