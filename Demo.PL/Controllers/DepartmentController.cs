using Demo.BLL.Interfaces;
using Demo.BLL.Repositories;
using Demo.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{
    [Authorize]
    public class DepartmentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentController(IUnitOfWork unitOfWork)   // Ask CLR for creating object from class that emplement interface
        {
            _unitOfWork = unitOfWork;
        }

        //BaseUrl/Department/Index
        public async Task<IActionResult> Index()
        {
            var departments = await _unitOfWork.DepartmentRepository.GetAllAsync();
            return View(departments);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Department department)
        {
            if (ModelState.IsValid)
            {
                 await _unitOfWork.DepartmentRepository.AddAsync(department);
                int Result = await _unitOfWork.CompleteAsync();
                if (Result > 0)
                {
                    TempData["Message"] = "Department is Created Successfully";
                }
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        public async Task<IActionResult> Details(int? id, string ViewName = "Details")
        {
            if (id is null)
                return BadRequest();   //status Code 400
            var department = await _unitOfWork.DepartmentRepository.GetByIdAsync(id.Value);
            if (department is null)
                return NotFound();
            return View(ViewName, department);

        }

        public async Task<IActionResult> Edit(int? id)
        {
            return await Details(id, "Edit");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Department department, [FromRoute] int id)
        {
            if (id != department.Id)
                return BadRequest();
            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.DepartmentRepository.Update(department);
                    await _unitOfWork.CompleteAsync();
                    return RedirectToAction(nameof(Index));

                }
                catch (System.Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }

            }
            return View(department);

        }


        public async Task<IActionResult> Delete(int id)
        {
            return await Details(id, "Delete");

        }

        [HttpPost]
        public async Task<IActionResult> Delete( Department department, [FromRoute] int id)
        {
            if(id != department.Id)
                return BadRequest();
            try
            {
                _unitOfWork.DepartmentRepository.Delete(department);
                 await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index));

            }
            catch(System.Exception ex)
            {
                ModelState.AddModelError(string.Empty,ex.Message);
                return View(department);

            }

        }
    }
}
