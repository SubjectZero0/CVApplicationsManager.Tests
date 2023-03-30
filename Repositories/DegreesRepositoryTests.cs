using CVApplicationsManager.Data;
using CVApplicationsManager.Models;
using CVApplicationsManager.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace CVApplicationsManager.Tests.Repositories
{
    public class DegreesRepositoryTests
    {
        // Create and seed in-memory database
        private static async Task<ApplicationDbContext> GetDbContext()
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
                                DegreeName = null,
                                DegreeId = 1,
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
        public async void DegreesRepository_DeleteUnusedAsync_ThrowsExceptionWhenDeletesUsed()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var degreesRepository = new DegreesRepository(dbContext);
            var cvApplicationsRepository = new CvApplicationsRepository(dbContext);

            const int ID = 1;

            //Act & Arrange
            await Assert.ThrowsAsync<Exception>
            (
                   () => degreesRepository.DeleteUnusedAsync(ID)
               );
        }

        [Fact]
        public async void DegreesRepository_DeleteUnusedAsync_DeletesUnusedDegree()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var degreesRepository = new DegreesRepository(dbContext);
            var cvApplicationsRepository = new CvApplicationsRepository(dbContext);

            await degreesRepository.AddAsync(
                new DegreesModel()
                {
                    DegreeName = "Test Degree 2"
                });

            const int ID = 2;

            //Act
            await degreesRepository.DeleteUnusedAsync(ID);

            // Assert
            Assert.False(await dbContext.Degrees.AnyAsync(x => x.Id == ID));
        }
    }
}