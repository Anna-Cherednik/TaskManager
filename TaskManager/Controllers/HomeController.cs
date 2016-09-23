using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    public class HomeController : Controller
    {
        public ITaskRepository manager;

        public HomeController(ITaskRepository m)
        {
            manager = m;
        }
        public HomeController()
        {
            manager = new TaskRepository();
        }

        // GET: TaskModels
        public ActionResult Index()
        {
            if (HttpContext.User.Identity.Name != "")
            {
                List<TaskModel> tasks = manager.GetMyTaskList(HttpContext.User.Identity.Name);
                return View(tasks);
            }
            else
                return RedirectToAction("Login", "Account");
        }

        [OutputCache(NoStore = true, Location = OutputCacheLocation.Client, Duration = 3)] // every 3 sec
        public ActionResult Refresh()
        {
            List<TaskModel> tasks = manager.GetMyTaskList(HttpContext.User.Identity.Name);
            return PartialView("_Refresher", tasks);
        }        

        // GET: TaskModels/Create
        public ActionResult Create()
        {
            ViewBag.AutorId = new SelectList(manager.GetUserList(), "Id", "Email");
            return View();
        }

        // POST: TaskModels/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TaskModel taskModel)
        {
            if (ModelState.IsValid)
            {
                manager.Save(taskModel, HttpContext.User.Identity.Name);
                return RedirectToAction("Index");
            }

            ViewBag.AutorId = new SelectList(manager.GetUserList(), "Id", "Email", taskModel.AutorId);
            return View(taskModel);
        }

        // GET: TaskModels/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaskModel taskModel = manager.GetTask(id);
            if (taskModel == null)
            {
                return HttpNotFound();
            }
            ViewBag.AutorId = new SelectList(manager.GetUserList(), "Id", "Email", taskModel.AutorId);
            return View(taskModel);
        }

        // POST: TaskModels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TaskModel taskModel)
        {
            if (ModelState.IsValid)
            {
                manager.Update(taskModel);
                return RedirectToAction("Index");
            }
            ViewBag.AutorId = new SelectList(manager.GetUserList(), "Id", "Email", taskModel.AutorId);
            return View(taskModel);
        }

        // GET: TaskModels/Share/5
        public ActionResult Share(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaskModel taskModel = manager.GetTask(id);
            if (taskModel == null)
            {
                return HttpNotFound();
            }
            ViewBag.Manager_Id = new SelectList(manager.GetUserList(), "Id", "Email");
            return View(taskModel);
        }

        // POST: TaskModels/Share/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Share(TaskModel taskModel, string Manager_Id)
        {
            if (ModelState.IsValid)
            {
                manager.SaveToTaskManagedUser(taskModel, Manager_Id);
                return RedirectToAction("Index");
            }
            ViewBag.Manager_Id = new SelectList(manager.GetUserList(), "Id", "Email");
            return View(taskModel);
        }

        // GET: TaskModels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaskModel taskModel = manager.GetTask(id);
            if (taskModel == null)
            {
                return HttpNotFound();
            }
            return View(taskModel);
        }

        // POST: TaskModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            manager.Delete(id);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                manager.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
