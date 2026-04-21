using Microsoft.AspNetCore.Mvc;
using MvcCoreCosmosDb.Models;
using MvcCoreCosmosDb.Services;

namespace MvcCoreCosmosDb.Controllers
{
    public class CochesController : Controller
    {
        private ServiceCosmosDb service;
        public CochesController(ServiceCosmosDb service)
        {
            this.service = service;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(string accion)
        {
            await this.service.CreateDatabaseAsync();
            ViewData["MENSAJE"] = "Database creada correctamente";
            return View();
        }
        public async Task<IActionResult> MisCoches()
        {
            List<Coche> coches = await this.service.GetCochesAsync();
            return View(coches);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Coche car,string existemotor)
        {
            if (existemotor == null)
            {
                car.Motor = null;
            }
            await this.service.CreateCocheAsync(car);
            return RedirectToAction("MisCoches");
        }
        public async Task<IActionResult> Delete(string id)
        {
            await this.service.DeleteCocheAsync(id);
            return RedirectToAction("MisCoches");
        }
        public async Task<IActionResult> Details(string id)
        {
            Coche car = await this.service.FindCocheAsync(id);
            return View(car);
        }
        public async Task<IActionResult> Edit(string id)
        {
            Coche car = await this.service.FindCocheAsync(id);
            return View(car);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Coche car)
        {
            await this.service.UpdateCocheAsync(car);
            return RedirectToAction("MisCoches");
        }
    }
}
