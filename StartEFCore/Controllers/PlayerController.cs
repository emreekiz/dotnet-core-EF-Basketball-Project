using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using StartEFCore.Entityframework;
using StartEFCore.Models;

namespace StartEFCore.Controllers
{
    public class PlayerController : Controller
    {
        private readonly StartEFCoreDbContext _context;
        public PlayerController(StartEFCoreDbContext context)
        {
            _context = context;
        }

        //TODO: TUM OYUNCULARIN INDEX ACTİON İ YAP
        public IActionResult Index()
        {
            List<Player> list = _context.Players.ToList();
            return View(list);
        }


        //TeamId degerine eşit gelecek id paramatresi alır
        //TODO: TAKIMIN OYUNCULARINI LİSTELEMEK(LIST)
        public IActionResult TeamPlayers(int id)
        {
            List<Player> list = _context.Players.Where(x => x.TeamId == id).ToList();
            ViewData["TeamId"] = id;
            return View(list);
        }
        //TODO:YENİ OYUNCU OLUSTURMAK BELİRTİLEN TAKIM İÇİN(CREATE)
        //[Route("create/team'player/{id}")]
        public IActionResult CreatePlayerToTeam(int teamId)
        {
            Player model=new Player();
            model.TeamId = teamId;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePlayerToTeam(Player model)
        {
            if (ModelState.IsValid)
            {
                //model dogruysa 
                //modeli context'e ekle
                _context.Players.Add(model);
                //contextteki tüm degisikleri kaydet
                _context.SaveChanges();
                return RedirectToAction("TeamPlayers", new {id = model.TeamId});

            }

            return View(model);
        }
        //TODO: Id'Sİ ESİT OLAN OYUNCUNUN BİLGİLERİ (DETAIL)
        public IActionResult Details(int id)
        {
            Player model = _context.Players.Find(id);
            return View(model);
        }
        //TODO: Id'Sİ ESİT OLAN OYUNCUNUN BİLGİLERİNİ GUNCELLE (UPDATE)
        public IActionResult Edit(int id)
        {
            Player model = _context.Players.Find(id);
            ViewBag.TeamsDDL = _context.Teams.Select(u => new SelectListItem
            {
                Selected = false,
                Text = u.Name,
                Value = u.Id.ToString()
            }).ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Player model)
        {
            if (id != model.Id)//güncellenecek kayıtın id kontrolu yapılıyor(farklı ise hata verdiricek)
            {
                return NotFound();

            }

            if (ModelState.IsValid)
            {
                //context ile guncelleme
                try
                {
                    TryToUpdatePlayer(model);
                    return RedirectToAction("Edit", new {id = id});
                }
                catch (DBConcurrencyException ex)
                {
                    if (_context.Players.Find(id)==null)
                    {
                        return NotFound();
                    }
                    throw ex;
                }
            }

            return View(model);
        }

        //void method geri dönüş değeri yoktur.
        private void TryToUpdatePlayer(Player model)
        {
            _context.Players.Update(model);
            _context.SaveChanges();
        }

        //TODO: Id'Sİ ESİT OLAN OYUNCUNUN BİLGİLERİNİ SİL (DELETE)
        //public IActionResult Delete(int id)
        //{
        //    Player model = _context.Players.Find(id);
        //    return View(model);
        //}
        public IActionResult Delete(int id, string returnUrl = "")
        {
            ViewBag.ReturnUrl = string.IsNullOrEmpty(returnUrl) ? "" : returnUrl;
            Player model = _context.Players.Find(id);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id,Player model, string returnUrl = "")
        {
            if (_context.Players.Find(model.Id) == null|| id!=model.Id)//güvenlik,modeldeki dogrulugu kontrol eder, veritabanını da kontrol eder(3 guvenlık asaması saglar)
            {
                return NotFound();
               
            }

            try
            {
                Player player = _context.Players.Find(model.Id);
                _context.Players.Remove(player);
                _context.SaveChanges();
                //return RedirectToAction("TeamPlayers",
                //    new { id = player.TeamId });
                if (!string.IsNullOrEmpty(returnUrl)
                    && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index");
            }
            catch (DBConcurrencyException ex)
            {
                throw (ex);
            }
           
        }
    }

}