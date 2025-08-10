using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PrestitiBiblioteca.Models;

namespace PrestitiBiblioteca.Controllers
{
    public class PrestitiController : Controller
    {
        private readonly PrestitiBibliotecaContext _context;

        public PrestitiController(PrestitiBibliotecaContext context)
        {
            
            _context = context;
        }

        // GET: Prestiti
        public async Task<IActionResult> Index(int pagina, string idStudente, string idLibro, string ordina)
        {
            ViewBag.Header = "Lista Dei Prestiti";

            //var prestitiBibliotecaContext = _context.Prestitos.Include(p => p.IdLibroNavigation).Include(p => p.MatricolaNavigation);


            var record = 50;
            if (pagina == 0)
                pagina = 1;

            var prestiti = from p in _context.Prestitos.Include(pr => pr.IdLibroNavigation).Include(pr => pr.MatricolaNavigation) select p;

            // Ricerca per Libro prestato
            if (!string.IsNullOrEmpty(idLibro))
            {
                prestiti = prestiti.Where(pr => pr.IdLibroNavigation.Titolo.Contains(idLibro));
            }

            // Filtro per Nome dello Student 
            if (!string.IsNullOrEmpty(idStudente))
            {
                prestiti = prestiti.Where(pr => pr.MatricolaNavigation.Nome.Contains(idStudente));
            }

            // Ordinamento crescente o decrescente
            ViewData["CurrentSort"] = string.IsNullOrEmpty(ordina) ? "desc" : "";
            if (ordina == "desc")
            {
                prestiti = prestiti.OrderByDescending(st => st.MatricolaNavigation.Nome);
                ordina = "";
            }
            else
            {
                prestiti = prestiti.OrderBy(st => st.MatricolaNavigation.Nome);
                ordina = "desc";
            }

            ViewBag.TitoloLibro = idLibro;
            ViewBag.Brand = idStudente;
            ViewBag.Pagine = prestiti.ToList().Count / record; //numero pagine
            ViewBag.Pagina = pagina;
            return View(await prestiti.Skip((pagina - 1) * record).Take(record).ToListAsync());            //return View(await prestitiBibliotecaContext.ToListAsync());
        }

        // GET: Prestiti/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prestito = await _context.Prestitos
                .Include(p => p.IdLibroNavigation)
                .Include(p => p.MatricolaNavigation)
                .FirstOrDefaultAsync(m => m.IdLibro == id);
            if (prestito == null)
            {
                return NotFound();
            }

            return View(prestito);
        }

        // GET: Prestiti/Create
        public IActionResult Create()
        {
            ViewData["IdLibro"] = new SelectList(_context.Libros, "Codice", "Codice");
            ViewData["Matricola"] = new SelectList(_context.Studentes, "Matricola", "Matricola");
            return View();
        }

        // POST: Prestiti/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdLibro,Matricola,DataPrestito,DataRestituzione")] Prestito prestito)
        {
            if (ModelState.IsValid)
            {
                _context.Add(prestito);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdLibro"] = new SelectList(_context.Libros, "Codice", "Codice", prestito.IdLibro);
            ViewData["Matricola"] = new SelectList(_context.Studentes, "Matricola", "Matricola", prestito.Matricola);
            return View(prestito);
        }

        // GET: Prestiti/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prestito = await _context.Prestitos.FindAsync(id);
            if (prestito == null)
            {
                return NotFound();
            }
            ViewData["IdLibro"] = new SelectList(_context.Libros, "Codice", "Codice", prestito.IdLibro);
            ViewData["Matricola"] = new SelectList(_context.Studentes, "Matricola", "Matricola", prestito.Matricola);
            return View(prestito);
        }

        // POST: Prestiti/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdLibro,Matricola,DataPrestito,DataRestituzione")] Prestito prestito)
        {
            if (id != prestito.IdLibro)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prestito);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrestitoExists(prestito.IdLibro))
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
            ViewData["IdLibro"] = new SelectList(_context.Libros, "Codice", "Codice", prestito.IdLibro);
            ViewData["Matricola"] = new SelectList(_context.Studentes, "Matricola", "Matricola", prestito.Matricola);
            return View(prestito);
        }

        // GET: Prestiti/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prestito = await _context.Prestitos
                .Include(p => p.IdLibroNavigation)
                .Include(p => p.MatricolaNavigation)
                .FirstOrDefaultAsync(m => m.IdLibro == id);
            if (prestito == null)
            {
                return NotFound();
            }

            return View(prestito);
        }

        // POST: Prestiti/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prestito = await _context.Prestitos.FindAsync(id);
            if (prestito != null)
            {
                _context.Prestitos.Remove(prestito);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PrestitoExists(int id)
        {
            return _context.Prestitos.Any(e => e.IdLibro == id);
        }
    }
}
