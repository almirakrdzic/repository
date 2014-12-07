using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Repository.Models;
namespace Repository.Controllers
{
    public static class Converters
    {
        public static Book ToContract(this books book)
        {
            var newBook = new Book()
            {
                Id = book.id,
                AddedBy = new User()
                {
                    Id = book.added_by,
                    FirstName = book.users.first_name,
                    LastName = book.users.last_name,
                    Username = book.users.username
                },
                Description = book.description,
                Edition = book.edition,
                ISBN = book.isbn,
                Title = book.title,
                Rating = (double)book.rating,
                Datum = (DateTime)book.date,
                People = (int)book.ratingpeople
            };
            return newBook;
        }


        public static Author ToContract(this authors author)
        {
            Author newAuthor = new Author()
            {
                Id = author.id,
                FirstName = author.first_name,
                LastName = author.last_name
            };
            return newAuthor;
        }

        public static Comment ToContract(this comments comment)
        {
            Comment newComment = new Comment()
            {
                Id = comment.id,
                Text = comment.text,
                IdUser = new User()
                {
                    Id = comment.idUser,
                    FirstName = comment.users.first_name,
                    LastName = comment.users.last_name,
                    Image = System.Convert.ToBase64String(comment.users.image)
                },
                IdBook = comment.idBook
            };
            return newComment;
        }

        public static User ToContract(this users user)
        {
            User newUser = new User()
            {
                Id = user.id,
                Username = user.username,
                FirstName = user.first_name,
                LastName = user.last_name,
                Email = user.email,
                Image = System.Convert.ToBase64String(user.image),
                Password = user.password              

            };
            return newUser;
        }

        public static UserType ToContract(this user_types usertype)
        {
            UserType utype = new UserType()
            {
                Id = usertype.id,
                Name = usertype.name
            };

            return utype;
        }

        public static Genre ToContract(this genre genre)
        {
            Genre ugenre = new Genre()
            {
                Id = genre.id,
                Name = genre.name
            };

            return ugenre;
        }

        public static users ToModel(this User user)
        {
            users newUser = new users()
            {
                id = user.Id,
                username = user.Username,
                first_name = user.FirstName,
                last_name = user.LastName,
                email = user.Email,
                password = user.Password,
                confirmationToken = user.ConfirmationToken,
                isConfirmed = user.IsConfirmed,
                active = user.IsActive,
                type = user.Type.Id
            };
            return newUser;
        }
    }
}