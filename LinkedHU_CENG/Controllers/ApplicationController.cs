﻿using Microsoft.AspNetCore.Mvc;
using LinkedHU_CENG.Models;
using LinkedHU_CENG.Models.ViewModels;

namespace LinkedHU_CENG.Controllers
{
    public class ApplicationController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ApplicationController(ApplicationDbContext db, IWebHostEnvironment hostEnvironment)
        {
            this.db = db;
            this.webHostEnvironment = hostEnvironment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<Application> applications = db.Applications.ToList();
            ViewData["Application"] = applications;
            return View();
        }

        public IActionResult Create(AdvertisementViewModel viewModel)
        {
            viewModel.Advertisement = db.Advertisements.Find(viewModel.AdvertisementId);
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult CreateApplication(AdvertisementViewModel viewModel)
        {
            Application application = viewModel.Application;
            int advertisementId = viewModel.AdvertisementId;

            var userId = HttpContext.Session.GetInt32("UserID");
            application.UserId = userId;
            var user = db.Users.Find(userId);
            application.UserName = user.Name + " " + user.Surname;
            application.AdvertisementId = advertisementId;

            if (application.Resume != null)
            {
                string uniqueFileName = UploadedResume(application);
                application.ResumePath = uniqueFileName;
            }

            db.Applications.Add(application);
            db.SaveChanges();
            return RedirectToAction("Index", "Advertisement");
        }

        private string UploadedResume(Application application)
        {
            string uniqueFileName = null;

            string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "Resumes");
            uniqueFileName = Guid.NewGuid().ToString() + "_" + application.Resume.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                application.Resume.CopyTo(fileStream);
            }
            return uniqueFileName;
        }
    }
}