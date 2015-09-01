using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using GeekQuiz.Models;

namespace GeekQuiz.Controllers
{
    public class TriviaOptionController : ApiController
    {
        private TriviaContext db = new TriviaContext();

        // GET: api/TriviaOption
        public IQueryable<TriviaOption> GetTriviaOptions()
        {
            return db.TriviaOptions;
        }

        // GET: api/TriviaOption/5
        [ResponseType(typeof(TriviaOption))]
        public async Task<IHttpActionResult> GetTriviaOption(int id)
        {
            TriviaOption triviaOption = await db.TriviaOptions.FindAsync(id);
            if (triviaOption == null)
            {
                return NotFound();
            }

            return Ok(triviaOption);
        }

        // PUT: api/TriviaOption/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTriviaOption(int id, TriviaOption triviaOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != triviaOption.QuestionId)
            {
                return BadRequest();
            }

            db.Entry(triviaOption).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TriviaOptionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/TriviaOption
        [ResponseType(typeof(TriviaOption))]
        public async Task<IHttpActionResult> PostTriviaOption(TriviaOption triviaOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TriviaOptions.Add(triviaOption);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TriviaOptionExists(triviaOption.QuestionId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = triviaOption.QuestionId }, triviaOption);
        }

        // DELETE: api/TriviaOption/5
        [ResponseType(typeof(TriviaOption))]
        public async Task<IHttpActionResult> DeleteTriviaOption(int id)
        {
            TriviaOption triviaOption = await db.TriviaOptions.FindAsync(id);
            if (triviaOption == null)
            {
                return NotFound();
            }

            db.TriviaOptions.Remove(triviaOption);
            await db.SaveChangesAsync();

            return Ok(triviaOption);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TriviaOptionExists(int id)
        {
            return db.TriviaOptions.Count(e => e.QuestionId == id) > 0;
        }
    }
}