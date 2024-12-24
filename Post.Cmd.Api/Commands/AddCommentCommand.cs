using CQRS.Core.Commands;

namespace Post.Cmd.Api.Commands
{
    public class AddCommentCommand:BaseCommand
    {
        public string Content { get; set; }
        public string Username { get; set; }
    }
}
