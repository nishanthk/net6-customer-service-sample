using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerService.Common.Models.Configuration
{
    public class KafkaProducerOptions
    {
        public string KafkaTopicName { get; set; }
        public string KafkaBootstrapServers { get; set; }
    }
}
