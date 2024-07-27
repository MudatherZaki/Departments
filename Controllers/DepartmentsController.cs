using Departments.Application.DepartmentConnections;
using Departments.Application.Departments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Departments.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly DepartmentService _departmentService;

        public DepartmentsController(DepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet("{Id}/subs")]
        public async Task<IActionResult> GetSubDepartments([FromRoute(Name = "Id")] int id)
        {
            var subDepartments = await _departmentService
                .GetChildren(id);

            return Ok(subDepartments);
        }

        [HttpGet("{Id}/parents")]
        public async Task<IActionResult> GetParentDepartments([FromRoute(Name = "Id")] int id)
        {
            var subDepartments = await _departmentService
                .GetParents(id);

            return Ok(subDepartments);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] DepartmentDto department)
        {
            var createdDepartmentId = await _departmentService
                .AddDepartment(department);

            return Ok(createdDepartmentId);
        }

        [HttpPost("{Id}/parents")]
        public async Task<IActionResult> CreateParentDepartment([FromRoute(Name = "Id")] int id, [FromBody] List<int> parentIds)
        {
            try
            {
                await _departmentService.AddParents(
                    id, 
                    parentIds);
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpPost("{Id}/subs")]
        public async Task<IActionResult> CreateSubDepartment([FromRoute(Name = "Id")] int id, [FromBody] List<int> subIds)
        {
            try
            {
                await _departmentService.AddSubDepartments(
                    id,
                    subIds);
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

    }
}
