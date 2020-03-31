using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace cw3.DTOs.Requests
{
    public class EnrollStudentRequest
    {
        [Required]
        public string IndexNumber { get; set; }

        [Required (ErrorMessage = "Musisz podać imię")]
        [MaxLength(20)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Musisz podać nazwisko")]
        [MaxLength(255)]
        public string LastName { get; set; }
        [Required]
        public string BirthDate { get; set; }

        [Required]
        public string StudiesName { get; set; }

    }
}
