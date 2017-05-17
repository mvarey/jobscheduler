using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSchedulerAgent
{
    public class Variable
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public Variable(string Key, string Value)
        {
            this.Key = Key;
            this.Value = Value;
        }
    }
}
