using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Students.DAL.Models;
using Students.BLL;
using Students.DAL.Models.Base;
using Students.DAL.Repositories.Base;

namespace VKCommunityWebApi.Controllers
{
    public interface IWebApiBase<TEntity>
        where TEntity : class
    {
        ActionResult<IEnumerable<TEntity>> Get();
        ActionResult<TEntity> Get(int id);
        ActionResult<TEntity> Create(TEntity item);
        ActionResult Update(int id, TEntity itemNew);
        Task<ActionResult> DeleteAsync(int id);
        ActionResult Patch(int id, JsonPatchDocument<TEntity> patchDoc);
    }

    public abstract class WebApiBase<TEntity, TController> : ControllerBase, IWebApiBase<TEntity>
        where TEntity : BaseModel
    {
        private readonly IRepository<TEntity> _repository;
        private readonly ILogger<TController> _logger;

        public WebApiBase(IRepository<TEntity> repository, ILogger<TController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpPost]
        public ActionResult<TEntity> Create(TEntity item)
        {
            _repository.AddAsync(item);
            _repository.SaveAsync();

            return CreatedAtRoute(nameof(Get), new { id = item.Id }, item);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            var item = _repository.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            await _repository.DeleteAsync(item.Result);
            await _repository.SaveAsync();

            return NoContent();
        }

        [HttpGet]
        public ActionResult<IEnumerable<TEntity>> Get()
        {
            var items = _repository.GetAllAsync();
            if (items.Result.Count() <= 0)
            {
                return NotFound();
            }

            return Ok(items);
        }

        [HttpGet("{id}")]
        public ActionResult<TEntity> Get(int id)
        {
            var item = _repository.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        [HttpPatch("{id}")]
        public ActionResult Patch(int id, JsonPatchDocument<TEntity> patchDoc)
        {
            var item = _repository.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            patchDoc.ApplyTo(item.Result, ModelState);

            if (!TryValidateModel(item))
            {
                return ValidationProblem(ModelState);
            }

             _repository.SaveAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
        public ActionResult Update(int id, TEntity itemNew)
        {
            var itemOld = _repository.GetByIdAsync(id);
            if (itemOld == null)
            {
                return NotFound();
            }

            _repository.UpdateAsync(itemNew);
            _repository.SaveAsync();

            return NoContent();
        }
    }
}