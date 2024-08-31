using AutoMapper;
using Demo.BLL.Interfaces;
using Demo.DAL.Models;
using Demo.PL.Helpers;
using Demo.PL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EmployeeController(IUnitOfWork unitOfWork,
            IMapper mapper)   // Ask CLR for creating object from class that emplement interface IUnitOfWork
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index(string SearchValue)
        {
            IEnumerable<Employee> employees;
            if(string.IsNullOrEmpty(SearchValue))
                employees = await _unitOfWork.EmployeeRepository.GetAllAsync();
            else
            employees = _unitOfWork.EmployeeRepository.GetEmployeesByName(SearchValue);
            var MappedEmployees = _mapper.Map<IEnumerable<Employee>,IEnumerable<EmployeeViewModel>>(employees);
            return View(MappedEmployees);
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.Departments = await _unitOfWork.DepartmentRepository.GetAllAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(EmployeeViewModel employeeVM)
        {
            if(ModelState.IsValid)
            {

                employeeVM.ImageName = DocumentSettings.UploadFile(employeeVM.Image, "Images");
                var MappedEmployee = _mapper.Map<EmployeeViewModel,Employee>(employeeVM);
                 await _unitOfWork.EmployeeRepository.AddAsync(MappedEmployee);
               int Result = await _unitOfWork.CompleteAsync();
                if (Result > 0)
                {
                    TempData["Message"] = "Employee is Added Successfully ";
                }
                await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employeeVM);
        }

        public async Task<IActionResult> Details(int? id, string ViewName ="Details")
        {
            if (id is null)
                return BadRequest();
            var employee = await _unitOfWork.EmployeeRepository.GetByIdAsync(id.Value); 
            if(employee is null)
                return NotFound();
            //ViewBag.Departments = _unitOfWork.DepartmentRepository.GetAll();
            var MappedEmployee = _mapper.Map<Employee, EmployeeViewModel>(employee);
            return View(ViewName,MappedEmployee);
        }

        public async Task<IActionResult> Edit(int? id)
        {

            return await Details(id, "Edit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeViewModel employeeVm, [FromRoute] int id)
        {
            if (id != employeeVm.Id)
                return BadRequest();
            if (ModelState.IsValid)
                try
                {
                    employeeVm.ImageName = DocumentSettings.UploadFile(employeeVm.Image, "Images");
                    var MappedEmployee = _mapper.Map<EmployeeViewModel, Employee>(employeeVm);
                    _unitOfWork.EmployeeRepository.Update(MappedEmployee);
                    await _unitOfWork.CompleteAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch(System.Exception ex) 
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            return View(employeeVm); 

        }

        public async Task<IActionResult> Delete(int id)
        {
            return await Details(id, "Delete");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(EmployeeViewModel employeeVm, [FromRoute] int id)
        {
            if(id != employeeVm.Id)
                return BadRequest();
            try
            {
                var MappedEmployee = _mapper.Map<EmployeeViewModel, Employee>(employeeVm);
                _unitOfWork.EmployeeRepository.Delete(MappedEmployee);
                var Result = await _unitOfWork.CompleteAsync();
                if(Result > 0 && employeeVm.ImageName is not null)
                {
                    DocumentSettings.DeleteFile("Images", employeeVm.ImageName);
                }
                return RedirectToAction(nameof(Index));
            }
            catch(System.Exception ex)
            {
                ModelState.AddModelError(string.Empty,ex.Message);
                return View(employeeVm);

            }

        }


    }
}
