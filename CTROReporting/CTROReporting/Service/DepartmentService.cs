using CTROReporting.Infrastructure;
using CTROReporting.Models;
using CTROReporting.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace CTROReporting.Service
{
    public interface IDepartmentService
    {
        Department GetByDepartmentID(int departmentid);
        Department GetByDepartmentName(string departmentname);
        IEnumerable<Department> GetDepartments();
    }

    public class DepartmentServiceController : ApiController, IDepartmentService
    {
        private readonly IDepartmentRepository departmentRepository;
        private readonly IUnitOfWork unitOfWork;


        public DepartmentServiceController(IDepartmentRepository departmentRepository, IUnitOfWork unitOfWork)
        {
            this.departmentRepository = departmentRepository;
            this.unitOfWork = unitOfWork;
        }

        public Department GetByDepartmentName(string departmentname)
        {
            var department = departmentRepository.Get(x => x.DepartmentName == departmentname);
            return department;
        }

        public Department GetByDepartmentID(int departmentid)
        {
            var department = departmentRepository.Get(x => x.DepartmentId == departmentid);
            return department;
        }

        public IEnumerable<Department> GetDepartments()
        {
            var departments = departmentRepository.GetAll();
            return departments;
        }

        public void SaveDepartment()
        {
            unitOfWork.Commit();
        }

    }
}