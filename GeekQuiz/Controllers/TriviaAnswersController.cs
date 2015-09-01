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
    public class TriviaAnswersController : ApiController
    {
        private TriviaContext db = new TriviaContext();

        // GET: api/TriviaAnswers
        public IQueryable<TriviaAnswer> GetTriviaAnswers()
        {
            return db.TriviaAnswers;
        }

        // GET: api/TriviaAnswers/5
        [ResponseType(typeof(TriviaAnswer))]
        public async Task<IHttpActionResult> GetTriviaAnswer(int id)
        {
            TriviaAnswer triviaAnswer = await db.TriviaAnswers.FindAsync(id);
            if (triviaAnswer == null)
            {
                return NotFound();
            }

            return Ok(triviaAnswer);
        }

        // PUT: api/TriviaAnswers/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTriviaAnswer(int id, TriviaAnswer triviaAnswer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != triviaAnswer.Id)
            {
                return BadRequest();
            }

            db.Entry(triviaAnswer).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TriviaAnswerExists(id))
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

        // POST: api/TriviaAnswers
        [ResponseType(typeof(TriviaAnswer))]
        public async Task<IHttpActionResult> PostTriviaAnswer(TriviaAnswer triviaAnswer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            triviaAnswer.UserId = User.Identity.Name;

            this.db.TriviaAnswers.Add(triviaAnswer);

            await db.SaveChangesAsync();
            var selectedOption = await db.TriviaOptions.FirstOrDefaultAsync(o => o.Id == triviaAnswer.OptionId
                && o.QuestionId == triviaAnswer.QuestionId);

            return this.Ok<bool>(selectedOption.IsCorrect);
        }

        // DELETE: api/TriviaAnswers/5
        [ResponseType(typeof(TriviaAnswer))]
        public async Task<IHttpActionResult> DeleteTriviaAnswer(int id)
        {
            TriviaAnswer triviaAnswer = await db.TriviaAnswers.FindAsync(id);
            if (triviaAnswer == null)
            {
                return NotFound();
            }

            db.TriviaAnswers.Remove(triviaAnswer);
            await db.SaveChangesAsync();

            return Ok(triviaAnswer);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TriviaAnswerExists(int id)
        {
            return db.TriviaAnswers.Count(e => e.Id == id) > 0;
        }
    }
}