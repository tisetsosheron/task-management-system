using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models;
using TaskManagementAPI.Models.DTOs;

namespace TaskManagementAPI.Controllers 
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
	{ 
		private readonly TaskDbContext _context;

        public TasksController(TaskDbContext context)
        {
            _context = context;
        }

        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(t => t.Id == id);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TaskDto taskDto)
        {
            if (string.IsNullOrWhiteSpace(taskDto.Title) || string.IsNullOrWhiteSpace(taskDto.Description))
            {
                return BadRequest("Title and Description are required.");
            }
            if (taskDto.DueDate < DateTime.UtcNow) return BadRequest("Due date cannot be in the past.");
            var task = new TaskItem
            {
                Title = taskDto.Title,
                Description = taskDto.Description,
                Priority = taskDto.Priority,
                Status = taskDto.Status,
                DueDate = taskDto.DueDate,
                AssignedUser = taskDto.AssignedUser 
            };
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> CreateTasks([FromBody] List<TaskDto> taskDtos)
        {
            if (taskDtos == null || !taskDtos.Any()) return BadRequest("Task list cannot be empty.");

            var validTasks = taskDtos
                .Where(dto => dto.DueDate >= DateTime.UtcNow) 
                .Select(dto => new TaskItem
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    Priority = dto.Priority,
                    Status = dto.Status,
                    DueDate = dto.DueDate,
                    AssignedUser = dto.AssignedUser  
                }).ToList();

            if (!validTasks.Any()) return BadRequest("All tasks have invalid due dates."); 

            await _context.Tasks.AddRangeAsync(validTasks);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAllTasks), validTasks);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> GetTask(int id)
        {
            try
            {
                var task = await _context.Tasks.FindAsync(id);
                return task == null ? NotFound() : Ok(task);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetAllTasks()
        {
            var tasks = await _context.Tasks.ToListAsync();
            if (!tasks.Any()) return NoContent();
            return Ok(tasks);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskItem updatedTask)
        {
            if (id != updatedTask.Id) return BadRequest("Task ID mismatch");
            
            _context.Entry(updatedTask).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating task: {ex.Message}");
            }
            return NoContent();

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();
            try
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting task: {ex.Message}");
            }
            return NoContent();

        }
    }
}