using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Models;
using BookStore.Models.Repositories;
using BookStore.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookStoreRepoitory<Book> bookStoreRepoitory;
        private readonly IBookStoreRepoitory<Author> authorRepository;

        public BookController(IBookStoreRepoitory<Book> bookStoreRepoitory, IBookStoreRepoitory<Author> authorRepository)
        {
            this.bookStoreRepoitory = bookStoreRepoitory;
            this.authorRepository = authorRepository;
        }
        // GET: BookController
        public ActionResult Index()
        {
            var books = bookStoreRepoitory.List();
            return View(books);
        }

        // GET: BookController/Details/5
        public ActionResult Details(int id)
        {
            var book = bookStoreRepoitory.Find(id);
            return View(book);
        }

        // GET: BookController/Create
        public ActionResult Create()
        {
            
            return View(GetAllAuthor());
        }

        // POST: BookController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BookAuthorViewModel model)
        {
            if (ModelState.IsValid) 
            {
                try
                {
                    if (model.AuthorId == -1)
                    {
                        ViewBag.Message = "Please Select an Author from the List";
                       
                        return View(GetAllAuthor());
                    }
                    var author = authorRepository.Find(model.AuthorId);
                    Book book = new Book
                    {
                        Id = model.BookId,
                        Title = model.Title,
                        Description = model.Description,
                        Author = author
                    };
                    bookStoreRepoitory.Add(book);
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View();
                }
            }
            
            ModelState.AddModelError("", "You have to fill all the required fields");
            return View(GetAllAuthor());
        }

        // GET: BookController/Edit/5
        public ActionResult Edit(int id)
        {   
            var book = bookStoreRepoitory.Find(id);
            var authorId = book.Author == null ? book.Author.Id = 0 : book.Author.Id;
            var viewModel = new BookAuthorViewModel
            {
                BookId = book.Id,
                Title = book.Title,
                Description = book.Description,
                AuthorId = authorId,
                Authors = authorRepository.List().ToList()
            };
            return View(viewModel);
        }

        // POST: BookController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BookAuthorViewModel viewModel)
        {
            try
            {
                var author = authorRepository.Find(viewModel.AuthorId);
                Book book = new Book
                {
                    Title = viewModel.Title,
                    Description = viewModel.Description,
                    Author = author
                };
                bookStoreRepoitory.Update(viewModel.BookId, book);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BookController/Delete/5
        public ActionResult Delete(int id)
        {
            var book = bookStoreRepoitory.Find(id);
            return View(book);
        }

        // POST: BookController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmDelete(int id)
        {
            try
            {
                bookStoreRepoitory.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        List<Author> FillSelectList()
        {
            var authors = authorRepository.List().ToList();
            authors.Insert(0,new Author{Id=-1, FullName="--- Please Select an Author --- "});
            return authors;
        }
        BookAuthorViewModel GetAllAuthor()
        {
            var vmodel = new BookAuthorViewModel
            {
                Authors = FillSelectList()
            };
            return vmodel;
        }
    }
}
