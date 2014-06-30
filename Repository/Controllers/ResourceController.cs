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
    [Authorize]
    public class ResourceController : ApiController
    {

        const string elastic_service = "http://192.168.1.13/nwt/elasticservice/elasticservice.php?";
        //string elastic_service = "http://10.102.216.70/elasticservice.php?";

        [HttpPost]

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
                _comment.text = comment.Text;
                db.comments.Add(_comment);
                db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, "success!");
            }
            return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
        }

        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<Details> Get()
        {
            DateTime today = DateTime.Now;
            List<Details> resources = new List<Details>();
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
                    string response = wb.DownloadString(elastic_service + "request=getdetail&id=" + id);
                    //string response = "{\"download\":\"localhost/nwt//elasticservice//elasticservice.php?request=getbook&id=Ey3EmpVVSvqElHMu-MNSBg\",\"imgurl\":\"localhost/nwt//elasticservice//elasticservice.php?request=getimage&id=Ey3EmpVVSvqElHMu-MNSBg\",\"detailurl\":\"localhost/nwt//elasticservice//elasticservice.php?request=getdetail&id=Ey3EmpVVSvqElHMu-MNSBg\",\"isbn\":\"04432245\",\"title\":\"Rate and Power Allocation for Multiuser OFDM: An Effective Heuristic Verified by Branch-and-Bound: \",\"description\":\"The present correspondence deals with the rate and power allocation problem in multiuser orthogonal frequency division multiple (OFDM) access systems. We first derive the solution of the single user OFDM power allocation problem explicitly for a class of general rate-power functions by means of directional derivatives. This solution is employed for both designing a new heuristic and obtaining bounds in a branch-and-bound algorithm for allocating power to subcarriers. The branch-and-bound algorithm is used for performance evaluation of our new and two known power allocation heuristics by computing the exact optimum, given the number of allocated subcarriers per user.\",\"elasticid\":\"ARV60YFERbmL-QgWnGVemg\",\"numofpages\":\"\",\"pubdate\":\"\",\"publisher\":\"Wireless Communications, IEEE Transactions on\",\"institution\":\"IEEE\"}";

                    Details result = new Details();
                    System.Web.Script.Serialization.JavaScriptSerializer converter = new System.Web.Script.Serialization.JavaScriptSerializer();
                    result = (Details)converter.Deserialize(response, typeof(Details));
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
                return comments;
            }
            comments = book.comments.Select(comment => comment.ToContract()).ToList();
            return comments;
        }


        [HttpGet]
        public IEnumerable<Resource> GetUploads(string username)
        {
            var db = new Models.digital_libraryEntities();
            var _user = db.users.Where(user => user.username == username).FirstOrDefault();
            var id = _user.id;
            List<Resource> resources = new List<Resource>();

            var _books = db.books.ToList().Where(book => book.added_by == id).ToList();
            if (_books == null)
            {
                throw new Exception("There are no books present!");
            }
            List<string> elasticIds = (from e in _books
                                       select e.elastic_id).ToList();

            foreach (string eId in elasticIds)
            {
                using (var wb = new WebClient())
                {
                    Resource resource = new Resource();
                    string response = wb.DownloadString(elastic_service + "request=getdetail&id=" + id);
                    //string response = "{\"download\":\"localhost/nwt//elasticservice//elasticservice.php?request=getbook&id=Ey3EmpVVSvqElHMu-MNSBg\",\"imgurl\":\"localhost/nwt//elasticservice//elasticservice.php?request=getimage&id=Ey3EmpVVSvqElHMu-MNSBg\",\"detailurl\":\"localhost/nwt//elasticservice//elasticservice.php?request=getdetail&id=Ey3EmpVVSvqElHMu-MNSBg\",\"isbn\":\"04432245\",\"title\":\"Rate and Power Allocation for Multiuser OFDM: An Effective Heuristic Verified by Branch-and-Bound: \",\"description\":\"The present correspondence deals with the rate and power allocation problem in multiuser orthogonal frequency division multiple (OFDM) access systems. We first derive the solution of the single user OFDM power allocation problem explicitly for a class of general rate-power functions by means of directional derivatives. This solution is employed for both designing a new heuristic and obtaining bounds in a branch-and-bound algorithm for allocating power to subcarriers. The branch-and-bound algorithm is used for performance evaluation of our new and two known power allocation heuristics by computing the exact optimum, given the number of allocated subcarriers per user.\",\"elasticid\":\"ARV60YFERbmL-QgWnGVemg\",\"numofpages\":\"\",\"pubdate\":\"\",\"publisher\":\"Wireless Communications, IEEE Transactions on\",\"institution\":\"IEEE\"}";

                    Details result = new Details();
                    System.Web.Script.Serialization.JavaScriptSerializer converter = new System.Web.Script.Serialization.JavaScriptSerializer();
                    result = (Details)converter.Deserialize(response, typeof(Details));
                    resource.details = result;
                    var book = db.books.Where(boo => boo.elastic_id == eId).FirstOrDefault();
                    resource.rating = (double)book.rating;
                    resources.Add(resource);
                }
            }

            return resources;
        }

        [HttpGet]
        public IEnumerable<Resource> GetDownloads(string username)
        {
            var db = new Models.digital_libraryEntities();

            List<Resource> resources = new List<Resource>();

            var _user = db.users.Where(user => user.username == username).FirstOrDefault();
            var id = _user.id;
            if (_user == null)
            {
                throw new Exception("User with selected username does not exist!");
            }
            var _books = _user.books1.ToList();

            if (_books == null)
            {
                throw new Exception("There are no books present!");
            }
            List<string> elasticIds = (from e in _books
                                       select e.elastic_id).ToList();

            foreach (string eId in elasticIds)
            {
                using (var wb = new WebClient())
                {
                    Resource resource = new Resource();
                    string response = wb.DownloadString(elastic_service + "request=getdetail&id=" + id);
                    //string response = "{\"download\":\"localhost/nwt//elasticservice//elasticservice.php?request=getbook&id=Ey3EmpVVSvqElHMu-MNSBg\",\"imgurl\":\"localhost/nwt//elasticservice//elasticservice.php?request=getimage&id=Ey3EmpVVSvqElHMu-MNSBg\",\"detailurl\":\"localhost/nwt//elasticservice//elasticservice.php?request=getdetail&id=Ey3EmpVVSvqElHMu-MNSBg\",\"isbn\":\"04432245\",\"title\":\"Rate and Power Allocation for Multiuser OFDM: An Effective Heuristic Verified by Branch-and-Bound: \",\"description\":\"The present correspondence deals with the rate and power allocation problem in multiuser orthogonal frequency division multiple (OFDM) access systems. We first derive the solution of the single user OFDM power allocation problem explicitly for a class of general rate-power functions by means of directional derivatives. This solution is employed for both designing a new heuristic and obtaining bounds in a branch-and-bound algorithm for allocating power to subcarriers. The branch-and-bound algorithm is used for performance evaluation of our new and two known power allocation heuristics by computing the exact optimum, given the number of allocated subcarriers per user.\",\"elasticid\":\"ARV60YFERbmL-QgWnGVemg\",\"numofpages\":\"\",\"pubdate\":\"\",\"publisher\":\"Wireless Communications, IEEE Transactions on\",\"institution\":\"IEEE\"}";

                    Details result = new Details();
                    System.Web.Script.Serialization.JavaScriptSerializer converter = new System.Web.Script.Serialization.JavaScriptSerializer();
                    result = (Details)converter.Deserialize(response, typeof(Details));
                    resource.details = result;
                    var book = db.books.Where(boo => boo.elastic_id == eId).FirstOrDefault();
                    resource.rating = (double)book.rating;
                    resources.Add(resource);
                }
            }

            return resources;
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

        [HttpGet]
        public Resource Get(string id)
        {
            Resource resource = new Resource();
            string response;
            using (var wb = new WebClient())
            {
                response = wb.DownloadString(elastic_service + "request=getdetail&id=" + id);
                // response = "{\"download\":\"localhost/nwt//elasticservice//elasticservice.php?request=getbook&id=Ey3EmpVVSvqElHMu-MNSBg\",\"imgurl\":\"localhost/nwt//elasticservice//elasticservice.php?request=getimage&id=Ey3EmpVVSvqElHMu-MNSBg\",\"detailurl\":\"localhost/nwt//elasticservice//elasticservice.php?request=getdetail&id=Ey3EmpVVSvqElHMu-MNSBg\",\"isbn\":\"04432245\",\"title\":\"Rate and Power Allocation for Multiuser OFDM: An Effective Heuristic Verified by Branch-and-Bound: \",\"description\":\"The present correspondence deals with the rate and power allocation problem in multiuser orthogonal frequency division multiple (OFDM) access systems. We first derive the solution of the single user OFDM power allocation problem explicitly for a class of general rate-power functions by means of directional derivatives. This solution is employed for both designing a new heuristic and obtaining bounds in a branch-and-bound algorithm for allocating power to subcarriers. The branch-and-bound algorithm is used for performance evaluation of our new and two known power allocation heuristics by computing the exact optimum, given the number of allocated subcarriers per user.\",\"elasticid\":\"ARV60YFERbmL-QgWnGVemg\",\"numofpages\":\"\",\"pubdate\":\"\",\"publisher\":\"Wireless Communications, IEEE Transactions on\",\"institution\":\"IEEE\"}";

                Details result = new Details();
                System.Web.Script.Serialization.JavaScriptSerializer converter = new System.Web.Script.Serialization.JavaScriptSerializer();
                result = (Details)converter.Deserialize(response, typeof(Details));
                resource.details = result;
            }
            var db = new Models.digital_libraryEntities();
            var book = db.books.Where(boo => boo.elastic_id == id).FirstOrDefault();
            if (book == null)
            {
                books b = new books();
                b.elastic_id = id;
                var user = db.users.Where(us => us.username == User.Identity.Name).FirstOrDefault();
                b.added_by = user.id;
                b.date = DateTime.Now;
                b.ratingpeople = 0;
                b.ratingscore = 0;
                b.rating = 0;
                db.books.Add(b);
                db.SaveChanges();
            }
            book = db.books.Where(boo => boo.elastic_id == id).FirstOrDefault();
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
        public EResult SearchBooks(string query, string field)
        {
            EResult result = new EResult();
            string downloadString = "";

            //string elastic_service = "http://192.168.1.13/nwt/elasticservice/elasticservice.php?";
            //string elastic_service = "http://10.102.216.70/elasticservice.php?";

            switch (field)
            {
                case "Data":
                    {
                        downloadString = elastic_service + "request=search&query=" + query;
                        break;
                    }
                case "Title":
                    {
                        downloadString = elastic_service + "request=search&query=title:" + query;
                        break;
                    }
                case "Author":
                    {
                        downloadString = elastic_service + "request=search&query=author:" + query;
                        break;
                    }
                case "Podaci":
                    {
                        downloadString = elastic_service + "request=search&query=" + query;
                        break;
                    }
                case "Naslov":
                    {
                        downloadString = elastic_service + "request=search&query=title:" + query;
                        break;
                    }
                case "Autor":
                    {
                        downloadString = elastic_service + "request=search&query=author:" + query;
                        break;
                    }
                default:
                    {
                        return result;
                    }

            }

            string response;
            using (var wb = new WebClient())
            {

                response = wb.DownloadString(downloadString);
                //response = "{\"took\":30,\"timed_out\":false,\"num_results\":17,\"results\":[{\"id\":\"ARV60YFERbmL-QgWnGVemg\",\"download\":\"localhost/nwt//elasticservice//elasticservice.php?request=getbook&id=Ey3EmpVVSvqElHMu-MNSBg\",\"imgurl\":\"localhost/nwt//elasticservice//elasticservice.php?request=getimage&id=Ey3EmpVVSvqElHMu-MNSBg\",\"detailurl\":\"localhost/nwt//elasticservice//elasticservice.php?request=getdetail&id=Ey3EmpVVSvqElHMu-MNSBg\",\"score\":0.1530751,\"isbn\":\"04432234\",\"highlight\":\"I E E E TRANSACTIONS ON\n\n<em>WIRELESS</em> C O M M U N I C AT I O N S\nA PUBLICATION OF THE IEEE COMMUNICATIONS\"}]}";

                System.Web.Script.Serialization.JavaScriptSerializer converter = new System.Web.Script.Serialization.JavaScriptSerializer();
                EResult result1 = (EResult)converter.Deserialize(response, typeof(EResult));

                result.results = new List<EResource>();
                foreach (EResource resource in result1.results)
                {
                    response = wb.DownloadString(resource.detailurl);
                    //response = "{\"download\":\"localhost/nwt//elasticservice//elasticservice.php?request=getbook&id=Ey3EmpVVSvqElHMu-MNSBg\",\"imgurl\":\"localhost/nwt//elasticservice//elasticservice.php?request=getimage&id=Ey3EmpVVSvqElHMu-MNSBg\",\"detailurl\":\"localhost/nwt//elasticservice//elasticservice.php?request=getdetail&id=Ey3EmpVVSvqElHMu-MNSBg\",\"isbn\":\"04432245\",\"title\":\"Rate and Power Allocation for Multiuser OFDM: An Effective Heuristic Verified by Branch-and-Bound: \",\"description\":\"The present correspondence deals with the rate and power allocation problem in multiuser orthogonal frequency division multiple (OFDM) access systems. We first derive the solution of the single user OFDM power allocation problem explicitly for a class of general rate-power functions by means of directional derivatives. This solution is employed for both designing a new heuristic and obtaining bounds in a branch-and-bound algorithm for allocating power to subcarriers. The branch-and-bound algorithm is used for performance evaluation of our new and two known power allocation heuristics by computing the exact optimum, given the number of allocated subcarriers per user.\",\"elasticid\":\"ARV60YFERbmL-QgWnGVemg\",\"numofpages\":\"\",\"pubdate\":\"\",\"publisher\":\"Wireless Communications, IEEE Transactions on\",\"institution\":\"IEEE\"}";

                    Details details = (Details)converter.Deserialize(response, typeof(Details));
                    resource.title = details.title;
                    result.results.Add(resource);
                }
            }

            return result;
        }

        [HttpGet]

        public void insertBook(string id)
        {
            books book = new books();
            book.elastic_id = id;
            book.ratingpeople = 0;
            book.ratingscore = 0;
            book.rating = 0;
            book.date = DateTime.Now;

            var db = new digital_libraryEntities();
            var user = db.users.Where(us => us.username == User.Identity.Name).FirstOrDefault();
            book.added_by = user.id;
            db.books.Add(book);
            db.SaveChanges();
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