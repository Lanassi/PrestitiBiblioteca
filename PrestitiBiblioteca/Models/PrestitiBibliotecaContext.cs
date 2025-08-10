using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PrestitiBiblioteca.Models;

public partial class PrestitiBibliotecaContext : DbContext
{
    public PrestitiBibliotecaContext()
    {
    }

    public PrestitiBibliotecaContext(DbContextOptions<PrestitiBibliotecaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Libro> Libros { get; set; }

    public virtual DbSet<Prestito> Prestitos { get; set; }

    public virtual DbSet<Studente> Studentes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Libro>(entity =>
        {
            entity.HasKey(e => e.Codice).HasName("PK_Biblioteca_f");

            entity.ToTable("Libro");

            entity.Property(e => e.Codice).ValueGeneratedNever();
            entity.Property(e => e.Autore).HasMaxLength(50);
            entity.Property(e => e.Classificazione).HasMaxLength(50);
            entity.Property(e => e.Collocazione).HasMaxLength(50);
            entity.Property(e => e.Editore).HasMaxLength(50);
            entity.Property(e => e.Luogo).HasMaxLength(50);
            entity.Property(e => e.Pagine).HasMaxLength(50);
            entity.Property(e => e.Titolo).HasMaxLength(50);
        });

        modelBuilder.Entity<Prestito>(entity =>
        {
            entity.HasKey(e => new { e.IdLibro, e.Matricola }).HasName("PK_Prestito_1");

            entity.ToTable("Prestito");

            entity.Property(e => e.DataPrestito).HasColumnType("datetime");
            entity.Property(e => e.DataRestituzione).HasColumnType("datetime");

            entity.HasOne(d => d.IdLibroNavigation).WithMany(p => p.Prestitos)
                .HasForeignKey(d => d.IdLibro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prestito_Libro");

            entity.HasOne(d => d.MatricolaNavigation).WithMany(p => p.Prestitos)
                .HasForeignKey(d => d.Matricola)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prestito_Studente");
        });

        modelBuilder.Entity<Studente>(entity =>
        {
            entity.HasKey(e => e.Matricola).HasName("PK_Studenti");

            entity.ToTable("Studente");

            entity.Property(e => e.Matricola).ValueGeneratedNever();
            entity.Property(e => e.Classe).HasMaxLength(50);
            entity.Property(e => e.Cognome).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Nome).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
