using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace libraryGenerator
{
    public class Writer
    {
        public string dir { get; private set; }

        public Writer() { }

        public void CheckAndSetDir (string directory)
        {
            if (directory == null)
                throw new ArgumentNullException(nameof(directory));
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            dir = directory;
        }

        public async Task Write(List<TestClass> classes)
        {
            string path = Path.Combine(dir, classes[0].name + ".cs");
            FileStream stream = new FileStream(path,FileMode.Create,FileAccess.Write);
            foreach (TestClass testClass in classes)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(testClass.include);
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }
        }
    }
}
