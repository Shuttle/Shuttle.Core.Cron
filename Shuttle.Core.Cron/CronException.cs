using System;

namespace Shuttle.Core.Cron;

public class CronException : Exception
{
    public CronException(string message) : base(message)
    {
    }
}