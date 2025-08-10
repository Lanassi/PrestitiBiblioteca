using System;
using System.Collections.Generic;

namespace PrestitiBiblioteca.Models;

public partial class Libro
{
    public int Codice { get; set; }

    public string Autore { get; set; } = null!;

    public string Titolo { get; set; } = null!;

    public string Editore { get; set; } = null!;

    public int? Anno { get; set; }

    public string? Luogo { get; set; }

    public string Pagine { get; set; } = null!;

    public string Classificazione { get; set; } = null!;

    public string Collocazione { get; set; } = null!;

    public int? Copie { get; set; }

    public virtual ICollection<Prestito> Prestitos { get; set; } = new List<Prestito>();
}
