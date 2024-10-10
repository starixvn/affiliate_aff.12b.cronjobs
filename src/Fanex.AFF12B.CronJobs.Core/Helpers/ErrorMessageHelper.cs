using Fanex.Data;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Fanex.AFF12B.CronJobs.Core.Helpers
{
    public static class ErrorMessageHelper
    {
        public static Exception FormatException(string initialMessage, object param = null)
        {
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine(initialMessage);
            messageBuilder.Append($"Parameters: {JsonConvert.SerializeObject(param)}");

            return new FormatException(messageBuilder.ToString());
        }

        public static string GenerateMessage(Exception exception, HttpContext context = null, string message = null)
        {
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine(exception.Message);
            messageBuilder.AppendLine(message);
            AddRequestInfo(context, messageBuilder);
            AddDatabaseInfo(messageBuilder, exception);
            AddApiException(messageBuilder, exception);

            const int limitedNumOfInnerException = 3;
            AddExceptionInfo(messageBuilder, exception, limitedNumOfInnerException);
            return messageBuilder.ToString();
        }

        private static void AddRequestInfo(HttpContext context, StringBuilder builder)
        {
            builder.AppendLine();
            builder.AppendLine("REQUEST INFO");

            var webInfo = GetRequestInfo(context);

            foreach (var item in webInfo)
            {
                builder.AppendLine($"{item.Key}: {item.Value}");
            }
        }

        private static void AddDatabaseInfo(StringBuilder builder, Exception exception)
        {
            builder.AppendLine();
            builder.AppendLine("DATABASE INFO");

            var dbInfo = GetDbExceptionInfo(exception);

            foreach (var item in dbInfo)
            {
                builder.AppendLine($"{item.Key}: {item.Value}");
            }
        }

        private static void AddExceptionInfo(StringBuilder builder, Exception exception,
            int limitedInnerException)
        {
            builder.AppendLine();
            builder.AppendLine("EXCEPTION INFO");
            builder.AppendLine($"Message: {exception.Message}");
            builder.AppendLine($"Source: {exception.Source}");
            builder.AppendLine($"TargetSite: {exception.TargetSite}");
            builder.AppendLine($"StackTrace: {exception.StackTrace}");
            if (exception.InnerException != null && limitedInnerException > 0)
            {
                limitedInnerException--;
                AddExceptionInfo(builder, exception.InnerException, limitedInnerException);
            }
        }

        private static void AddApiException(StringBuilder content, Exception ex)
        {
            if (ex is ApiException apiException)
            {
                content.AppendLine();
                content.AppendLine("API EXCEPTION INFO");
                content.AppendLine($"StatusCode: {apiException.StatusCode}");
                content.AppendLine($"Content: {apiException.Content}");
                content.AppendLine($"Header: ${apiException.ContentHeaders}");
                content.AppendLine($"Request: {apiException.RequestMessage}");
            }
        }

        private static Dictionary<string, object> GetDbExceptionInfo(Exception exception)
        {
            var dbInfo = new Dictionary<string, object>();
            BindSqlExceptionInfo(exception, dbInfo);
            BindFanexDataExceptionInfo(exception, dbInfo);
            return dbInfo;
        }

        private static void BindFanexDataExceptionInfo(Exception exception, IDictionary<string, object> dbInfo)
        {
            if (exception is ObjectDbException fanexDataException)
            {
                BindSqlExceptionInfo(exception.InnerException, dbInfo);

                var connString = new SqlConnectionStringBuilder(fanexDataException.ConnectionString);

                dbInfo["Server"] = connString.DataSource;
                dbInfo["DbName"] = fanexDataException.DbName;
                dbInfo["SpName"] = fanexDataException.SpName;
                dbInfo["UserId"] = connString.UserID;
                dbInfo["Parameters"] = GetParameters(fanexDataException.Parameters);
                dbInfo["CommandTimeout"] = fanexDataException.CommandTimeout;
            }
        }

        private static void BindSqlExceptionInfo(Exception exception, IDictionary<string, object> dbInfo)
        {
            if (exception is SqlException sqlException)
            {
                dbInfo["Server"] = sqlException.Server;
                dbInfo["SpName"] = sqlException.Procedure;
                dbInfo["Line"] = sqlException.LineNumber;
            }
        }

        private static string GetParameters(IDictionary<string, object> parameters)
           => parameters?.Count > 0
               ? parameters.Select(parameter => $"{parameter.Key}={parameter.Value}")
                       .Aggregate((firstPair, secondPair) => $"{firstPair};{secondPair}")
               : string.Empty;

        private static Dictionary<string, object> GetRequestInfo(HttpContext context)
        {
            var request = context?.Request;

            return request == null
                    ? new Dictionary<string, object>()
                    : new Dictionary<string, object>
                    {
                        [nameof(request.Host)] = request.Host,
                        [nameof(request.Path)] = request.Path,
                        [nameof(request.Body)] = request.Body,
                        ["Remote IP"] = context.Connection.RemoteIpAddress
                    };
        }
    }
}
