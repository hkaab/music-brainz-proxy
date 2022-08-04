using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Domain
{
    [DataContract]
    public class ReleaseCollection
    {
        [DataMember(Name = "count")]
        public int Count { get; set; }

        [DataMember(Name = "offset")]
        public int Offset { get; set; }

        [DataMember(Name = "releases")]
        public List<Release> Releases { get; set; }
    }
}
