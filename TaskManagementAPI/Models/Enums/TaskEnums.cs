using System;

namespace TaskManagementAPI.Models.Enums
{
    public enum Priority
    {
        Low,
        Medium,
        High
    }

    public enum Status
    {
        New,
        InProgress,
        Blocked,
        AwaitingRelease,
        Testing,
        Done
    }
}
