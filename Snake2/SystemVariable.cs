using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake2
{
    class IntSystemVariable
    {
        private string name;
        EnvironmentVariableTarget target;
        public IntSystemVariable(string vname, EnvironmentVariableTarget target)
        {
            name = vname;
            this.target = target;
        } 
        public void set(int v)
        {
            Environment.SetEnvironmentVariable(name, v.ToString(), target);
        }
        public int get()
        {
            string rs = Environment.GetEnvironmentVariable(name, target);
            if (rs != null)
                return int.Parse(rs);
            else return 0;
        }
    }
}
