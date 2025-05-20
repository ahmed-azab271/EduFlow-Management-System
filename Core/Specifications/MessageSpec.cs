using Core.Entites.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class MessageSpec : Specification<Message>
    {
        public MessageSpec(string senderId, string receverId) :base(I=>
        (I.ReceiverId == senderId  && I.SenderId == receverId) 
        ||
        (I.ReceiverId == receverId && I.SenderId == senderId)
        )
        {
            AddOrderBy(I => I.SentAt);
        }
        public MessageSpec(DateTime afterSemister) :base (I=>I.SentAt < afterSemister)
        {
            
        }
    }
}
