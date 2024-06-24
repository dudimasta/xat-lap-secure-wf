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
    public class TestRdu
    {
        private readonly ILogger<ProduceInvoice> logger;

        public TestRdu(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<ProduceInvoice>();
        }

        /// <summary>
        /// Executes the logic app workflow.
        /// </summary>
        /// <param name="zipCode">The zip code.</param>
        /// <param name="temperatureScale">The temperature scale (e.g., Celsius or Fahrenheit).</param>
        [Function("TestRdu")]
        public Task<string> Run([WorkflowActionTrigger] int zipCode, string temperatureScale)
        {
            //this.logger.LogInformation("Starting ProduceInvoice with Zip Code: " + zipCode + " and Scale: " + temperatureScale);
            
            string ret = "hello Rdu";

            return Task.FromResult(ret);
        }
        

        /// <summary>
        /// Represents the weather information for ProduceInvoice.
        /// </summary>
        public class Weather
        {
            /// <summary>
            /// Gets or sets the zip code.
            /// </summary>
            public int ZipCode { get; set; }

            /// <summary>
            /// Gets or sets the current weather.
            /// </summary>
            public string CurrentWeather { get; set; }

            /// <summary>
            /// Gets or sets the low temperature for the day.
            /// </summary>
            public string DayLow { get; set; }

            /// <summary>
            /// Gets or sets the high temperature for the day.
            /// </summary>
            public string DayHigh { get; set; }
        }
    }
}