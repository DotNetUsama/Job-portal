﻿// <auto-generated />
using System;
using Job_Portal_System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Job_Portal_System.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20190425153104_AddPeer3ToNotificationsTable")]
    partial class AddPeer3ToNotificationsTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.0-rtm-35687")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Job_Portal_System.Models.Announcement", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content")
                        .IsRequired();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Headline")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("Image");

                    b.Property<DateTime?>("UpdatedAt");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Announcements");
                });

            modelBuilder.Entity("Job_Portal_System.Models.Applicant", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("JobSeekerId");

                    b.Property<string>("JobVacancyId");

                    b.Property<string>("RecruiterId");

                    b.Property<string>("ResumeId");

                    b.Property<int>("Status");

                    b.Property<DateTime>("SubmittedAt");

                    b.HasKey("Id");

                    b.HasIndex("JobSeekerId");

                    b.HasIndex("JobVacancyId");

                    b.HasIndex("RecruiterId");

                    b.HasIndex("ResumeId");

                    b.ToTable("Applicants");
                });

            modelBuilder.Entity("Job_Portal_System.Models.City", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double?>("Latitude");

                    b.Property<double?>("Longitude");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("Cities");
                });

            modelBuilder.Entity("Job_Portal_System.Models.Company", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Approved");

                    b.Property<string>("Description");

                    b.Property<string>("Email");

                    b.Property<int?>("EmployeesNum");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("Website");

                    b.HasKey("Id");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("Job_Portal_System.Models.CompanyDepartment", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CityId");

                    b.Property<string>("CompanyId");

                    b.Property<string>("DetailedAddress")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("CityId");

                    b.HasIndex("CompanyId");

                    b.ToTable("CompanyDepartments");
                });

            modelBuilder.Entity("Job_Portal_System.Models.DesiredSkill", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("JobVacancyId");

                    b.Property<double?>("Min");

                    b.Property<int>("MinimumYears");

                    b.Property<double?>("Range");

                    b.Property<string>("SkillId");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("JobVacancyId");

                    b.HasIndex("SkillId");

                    b.ToTable("DesiredSkills");
                });

            modelBuilder.Entity("Job_Portal_System.Models.Education", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Degree");

                    b.Property<DateTime?>("EndDate");

                    b.Property<long?>("FieldOfStudyId");

                    b.Property<string>("ResumeId");

                    b.Property<string>("SchoolId");

                    b.Property<DateTime?>("StartDate")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("FieldOfStudyId");

                    b.HasIndex("ResumeId");

                    b.HasIndex("SchoolId");

                    b.ToTable("Educations");
                });

            modelBuilder.Entity("Job_Portal_System.Models.EducationQualification", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Degree");

                    b.Property<long?>("FieldOfStudyId");

                    b.Property<string>("JobVacancyId");

                    b.Property<double?>("Min");

                    b.Property<int>("MinimumYears");

                    b.Property<double?>("Range");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("FieldOfStudyId");

                    b.HasIndex("JobVacancyId");

                    b.ToTable("EducationQualifications");
                });

            modelBuilder.Entity("Job_Portal_System.Models.FieldOfStudy", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(70);

                    b.HasKey("Id");

                    b.ToTable("FieldOfStudies");
                });

            modelBuilder.Entity("Job_Portal_System.Models.FieldOfStudySimilarity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long?>("FieldOfStudyId");

                    b.Property<string>("SimilarTitleId");

                    b.HasKey("Id");

                    b.HasIndex("FieldOfStudyId");

                    b.HasIndex("SimilarTitleId");

                    b.ToTable("FieldOfStudySimilarities");
                });

            modelBuilder.Entity("Job_Portal_System.Models.JobSeeker", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsSeeking");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("JobSeekers");
                });

            modelBuilder.Entity("Job_Portal_System.Models.JobTitle", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(70);

                    b.HasKey("Id");

                    b.ToTable("JobTitles");
                });

            modelBuilder.Entity("Job_Portal_System.Models.JobTitleSimilarity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long?>("JobTitleId");

                    b.Property<string>("SimilarTitleId");

                    b.HasKey("Id");

                    b.HasIndex("JobTitleId");

                    b.HasIndex("SimilarTitleId");

                    b.ToTable("JobTitleSimilarities");
                });

            modelBuilder.Entity("Job_Portal_System.Models.JobVacancy", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AwaitingApplicants");

                    b.Property<string>("CompanyDepartmentId");

                    b.Property<string>("DecisionTreeFile")
                        .HasMaxLength(64);

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<DateTime?>("FinishedAt");

                    b.Property<long?>("JobTitleId");

                    b.Property<double>("MaxSalary");

                    b.Property<int>("Method");

                    b.Property<double>("MinSalary");

                    b.Property<DateTime>("PublishedAt");

                    b.Property<string>("RecruiterId");

                    b.Property<int>("RequiredHires");

                    b.Property<int>("Status");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("CompanyDepartmentId");

                    b.HasIndex("JobTitleId");

                    b.HasIndex("RecruiterId");

                    b.HasIndex("UserId");

                    b.ToTable("JobVacancies");
                });

            modelBuilder.Entity("Job_Portal_System.Models.JobVacancyJobType", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("JobType");

                    b.Property<string>("JobVacancyId");

                    b.HasKey("Id");

                    b.HasIndex("JobVacancyId");

                    b.ToTable("JobVacancyJobTypes");
                });

            modelBuilder.Entity("Job_Portal_System.Models.Notification", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Peer1");

                    b.Property<string>("Peer2");

                    b.Property<string>("Peer3");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("Job_Portal_System.Models.OwnedSkill", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ResumeId");

                    b.Property<string>("SkillId");

                    b.Property<int>("Years");

                    b.HasKey("Id");

                    b.HasIndex("ResumeId");

                    b.HasIndex("SkillId");

                    b.ToTable("OwnedSkills");
                });

            modelBuilder.Entity("Job_Portal_System.Models.Recruiter", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CompanyId");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.HasIndex("UserId");

                    b.ToTable("Recruiters");
                });

            modelBuilder.Entity("Job_Portal_System.Models.Resume", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsPublic");

                    b.Property<string>("JobSeekerId");

                    b.Property<double>("MinSalary");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("JobSeekerId");

                    b.HasIndex("UserId");

                    b.ToTable("Resumes");
                });

            modelBuilder.Entity("Job_Portal_System.Models.ResumeJobType", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("JobType");

                    b.Property<string>("ResumeId");

                    b.HasKey("Id");

                    b.HasIndex("ResumeId");

                    b.ToTable("ResumeJobTypes");
                });

            modelBuilder.Entity("Job_Portal_System.Models.School", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CityId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("CityId");

                    b.ToTable("Schools");
                });

            modelBuilder.Entity("Job_Portal_System.Models.SeekedJobTitle", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long?>("JobTitleId");

                    b.Property<string>("ResumeId");

                    b.HasKey("Id");

                    b.HasIndex("JobTitleId");

                    b.HasIndex("ResumeId");

                    b.ToTable("SeekedJobTitles");
                });

            modelBuilder.Entity("Job_Portal_System.Models.SimilarFieldOfStudyTitle", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("SimilarFieldOfStudyTitles");
                });

            modelBuilder.Entity("Job_Portal_System.Models.SimilarJobTitle", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("SimilarJobTitles");
                });

            modelBuilder.Entity("Job_Portal_System.Models.Skill", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("Skills");
                });

            modelBuilder.Entity("Job_Portal_System.Models.UserNotification", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsRead");

                    b.Property<string>("NotificationId");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("NotificationId");

                    b.HasIndex("UserId");

                    b.ToTable("UserNotifications");
                });

            modelBuilder.Entity("Job_Portal_System.Models.WorkExperience", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CompanyId");

                    b.Property<string>("Description");

                    b.Property<DateTime?>("EndDate");

                    b.Property<long?>("JobTitleId");

                    b.Property<string>("ResumeId");

                    b.Property<DateTime?>("StartDate")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.HasIndex("JobTitleId");

                    b.HasIndex("ResumeId");

                    b.ToTable("WorkExperiences");
                });

            modelBuilder.Entity("Job_Portal_System.Models.WorkExperienceQualification", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long?>("JobTitleId");

                    b.Property<string>("JobVacancyId");

                    b.Property<double?>("Min");

                    b.Property<int>("MinimumYears");

                    b.Property<double?>("Range");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("JobTitleId");

                    b.HasIndex("JobVacancyId");

                    b.ToTable("WorkExperienceQualifications");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");

                    b.HasDiscriminator<string>("Discriminator").HasValue("IdentityUser");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128);

                    b.Property<string>("Name")
                        .HasMaxLength(128);

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Job_Portal_System.Models.User", b =>
                {
                    b.HasBaseType("Microsoft.AspNetCore.Identity.IdentityUser");

                    b.Property<DateTime>("BirthDate");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(25);

                    b.Property<byte>("Gender");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(25);

                    b.HasDiscriminator().HasValue("User");
                });

            modelBuilder.Entity("Job_Portal_System.Models.Announcement", b =>
                {
                    b.HasOne("Job_Portal_System.Models.User", "Author")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Job_Portal_System.Models.Applicant", b =>
                {
                    b.HasOne("Job_Portal_System.Models.User", "JobSeeker")
                        .WithMany()
                        .HasForeignKey("JobSeekerId");

                    b.HasOne("Job_Portal_System.Models.JobVacancy", "JobVacancy")
                        .WithMany("Applicants")
                        .HasForeignKey("JobVacancyId");

                    b.HasOne("Job_Portal_System.Models.User", "Recruiter")
                        .WithMany()
                        .HasForeignKey("RecruiterId");

                    b.HasOne("Job_Portal_System.Models.Resume", "Resume")
                        .WithMany()
                        .HasForeignKey("ResumeId");
                });

            modelBuilder.Entity("Job_Portal_System.Models.CompanyDepartment", b =>
                {
                    b.HasOne("Job_Portal_System.Models.City", "City")
                        .WithMany()
                        .HasForeignKey("CityId");

                    b.HasOne("Job_Portal_System.Models.Company", "Company")
                        .WithMany("Departments")
                        .HasForeignKey("CompanyId");
                });

            modelBuilder.Entity("Job_Portal_System.Models.DesiredSkill", b =>
                {
                    b.HasOne("Job_Portal_System.Models.JobVacancy", "JobVacancy")
                        .WithMany("DesiredSkills")
                        .HasForeignKey("JobVacancyId");

                    b.HasOne("Job_Portal_System.Models.Skill", "Skill")
                        .WithMany()
                        .HasForeignKey("SkillId");
                });

            modelBuilder.Entity("Job_Portal_System.Models.Education", b =>
                {
                    b.HasOne("Job_Portal_System.Models.FieldOfStudy", "FieldOfStudy")
                        .WithMany()
                        .HasForeignKey("FieldOfStudyId");

                    b.HasOne("Job_Portal_System.Models.Resume", "Resume")
                        .WithMany("Educations")
                        .HasForeignKey("ResumeId");

                    b.HasOne("Job_Portal_System.Models.School", "School")
                        .WithMany()
                        .HasForeignKey("SchoolId");
                });

            modelBuilder.Entity("Job_Portal_System.Models.EducationQualification", b =>
                {
                    b.HasOne("Job_Portal_System.Models.FieldOfStudy", "FieldOfStudy")
                        .WithMany()
                        .HasForeignKey("FieldOfStudyId");

                    b.HasOne("Job_Portal_System.Models.JobVacancy", "JobVacancy")
                        .WithMany("EducationQualifications")
                        .HasForeignKey("JobVacancyId");
                });

            modelBuilder.Entity("Job_Portal_System.Models.FieldOfStudySimilarity", b =>
                {
                    b.HasOne("Job_Portal_System.Models.FieldOfStudy", "FieldOfStudy")
                        .WithMany("Similarities")
                        .HasForeignKey("FieldOfStudyId");

                    b.HasOne("Job_Portal_System.Models.SimilarFieldOfStudyTitle", "SimilarTitle")
                        .WithMany("Similarities")
                        .HasForeignKey("SimilarTitleId");
                });

            modelBuilder.Entity("Job_Portal_System.Models.JobSeeker", b =>
                {
                    b.HasOne("Job_Portal_System.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Job_Portal_System.Models.JobTitleSimilarity", b =>
                {
                    b.HasOne("Job_Portal_System.Models.JobTitle", "JobTitle")
                        .WithMany("Similarities")
                        .HasForeignKey("JobTitleId");

                    b.HasOne("Job_Portal_System.Models.SimilarJobTitle", "SimilarTitle")
                        .WithMany("Similarities")
                        .HasForeignKey("SimilarTitleId");
                });

            modelBuilder.Entity("Job_Portal_System.Models.JobVacancy", b =>
                {
                    b.HasOne("Job_Portal_System.Models.CompanyDepartment", "CompanyDepartment")
                        .WithMany()
                        .HasForeignKey("CompanyDepartmentId");

                    b.HasOne("Job_Portal_System.Models.JobTitle", "JobTitle")
                        .WithMany()
                        .HasForeignKey("JobTitleId");

                    b.HasOne("Job_Portal_System.Models.Recruiter", "Recruiter")
                        .WithMany()
                        .HasForeignKey("RecruiterId");

                    b.HasOne("Job_Portal_System.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Job_Portal_System.Models.JobVacancyJobType", b =>
                {
                    b.HasOne("Job_Portal_System.Models.JobVacancy", "JobVacancy")
                        .WithMany("JobTypes")
                        .HasForeignKey("JobVacancyId");
                });

            modelBuilder.Entity("Job_Portal_System.Models.OwnedSkill", b =>
                {
                    b.HasOne("Job_Portal_System.Models.Resume", "Resume")
                        .WithMany("OwnedSkills")
                        .HasForeignKey("ResumeId");

                    b.HasOne("Job_Portal_System.Models.Skill", "Skill")
                        .WithMany()
                        .HasForeignKey("SkillId");
                });

            modelBuilder.Entity("Job_Portal_System.Models.Recruiter", b =>
                {
                    b.HasOne("Job_Portal_System.Models.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId");

                    b.HasOne("Job_Portal_System.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Job_Portal_System.Models.Resume", b =>
                {
                    b.HasOne("Job_Portal_System.Models.JobSeeker", "JobSeeker")
                        .WithMany()
                        .HasForeignKey("JobSeekerId");

                    b.HasOne("Job_Portal_System.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Job_Portal_System.Models.ResumeJobType", b =>
                {
                    b.HasOne("Job_Portal_System.Models.Resume", "Resume")
                        .WithMany("JobTypes")
                        .HasForeignKey("ResumeId");
                });

            modelBuilder.Entity("Job_Portal_System.Models.School", b =>
                {
                    b.HasOne("Job_Portal_System.Models.City", "City")
                        .WithMany()
                        .HasForeignKey("CityId");
                });

            modelBuilder.Entity("Job_Portal_System.Models.SeekedJobTitle", b =>
                {
                    b.HasOne("Job_Portal_System.Models.JobTitle", "JobTitle")
                        .WithMany()
                        .HasForeignKey("JobTitleId");

                    b.HasOne("Job_Portal_System.Models.Resume", "Resume")
                        .WithMany("SeekedJobTitles")
                        .HasForeignKey("ResumeId");
                });

            modelBuilder.Entity("Job_Portal_System.Models.UserNotification", b =>
                {
                    b.HasOne("Job_Portal_System.Models.Notification", "Notification")
                        .WithMany("UserNotifications")
                        .HasForeignKey("NotificationId");

                    b.HasOne("Job_Portal_System.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Job_Portal_System.Models.WorkExperience", b =>
                {
                    b.HasOne("Job_Portal_System.Models.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId");

                    b.HasOne("Job_Portal_System.Models.JobTitle", "JobTitle")
                        .WithMany()
                        .HasForeignKey("JobTitleId");

                    b.HasOne("Job_Portal_System.Models.Resume", "Resume")
                        .WithMany("WorkExperiences")
                        .HasForeignKey("ResumeId");
                });

            modelBuilder.Entity("Job_Portal_System.Models.WorkExperienceQualification", b =>
                {
                    b.HasOne("Job_Portal_System.Models.JobTitle", "JobTitle")
                        .WithMany()
                        .HasForeignKey("JobTitleId");

                    b.HasOne("Job_Portal_System.Models.JobVacancy", "JobVacancy")
                        .WithMany("WorkExperienceQualifications")
                        .HasForeignKey("JobVacancyId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
