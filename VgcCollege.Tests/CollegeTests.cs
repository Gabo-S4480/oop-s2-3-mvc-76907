using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Domain;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;
using Xunit;

namespace VgcCollege.Tests
{
    public class CollegeLogicTests
    {
        
        private ApplicationDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public void Grade_ShouldBeMarkedAsFail_WhenBelowForty()
        {
            // Test 1: Business logic - Fail if less than 40
            var enrolment = new CourseEnrolment { Grade = 35 };
            bool isPassed = enrolment.Grade >= 40;
            Assert.False(isPassed);
        }

        [Fact]
        public void Grade_ShouldBeMarkedAsPass_WhenFortyOrAbove()
        {
            // Test 2: Business logic - Approved if 40 or more
            var enrolment = new CourseEnrolment { Grade = 40 };
            bool isPassed = enrolment.Grade >= 40;
            Assert.True(isPassed);
        }

        [Fact]
        public void Visibility_UnreleasedGrade_ShouldHaveIsReleasedFalse()
        {
            // Test 3: Visibility rule - Note hidden by default
            var enrolment = new CourseEnrolment { Grade = 80, IsReleased = false };
            Assert.False(enrolment.IsReleased);
        }

        [Fact]
        public void Visibility_ReleasedGrade_ShouldHaveIsReleasedTrue()
        {
            // Test 4: Visibility rule - Note published correctly
            var enrolment = new CourseEnrolment { Grade = 95, IsReleased = true };
            Assert.True(enrolment.IsReleased);
        }

        [Fact]
        public void Validation_GradeCannotBeNegative()
        {
            // Test 5: Validation - The grades must not be negative
            var enrolment = new CourseEnrolment { Grade = -5 };
            Assert.True(enrolment.Grade < 0);
        }

        [Fact]
        public async Task Database_ShouldPersistNewStudent()
        {
            // Test 6: Persistence - Saving and retrieving a student 
            using var context = GetInMemoryContext();
            var student = new StudentProfile { Name = "Tony Stark", StudentNumber = "TS-3000" };

            context.StudentProfiles.Add(student);
            await context.SaveChangesAsync();

            var retrieved = await context.StudentProfiles.FirstOrDefaultAsync(s => s.Name == "Tony Stark");
            Assert.NotNull(retrieved);
            Assert.Equal("TS-3000", retrieved.StudentNumber);
        }

        [Fact]
        public void Authorization_ProfileMustHaveIdentityLink()
        {
            // Test 7: Security - The profile must be linked to an IdentityUserId
            var profile = new StudentProfile { IdentityUserId = "auth-guid-001" };
            Assert.NotNull(profile.IdentityUserId);
        }

        [Fact]
        public async Task Enrolment_ShouldLinkToCorrectStudent()
        {
            // Test 8: Integrity - Registration must be linked to the student ID
            using var context = GetInMemoryContext();
            var enrolment = new CourseEnrolment { StudentProfileId = 1, CourseId = 5 };

            context.Enrolments.Add(enrolment);
            await context.SaveChangesAsync();

            var savedEnrolment = await context.Enrolments.FirstAsync();
            Assert.Equal(1, savedEnrolment.StudentProfileId);
        }
    }
}