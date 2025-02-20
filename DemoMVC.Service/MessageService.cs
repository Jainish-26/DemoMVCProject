using DemoMVC.Models;
using System.Collections.Generic;
using System.Linq;

namespace DemoMVC.Service
{
    public class MessageService
    {
        private readonly MessageProvider _messageProvider;
        public MessageService()
        {
            _messageProvider = new MessageProvider();
        }
        public int CreateMessage(Message_Mst message)
        {
            return _messageProvider.CreateMessage(message);
        }
        public int UpdateMessage(Message_Mst message)
        {
            return _messageProvider.UpdateMessage(message);
        }
        public IQueryable<MessageGridModel> GetAllMessageData()
        {
            return _messageProvider.GetAllMessageData();
        }
        public Message_Mst GetMessageDataById(int Id)
        {
            return _messageProvider.GetMessageDataById(Id);
        }
        public string GetMessageByCode(string Code)
        {
            return _messageProvider.GetMessageByCode(Code);
        }
        public List<Message_Mst> CheckDuplicateMessageCode(string Code)
        {
            return _messageProvider.CheckDuplicateMessageCode(Code);
        }
    }

}
