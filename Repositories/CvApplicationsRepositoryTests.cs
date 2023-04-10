using CVApplicationsManager.Data;
using CVApplicationsManager.Models;
using CVApplicationsManager.Repositories;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CVApplicationsManager.Tests.Repositories
{
    public class CvApplicationsRepositoryTests
    {
        // Create and seed in-memory database
        private async Task<ApplicationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var dbContext = new ApplicationDbContext(options);

            dbContext.Database.EnsureCreated();

            if (!await dbContext.Degrees.AnyAsync())
            {
                for (int i = 0; i < 10; i++)
                {
                    await dbContext.Degrees.AddAsync
                        (
                            new DegreesModel()
                            {
                                DegreeName = $"Test Degree {i}"
                            }
                        );
                    await dbContext.SaveChangesAsync();
                }
            }

            if (!await dbContext.CvApplications.AnyAsync())
            {
                for (int i = 0; i < 10; i++)
                {
                    await dbContext.CvApplications.AddAsync
                        (
                            new CvApplicationModel()
                            {
                                Firstname = $"TestName {i}",
                                Lastname = $"TestLN {i}",
                                Email = $"test{i}@test.com",
                                Mobile = null,
                                Degree = null,
                                DegreeId = null,
                                CvBlob = null,
                                DateCreated = DateTime.Now
                            }
                        );

                    await dbContext.SaveChangesAsync();
                }
            }
            return dbContext;
        }

        [Fact]
        public async void CvApplicationsRepository_AddAsync_AddsEntity()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var cvApplicationsRepository = new CvApplicationsRepository(dbContext);

            var application = new CvApplicationModel()
            {
                Firstname = $"TestName 10",
                Lastname = $"TestLN 10",
                Email = $"test10@test.com",
                Mobile = null,
                Degree = null,
                DegreeId = null,
                CvBlob = null,
                DateCreated = DateTime.Now
            };

            // Act
            await cvApplicationsRepository.AddAsync(application);

            // Assert
            var result = await dbContext.CvApplications.AnyAsync(x => x.Id == application.Id);
            Assert.True(result);
        }

        [Fact]
        public async void CvApplicationsRepository_GetAllAsync_GetsAllEntities()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var cvApplicationsRepository = new CvApplicationsRepository(dbContext);

            // Act
            var applicationsList = await cvApplicationsRepository.GetAllAsync();

            // Assert
            Assert.Equal(10, applicationsList.Count());
        }

        [Fact]
        public async void CvApplicationsRepository_GetAsync_GetsSpecificEntity()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var cvApplicationsRepository = new CvApplicationsRepository(dbContext);

            const int ID = 3;

            // Act
            var application = await cvApplicationsRepository.GetAsync(ID);

            // Assert
            Assert.Equal(ID, application.Id);
        }

        [Fact]
        public async void CvApplicationsRepository_Exists_ReturnsBoolean()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var cvApplicationsRepository = new CvApplicationsRepository(dbContext);

            const int ID = 3;

            // Act
            var result = await cvApplicationsRepository.Exists(ID);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async void CvApplicationsRepository_UpdateAsync_UpdatesEntity()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var cvApplicationsRepository = new CvApplicationsRepository(dbContext);

            const int ID = 3;

            var application = await cvApplicationsRepository.GetAsync(ID);

            var updatedApplication = application;
            updatedApplication.Lastname = "UpdatedLastName";

            // Act
            await cvApplicationsRepository.UpdateAsync(updatedApplication);

            // Assert
            Assert.Equal(application.Lastname, updatedApplication.Lastname);
            Assert.Equal("UpdatedLastName", application.Lastname);
        }

        [Fact]
        public async void CvApplicationsRepository_DeleteAsync_DeletesEntity()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var cvApplicationsRepository = new CvApplicationsRepository(dbContext);

            const int ID = 3;

            // Act
            await cvApplicationsRepository.DeleteAsync(ID);

            // Assert
            var applicationExists = await dbContext.CvApplications.AnyAsync(x => x.Id == ID);
            Assert.False(applicationExists);
        }

        [Fact]
        public async void CvApplicationsRepository_UpdateOrCreateWithFile_DoesNotPassEmptyFileIntoEntity()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var cvApplicationsRepository = new CvApplicationsRepository(dbContext);

            var application = await cvApplicationsRepository.GetAsync(3);
            var file = A.Fake<IFormFile>();

            // Act
            await cvApplicationsRepository.UpdateOrCreateWithFile(file, application);

            // Assert
            application.CvBlob.Should().BeNull();
        }

        [Fact]
        public async void CvApplicationsRepository_UpdateOrCreateWithFile_UpdatesModelCVBlob()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var cvApplicationsRepository = new CvApplicationsRepository(dbContext);

            var testDirectory = Path.Combine(Path.GetTempPath(), "CvApplicationsRepositoryTests");

            Directory.CreateDirectory($"{testDirectory}\\wwwroot\\Files");
            Environment.CurrentDirectory = testDirectory;

            var application = await cvApplicationsRepository.GetAsync(3);

            var file = A.Fake<IFormFile>();
            A.CallTo(() => file.FileName).Returns("test.pdf");
            A.CallTo(() => file.Length).Returns(100);
            A.CallTo(() => file.ContentType).Returns("application/pdf");
            

            // Act
            await cvApplicationsRepository.UpdateOrCreateWithFile(file, application);

            // Assert
            application.CvBlob.Should().NotBeNull();
        }

        [Fact]
        public async void CvApplicationsRepository_UpdateOrCreateWithFile_ThrowsExceptionIfNotPdfOrWord()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var cvApplicationsRepository = new CvApplicationsRepository(dbContext);

            var _testDirectory = Path.Combine(Path.GetTempPath(), "CvApplicationsRepositoryTests");

            Directory.CreateDirectory(_testDirectory);
            Environment.CurrentDirectory = _testDirectory;

            var application = await cvApplicationsRepository.GetAsync(3);

            var file = A.Fake<IFormFile>();
            A.CallTo(() => file.FileName).Returns("test.txt");
            A.CallTo(() => file.Length).Returns(100);
            A.CallTo(() => file.ContentType).Returns("application/text");

            // Act
            await Assert.ThrowsAsync<Exception>
                (
                    () => cvApplicationsRepository.UpdateOrCreateWithFile(file, application)
                );
        }
    }
}