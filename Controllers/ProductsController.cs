
using System;
using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;
using System.Web.Mvc;
using SmartBook.Models;

namespace SmartBook.Controllers
{
    public class ProductsController : Controller
    {
        private readonly bookstore_dbEntities db =
            new bookstore_dbEntities();

        // HIEN THI DANH SACH SACH
        public ActionResult Index(int? categoryId, string keyword)
        {
            var query = db.books
                .Include("category")
                .Include("publisher")
                .AsQueryable();

            // Tim kiem sach
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                query = query.Where(x =>
                    x.book_title.Contains(keyword) ||
                    x.ISBN.Contains(keyword));
            }

            // Loc theo the loai
            if (categoryId.HasValue)
            {
                query = query.Where(x =>
                    x.category_id == categoryId.Value);
            }

            var books = query
                .OrderBy(x => x.category_id)
                .ThenBy(x => x.book_title)
                .ToList();

            // Lay ten tac gia theo ma sach
            var authorRows =
                (from ba in db.book_authors
                 join a in db.authors
                     on ba.author_id equals a.author_id
                 select new
                 {
                     ba.book_id,
                     a.author_name
                 }).ToList();

            var authorsByBook = authorRows
                .GroupBy(x => x.book_id)
                .ToDictionary(
                    g => g.Key,
                    g => string.Join(", ",
                        g.Select(x => x.author_name)
                         .Where(x => !string.IsNullOrWhiteSpace(x))
                         .Distinct())
                );

            ViewBag.AuthorsByBook = authorsByBook;

            ViewBag.Categories = db.categories
                .OrderBy(x => x.category_name)
                .ToList();

            ViewBag.SelectedCategory = categoryId;
            ViewBag.Keyword = keyword;

            return View(books);
        }

        // XEM CHI TIET SACH
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            // 1. Lay thong tin sach
            var book = db.books
                .Include("category")
                .Include("publisher")
                .FirstOrDefault(x => x.book_id == id.Value);

            if (book == null)
            {
                return HttpNotFound();
            }

            var model = new BookDetailsViewModel
            {
                Book = book
            };

            model.Authors = (
              from ba in db.book_authors
              join a in db.authors
              on ba.author_id equals a.author_id
              where ba.book_id == id.Value
              select new AuthorDetailViewModel
    {
        AuthorId = a.author_id,
        AuthorName = a.author_name,
        Biography = a.biography,
        Nationality = a.nationality,
        ImageUrl = a.author_image_url
    }
                ).ToList();

            model.Excerpts = db.book_excerpts
                .Where(x => x.book_id == id.Value)
                .OrderBy(x => x.excerpt_id)
                .Select(x => new BookExcerptViewModel
                {
                    ExcerptId = x.excerpt_id,
                    ExcerptText = x.excerpt_text,
                    SourceNote = x.source_note
                })
                .ToList();
            ViewBag.AuthorCount = model.Authors.Count;
            ViewBag.ExcerptCount = model.Excerpts.Count;

            return View(model);
        }

        // DONG KET NOI DATABASE
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
