using System;
using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.Models.Enums;

namespace TaskManagementAPI.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public Priority Priority { get; set; }
        public Status Status { get; set; }
        public DateTime DueDate { get; set; }
        public string? AssignedUser { get; set; }
    }
}