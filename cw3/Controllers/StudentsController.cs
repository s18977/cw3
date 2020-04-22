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

namespace cw3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        public string dbName = "Data Source=db-mssql; Initial Catalog=s18977; Integrated Security=True";
        public IConfiguration Configuration { get; set; }
        public StudentsController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetStudents()
        {
            var students = new List<Student>();
            var id = "s1234";

            using (var con = new SqlConnection(dbName))
            {
                using (var com = new SqlCommand())
                {
                    com.Connection = con;
                    com.CommandText = "SELECT * FROM Student WHERE IndexNumber = @id";
                    com.Parameters.AddWithValue("@id", id);

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

        [HttpPost]
        public IActionResult Login(LoginRequest request)
        {

            using (var con = new SqlConnection(dbName))
            {
                using (var com = new SqlCommand(dbName))
                {
                    var pass = CreateHash(request.Haslo, CreateSalt());

                    com.Connection = con;
                    com.CommandText = "SELECT * FROM student s WHERE s.IndexNumber = @index AND s.Password = @pass";
                    com.Parameters.AddWithValue("@index", request.Login);
                    com.Parameters.AddWithValue("@pass", pass);

                    con.Open();
                    SqlDataReader reader = com.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        return Unauthorized("Podany login lub hasło nie wystepuja w bazie!");
                    }
                }
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, request.Login),
                new Claim(ClaimTypes.Role, "Employee")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer: "Gakko",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds

            );

            return Ok(new {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken = Guid.NewGuid()
            });
        }

        //WYKLAD 8 59.07 FUNKCJA HASHUJACA
        public static string CreateHash(string value, string salt)
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                password: value,
                salt: Encoding.UTF8.GetBytes(salt),
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
                );

            return Convert.ToBase64String(valueBytes);
        }

        public static string CreateSalt()
        {
            byte[] randomBytes = new byte[128 / 8];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        public static bool Validate(string value, string salt, string hash) 
            => CreateHash(value, salt) == hash;

        [HttpPost("refresh-token/{token}")]
        public IActionResult RefreshToken(string refToken)
        {

            var claims = new[]
           {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "jo"),
                new Claim(ClaimTypes.Role, "Employee")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer: "Gakko",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds

            );

            return Ok();
        }

    }
}