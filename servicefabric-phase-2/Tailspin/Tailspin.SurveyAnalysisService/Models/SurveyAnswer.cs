﻿namespace Tailspin.SurveyAnalysisService.Models
{
    using System;
    using System.Collections.Generic;

    public class SurveyAnswer
    {
        public string SlugName { get; set; }

        public string Title { get; set; }

        public DateTime CreatedOn { get; set; }

        public IList<QuestionAnswer> QuestionAnswers { get; set; } = new List<QuestionAnswer>();
    }
}