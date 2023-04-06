using AutoMapper;
using CVApplicationsManager.Contracts;
using CVApplicationsManager.Controllers;
using CVApplicationsManager.Data;
using CVApplicationsManager.Models;
using CVApplicationsManager.Views;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CVApplicationsManager.Tests.Controllers
{
    public class CvApplicationsControllerTests
    {
        private ICvApplicationRepository _cvApplicationsRepository;
        private IDegreesRepository _degreesRepository;
        private IMapper _mapper;
        private CvApplicationsController _cvApplicationsController;

        public CvApplicationsControllerTests()
        {
            _cvApplicationsRepository = A.Fake<ICvApplicationRepository>();
            _degreesRepository = A.Fake<IDegreesRepository>();
            _mapper = A.Fake<IMapper>();
            _cvApplicationsController = new CvApplicationsController(_cvApplicationsRepository, _mapper, _degreesRepository);
        }

        [Fact]
        public void CvApplicationsController_Index_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = _cvApplicationsController.Index();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Task<IActionResult>>();
            result.Result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void CvApplicationsController_Details_ReturnsSuccess()
        {
            // Arrange
            var application = A.Fake<CvApplicationViewModel>();

            // Act
            var result = _cvApplicationsController.Details(application.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Task<IActionResult>>();
            result.Result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void CvApplicationsController_GETCreate_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = _cvApplicationsController.Create();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Task<IActionResult>>();
            result.Result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void CvApplicationsController_POSTCreate_ReturnsSuccess()
        {
            // Arrange
            var application = A.Fake<CvApplicationViewModel>();
            var inputFile = A.Fake<IFormFile>();

            // Act
            var result = _cvApplicationsController.Create(application, inputFile);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Task<IActionResult>>();
            result.Result.Should().BeOfType<RedirectToActionResult>();
        }

        [Fact]
        public void CvApplicationsController_GETEdit_ReturnsSuccess()
        {
            // Arrange
            var application = A.Fake<CvApplicationViewModel>();
            var inputFile = A.Fake<IFormFile>();

            // Act
            var result = _cvApplicationsController.Edit(application.Id, application, inputFile);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Task<IActionResult>>();
            result.Result.Should().BeOfType<RedirectToActionResult>();
        }
    }
}