using System;
using System.Linq;
using Serilog.Core;
using Serilog.Events;
using Microsoft.AspNetCore.Http;

namespace Serilog.Enrichers
{
    public class CorrelationIdHeaderEnricher : ILogEventEnricher
    {
        private const string CorrelationIdPropertyName = "CorrelationId";
        private readonly string _headerKey;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CorrelationIdHeaderEnricher(string headerKey, IHttpContextAccessor httpContextAccessor)
        {
            _headerKey = headerKey;
            _httpContextAccessor = httpContextAccessor;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (_httpContextAccessor.HttpContext == null)
                return;

            var correlationId = GetCorrelationId();
            var correlationIdProperty = new LogEventProperty(CorrelationIdPropertyName, new ScalarValue(correlationId));

            logEvent.AddPropertyIfAbsent(correlationIdProperty);
        }

        private string GetCorrelationId()
        {
            _httpContextAccessor.HttpContext.Request.Headers.TryGetValue(_headerKey, out var header);
            
            var correlationId = string.IsNullOrEmpty(header[0])
                ? Guid.NewGuid().ToString()
                : header[0];

            _httpContextAccessor.HttpContext.Response.Headers.Add(_headerKey, correlationId);

            return correlationId;
        }
    }
}