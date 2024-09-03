using UniTrade.Models;

namespace UniTrade.ViewModels
{
    public class SingleMessage
    {
        public SingleMessage(string sender, string receiver, string content, DateTime time)
        {
            this.sender = sender;
            this.receiver = receiver;
            this.content = content;
            this.time = time;
        }

        public static SingleMessage FromCommunication(COMMUNICATION communication)
        {
            return new SingleMessage(
            sender: communication.SENDER_ID,
            receiver: communication.RECEIVER_ID,
            content: communication.COMMUNICATION_CONTENT,
            time: communication.COMMUNICATION_TIME
                );
        }

        public string sender { get; set; }
        public string receiver { get; set; }
        public string content { get; set; }
        public DateTime time { get; set; }
    }
}
