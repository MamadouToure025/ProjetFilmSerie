package com.helha.backend.infrastructure.repository;

import com.helha.backend.domain.model.Film;
import com.helha.backend.infrastructure.persistence.FilmEntity;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.Optional;

@Repository
public interface FilmRepository extends JpaRepository<FilmEntity, Long> {

}