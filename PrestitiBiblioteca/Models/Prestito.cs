using System;
using System.Collections.Generic;

namespace PrestitiBiblioteca.Models;

public partial class Prestito
{
    public int IdLibro { get; set; }

    public int Matricola { get; set; }

    public DateTime DataPrestito { get; set; }

    public DateTime? DataRestituzione { get; set; }

    public virtual Libro IdLibroNavigation { get; set; } = null!;

    public virtual Studente MatricolaNavigation { get; set; } = null!;
}
