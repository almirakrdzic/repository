using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace Repository.Controllers
{
    public class ResourceController : ApiController
    {
        // GET api/resource
        [HttpGet]
        public IEnumerable<Book> Get()
        {
            DateTime today = DateTime.Now;           
            List<Book> books = new List<Book>();
            var db = new Models.digital_libraryEntities();           
            var _books = db.books.ToList().Where(book => today.Subtract((DateTime)book.date).TotalDays < 7).ToList();           
            if (_books == null)
            {
                throw new Exception("There are no books present!");
            }
            books = _books.ToList().Select(boo => boo.ToContract()).ToList();        
            
            return books;
        }

        [HttpGet]
        public IEnumerable<Comment> GetComments()
        {
            List<Comment> comments = new List<Comment>();
            var db = new Models.digital_libraryEntities();
            var _comments = db.comments;
            if (_comments == null)
            {
                throw new Exception("There are no books present!");
            }
            comments = _comments.ToList().Select(com => com.ToContract()).ToList();
            return comments;
        }


        [HttpGet]
        public IEnumerable<Author> GetAuthorsForBook(int id)
        {
            List<Author> authors = new List<Author>();
            var db = new Models.digital_libraryEntities();

            var book = db.books.Where(boo => boo.id == id).FirstOrDefault();
            if (book == null)
            {
                throw new Exception("Book with selected ID does not exist!");
            }
            authors = book.authors.Select(author => author.ToContract()).ToList();
            return authors;
        }


        public IEnumerable<Book> GetUploads(string username)
        {
            var db = new Models.digital_libraryEntities();
            var _user = db.users.Where(user => user.username==username ).FirstOrDefault();
            var id = _user.id;
            List<Book> books = new List<Book>();
            
            var _books = db.books.ToList().Where(book => book.added_by==id).ToList();
            if (_books == null)
            {
                throw new Exception("There are no books present!");
            }
            books = _books.ToList().Select(boo => boo.ToContract()).ToList();

            return books;
        }

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
        public Book Rate(int rate,int id)
        {
            var db = new Models.digital_libraryEntities();
            var book = db.books.Where(boo => boo.id == id).FirstOrDefault();
            book.ratingpeople = book.ratingpeople + 1;
            book.ratingscore = book.ratingscore + rate;
            book.rating = book.ratingscore/book.ratingpeople;
            db.SaveChanges();
            var newBook = book.ToContract();
            return newBook;
        }

        // GET api/resource/5
        [HttpGet]
        public Book Get(int id)
        {
            var db = new Models.digital_libraryEntities();          
            var book = db.books.Where(boo => boo.id == id).FirstOrDefault();
            var newBook = book.ToContract();
            return newBook;
        }

        [HttpGet]
        public User GetU(int id)
        {
            var db = new Models.digital_libraryEntities();          
            var book = db.books.Where(boo => boo.id == id).FirstOrDefault();
            var idAB = book.added_by;
            var user1 = db.users.Where(us=> us.id==idAB).FirstOrDefault();
            var newUser = user1.ToContract();
            return newUser;
        }

        [HttpGet]
        public User GetUser(int id)
        {
            var db = new Models.digital_libraryEntities();
            var user = db.users.Where(us => us.id == id).FirstOrDefault();
            var newUser = user.ToContract();
            return newUser;
        }

        // POST api/resource
        public void Post([FromBody]string value)
        {
        }

        // PUT api/resource/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/resource/5
        public void Delete(int id)
        {
        }
    }
}
