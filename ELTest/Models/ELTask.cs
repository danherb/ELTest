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

        //Neni required, protoze muzu pouzit stopky - to se pak vyplni samo 
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        //Co kdyz bude doba delsi jak 24 hodin? Pokud jsem pouzil DateTime od a do, tak to ve firefoxu neukazalo 
        //ten datepicker. Stejne to musim udelat takhle - zadani.
        [DataType(DataType.Time)]
        public DateTime? From { get; set; }

        [DataType(DataType.Time)]
        public DateTime? To { get; set; }

        public string Total => (To?.Year > 1) ? (To - From)?.ToString("hh\\:mm") : (DateTime.Now - From)?.ToString("hh\\:mm");
    }
}
