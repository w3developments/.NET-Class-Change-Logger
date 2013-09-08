using System;
using System.Collections.Generic;

using W3Developments.Auditor;

namespace AuditorTest
{
    class Program
    {
        static void Main(string[] args)
        {
            MyUser original = new MyUser(1, "Joe", "Bloggs", "joe.bloggs@test.com");
            MyUser changed = new MyUser(1, "John", "Bloggs", "john.bloggs@test.com", "Line 1", "Line 2", "City", "ZipCode");

            ChangeLogger logger = new ChangeLogger(original.Id, original, changed);
            if (!logger.Success)
            {
                Console.Write(logger.Exception.ToString());
                return;
            }

            // You can also change the value of the changed class before calling Audit
            changed.LastName = "Blogs";

            logger.Audit();
            if (!logger.Success)
            {
                Console.Write(logger.Exception.ToString());
                return;
            }

            foreach (ChangeLog change in logger.Changes)
            {
                Console.Write(string.Format("{2} property changed from {3} to {4} for class {1} with id {0}\r\n", change.ObjectId, change.ObjectType, change.Property, change.ValueOld, change.ValueNew));
            }

            Console.ReadKey();
        }
    }
}
