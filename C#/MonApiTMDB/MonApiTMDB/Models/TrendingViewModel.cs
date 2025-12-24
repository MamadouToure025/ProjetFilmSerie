using MonApiTMDB.Models;

namespace MonApiTMDB.Models
{
    public class TrendingViewModel
    {
        // Liste des films en vogue
        public List<Movie> Movies { get; set; } = new();

        // Liste des séries TV en vogue
        public List<TvShow> TvShows { get; set; } = new();

        // Liste des personnes (acteurs) en vogue
        public List<PersonDetail> People { get; set; } = new();
    }
}