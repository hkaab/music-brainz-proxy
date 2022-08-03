using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Domain
{
    [DataContract]
    public class ReleaseCollection
    {
        [DataMember(Name = "release-count")]
        public int Count { get; set; }

        [DataMember(Name = "release-offset")]
        public int Offset { get; set; }

        [DataMember(Name = "releases")]
        public List<Release> Releases { get; set; }
    }
}
