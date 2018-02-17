# Shuttle.Core.Cron

```
PM> Install-Package Shuttle.Core.Cron
```

Provides [cron](https://en.wikipedia.org/wiki/Cron) expression parsing.

## Cron Samples

Format is {minute} {hour} {day-of-month} {month} {day-of-week}

```
{minutes} : 0-59 , - * /
{hours} : 	0-23 , - * /
{day-of-month} 1-31 , - * ? / L W
{month} : 1-12 or JAN-DEC	, - * /
{day-of-week} : 1-7 or SUN-SAT , - * ? / L #

Examples:
* * * * * - is every minute of every hour of every day of every month
5,10-12,17/5 * * * * - minute 5, 10, 11, 12, and every 5th minute after that
```

