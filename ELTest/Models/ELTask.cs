using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ELTest.Models
{
    public class ELTask
    {
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }
        public int ActivityTypeID { get; set; }
        public ActivityType ActivityType { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public DateTime From { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public DateTime To { get; set; }

        //[DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        //[DataType(DataType.Time)]
        public string Total => (To - From).ToString("hh\\:mm");
    }
}
