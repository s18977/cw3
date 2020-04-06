using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using cw3.DAL;
using cw3.DTOs.Requests;
using cw3.Models;
using cw3.Services;
using Microsoft.AspNetCore.Mvc;

namespace cw3.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentsController : ControllerBase
    {
        public IStudentsDbService _service;
        public EnrollmentsController(IStudentsDbService service)
        {
            _service = service;
        }

        public string dbName = "Data Source=db-mssql;Initial Catalog=s18977;Integrated Security=True;";
        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {

            if (_service.EnrollStudent(request) == false)
                return BadRequest(400);

            return Ok(200);
        }

        [HttpPost]
        [Route("api/enrollments/promotions")]
        public IActionResult Promote(PromoteStudents promote)
        {
            if (_service.Promote(promote) == false)
            {
                return BadRequest(404);
            }

            return Ok(promote);
        }
    }
}