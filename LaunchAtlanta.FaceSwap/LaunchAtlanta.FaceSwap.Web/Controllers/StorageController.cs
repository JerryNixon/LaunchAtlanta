using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace LaunchAtlanta.FaceSwap.Web.Controllers
{
    public class StorageController : Controller
    {
        

        // GET: Storage
        public ActionResult Index()
        {
            

            return View();
        }

        // GET: Storage/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Storage/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Storage/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Storage/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Storage/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Storage/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Storage/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
