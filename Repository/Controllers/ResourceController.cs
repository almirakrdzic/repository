﻿using System;
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
        public Book Rate(double rate,int id)
        {
            var db = new Models.digital_libraryEntities();
            var book = db.books.Where(boo => boo.id == id).FirstOrDefault();
            book.rating = rate;
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
