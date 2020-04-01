using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace cw3.DTOs.Requests
{
    public class PromoteStudents
    {
        [Required]
        public string sname { get; set; }
        [Required]
        public int semester { get; set; }

        public bool done { get; set; }
    }
}
