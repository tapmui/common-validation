using System;
using System.Collections.Generic;
using System.Text;

namespace Collector.Common.Validation.AccountNumber.Models
{
    public class BankIdentityModel
    {
        public BankIdentityModel(string clearing, string name)
        {
            Clearing = clearing;
            Name = name;
        }

        public string Clearing { get; }
        public string Name { get; }
    }
}
