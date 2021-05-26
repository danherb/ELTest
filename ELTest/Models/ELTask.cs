using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELTest.Models
{
    public class ELTask
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public ActivityType ActivityType { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
