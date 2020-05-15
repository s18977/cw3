using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cw3.Models
{
    public class StudentDbContext : DbContext
    {
        public DbSet<Student> Student { get; set; }

        public DbSet<Studies> Studies { get; set; }

        public DbSet<Enrollment> Enrollments { get; set; }

        public StudentDbContext() { }

        public StudentDbContext(DbContextOptions options) : base(options){
            
        }
    }
}
