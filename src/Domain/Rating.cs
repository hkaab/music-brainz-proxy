using System.Runtime.Serialization;

namespace Domain
{
    [DataContract(Name = "rating")]
    public class Rating
    {
        [DataMember(Name = "votes-count")]
        public int VotesCount { get; set; }

        [DataMember(Name = "value")]
        public double? Value { get; set; }
    }
}
