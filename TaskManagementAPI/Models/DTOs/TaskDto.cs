using System;
using TaskManagementAPI.Models.Enums;
using TaskManagementAPI.Models;
using System.Text.Json.Serialization;

namespace TaskManagementAPI.Models.DTOs
{
    public class TaskDto
    {
        public string Title { get; set; }
        public string Description { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Priority Priority { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Status Status { get; set; }

        public DateTime DueDate { get; set; }
        public string? AssignedUser { get; set; }

    }
}

