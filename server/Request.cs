using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    [Serializable]
    public class Request
    {
        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public string type;
        public long instant = (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        public string body;
        public string nick;
    }
}
