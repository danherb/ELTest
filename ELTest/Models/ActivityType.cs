using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELTest.Models
{
    public class ActivityType
    {
        public int ActivityTypeID { get; set; }
        public string Name { get; set; }
        public List<ELTask> ELTasks { get; set; }
    }
}
