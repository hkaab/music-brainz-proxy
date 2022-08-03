using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Domain
{
    [DataContract]
    public class ArtistCollection
    {
        [DataMember(Name = "count")]
        public int Count { get; set; }

        [DataMember(Name = "offset")]
        public int Offset { get; set; }

        [DataMember(Name = "artists")]
        public List<Artist>  Artists{ get; set; }
    }
}
