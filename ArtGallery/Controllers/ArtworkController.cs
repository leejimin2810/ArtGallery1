﻿using ArtGallery.Data;
using ArtGallery.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.CodeDom;

namespace ArtGallery.Controllers
{
    [Authorize]
    public class ArtworkController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ArtworkController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index(string title, Category[] categories, double? minPrice, double? maxPrice, Status? status, string artistName)
        {
            var artworks = from a in _context.Artworks.Include(a => a.Artist)
                           select a;

            if (!string.IsNullOrEmpty(title))
            {
                artworks = artworks.Where(a => a.Title.Contains(title));
            }

            if (categories != null && categories.Length > 0)
            {
                artworks = artworks.Where(a => categories.Contains(a.Category));
            }

            if (minPrice.HasValue)
            {
                artworks = artworks.Where(a => a.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                artworks = artworks.Where(a => a.Price <= maxPrice.Value);
            }

            if (status.HasValue)
            {
                artworks = artworks.Where(a => a.Status == status.Value);
            }

            if (!string.IsNullOrEmpty(artistName))
            {
                artworks = artworks.Where(a => a.Artist.ArtistName.Contains(artistName));
            }

            var artworkResults = await artworks.ToListAsync();
            var artworkViews = _mapper.Map<List<ArtworkView>>(artworkResults);
            return View(artworkViews);
        }

        public async Task<IActionResult> Admin()
        {
            var artworks = await _context.Artworks.Include(c => c.Artist).ToListAsync();
            var artworkViews = _mapper.Map<List<ArtworkView>>(artworks);
            return View(artworkViews);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var artwork = await _context.Artworks.Include(c => c.Artist).FirstOrDefaultAsync(x => x.ArtworkId == id);
            if (artwork == null)
            {
                return NotFound();
            }
            var artworkView = _mapper.Map<ArtworkView>(artwork);
            return View(artworkView);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Artist = await _context.Artists.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArtworkCreate artworkCreate)
        {
            if (ModelState.IsValid)
            {
                var artwork = _mapper.Map<Artwork>(artworkCreate);
                artwork.CreateAt = DateTime.Now;
                _context.Add(artwork);
                await _context.SaveChangesAsync();
                return RedirectToAction("Admin");
            }

            ViewBag.Artist = await _context.Artists.ToListAsync();
            return View(artworkCreate);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var artwork = await _context.Artworks.FindAsync(id);
            if (artwork == null)
            {
                return NotFound();
            }
            var artworkEdit = _mapper.Map<ArtworkEdit>(artwork);

            ViewBag.Artist = await _context.Artists.ToListAsync();
            return View(artworkEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ArtworkEdit artworkEdit)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artwork = await _context.Artworks.FindAsync(id);
            if (artwork == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _mapper.Map(artworkEdit, artwork);
                artwork.UpdateAt = DateTime.Now;
                _context.Update(artwork);
                await _context.SaveChangesAsync();
                return RedirectToAction("Admin");
            }
            return View(artworkEdit);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var artwork = await _context.Artworks.FindAsync(id);
            if (artwork == null)
            {
                return NotFound();
            }
            var artworkView = _mapper.Map<ArtworkView>(artwork);
            return View(artworkView);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var artwork = await _context.Artworks.FindAsync(id);
            _context.Artworks.Remove(artwork);
            await _context.SaveChangesAsync();
            return RedirectToAction("Admin");
        }

        private bool ArtworkExists(int id)
        {
            return _context.Artworks.Any(e => e.ArtworkId == id);
        }
    }
}
