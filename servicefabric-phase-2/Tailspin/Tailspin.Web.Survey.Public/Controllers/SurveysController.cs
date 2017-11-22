﻿namespace Tailspin.Web.Survey.Public.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Tailspin.SurveyAnswerService.Client;
    using Tailspin.SurveyManagementService.Client;
    using Tailspin.Web.Survey.Public.Models;
    using Tailspin.Web.Shared.Models;
    using Tailspin.SurveyResponseService.Client;

    public class SurveysController : Controller
    {
        private readonly ISurveyManagementService surveyManagementService;
        private readonly ISurveyAnswerService surveyAnswerService;
        private readonly ISurveyResponseService surveyResponseService;

        public SurveysController(
            ISurveyManagementService surveyManagementService,
            ISurveyAnswerService surveyAnswerService,
            ISurveyResponseService surveyResponseService)
        {
            this.surveyManagementService = surveyManagementService ?? throw new ArgumentNullException(nameof(surveyManagementService));
            this.surveyAnswerService = surveyAnswerService ?? throw new ArgumentNullException(nameof(surveyAnswerService));
            this.surveyResponseService = surveyResponseService ?? throw new ArgumentNullException(nameof(surveyResponseService));
        }

        public async Task<ActionResult> Index()
        {
            var recentSurveys = await this.surveyManagementService.GetLatestSurveysAsync();
            var model = new PageViewData<IEnumerable<Survey>>(recentSurveys.Select(s => s.ToSurvey()).ToList());
            model.Title = "Existing surveys";
            return this.View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Display(string surveySlug)
        {
            var surveyAnswer = await this.CallGetSurveyAndCreateSurveyAnswerAsync(surveySlug);

            var model = new PageViewData<SurveyAnswer>(surveyAnswer);
            model.Title = surveyAnswer.Title;
            return this.View(model);
        }
        
        [HttpPost]
        public async Task<ActionResult> Display(string surveySlug, SurveyAnswer contentModel)
        {
            var surveyAnswer = await this.CallGetSurveyAndCreateSurveyAnswerAsync(surveySlug);

            if (surveyAnswer.QuestionAnswers.Count != contentModel.QuestionAnswers.Count)
            {
                throw new ArgumentException("The survey answers received have different amount of questions than the survey to be filled.");
            }

            for (int i = 0; i < surveyAnswer.QuestionAnswers.Count; i++)
            {
                surveyAnswer.QuestionAnswers[i].Answer = contentModel.QuestionAnswers[i].Answer;
            }

            if (!this.ModelState.IsValid)
            {
                var model = new PageViewData<SurveyAnswer>(surveyAnswer);
                model.Title = surveyAnswer.Title;
                return this.View(model);
            }

            await this.surveyResponseService.SaveSurveyResponseAsync(surveyAnswer.ToSurveyAnswer());

            return this.RedirectToAction("ThankYou");
        }
        
        public ActionResult ThankYou()
        {
            var model = new MasterPageViewData { Title = "Thank you for filling out the survey" };
            return this.View(model);
        }

        private async Task<SurveyAnswer> CallGetSurveyAndCreateSurveyAnswerAsync(string surveySlug)
        {
            var survey = await this.surveyManagementService.GetSurveyAsync(surveySlug);
            return survey.ToSurveyAnswer();
        }
    }
}
