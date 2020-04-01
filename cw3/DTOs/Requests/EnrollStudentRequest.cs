using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace cw3.DTOs.Requests
{
    public class EnrollStudentRequest
    {
        [Required(ErrorMessage = "Musisz podać index studenta od sXXXXX")]
        public string IndexNumber { get; set; }

        [Required(ErrorMessage = "Musisz podać imię")]
        [MaxLength(20)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Musisz podać nazwisko")]
        [MaxLength(255)]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Musisz podać date urodzenia")]
        public string BirthDate { get; set; }

        [Required(ErrorMessage = "Musisz podać nazwe studiow")]
        public string Studies { get; set; }

        public bool done {get; set;}

    }
}
