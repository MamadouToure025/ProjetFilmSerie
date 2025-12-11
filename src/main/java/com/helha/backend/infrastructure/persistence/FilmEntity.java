package com.helha.backend.infrastructure.persistence;

import jakarta.persistence.*;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Entity
@Table(name = "films")
@Data
@NoArgsConstructor
@AllArgsConstructor
public class FilmEntity {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(unique = true)
    private Integer tmdbId;

    @Column(nullable = false)
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