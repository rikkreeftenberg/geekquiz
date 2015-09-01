using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Description;
using GeekQuiz.Models;
using System.Data.Entity.Infrastructure;

namespace GeekQuiz.Controllers
{
    [Authorize]
    public class TriviaQuestionController : ApiController
    {
        private TriviaContext db = new TriviaContext();

        // GET api/TriviaQuestion/0
        [ResponseType(typeof(TriviaQuestion))]
        public async Task<IHttpActionResult> Get(int id)
        {
            var userId = User.Identity.Name;

            TriviaQuestion nextQuestion = await this.NextQuestionAsync(userId);

            if (nextQuestion == null)
            {
                return this.NotFound();
            }

            return this.Ok(nextQuestion);
        }

        // GET api/TriviaQuestion
        public async Task<IHttpActionResult> Get()
        {
            var data = await db.TriviaQuestions.OrderBy(q => q.DisplayOrder).ToListAsync();
            return Ok(data);
        }

        // POST api/TriviaQuestion
        [ResponseType(typeof(TriviaQuestion))]
        public async Task<IHttpActionResult> Post(TriviaQuestion triviaQuestion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var lastDisplayOrder = await this.db.TriviaQuestions
            .OrderByDescending(q => q.DisplayOrder)
            .Select(q => q.DisplayOrder)
            .FirstOrDefaultAsync();
            triviaQuestion.DisplayOrder = lastDisplayOrder + 1;
            db.TriviaQuestions.Add(triviaQuestion);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TriviaQuestionExists(triviaQuestion.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = triviaQuestion.Id }, triviaQuestion);
        }

        // PUT api/<controller>/5 testeee
        public HttpResponseMessage Put(int id, [FromBody]TriviaQuestion trivia)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != trivia.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(trivia).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private async Task<TriviaQuestion> NextQuestionAsync(string userId)
        {
            try
            {
                var lastQuestionId = await this.db.TriviaAnswers
                    .Where(a => a.UserId == userId)
                    .GroupBy(a => a.QuestionId)
                    .Select(g => new { QuestionId = g.Key, Count = g.Count(), Max = g.Max(q => q.Id) })
                    .OrderByDescending(q => new { q.Count, q.Max })
                    .Select(q => q.QuestionId)
                    .FirstOrDefaultAsync();

                var lastDisplayOrder = await this.db.TriviaQuestions
                    .Where(q => q.Id == lastQuestionId)
                    .Select(q => q.DisplayOrder)
                    .FirstOrDefaultAsync();

                var nextQuestionInDisplayOrder = await this.db.TriviaQuestions
                    .Where(q => q.DisplayOrder > lastDisplayOrder)
                    .OrderBy(q => q.DisplayOrder)
                    .FirstOrDefaultAsync();

                if (nextQuestionInDisplayOrder == null)
                {
                    nextQuestionInDisplayOrder = await this.db.TriviaQuestions
                    .OrderBy(q => q.DisplayOrder)
                    .FirstOrDefaultAsync();
                }

                //var questionsCount = await this.db.TriviaQuestions.CountAsync();

                //var nextQuestionId = (lastQuestionId % questionsCount) + 1;
                //return await this.db.TriviaQuestions.FindAsync(CancellationToken.None, nextQuestionId);
                return await this.db.TriviaQuestions.FindAsync(CancellationToken.None, nextQuestionInDisplayOrder.Id);
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }

        // DELETE: api/TriviaQuestion/5
        [ResponseType(typeof(TriviaQuestion))]
        public async Task<IHttpActionResult> DeleteTriviaQuestion(int id)
        {
            TriviaQuestion triviaQuestion = await db.TriviaQuestions.FindAsync(id);
            if (triviaQuestion == null)
            {
                return NotFound();
            }

            db.TriviaQuestions.Remove(triviaQuestion);
            await db.SaveChangesAsync();

            return Ok(triviaQuestion);
        }

        private bool TriviaQuestionExists(int id)
        {
            return db.TriviaQuestions.Count(e => e.Id == id) > 0;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
