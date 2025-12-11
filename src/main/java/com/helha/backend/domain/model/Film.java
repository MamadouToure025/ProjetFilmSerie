package com.helha.backend.domain.model;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class Film {
    private Long id;
    private Integer tmdbId;
    private String title;
    private String originalTitle;
    private String overview;
    private String releaseDate;
    private Integer runtime;
    private String posterPath;
    private String backdropPath;
    private Double voteAverage;
    private String director;
    private String trailerUrl;
}