package com.helha.backend.controller;

import com.helha.backend.application.service.SerieService;
import com.helha.backend.domain.model.Serie;
import lombok.RequiredArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;
/*
@RestController
@RequestMapping("/api/series")
@RequiredArgsConstructor
public class SerieController {

    private final SerieService serieService;

    @GetMapping
    public List<Serie> getAllSeries() {
        return serieService.getAllSeries();
    }

    @GetMapping("/{id}")
    public ResponseEntity<Serie> getSerie(@PathVariable Long id) {
        Serie serie = serieService.getSerieById(id);
        return serie != null ? ResponseEntity.ok(serie) : ResponseEntity.notFound().build();
    }

    @PostMapping
    public Serie createSerie(@RequestBody Serie serie) {
        return serieService.createSerie(serie);
    }

    @PutMapping("/{id}")
    public Serie updateSerie(@PathVariable Long id, @RequestBody Serie serie) {
        serie.setId(id);
        return serieService.updateSerie(serie);
    }

    @DeleteMapping("/{id}")
    public ResponseEntity<Void> deleteSerie(@PathVariable Long id) {
        serieService.deleteSerie(id);
        return ResponseEntity.noContent().build();
    }
}*/