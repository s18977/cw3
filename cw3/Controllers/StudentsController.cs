using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using cw3.DAL;
using cw3.Models;
using Microsoft.AspNetCore.Mvc;

namespace cw3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IDbService _dbService;

        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet]
        public IActionResult GetStudents()
        {
            var students = new List<Student>();
            var id = "s1234";

            using (var con = new SqlConnection("Data Source=db-mssql; Initial Catalog=s18977; Integrated Security=True"))
            {
                using (var com = new SqlCommand())
                {
                    com.Connection = con;
                    com.CommandText = "SELECT * FROM Student";
                    //com.Parameters.AddWithValue("id", id);

                    con.Open();
                    var dr = com.ExecuteReader();

                    while (dr.Read())
                    {
                        var st = new Student();
                        st.FirstName = dr["FirstName"].ToString();
                        st.LastName = dr["LastName"].ToString();
                        st.LastName = dr["LastName"].ToString();
                        st.BirthDate = DateTime.Parse(dr["BirthDate"].ToString());
                        st.Studies = dr["IdEnrollment"].ToString();

                        students.Add(st);
                    }

                    return Ok(students);
                }
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetStudent(string id)
        {
            var enrollment = new List<string>();
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18977; Integrated Security=True"))
            {
                using (var com = new SqlCommand())
                {
                    com.Connection = con;
                    com.CommandText = "SELECT * FROM Enrollment WHERE IdEnrollment=(SELECT IdEnrollment FROM Student WHERE IndexNumber = \'" + id + "\')";

                    con.Open();
                    var dr = com.ExecuteReader();

                    while (dr.Read())
                    {
                        enrollment.Add(dr["Semester"].ToString());
                    }

                    return Ok(enrollment);
                }
            }
        }


    }
}