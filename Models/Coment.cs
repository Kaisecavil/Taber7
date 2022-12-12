namespace Taber7.Models
{
    public class Coment
    {
        public string Id { get; set; }
        public string PostId { get; set; }
        public Post Post { get; set; }
        public string Description { get; set; }
        public int Rating { get; set; }
    }
}
