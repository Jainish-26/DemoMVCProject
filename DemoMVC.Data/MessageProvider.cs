using DemoMVC.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DemoMVC.Models
{
    public class MessageProvider : BaseProvider
    {
        public int CreateMessage(Message_Mst message)
        {
            try
            {
                _db.Message_Mst.Add(message);
                _db.SaveChanges();
                return message.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int UpdateMessage(Message_Mst message)
        {
            try
            {
                _db.Entry(message).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                return message.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Message_Mst GetMessageDataById(int Id)
        {
            return _db.Message_Mst.Find(Id);
        }
        public IQueryable<MessageGridModel> GetAllMessageData()
        {
            return (from msg in _db.Message_Mst
                    select new MessageGridModel()
                    {
                        Id = msg.Id,
                        Code = msg.Code,
                        Message = msg.Message,
                        IsActive = msg.IsActive,
                        Comment = msg.Comment,
                        UpdatedOn = msg.ModifiedOn == null ? msg.CreatedOn : msg.ModifiedOn
                    });
        }
        public string GetMessageByCode(string Code)
        {
            try
            {
                string Name = _db.Message_Mst.Where(a => a.Code != null && a.IsActive == true && a.Code == Code).FirstOrDefault()?.Message;
                return Name;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Message_Mst> CheckDuplicateMessageCode(string Code)
        {
            var getMessageDetails = (from messageDetails in _db.Message_Mst
                                     where messageDetails.Code.ToUpper().Trim() == Code.ToUpper().Trim()
                                     select messageDetails).ToList();
            return getMessageDetails;
        }
    }
}
