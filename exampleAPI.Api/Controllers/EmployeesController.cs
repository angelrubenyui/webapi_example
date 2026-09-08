using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiApi.Application.Commands;
using MiApi.Application.DTOs;
using MiApi.Application.Queries;

namespace MiApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmployeesController(IMediator mediator) => _mediator = mediator;


    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllEmployeesQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeDto>> GetById(int id, CancellationToken ct)
    {
        var employee = await _mediator.Send(new GetEmployeeByIdQuery(id), ct);
        return Ok(employee);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> Create([FromBody] CreateEmployeeCommand command, CancellationToken ct)
    {
        var employee = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest("El ID proporcionado en la URL no coincide con el ID del cuerpo de la petición.");
        }

        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteEmployeeCommand(id), ct);
        return NoContent();
    }
}