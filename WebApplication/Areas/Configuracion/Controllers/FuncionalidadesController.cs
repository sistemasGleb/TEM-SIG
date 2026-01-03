using System.Web.Mvc;

namespace WebApplication.Areas.Configuracion.Controllers
{
    public class FuncionalidadesController : Controller
    {
        // GET: Configuracion/Funcionalidades
        public ActionResult Index()
        {
            return View();
        }

        // GET: Configuracion/Funcionalidades/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Configuracion/Funcionalidades/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Configuracion/Funcionalidades/Create
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

        // GET: Configuracion/Funcionalidades/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Configuracion/Funcionalidades/Edit/5
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

        // GET: Configuracion/Funcionalidades/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Configuracion/Funcionalidades/Delete/5
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
