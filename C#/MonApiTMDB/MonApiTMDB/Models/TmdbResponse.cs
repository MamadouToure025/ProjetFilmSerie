namespace MonApiTMDB.Models
{
    public class TmdbResponse
    {
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int TotalResults { get; set; }
        public IEnumerable<Movie>? Results { get; set; } // Liste des films
    }
}