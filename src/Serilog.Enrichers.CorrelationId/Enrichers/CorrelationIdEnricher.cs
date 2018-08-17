using System;
using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Enrichers.CorrelationId.Enrichers
{
    public class CorrelationIdEnricher : ILogEventEnricher
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string CorrelationIdPropertyName = "CorrelationId";
        private static readonly string CorrelationIdItemName = $"{typeof(CorrelationIdEnricher).Name}+CorrelationId";

        public CorrelationIdEnricher(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (_httpContextAccessor == null)
                return;

            var correlationId = GetCorrelationId();
            var correlationIdProperty = new LogEventProperty(CorrelationIdPropertyName, new ScalarValue(correlationId));

            logEvent.AddPropertyIfAbsent(correlationIdProperty);
        }

        private string GetCorrelationId()
        {
            return (string) (_httpContextAccessor.HttpContext.Items[CorrelationIdItemName] ??
                             (_httpContextAccessor.HttpContext.Items[CorrelationIdItemName] = Guid.NewGuid().ToString()));
        }
    }
}