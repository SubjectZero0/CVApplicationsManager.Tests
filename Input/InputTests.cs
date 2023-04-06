using CVApplicationsManager.Data;
using CVApplicationsManager.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVApplicationsManager.Tests.Input
{
    public class InputTests
    {
        private async Task<ApplicationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var dbContext = new ApplicationDbContext(options);

            dbContext.Database.EnsureCreated();

            return dbContext;
        }

        [Fact]
        public async void InputTests_CvApplicationsTable_CannotAddFirstNameLongerThan20()
        {
            // Arrange
            var dbContext = await GetDbContext();

            // Act
            var result = await dbContext.CvApplications.AddAsync
                (
                     new CvApplicationModel()
                     {
                         Firstname = "abcdefghijklmnopqrstuvxyz",
                         Lastname = "TestLN",
                         Email = "test@test.com",
                         Mobile = null,
                         Degree = null,
                         DegreeId = null,
                         CvBlob = null,
                         DateCreated = DateTime.Now
                     }
                );

            // Assert
            Assert.NotNull(result);
            Assert.False(await dbContext.CvApplications.AnyAsync(x => x.Id == 1));
        }

        [Fact]
        public async void InputTests_CvApplicationsTable_CannotAddLastNameLongerThan20()
        {
            // Arrange
            var dbContext = await GetDbContext();

            // Act
            var result = await dbContext.CvApplications.AddAsync
                (
                     new CvApplicationModel()
                     {
                         Firstname = "Test",
                         Lastname = "LNabcdefghijklmnopqrstuvxyz",
                         Email = "test@test.com",
                         Mobile = null,
                         Degree = null,
                         DegreeId = null,
                         CvBlob = null,
                         DateCreated = DateTime.Now
                     }
                );

            // Assert
            Assert.NotNull(result);
            Assert.False(await dbContext.CvApplications.AnyAsync(x => x.Id == 1));
        }

        [Fact]
        public async void InputTests_CvApplicationsTable_CannotAddEmailNotEmail()
        {
            // Arrange
            var dbContext = await GetDbContext();

            // Act
            var result = await dbContext.CvApplications.AddAsync
                (
                     new CvApplicationModel()
                     {
                         Firstname = "Test",
                         Lastname = "Testakis",
                         Email = "testtest.com",
                         Mobile = null,
                         Degree = null,
                         DegreeId = null,
                         CvBlob = null,
                         DateCreated = DateTime.Now
                     }
                );

            // Assert
            Assert.NotNull(result);
            Assert.False(await dbContext.CvApplications.AnyAsync(x => x.Id == 1));
        }

        [Fact]
        public async void InputTests_CvApplicationsTable_CannotAddMobileLongerThan10()
        {
            // Arrange
            var dbContext = await GetDbContext();

            // Act
            var result = await dbContext.CvApplications.AddAsync
                (
                     new CvApplicationModel()
                     {
                         Firstname = "Test",
                         Lastname = "Testakis",
                         Email = "test@test.com",
                         Mobile = "123456789123",
                         Degree = null,
                         DegreeId = null,
                         CvBlob = null,
                         DateCreated = DateTime.Now
                     }
                );

            // Assert
            Assert.NotNull(result);
            Assert.False(await dbContext.CvApplications.AnyAsync(x => x.Id == 1));
        }

        [Fact]
        public async void InputTests_CvApplicationsTable_CannotAddCvBlobLongerThan60()
        {
            // Arrange
            var dbContext = await GetDbContext();

            // Act
            var result = await dbContext.CvApplications.AddAsync
                (
                     new CvApplicationModel()
                     {
                         Firstname = "Test",
                         Lastname = "Testakis",
                         Email = "test@test.com",
                         Mobile = null,
                         Degree = null,
                         DegreeId = null,
                         CvBlob = "0123456789,0123456789,0123456789,0123456789,,0123456789,0123456789",
                         DateCreated = DateTime.Now
                     }
                );

            // Assert
            Assert.NotNull(result);
            Assert.False(await dbContext.CvApplications.AnyAsync(x => x.Id == 1));
        }
    }
}