using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using cw3.DTOs.Requests;
using cw3.Models;
using Microsoft.AspNetCore.Mvc;

namespace cw3.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollemntsStudentRequest : ControllerBase
    {
        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            var Enrollments = new List<string>();
            var student = new Student();
            string dbName = "Data Source=db-mssql; Initial Catalog=s18977; Integrated Security=True";
            
            using (var con = new SqlConnection(dbName))
            {
                using (var com = new SqlCommand()) {
                    com.Connection = con;
                    con.Open();
                    SqlTransaction transaction = con.BeginTransaction();

                    try
                    {
//                      SPRAWDZANIE CZY PODANE STUDIA ISTNIEJA
                        com.CommandText = "select * from studies where name = @name";
                        com.Parameters.AddWithValue("@name", request.StudiesName);
                        com.Transaction = transaction;
                        SqlDataReader reader = com.ExecuteReader();

                        if (!reader.HasRows)
                        {
                            return BadRequest(400 + " - Studia o podanej nazwie nie istnieja");
                        }
                        reader.Close();

//                      DODAWANIE DO ENROLLMENT
                        com.CommandText = "select * from enrollment e inner join studies s on s.idstudy = e.idstudy where s.name = @name and semester = 1";
                        com.Parameters.AddWithValue("@name", request.StudiesName);

                        if (!reader.HasRows)
                        {
                            com.CommandText = "insert into enrollment(idenrollment, semester, idstudy, startdate)" +
                                "values((select Max(idEnrollment)+1 from enrollment), 1, select idstudy from studies where name = @name), GETDATE())";

                            com.ExecuteNonQuery();
                            reader.Close();
                        }

//                      DODAWANIE NOWEGO STUDENTA
                        com.CommandText = "select * from studentsapbd where indexnumber = @index";
                        com.Parameters.AddWithValue("@index", request.IndexNumber);

                        if (reader.HasRows)
                        {
                            return BadRequest(400);
                        }
                        else
                        {
                            com.CommandText = "insert into students(indexnumber, firstname, lastname, birthdate, idenrollment)" +
                                "values(@index, @firstname, @lastname, @birthdate, (select idenrollment from enrollment n inner join studies on enrollment.idstudy = studies.idstudy where studies.name = @name and semester = 1))";
                            com.Parameters.AddWithValue("@firstname", request.FirstName);
                            com.Parameters.AddWithValue("@lastname", request.LastName);
                            com.Parameters.AddWithValue("@birthdate", request.BirthDate);

                            com.ExecuteNonQuery();
                            reader.Close();
                        }

                        transaction.Commit();

                        return Ok(201);
                    } catch (Exception e)
                    {
                        return BadRequest("Error" + e);

                        try
                        {
                            transaction.Rollback();
                        }catch(Exception ex)
                        {
                            Console.WriteLine("Error at Rollback! " + ex.Message);
                        }
                    }
                }
            }
        }
    }
}