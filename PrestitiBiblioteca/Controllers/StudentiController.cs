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
    public class StudentiController : Controller
    {
        private readonly PrestitiBibliotecaContext _context;

        public StudentiController(PrestitiBibliotecaContext context)
        {
            _context = context;
        }

        // GET: Studenti
        public async Task<IActionResult> Index(int pagina, string nome, string cognome, string ordina)
        {
            ViewBag.Header = "Lista Degli Studenti";

            var record = 10;
            if (pagina == 0)
                pagina = 1;

            var studenti = _context.Studentes.AsQueryable();

            // Ricerca per Cognome degli Studenti
            if (!string.IsNullOrEmpty(cognome))
            {
                studenti = studenti.Where(st => st.Cognome.Contains(cognome));
            }

            // Filtro per Nome degli Studenti
            if (!string.IsNullOrEmpty(nome))
            {
                //libri = libri.Where(l => l.Brand.BrandName.Contains(brand));
                studenti = studenti.Where(st => st.Nome.Contains(nome));
            }

            // Ordinamento crescente o decrescente
            ViewData["CurrentSort"] = string.IsNullOrEmpty(ordina) ? "desc" : "";
            if (ordina == "desc")
            {
                studenti = studenti.OrderByDescending(st => st.Nome);
                ordina = "";
            }
            else
            {
                studenti = studenti.OrderBy(st => st.Nome);
                ordina = "desc";
            }

            ViewBag.TitoloLibro = cognome;
            ViewBag.Brand = nome;
            ViewBag.Pagine = studenti.ToList().Count / record; //numero pagine
            ViewBag.Pagina = pagina;
            return View(await studenti.Skip((pagina - 1) * record).Take(record).ToListAsync());            //return View(await _context.Studentes.ToListAsync());
        }

        // GET: Studenti/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studente = await _context.Studentes
                .FirstOrDefaultAsync(m => m.Matricola == id);
            if (studente == null)
            {
                return NotFound();
            }

            return View(studente);
        }

        // GET: Studenti/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Studenti/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Matricola,Nome,Cognome,Email,Classe")] Studente studente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(studente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(studente);
        }

        // GET: Studenti/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studente = await _context.Studentes.FindAsync(id);
            if (studente == null)
            {
                return NotFound();
            }
            return View(studente);
        }

        // POST: Studenti/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Matricola,Nome,Cognome,Email,Classe")] Studente studente)
        {
            if (id != studente.Matricola)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudenteExists(studente.Matricola))
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
            return View(studente);
        }

        // GET: Studenti/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studente = await _context.Studentes
                .FirstOrDefaultAsync(m => m.Matricola == id);
            if (studente == null)
            {
                return NotFound();
            }

            return View(studente);
        }

        // POST: Studenti/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studente = await _context.Studentes.FindAsync(id);
            if (studente != null)
            {
                _context.Studentes.Remove(studente);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudenteExists(int id)
        {
            return _context.Studentes.Any(e => e.Matricola == id);
        }
    }
}
