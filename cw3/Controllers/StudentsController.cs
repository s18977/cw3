using System;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using cw3.DTOs.Requests;
using cw3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Linq;

namespace cw3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly StudentDbContext _context;

        public StudentsController(StudentDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetStudent()
        {
            return Ok(_context.Student.ToList());
        }

        [HttpPost]
        public IActionResult EditStudent(StudentUpdateRequest request)
        {
            var st =_context.Student.Find(request.IndexNumber);
            var value = request.LastName;

            if (st != null)
            {
                st.FirstName = request.FirstName;
                st.LastName = request.LastName;
                
                _context.Student.Update(st);
                _context.SaveChanges();
            }


            return Ok(st);
        }

        [HttpDelete]
        public IActionResult DeleteStudent(StudentUpdateRequest request)
        {
            var st = _context.Student.Find(request.IndexNumber);

            if (st != null)
            {
                try
                {
                    _context.Student.Remove(st);
                    _context.SaveChanges();
                }catch (Exception ex)
                {
                    return BadRequest("Podany student nie widnieje w bazie!");
                }
            }

            return Ok(200);
        }

        //public static string dbName = "Data Source=db-mssql; Initial Catalog=s18977; Integrated Security=True";
        //public IConfiguration Configuration { get; set; }
        //public StudentsController(IConfiguration configuration)
        //{
        //    Configuration = configuration;
        //}

        //[HttpGet]
        //public IActionResult GetStudents()
        //{
        //    var students = new List<Student>();
        //    var id = "s1234";

        //    using (var con = new SqlConnection(dbName))
        //    {
        //        using (var com = new SqlCommand())
        //        {
        //            com.Connection = con;
        //            com.CommandText = "SELECT * FROM Student WHERE IndexNumber = @id";
        //            com.Parameters.AddWithValue("@id", id);

        //            con.Open();
        //            var dr = com.ExecuteReader();

        //            while (dr.Read())
        //            {
        //                var st = new Student();
        //                st.FirstName = dr["FirstName"].ToString();
        //                st.LastName = dr["LastName"].ToString();
        //                st.LastName = dr["LastName"].ToString();
        //                st.BirthDate = DateTime.Parse(dr["BirthDate"].ToString());
        //                st.Studies = dr["IdEnrollment"].ToString();

        //                students.Add(st);
        //            }

        //            return Ok(students);
        //        }
        //    }
        //}

        //[HttpGet("{id}")]
        //public IActionResult GetStudent(string id)
        //{
        //    var enrollment = new List<string>();
        //    using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18977; Integrated Security=True"))
        //    {
        //        using (var com = new SqlCommand())
        //        {
        //            com.Connection = con;
        //            com.CommandText = "SELECT * FROM Enrollment WHERE IdEnrollment=(SELECT IdEnrollment FROM Student WHERE IndexNumber = \'" + id + "\')";

        //            con.Open();
        //            var dr = com.ExecuteReader();

        //            while (dr.Read())
        //            {
        //                enrollment.Add(dr["Semester"].ToString());
        //            }

        //            return Ok(enrollment);
        //        }
        //    }
        //}

        //[HttpPost]
        //public IActionResult Login(LoginRequest request)
        //{
        //    var pass = String.Empty;
        //    var salt = String.Empty;

        //    var refreshToken = Guid.NewGuid();
        //    Console.WriteLine(refreshToken);

        //    using (var con = new SqlConnection(dbName))
        //    {
        //        using (var com = new SqlCommand(dbName))
        //        {
        //            //pass = CreateHash(request.Haslo, CreateSalt(request.Login), request.Login);

        //            com.Connection = con;
        //            com.CommandText = "SELECT * FROM student s WHERE s.IndexNumber = @index";
        //            com.Parameters.AddWithValue("@index", request.Login);

        //            con.Open();
        //            SqlDataReader reader = com.ExecuteReader();

        //            if (!reader.HasRows)
        //            {
        //                return Unauthorized("Podany login lub hasło nie wystepuja w bazie!");
        //            } else if (reader.Read())
        //            {
        //                 pass = reader.GetString(reader.GetOrdinal("Password"));
        //                 salt = reader.GetString(reader.GetOrdinal("salt"));

        //                reader.DisposeAsync();

        //                com.CommandText = "UPDATE student SET refreshToken = @token WHERE IndexNumber = @id";
        //                com.Parameters.AddWithValue("@token", refreshToken);
        //                com.Parameters.AddWithValue("@id", request.Login);
        //                com.ExecuteNonQuery();
        //            }
        //            con.Close();
        //        }
        //    }

        //    if (Validate(request.Haslo, salt, pass, request.Login))
        //    {

        //        var claims = new[]
        //        {
        //        new Claim(ClaimTypes.NameIdentifier, "1"),
        //        new Claim(ClaimTypes.Name, request.Login),
        //        new Claim(ClaimTypes.Role, "Employee")
        //    };

        //        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
        //        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //        var token = new JwtSecurityToken
        //        (
        //            issuer: "Gakko",
        //            audience: "Students",
        //            claims: claims,
        //            expires: DateTime.Now.AddMinutes(10),
        //            signingCredentials: creds

        //        );



        //        return Ok(
        //            new
        //        {
        //            token = new JwtSecurityTokenHandler().WriteToken(token),
        //            refreshToken = refreshToken,
        //            Message = "Zalogowano pomyślnie"
        //            });
        //    }

        //    return NotFound("Podany login lub hasło nie istnieja");
        //}

        //WYKLAD 8 59.07 FUNKCJA HASHUJACA
        //public static string CreateHash(string value, string salt, string id)
        //{
        //    var valueBytes = KeyDerivation.Pbkdf2(
        //        password: value,
        //        salt: Encoding.UTF8.GetBytes(salt),
        //        prf: KeyDerivationPrf.HMACSHA512,
        //        iterationCount: 10000,
        //        numBytesRequested: 256 / 8
        //        );

        //    using (var con =new SqlConnection(dbName))
        //    {
        //        using (var com = new SqlCommand(dbName))
        //        {
        //            com.Connection = con;
        //            com.CommandText = "UPDATE student SET Password = @pass WHERE indexNumber = @id";
        //            com.Parameters.AddWithValue("@id", id);
        //            com.Parameters.AddWithValue("@pass", Convert.ToBase64String(valueBytes));
        //            con.Open();
        //            com.ExecuteNonQuery();
        //            con.Close();

        //        }
        //    }

        //        return Convert.ToBase64String(valueBytes);
        //}

        //public static string CreateSalt(string id)
        //{
        //    byte[] randomBytes = new byte[128 / 8];
        //    using (var generator = RandomNumberGenerator.Create())
        //    {
        //        generator.GetBytes(randomBytes);
        //        using (var con = new SqlConnection(dbName))
        //        {
        //            using (var com = new SqlCommand(dbName))
        //            {
        //                com.Connection = con;
        //                com.CommandText = "UPDATE student SET salt = @pass WHERE IndexNumber = @id";
        //                com.Parameters.AddWithValue("@id", id);
        //                com.Parameters.AddWithValue("@pass", Convert.ToBase64String(randomBytes));
        //                con.Open();
        //                com.ExecuteNonQuery();
        //                con.Close();
        //            }
        //        }

        //        return Convert.ToBase64String(randomBytes);
        //    }
        //}

        //public static bool Validate(string value, string salt, string hash, string id) 
        //    => CreateHash(value, salt, id) == hash;

        //[HttpPost("refresh-token/{token}")]
        //public IActionResult RefreshToken(string refToken)
        //{

        //    var claims = new[]
        //   {
        //        new Claim(ClaimTypes.NameIdentifier, "1"),
        //        new Claim(ClaimTypes.Name, "jo"),
        //        new Claim(ClaimTypes.Role, "Employee")
        //    };

        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //    var token = new JwtSecurityToken
        //    (
        //        issuer: "Gakko",
        //        audience: "Students",
        //        claims: claims,
        //        expires: DateTime.Now.AddMinutes(10),
        //        signingCredentials: creds

        //    );

        //    using (var con = new SqlConnection(dbName))
        //    {
        //        using (var com = new SqlCommand(dbName))
        //        {
        //            com.Connection = con;
        //            com.CommandText = "UPDATE student SET refreshToken = @token WHERE refreshToken = @oldToken";
        //            com.Parameters.AddWithValue("@oldToken", refToken);
        //            com.Parameters.AddWithValue("@token", new JwtSecurityTokenHandler().WriteToken(token));
        //            con.Open();
        //            com.ExecuteNonQuery();
        //            con.Close();
        //        }
        //    }

        //    return Ok("Token zaktualizowany");
        //}

    }
}