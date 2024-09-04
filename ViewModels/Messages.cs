using UniTrade.Models;
using UniTrade.ViewModels;

namespace UniTrade.ViewModel
{
    public class MessagesViewModel
    {
        public MessagesViewModel(string username, IEnumerable<SingleMessage> contents)
        {
            this.username = username;
            this.contents = contents;
        }

        public string username { get; set; }
        public IEnumerable<SingleMessage> contents { get; set; }
    }
}