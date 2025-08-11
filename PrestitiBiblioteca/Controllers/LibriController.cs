using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using PrestitiBiblioteca.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrestitiBiblioteca.Controllers
{
    public class LibriController : Controller
    {
        private readonly PrestitiBibliotecaContext _context;
        private readonly ILogger<LibriController> _logger;

        // Costruttore principale
        public LibriController(PrestitiBibliotecaContext context, ILogger<LibriController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: Libri
        public async Task<IActionResult> Index(int pagina, string editore, string titoloLibro, string ordina)
        {
            ViewBag.Header = "Lista Dei Libri";

            var record = 10;
            if (pagina == 0)
                pagina = 1;

            try
            {
                _logger.LogInformation("Richiesta della lista dei Libri");

                var libri = _context.Libros.AsQueryable();

                // Ricerca per nome del libro
                if (!string.IsNullOrEmpty(titoloLibro))
                {
                    libri = libri.Where(lb => lb.Titolo.Contains(titoloLibro));
                }

                // Filtro per Editore
                if (!string.IsNullOrEmpty(editore))
                {
                    //libri = libri.Where(l => l.Brand.BrandName.Contains(brand));
                    libri = libri.Where(lb => lb.Editore.Contains(editore));
                }

                // Ordinamento crescente o decrescente
                ViewData["CurrentSort"] = string.IsNullOrEmpty(ordina) ? "desc" : "";
                if (ordina == "desc")
                {
                    libri = libri.OrderByDescending(lb => lb.Titolo);
                    ordina = "";
                }
                else
                {
                    libri = libri.OrderBy(lb => lb.Titolo);
                    ordina = "desc";
                }

                ViewBag.TitoloLibro = titoloLibro;
                ViewBag.Brand = editore;
                ViewBag.Pagine = libri.ToList().Count / record; //numero pagine
                ViewBag.Pagina = pagina;
                return View(await libri.Skip((pagina - 1) * record).Take(record).ToListAsync());            //return View(await _context.Libros.ToListAsync());
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei libri");
                ModelState.AddModelError(string.Empty, "Si è verificato un errore durante il recupero dei libri. Riprova più tardi.");
                return View(new List<Libro>());

            }
        }

        // GET: Libri/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libro = await _context.Libros
                .FirstOrDefaultAsync(m => m.Codice == id);
            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        // GET: Libri/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Libri/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Codice,Autore,Titolo,Editore,Anno,Luogo,Pagine,Classificazione,Collocazione,Copie")] Libro libro)
        {
            if (ModelState.IsValid)
            {
                _context.Add(libro);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(libro);
        }

        // GET: Libri/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libro = await _context.Libros.FindAsync(id);
            if (libro == null)
            {
                return NotFound();
            }
            return View(libro);
        }

        // POST: Libri/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Codice,Autore,Titolo,Editore,Anno,Luogo,Pagine,Classificazione,Collocazione,Copie")] Libro libro)
        {
            if (id != libro.Codice)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(libro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LibroExists(libro.Codice))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(libro);
        }

        // GET: Libri/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libro = await _context.Libros
                .FirstOrDefaultAsync(m => m.Codice == id);
            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        // POST: Libri/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var libro = await _context.Libros.FindAsync(id);
            if (libro != null)
            {
                _context.Libros.Remove(libro);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LibroExists(int id)
        {
            return _context.Libros.Any(e => e.Codice == id);
        }
    }
}
