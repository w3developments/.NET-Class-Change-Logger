using System;
using System.Collections.Generic;

using W3Developments.Auditor;

namespace AuditorTest
{
    public class MyUser
    {
        public Int64 Id { get; set; }
        [Audited]
        public string FirstName { get; set; }
        [Audited]
        public string LastName { get; set; }
        [Audited]
        public string EmailAddress { get; set; }
        [Audited]
        public Address Address { get; set; }

        public MyUser(Int64 id, string firstName, string lastName, string emailAddress)
        {
            this.Id = id;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.EmailAddress = emailAddress;
            this.Address = new Address();
        }

        public MyUser(Int64 id, string firstName, string lastName, string emailAddress, string line1, string line2, string city, string zipCode)
        {
            this.Id = id;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.EmailAddress = emailAddress;
            this.Address = new Address(line1, line2, city, zipCode);
        }
    }
}
