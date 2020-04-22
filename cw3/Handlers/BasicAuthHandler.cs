using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Data.SqlClient;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace cw3.Handlers
{

    public class BasicAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public string dbName = "Data Source=db-mssql;Initial Catalog=s18977;Integrated Security=True;";
        public BasicAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            //IStudentDbService service
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
        
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
           if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing Authorization header!");

            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            var credentialsBytes = Convert.FromBase64String(authHeader.Parameter);
            var credentials = Encoding.UTF8.GetString(credentialsBytes).Split(":");

            if (credentials.Length != 2)
                return AuthenticateResult.Fail("Incorrect authorization header value");

            using (var con = new SqlConnection(dbName))
            {
                using (var com = new SqlCommand(dbName))
                {
                    com.Connection = con;
                    com.CommandText = "SELECT * FROM student s WHERE s.IndexNumber = @index AND s.Password = @pass";
                    com.Parameters.AddWithValue("@index", credentials[0]);
                    com.Parameters.AddWithValue("@pass", credentials[1]);

                    con.Open();
                    SqlDataReader reader = com.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        return AuthenticateResult.Fail("Podany login lub hasło nie wystepuja w bazie!");
                    }
                }
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, credentials[0]),
                new Claim(ClaimTypes.Role, "Employee")
            };

            var identitity = new ClaimsIdentity(claims, Scheme.Name); //rodzaj uwierzytelnienia
            var principal = new ClaimsPrincipal(identitity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);


            return AuthenticateResult.Success(ticket);
        }
    }
}
