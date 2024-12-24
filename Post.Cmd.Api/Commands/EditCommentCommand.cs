namespace Post.Cmd.Api.Commands
{
    public class EditCommentCommand
    {
        public Guid CommentId { get; set; }
        public string Comment { get; set; }
        public string Username { get; set; }
    }
}
