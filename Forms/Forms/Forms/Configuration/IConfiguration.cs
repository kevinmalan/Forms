using System;
using System.Collections.Generic;
using System.Text;

namespace Forms.Configuration
{
    public interface IConfiguration
    {
        string ApiBaseAddress { get; set; }
    }
}
