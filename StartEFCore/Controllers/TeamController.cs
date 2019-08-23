using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StartEFCore.Entityframework;
using StartEFCore.Models;

namespace StartEFCore.Controllers
{
    public class TeamController : Controller
    {
        private readonly StartEFCoreDbContext _context; //Context'i içeriye aldık

        //const. olusturduk
        public TeamController(StartEFCoreDbContext context)
        {
            _context = context;
        }

        //TODO: TAKIMLARI LİSTELEMEK(LIST)
        public IActionResult Index()
        {
            List<Team> list = _context.Teams.ToList();
            return View(list);
        }

        //TODO:YENİ TAKIM OLUSTURMAK(CREATE)
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Team model)
        {
            if (ModelState.IsValid) //Her zaman yaz(Guvenlik icin onemli)--Required alanları kontrol et(veri tabanı ve class tarafında).
            {
                _context.Teams.Add(model);
                _context.SaveChanges();//sql komutu calıstırmak ıcın yazılmalı
                return RedirectToAction("Index");//calısıp calısmadıgını kontrol etmek ıcın ındex sayfasına atsın.

            }
            return View();
        }

        //TODO: Id'Sİ ESİT OLAN TAKIMIN BİLGİLERİ (DETAIL)
        public IActionResult Detail(int id)
        {
            Team model = _context.Teams.Where(x => x.Id == id).Include(x => x.Players).FirstOrDefault();//include const. ları iceri alır
            return View(model);
        }

        //TODO: Id'Sİ ESİT OLAN TAKIMIN BİLGİLERİ GUNCELLE (UPDATE)
        public ActionResult Edit(int id)
        {
            Team model = _context.Teams.Find(id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Team model)
        {
            if (id != model.Id )//güncellenecek kayıtın id kontrolu yapılıyor(farklı ise hata verdiricek)
            {
                return NotFound();

            }
            if (ModelState.IsValid)
            {
                //context ile guncelleme
                try
                {
                    _context.Teams.Update(model);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DBConcurrencyException ex)
                {
                    if (_context.Teams.Find(id)==null)
                    {
                        return NotFound();
                    }
                    throw ex;
                }
            }

            return View(model);
        }

        //TODO: Id'Sİ ESİT OLAN TAKIMIN BİLGİLERİ SİL (DELETE)
        public IActionResult Delete(int id)
        {
            Team model = _context.Teams.Find(id);
            return View(model);
        }
    

        //delete yapılırken takıma ait oyuncu var mı kotrol et
        //yoksa sil
        //varsa silme
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, Team model)
        {
            
            if (id!=model.Id)//güvenlik,modeldeki dogrulugu kontrol eder, veritabanını da kontrol eder(3 guvenlık asaması saglar)
            {
                return NotFound();
            }
            try
            {
                if (_context.Players.Where(x => x.TeamId == model.Id).Any())
                {
                    return NotFound();
                }
                //Team team = _context.Teams.Find(model.Id);
                _context.Teams.Remove(model);
                _context.SaveChanges();     
                return RedirectToAction("Index");
                // return RedirectToAction("TeamPlayers", new {id = player.TeamId});
            }
            catch (DBConcurrencyException ex)
            {
                throw (ex);
            }

        }
      
    }
}   