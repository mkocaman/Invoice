using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESF_kz
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class MyCustomAttribute: System.Attribute
	{
        public string tagName;

        public MyCustomAttribute(string tag)
		{
            tagName = tag;
		}
	}
}
