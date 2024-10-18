using AutoMapper;
using Contracts.Dtos.SpecializationDtos;
using Contracts.Dtos.UserDtos;
using Domain.Entities;
using Domain.Exceptions.SpecializationExceptions;
using Domain.RepositoryInterfaces;
using FluentValidation;
using Moq;
using Services.Abstractions;
using Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace ProjectTests.ServiceTests
{
    public class SpecializationServiceTests
    {
        private readonly SpecializationService _specializationService;
        private readonly Mock<IRepositoryManager> _repositoryManagerMock = new Mock<IRepositoryManager>();
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
        private readonly Mock<IValidatorManager> _validatorManagerMock = new Mock<IValidatorManager>();

        public SpecializationServiceTests()
        {
            _specializationService = new SpecializationService(_repositoryManagerMock.Object, _mapperMock.Object, _validatorManagerMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ValidSpecialization_ShouldCreateSpecialization()
        {
            // Arrange
            var specializationDtoForCreate = new SpecializationDtoForCreate("TestSpecialization");
            var specialization = new Specialization { Name = "TestSpecialization" };
            var specializationDto = new SpecializationDto(Guid.NewGuid(), "TestSpecialization", new List<UserDto> { });

            _mapperMock.Setup(m => m.Map<Specialization>(specializationDtoForCreate)).Returns(specialization);
            _mapperMock.Setup(m => m.Map<SpecializationDto>(specialization)).Returns(specializationDto);
            _repositoryManagerMock.Setup(r => r.SpecializationRepository.Insert(It.IsAny<Specialization>()));
            _repositoryManagerMock.Setup(r => r.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _specializationService.CreateAsync(specializationDtoForCreate);

            // Assert
            _repositoryManagerMock.Verify(r => r.SpecializationRepository.Insert(specialization), Times.Once);
            _repositoryManagerMock.Verify(r => r.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal(specializationDto.Name, result.Name);
        }

        [Fact]
        public async Task CreateAsync_InvalidSpecialization_ShouldThrowValidationException()
        {
            // Arrange
            var specializationDtoForCreate = new SpecializationDtoForCreate("Invalid");

            _validatorManagerMock.Setup(v => v.ValidateAsync(specializationDtoForCreate, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException("Validation error"));

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _specializationService.CreateAsync(specializationDtoForCreate));
        }

        [Fact]
        public async Task DeleteAsync_ValidSpecializationId_ShouldDeleteSpecialization()
        {
            // Arrange
            Guid specializationId = Guid.NewGuid();
            var specialization = new Specialization { Id = specializationId, Name = "MustBeDeleted" };
            _repositoryManagerMock.Setup(r => r.SpecializationRepository.GetSpecializationByIdAsync(specializationId, It.IsAny<CancellationToken>())).ReturnsAsync(specialization);
            _repositoryManagerMock.Setup(r => r.SpecializationRepository.Remove(It.IsAny<Specialization>()));
            _repositoryManagerMock.Setup(r => r.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            await _specializationService.DeleteAsync(specializationId);

            //Assert
            _repositoryManagerMock.Verify(r => r.SpecializationRepository.Remove(specialization), Times.Once);
            _repositoryManagerMock.Verify(r => r.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_InvalidSpecializationId_ShouldThrowNotFoundSpecializationException()
        {
            // Arrange
            var specializationId = Guid.NewGuid();
            _repositoryManagerMock.Setup(r => r.SpecializationRepository.GetSpecializationByIdAsync(specializationId, It.IsAny<CancellationToken>())).ReturnsAsync((Specialization)null); // Specialization not found

            // Act & Assert
            await Assert.ThrowsAsync<SpecializationNotFoundException>(() => _specializationService.DeleteAsync(specializationId));
        }

        [Fact]
        public async Task UpdateAsync_ValidSpecializationId_ShouldUpdateSpecialization()
        {
            // Arrange
            var specializationId = Guid.NewGuid();
            var specialization = new Specialization { Id = specializationId, Name = "Old" };
            var specializationDtoForUpdate = new SpecializationDtoForUpdate("New");
            _repositoryManagerMock.Setup(r => r.SpecializationRepository.GetSpecializationByIdAsync(specializationId, It.IsAny<CancellationToken>())).ReturnsAsync(specialization);
            _repositoryManagerMock.Setup(r => r.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            await _specializationService.UpdateAsync(specializationId, specializationDtoForUpdate, It.IsAny<CancellationToken>());

            // Assert
            _mapperMock.Verify(m => m.Map(specializationDtoForUpdate, specialization), Times.Once);
            _repositoryManagerMock.Verify(r => r.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_InvalidSpecializationId_ShouldThrowNotFoundSpecializationException()
        {
            // Arrange
            var specializationId = Guid.NewGuid();
            var specializationDtoForUpdate = new SpecializationDtoForUpdate("ShouldBeThrowException");
            _repositoryManagerMock.Setup(r => r.SpecializationRepository.GetSpecializationByIdAsync(specializationId, It.IsAny<CancellationToken>())).ReturnsAsync((Specialization)null);

            //Act & Assert
            await Assert.ThrowsAsync<SpecializationNotFoundException>(() => _specializationService.UpdateAsync(specializationId, specializationDtoForUpdate));
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnListOfSpecializations()
        {
            // Arrange
            Guid specializationId = Guid.NewGuid();
            var specialization = new Specialization { Id = specializationId, Name = "spec" };
            var specializationDto = new SpecializationDto(specializationId, "spec", new List<UserDto>());
            var specializations = new List<Specialization> { specialization };
            _mapperMock.Setup(m => m.Map<IEnumerable<SpecializationDto>>(specializations)).Returns(new List<SpecializationDto> { specializationDto });
            _repositoryManagerMock.Setup(r => r.SpecializationRepository.GetAllSpecializationsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(specializations);

            // Act
            var result = await _specializationService.GetAllAsync(It.IsAny<CancellationToken>());

            // Assert
            _repositoryManagerMock.Verify(r => r.SpecializationRepository.GetAllSpecializationsAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mapperMock.Verify(m => m.Map<IEnumerable<SpecializationDto>>(specializations));
            Assert.Equal(specializations.Count, result.Count); // Not Final Decision;
        }

        [Fact]
        public async Task GetSpecializationById_ValidSpecializationId_ShouldReturnSpecialization()
        {
            // Arrange
            Guid specializationId = Guid.NewGuid();
            List<UserDto> usersDtos = new List<UserDto>();
            List<User> users = new List<User>();
            var specialization = new Specialization { Id = specializationId, Name = "spec", Users = users };
            var specializationDto = new SpecializationDto(specializationId, "spec", usersDtos);
            _mapperMock.Setup(m => m.Map<SpecializationDto>(specialization)).Returns(specializationDto);
            _repositoryManagerMock.Setup(r => r.SpecializationRepository.GetSpecializationByIdAsync(specializationId, It.IsAny<CancellationToken>())).ReturnsAsync(specialization);

            // Act
            var result = await _specializationService.GetSpecializationById(specializationId, It.IsAny<CancellationToken>());

            // Assert
            _repositoryManagerMock.Verify(r => r.SpecializationRepository.GetSpecializationByIdAsync(specializationId, It.IsAny<CancellationToken>()), Times.Once);
            _mapperMock.Verify(m => m.Map<SpecializationDto>(specialization), Times.Once);
            Assert.Equal(specialization.Name, result.Name);
        }

        [Fact]
        public async Task GetSpecializationById_InvalidSpecializationIdShouldThrowSpecializatioNotFoundException()
        {
            // Arrange
            Guid specializationId = Guid.NewGuid();
            _repositoryManagerMock.Setup(r => r.SpecializationRepository.GetSpecializationByIdAsync(specializationId, It.IsAny<CancellationToken>())).ReturnsAsync((Specialization)null);

            // Act & Assert
            await Assert.ThrowsAsync<SpecializationNotFoundException>(() => _specializationService.GetSpecializationById(specializationId, It.IsAny<CancellationToken>()));
        }
    }
}
