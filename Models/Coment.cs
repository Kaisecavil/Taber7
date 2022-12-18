﻿namespace Taber7.Models
{
    public class Coment
    {
        

        public string Id { get; set; }
        public string PostId { get; set; }
        public Post Post { get; set; }
        public string Description { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedDate { get; set; }

        public Coment(string postId, string description, int rating = 0)
        {
            PostId = postId;
            Description = description;
            Rating = rating;
        }

        
        public string ToString()
        {
            return Id + " " + PostId + " " + Description + " " + Rating + " " + CreatedDate;
        }
    }
}