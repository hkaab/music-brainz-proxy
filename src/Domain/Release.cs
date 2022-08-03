using System.Runtime.Serialization;

namespace Domain
{
    [DataContract(Name = "release")]
    public class Release
    {

        [DataMember(Name = "id")]
        public string Id { get; set; }
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "quality")]
        public string Quality { get; set; }

        [DataMember(Name = "country")]
        public string Country { get; set; }

        [DataMember(Name = "date")]
        public string Date { get; set; }

    }
}
