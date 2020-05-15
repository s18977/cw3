using cw3.DTOs.Requests;
using cw3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace cw3.Services
{
    public class EnrollmentDbService : IStudentsDbService
    {
        private readonly StudentDbContext _context;
        public EnrollmentDbService(StudentDbContext context)
        {
            _context = context;
        }

        public string dbName = "Data Source=db-mssql;Initial Catalog=s18977;Integrated Security=True;";
        
        [Authorize(Roles = "Employee")]
        public bool EnrollStudent(EnrollStudentRequest request)
        {
            try
            {
                var studies = _context.Studies.Find(request.Studies);
                var student = _context.Student.Find(request.IndexNumber);

                //sprawdzanie czy jest dany student
                if (student == null)
                {
                    var tmp = new Student();
                    tmp.IndexNumber = request.IndexNumber;
                    tmp.FirstName = request.FirstName;
                    tmp.LastName = request.LastName;
                    tmp.BirthDate = DateTime.Parse(request.BirthDate);

                    _context.Add<Student>(tmp);
                    _context.Student.Add(tmp);
                }
                
                //sprawdzenie czy sa dane studia
                if (studies == null)
                {
                    return false;
                }

                //sprawdzanie czy jest dany enrollment
                var tmpEnrol = _context.Enrollments.Find(request.Studies);

                if (tmpEnrol == null)
                {
                    tmpEnrol = new Enrollment();
                    tmpEnrol.IdEnrollment = _context.Enrollments.LastOrDefault().IdEnrollment + 1;
                    tmpEnrol.Semester = 1;
                    tmpEnrol.IdStudy = _context.Studies.Find(request.Studies).idStudy;
                    tmpEnrol.StartDate = DateTime.Now;

                    _context.Add<Enrollment>(tmpEnrol);
                }

                _context.SaveChanges();
                return true;
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            /*
                        using (var con = new SqlConnection(dbName))
                        {
                            using (var com = new SqlCommand())
                            {
                                com.Connection = con;
                                con.Open();
                                SqlTransaction transaction = con.BeginTransaction();

                                try
                                {
            //                      SPRAWDZANIE CZY PODANE STUDIA ISTNIEJA
                                    com.CommandText = "SELECT * FROM Studies WHERE name = name";
                                    com.Parameters.AddWithValue("name", request.Studies);
                                    com.Transaction = transaction;
                                    SqlDataReader reader = com.ExecuteReader();

                                    if (!reader.HasRows)
                                    {
                                        return false;
                                    }

            //                      DODAWANIE DO ENROLLMENT
                                    com.CommandText = "SELECT * FROM enrollment e INNER JOIN studies s on s.idstudy = e.idstudy WHERE semester = 1 AND name = sname";
                                    com.Parameters.AddWithValue("sname", request.Studies);

                                    if (!reader.HasRows)
                                    {
                                        com.CommandText = "INSERT INTO enrollment(idenrollment, semester, idstudy, startdate)" +
                                            "VALUES((select Max(idEnrollment)+1 from enrollment), 1, SELECT idstudy FROM Studies WHERE name = sname), GETDATE())";
                                        com.Parameters.AddWithValue("sname", request.Studies);

                                        com.ExecuteNonQuery();
                                    }
                                    reader.Close();

            //                     DODAWANIE NOWEGO STUDENTA
            //                     zmienne sa z @ bo inaczej SQL krzyczal ze mu nie pasuje
                                    com.CommandText = "SELECT * FROM Student WHERE indexnumber = @es";
                                    com.Parameters.AddWithValue("@es", request.IndexNumber);
                                    reader = com.ExecuteReader();

                                    if (!reader.HasRows)
                                    {
                                        reader.Close();
                                        com.CommandText = "INSERT INTO Student(indexnumber, firstname, lastname, birthdate, idenrollment)" +
                                        "values(@es, @firstname, @lastname, @birthdate, (SELECT idenrollment FROM enrollment e INNER JOIN Studies s on e.idstudy = s.idstudy WHERE semester = 1 AND s.name = @sname))";
                                        com.Parameters.AddWithValue("@firstname", request.FirstName);
                                        com.Parameters.AddWithValue("@lastname", request.LastName);
                                        com.Parameters.AddWithValue("@birthdate", DateTime.Parse(request.BirthDate));

                                        com.ExecuteNonQuery();
                                        reader.Close();
                                    }
                                    else
                                    {
                                        Console.WriteLine("insert");
                                        return false;
                                    }

                                    transaction.Commit();
                                    transaction.Dispose();

                                    return true;
                                }
                                catch (Exception e)
                                {
                                    try
                                    {
                                        transaction.Rollback();
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("Error at Rollback! " + ex.Message);
                                    }

                        return false;
                    }
                }
            }
            */

            return false;
        }

        [Authorize(Roles = "Employee")]
        public bool Promote(PromoteStudents promote)
        {
            try
            {
                var enrollments = _context.Student.ToList();

                if (enrollments.Count == 0)
                {
                    return false;
                }
                _context.Database.BeginTransaction();
                _context.Database.ExecuteSqlCommand("PromoteStudents", promote.sname, promote.semester);
                _context.Database.CommitTransaction();

                _context.SaveChanges();
                return true;
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            //using (SqlConnection con = new SqlConnection(dbName))
            //{
            //    using (SqlCommand com = new SqlCommand())
            //    {
            //        com.Connection = con;
            //        con.Open();
            //        SqlTransaction transaction = con.BeginTransaction();

            //        com.CommandText = "SELECT * FROM Enrollment e INNER JOIN Studies s ON s.idstudy = e.idstudy WHERE e.semester = reqSemester AND s.name = sname";
            //        com.Parameters.AddWithValue("sname", promote.sname);
            //        com.Parameters.AddWithValue("reqSemester", promote.semester);
            //        SqlDataReader reader = com.ExecuteReader();

            //        if (!reader.Read())
            //        {
            //            transaction.Rollback();
            //            reader.Close();
            //            return false;
            //        }

            //        reader.Close();

            //        com.Parameters.Clear();

            //        com.CommandText = "PromoteStudents";
            //        com.CommandType = CommandType.StoredProcedure;
            //        com.Parameters.AddWithValue("sname", promote.sname);
            //        com.Parameters.AddWithValue("reqSemester", promote.semester);
            //        com.ExecuteNonQuery();
            //        transaction.Commit();
            //        transaction.Dispose();

            //        return true;
            //    }
            //}
        }

        public bool CheckIndex(string id)
        {
            using (var con = new SqlConnection("Data Source=db-mssql; Initial Catalog=s18977; Integrated Security=True"))
            {
                using (var com = new SqlCommand())
                {
                    Console.WriteLine(id);

                    com.Connection = con;
                    com.CommandText = "SELECT * FROM Student s WHERE s.indexNumber = @index";
                    com.Parameters.AddWithValue("@index", id);
                    con.Open();

                    var dr = com.ExecuteReader();
                    Console.WriteLine(dr.Read());

                    if (dr.HasRows)
                    {
                        return true;
                    }

                    return false;
                }
            }
        }

    }
}
