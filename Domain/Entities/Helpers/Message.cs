using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Helpers
{
    public class Message
    {
        public string Sender { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }

}
