
using System.Collections.Generic;

namespace SmartBook.Models
{
    // info chi tiết 1 cuốn
    public class BookDetailsViewModel
    {
        public book Book { get; set; }

        public List<AuthorDetailViewModel> Authors { get; set; }

        public List<BookExcerptViewModel> Excerpts { get; set; }

        public BookDetailsViewModel()
        {
            Authors = new List<AuthorDetailViewModel>();
            Excerpts = new List<BookExcerptViewModel>();
        }
    }

    // info tác giả
    public class AuthorDetailViewModel
    {
        public int AuthorId { get; set; }

        public string AuthorName { get; set; }

        public string Biography { get; set; }

        public string Nationality { get; set; }

        public string ImageUrl { get; set; }
    }

    // trích đoạn của sách
    public class BookExcerptViewModel
    {
        public int ExcerptId { get; set; }

        public string ExcerptText { get; set; }

        public string SourceNote { get; set; }
    }
}
