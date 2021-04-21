using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EzPlatform.EzOnboarding.Core.Tasks
{
    public class TaskExecutionResult
    {
        public bool IsSuccess => !Errors.Any();

        public List<ResultError> Errors { get; } = new List<ResultError>();

        public static TaskExecutionResult Failed(Exception exception)
        {
            var result = new TaskExecutionResult();
            result.AddError(exception);

            return result;
        }

        public TaskExecutionResult AddError(Exception exception)
        {
            if (exception is null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            AddError(new ResultError
            {
                Summary = exception.Message,
                Detail = exception.ToString()
            });

            if (exception.InnerException != null)
            {
                AddError(exception.InnerException);
            }

            return this;
        }

        public TaskExecutionResult AddError(string error)
        {
            Errors.Add(new ResultError
            {
                Summary = error
            });
            return this;
        }

        public TaskExecutionResult AddError(ResultError error)
        {
            Errors.Add(error);
            return this;
        }

        public TaskExecutionResult AppendErrors(List<ResultError> errors)
        {
            Errors.AddRange(errors);
            return this;
        }

        public string GetErrorSummary()
        {
            if (Errors.Count == 0)
            {
                return string.Empty;
            }

            var summary = new StringBuilder();
            Errors.ForEach(e => summary.AppendLine($"Detail: {e.Summary}"));
            return summary.ToString();
        }

        public string GetErrorDetails()
        {
            if (Errors.Count == 0)
            {
                return string.Empty;
            }

            var summary = new StringBuilder();
            Errors.ForEach(e => summary.AppendLine(
                $"Summary: {e.Summary}" +
                $" Detail {e.Detail}" +
                $" Exception(s) {(e.ExceptionDetails == null || e.ExceptionDetails.Count == 0 ? string.Empty : string.Join("; ", e.ExceptionDetails))}"));
            return summary.ToString();
        }
    }

    public class ResultError
    {
        public string? Summary { get; set; }

        public string? Detail { get; set; }

        public List<string>? ExceptionDetails { get; set; }
    }
}
