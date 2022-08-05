using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Domain
{
    [DataContract(Name = "artist")]
    public class Artist
    {
        [DataMember(Name = "id", IsRequired = true)]
        public string Id { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "gender")]
        public string Gender { get; set; }

        [DataMember(Name = "rating")]
        public Rating Rating { get; set; }

        [DataMember(Name = "country")]
        public string Country { get; set; }

        [DataMember(Name = "score")]
        public int Score { get; set; }

        [DataMember(Name = "releases")]
        public List<Release> Releases { get; set; }
    }
}
