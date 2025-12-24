using System.Text.Json.Serialization;

namespace MonApiTMDB.Models
{
    // L'objet principal : La Saga (Collection)
    public class CollectionDetail
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("overview")]
        public string Overview { get; set; }

        [JsonPropertyName("poster_path")]
        public string PosterPath { get; set; }

        [JsonPropertyName("backdrop_path")]
        public string BackdropPath { get; set; }

        // La liste des films de la saga
        [JsonPropertyName("parts")]
        public List<CollectionPart> Parts { get; set; } = new();
    }

    // Le détail d'un film dans la collection
    public class CollectionPart
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("overview")]
        public string Overview { get; set; }

        [JsonPropertyName("poster_path")]
        public string PosterPath { get; set; }

        [JsonPropertyName("release_date")]
        public string ReleaseDate { get; set; }

        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }
        
        [JsonPropertyName("popularity")]
        public double Popularity { get; set; }
    }
}