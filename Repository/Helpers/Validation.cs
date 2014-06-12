using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using Repository.Filters;
using Repository.Models;
using System.DirectoryServices;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Drawing;
using System.IO;

namespace Repository.Helpers
{
    public class Validation
    {
        public bool ValidateUser(string userName, string password)
        {
            bool validation;
            try
            {
                LdapConnection ldc = new LdapConnection(new LdapDirectoryIdentifier("webmail.etf.unsa.ba", 389));
                NetworkCredential nc = new NetworkCredential("uid=" + userName + ",ou=people,dc=etf,dc=unsa,dc=ba", password);
                ldc.Credential = nc;
                ldc.SessionOptions.SecureSocketLayer = false;
                ldc.SessionOptions.ProtocolVersion = 3;
                ldc.AuthType = AuthType.Basic;
                ldc.Bind(nc); // user has authenticated at this point, as the credentials were used to login to the dc.                
                string[] atrributes = new[] { "givenName", "sn", "mail" };
                SearchRequest request = new SearchRequest("ou=people,dc=etf,dc=unsa,dc=ba", "uid=" + userName, System.DirectoryServices.Protocols.SearchScope.Subtree, atrributes);
                SearchResponse response = (SearchResponse)ldc.SendRequest(request);
                String Firstname = "";
                String Lastname = "";
                String Email = "";
                foreach (SearchResultEntry entry in response.Entries)
                {
                    Firstname = entry.Attributes["givenName"][0].ToString();
                    Lastname = entry.Attributes["sn"][0].ToString();
                    Email = entry.Attributes["mail"][0].ToString();
                    break;
                }
                var database = new Models.digital_libraryEntities();
                var user = database.users.Where(u => u.username == userName).FirstOrDefault();
                if (user != null) return true;
                user = new Models.users();
                user.active = true;
                user.email = Email;
                user.first_name = Firstname;
                user.last_name = Lastname;
                user.username = userName;
                user.type = 2;
                MemoryStream stream = new MemoryStream();
                System.Drawing.Bitmap bitmap1 = Repository.Properties.Resources.user;
                Image im = (Image)bitmap1;
                im.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                byte[] bytes = stream.ToArray();
                user.image = bytes;
                database.users.Add(user);
                database.SaveChanges();
                validation = true;
            }
            catch (Exception e)
            {
                validation = false;
            }
            return validation;
        }

        public byte[] GetProfilePic(string username)
        {
            var userImage = new byte[] { };
            var database = new Models.digital_libraryEntities();
            var user = database.users.Where(us => us.username == username).FirstOrDefault();
            if (user.image == null)
            {
                MemoryStream stream = new MemoryStream();
                Image im = Image.FromFile("C:/Users/almira/Source/Repos/repository/Repository/Content/images/user.png");
                im.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                byte[] bytes = stream.ToArray();
                return bytes;
            }

            return user.image;
        }
    }
}