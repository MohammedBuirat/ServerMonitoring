using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsumerService
{
    public class GetEnvironmentVariable
    {
        private readonly IConfiguration _configuration;

        public GetEnvironmentVariable(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GetConfigValue(string key)
        {
            string valueFromEnv = Environment.GetEnvironmentVariable(key);
            if (!string.IsNullOrEmpty(valueFromEnv))
            {
                return valueFromEnv;
            }

            string valueFromConfig = _configuration[key];
            if (!string.IsNullOrEmpty(valueFromConfig))
            {
                return valueFromConfig;
            }

            return null;
        }
    }
}
