using System;

using W3Developments.Auditor;

namespace AuditorTest
{
    public class Address
    {
        public Int64 Id { get; set; }
        [Audited]
        public string Line1 { get; set; }
        [Audited]
        public string Line2 { get; set; }
        [Audited]
        public string City { get; set; }
        [Audited]
        public string ZipCode { get; set; }

        public Address()
        {
        }

        public Address(string line1, string line2, string city, string zipCode)
        {
            this.Line1 = line1;
            this.Line2 = line2;
            this.City = city;
            this.ZipCode = zipCode;
        }
    }
}
