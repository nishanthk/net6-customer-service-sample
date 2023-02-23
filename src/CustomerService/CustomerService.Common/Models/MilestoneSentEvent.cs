using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerService.Common.Models
{
    internal class MilestoneSentEvent
    {
        public Guid CorrelationId { get; set; }
        public DateTime SentDateTime { get; set; }
    }
}
