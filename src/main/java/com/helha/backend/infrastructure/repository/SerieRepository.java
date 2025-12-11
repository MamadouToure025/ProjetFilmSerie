package com.helha.backend.infrastructure.repository;

import com.helha.backend.domain.model.Serie;
import com.helha.backend.infrastructure.persistence.SerieEntity;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.Optional;

@Repository
public interface SerieRepository extends JpaRepository<SerieEntity, Long> {
}