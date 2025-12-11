package com.helha.backend.infrastructure.persistence;

import jakarta.persistence.*;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Entity
@Table(name = "series")
@Data
@NoArgsConstructor
@AllArgsConstructor
public class SerieEntity {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(unique = true)
    private Integer tmdbId;

    @Column(nullable = false)
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