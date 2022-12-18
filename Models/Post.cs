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
        public string CliveHanger { get; set; }
        public string Tags { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedDate { get; set;}


        public Post( string title, string cliveHanger, string tags, string html, string applicationUserId, int rating = 0)
        {
            Title = title;
            CliveHanger = cliveHanger;
            Tags = tags;
            Html = html;
            ApplicationUserId = applicationUserId;
            Rating = rating;
        }
    }
}
