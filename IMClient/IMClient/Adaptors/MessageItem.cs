namespace IMClient.Adaptors
{
    public class MessageItem
    {
        public readonly string Name;
        public readonly string Content;

        public MessageItem(string name, string content)
        {
            Name = name;
            Content = content;
        }
    }
}