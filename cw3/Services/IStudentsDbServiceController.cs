
using cw3.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;

namespace cw3.Services
{
    public interface IStudentsDbService
    {
        public bool EnrollStudent(EnrollStudentRequest request);

        public bool Promote(PromoteStudents promote);
    }
}