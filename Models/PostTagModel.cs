namespace DevBook.Models
{
    public class PostTagModel
    {
        public int PostId { get; set; }
        public PostModel? Post { get; set; }

        public int TagId { get; set; }
        public TagModel? Tag { get; set; }
    }
}
