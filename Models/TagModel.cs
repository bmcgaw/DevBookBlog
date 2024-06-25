namespace DevBook.Models
{
    public class TagModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public ICollection<PostTagModel>? PostTags { get; set; }
    }
}
