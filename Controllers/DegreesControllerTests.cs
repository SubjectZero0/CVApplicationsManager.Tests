using AutoMapper;
using CVApplicationsManager.Contracts;
using CVApplicationsManager.Controllers;
using CVApplicationsManager.Models;
using CVApplicationsManager.Views;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace CVApplicationsManager.Tests.Controllers
{
    public class DegreesControllerTests
    {
        // Setup
        private IDegreesRepository _degreesRepository;

        private IMapper _mapper;
        private DegreesController _degreesController;

        public DegreesControllerTests()
        {
            _degreesRepository = A.Fake<IDegreesRepository>();
            _mapper = A.Fake<IMapper>();
            _degreesController = new DegreesController(_degreesRepository, _mapper);
        }

        [Fact]
        public void DegreesController_Index_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = _degreesController.Index();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Task<IActionResult>>();
            result.Result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void DegreesController_GETCreate_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = _degreesController.Create();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Task<IActionResult>>();
            result.Result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void DegreesController_POSTCreate_ReturnsSuccess()
        {
            // Arrange
            var degree = A.Fake<DegreesModel>();
            var expectedDegreeVM = _mapper.Map<CreateDegreeViewModel>(degree);

            // Act
            var result = _degreesController.Create(expectedDegreeVM);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Task<IActionResult>>();
            result.Result.Should().BeOfType<RedirectToActionResult>();
        }

        [Fact]
        public void DegreesController_GETEdit_ReturnsSuccess()
        {
            // Arrange
            var degree = new DegreesModel
            {
                Id = 1,
                DegreeName = "Test",
            };
            var expectedDegreeVM = _mapper.Map<CreateDegreeViewModel>(degree);

            // Act
            var result = _degreesController.Edit(expectedDegreeVM.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Task<IActionResult>>();
            result.Result.Should().BeOfType<ViewResult>();
        }
    }
}