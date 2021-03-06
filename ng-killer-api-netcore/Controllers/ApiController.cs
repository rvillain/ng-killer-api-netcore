﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NgKillerApiCore.DAL;
using NgKillerApiCore.Models;
using Microsoft.AspNetCore.SignalR;
using NgKillerApiCore.Hubs;

namespace NgKillerApiCore.Controllers
{

    /// <summary>
    /// Controlleur générique API CRUD
    /// </summary>
    /// <typeparam name="T">Type d'entité</typeparam>
    /// <typeparam name="K">Type de la clef primaire de l'entité</typeparam>
    /// <typeparam name="C">Context de la clef primaire</typeparam>
    [Route("api/[controller]")]
    [EnableCors("AllowAll")]
    public abstract class ApiController<T, K, C, H> : Controller
        where T : class, IEntity<K>, new()
        where C : DbContext
        where H : Hub
    {
        protected C Context { get; }
        protected IHubContext<H> _hubContext { get; set; }

        /// <summary>
        /// Création du controlleur avec son contexte
        /// </summary>
        /// <param name="context">contexte de donnée</param>
        protected ApiController(C context, IHubContext<H> hubContext)
        {
            Context = context;
            this._hubContext = hubContext;
        }

        /// <summary>
        /// Inclusions relationnelles supplémentaires
        /// </summary>
        protected List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();

        /// <summary>
        /// Récupère tous les élements 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<T> GetAll()
        {
            var query = Context.Set<T>().AsNoTracking().AsQueryable();
            //Includes.Aggregate(query, (dbSet, inc) => dbSet.Include(inc));
            var result = query.ToList();
            return result;
        }

        /// <summary>
        /// Récupère un élément identifié par sa clef primaire
        /// </summary>
        /// <param name="id">identifiant unique de l'entité</param>
        /// <returns>ObjectResult contenant le résultat ou NotFound si non trouvé</returns>
        [HttpGet("{id}")]
        public virtual IActionResult GetById(K id)
        {
            T item = Context.Set<T>().AsNoTracking().SingleOrDefault(i => i.Id.Equals(id));
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
        public virtual T Create([FromBody]T item)
        {
            if (item == null)
            {
                throw new Exception();
            }

            Context.Set<T>().Attach(item);
            Context.Entry(item).State = EntityState.Added;
            Context.SaveChanges();
            return item;
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

            Context.Set<T>().Attach(item);
            Context.Entry(item).State = EntityState.Modified;
            Context.SaveChanges();
            return new NoContentResult();
        }

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

        /// <summary>
        /// Envoyer un message via signalR à tous les joueurs d'une partie
        /// </summary>
        /// <param name="room"></param>
        /// <param name="method"></param>
        /// <param name="data"></param>
        protected void SendToAll(string room, string method, object data)
        {
            this._hubContext.Clients.Group(room).SendAsync(method, data);
        }
        
    }
}
