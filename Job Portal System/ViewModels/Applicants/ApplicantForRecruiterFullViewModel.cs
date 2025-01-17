﻿using System;
using Job_Portal_System.Enums;
using Job_Portal_System.Models;

namespace Job_Portal_System.ViewModels.Applicants
{
    public class ApplicantForRecruiterFullViewModel
    {
        public string Id { get; set; }
        public ApplicantStatus Status { get; set; }
        public DateTime SubmittedAt { get; set; }
        public Resume Resume { get; set; }
    }
}
