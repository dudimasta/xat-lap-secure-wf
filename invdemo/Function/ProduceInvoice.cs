namespace xat.InvUtils
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Azure.Functions.Extensions.Workflows;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Represents the ProduceInvoice flow invoked function.
    /// </summary>
    public class ProduceInvoice
    {
        private readonly ILogger<ProduceInvoice> logger;

        public ProduceInvoice(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<ProduceInvoice>();
        }

        /// <summary>
        /// Executes the logic app workflow.
        /// <param name="num">Integer parameter.</param>
        /// </summary>
        [Function("ProduceInvoice")]
        public Task<InvoiceEnvelope> Run([WorkflowActionTrigger] int num)
        {
            // this.logger.LogInformation("Starting ProduceInvoice with Zip Code: " + zipCode + " );

            InvoiceGenerator generator = new InvoiceGenerator();
  
            Invoice invoice = generator.GenerateInvoice();

            InvoiceEnvelope envelope = new InvoiceEnvelope(invoice);
            
            return Task.FromResult(envelope);
        }
    }
}