using Examination_System.Features.Questions.Commands;
using Examination_System.Features.Questions.DTOs;
using Examination_System.Features.Questions.Orchestrators;
using Examination_System.Features.Questions.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Examination_System.Features.Questions
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public QuestionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddQuestion(AddQuestionViewModel addQuestionViewModel)
        {
            var question = new AddQuestionDTO
            {
                QuizId = addQuestionViewModel.QuizId,
                Text = addQuestionViewModel.Text,
                Options = addQuestionViewModel.Options.Select(o => new AddOptionDTO
                {
                    Text = o.Text,
                    IsCorrect = o.IsCorrect
                }).ToList(),
                OrderIndex = addQuestionViewModel.OrderIndex,
                Explanation = addQuestionViewModel.Explanation
            };
            var result = await _mediator.Send(new AddQuestionCommand(question));

            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateQuestion(UpdateQuestionViewModel updateQuestionViewModel)
        {
            var question = new UpdateQuestionDTO
            {
                QuestionId = updateQuestionViewModel.QuestionId,
                QuizId = updateQuestionViewModel.QuizId,
                Text = updateQuestionViewModel.Text,
                Options = updateQuestionViewModel.Options.Select(o => new UpdateOptionDTO
                {
                    Id = o.Id,
                    Text = o.Text,
                    IsCorrect = o.IsCorrect
                }).ToList(),
                OrderIndex = updateQuestionViewModel.OrderIndex,
                Explanation = updateQuestionViewModel.Explanation
            };
            var result = await _mediator.Send(new UpdateQuestionOrchestrator(question));

            return Ok(result);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteQuestion(Guid questionId)
        {
            var result = await _mediator.Send(new DeleteQuestionCommand(questionId));
            return Ok(result);
        }
    }
}
