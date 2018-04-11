using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NgKillerApiCore.DAL;
using NgKillerApiCore.Models;

namespace NgKillerApiCore.Controllers
{
    /// <summary>
    /// Controlleur générique API CRUD
    /// </summary>
    /// <typeparam name="T">Type d'entité</typeparam>
    /// <typeparam name="K">Type de la clef primaire de l'entité</typeparam>
    /// <typeparam name="C">Context de la clef primaire</typeparam>
    [Route("api/[controller]")]
    public abstract class ApiController<T, K, C> : Controller
        where T : class, IEntity<K>, new()
        where C : DbContext
    {
        protected C Context { get; }

        /// <summary>
        /// Création du controlleur avec son contexte
        /// </summary>
        /// <param name="context">contexte de donnée</param>
        protected ApiController(C context)
        {
            Context = context;
        }

        /// <summary>
        /// Récupère tous les élements 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<T> GetAll()
        {
            return Context.Set<T>().ToList();
        }

        /// <summary>
        /// Récupère un élément identifié par sa clef primaire
        /// </summary>
        /// <param name="id">identifiant unique de l'entité</param>
        /// <returns>ObjectResult contenant le résultat ou NotFound si non trouvé</returns>
        [HttpGet("{id}", Name = "GetById")]
        public IActionResult GetById(K id)
        {
            T item = Context.Set<T>().Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        /// <summary>
        /// Ajout de l'élément
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create([FromBody]T item)
        {
            if (item == null)
            {
                return BadRequest();
            }

            Context.Add(item);
            Context.SaveChanges();

            return CreatedAtRoute("GetById", new { id = item.Id }, item);
        }

        /// <summary>
        /// Mise à jour de l'élément
        /// </summary>
        /// <param name="id">identifiant unique de l'élément</param>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public IActionResult Update(K id, [FromBody] T item)
        {
            if (item == null || !item.Id.Equals(id))
            {
                return BadRequest();
            }

            var dbItem = Context.Set<T>().Find(id);
            if (dbItem == null)
            {
                return NotFound();
            }

            UpdateRange(dbItem, item);

            Context.Set<T>().Update(dbItem);
            Context.SaveChanges();
            return new NoContentResult();
        }

        /// <summary>
        /// Mappage des propriété a mettre a jour lors d'un update ou create
        /// Ex: dbItem.Name = bodyItem.Name
        /// </summary>
        /// <param name="dbItem">Item qui sera utilisé</param>
        /// <param name="item">Item provenant d'un post / put (sera </param>
        protected abstract void UpdateRange(T dbItem, T item);

        //protected abstract void Updating(T DbItem, )

        /// <summary>
        /// Supprime l'entrée correspondant à l'identifiant
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(K id)
        {
            var todo = Context.Set<T>().Find(id);
            if (todo == null)
            {
                return NotFound();
            }
            Context.Remove(todo);
            Context.SaveChanges();
            return new NoContentResult();
        }
    }
}
