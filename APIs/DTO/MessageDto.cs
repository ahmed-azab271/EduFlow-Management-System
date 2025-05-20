namespace APIs.DTO
{
    public class MessageDto
    {
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; } 
    }
}
