using cw3.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace cw3.DAL
{
    public class MockDbService : IDbService
    {
        private static IEnumerable<Student> _students;

        public IEnumerable<Student> GetStudents()
        {
            return _students;
        }
    }
}
