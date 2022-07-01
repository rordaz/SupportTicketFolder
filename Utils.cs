using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vsmTicketFolder
{
    public static class  Utils
    {
        public static void setEnviromentVars()
        {
            var name = "PATH";
            var scope = EnvironmentVariableTarget.Machine; // or User
            var oldValue = Environment.GetEnvironmentVariable(name, scope);
            if (!oldValue.Contains("VSMTicketFolder"))
            {
                var newValue = oldValue + @";C:\Program Files (x86)\SupportCLI\VSMTicketFolder\";
                Environment.SetEnvironmentVariable(name, newValue, scope);
            }


        }
    }
}
