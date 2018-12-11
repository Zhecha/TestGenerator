using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace libraryGenerator
{
    public class Reader
    {
        public async Task<string> Read(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            StreamReader stream = new StreamReader(path);
            return await stream.ReadToEndAsync();
        }
    }
}
