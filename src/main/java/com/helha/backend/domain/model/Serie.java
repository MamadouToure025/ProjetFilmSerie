package com.helha.backend.domain.model;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class Serie {
    private Long id;
    private Integer tmdbId;
    private String name;
    private String originalName;
    private String overview;
    private String firstAirDate;
    private String lastAirDate;
    private Integer numberOfSeasons;
    private Integer numberOfEpisodes;
    private Integer episodeRuntime;
    private String posterPath;
    private String backdropPath;
    private Double voteAverage;
    private String createdBy;
    private String trailerUrl;
}