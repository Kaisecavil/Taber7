using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Taber7.Areas.Identity.Data;

namespace Taber7.Models
{
    public class Post
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Html { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedDate { get; set;}


        public Post(string title, string html, string applicationUserId, int rating = 0)
        {
            Title = title;
            Html = html;
            ApplicationUserId = applicationUserId;
            Rating = rating;
        }
    }
}
