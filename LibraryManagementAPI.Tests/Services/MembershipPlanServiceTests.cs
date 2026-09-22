using LibraryManagementAPI.Application.DTOs.MembershipPlan;
using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Application.Services;
using LibraryManagementAPI.Domain.Entities;
using Moq;

namespace LibraryManagementAPI.Tests.Services
{
    public class MembershipPlanServiceTests
    {
        private readonly Mock<IMembershipPlanRepository> _repositoryMock;
        private readonly MembershipPlanService _service;

        public MembershipPlanServiceTests()
        {
            _repositoryMock = new Mock<IMembershipPlanRepository>();
            _service = new MembershipPlanService(_repositoryMock.Object);
        }

        private MembershipPlan CreateMembershipPlan()
        {
            return new MembershipPlan
            {
                Id = Guid.NewGuid(),
                Name = "Standart Paket",
                MaxBorrowLimit = 5,
                Price = 100,
                DurationInDays = 30,
                IsDeleted = false
            };
        }

        [Fact]
        public void GetAll_ShouldReturnMembershipPlans()
        {
            var plans = new List<MembershipPlan>
            {
                CreateMembershipPlan(),
                CreateMembershipPlan()
            };

            _repositoryMock
                .Setup(x => x.GetAll())
                .Returns(plans);

            var result = _service.GetAll();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetById_ShouldReturnMembershipPlan_WhenPlanExists()
        {
            var plan = CreateMembershipPlan();

            _repositoryMock
                .Setup(x => x.GetById(plan.Id))
                .Returns(plan);

            var result = _service.GetById(plan.Id);

            Assert.NotNull(result);
            Assert.Equal(plan.Id, result!.Id);
            Assert.Equal(plan.Name, result.Name);
            Assert.Equal(plan.Price, result.Price);
        }

        [Fact]
        public void GetById_ShouldReturnNull_WhenPlanDoesNotExist()
        {
            var id = Guid.NewGuid();

            _repositoryMock
                .Setup(x => x.GetById(id))
                .Returns((MembershipPlan?)null);

            var result = _service.GetById(id);

            Assert.Null(result);
        }

        [Fact]
        public void Add_ShouldCallRepositoryAdd()
        {
            var dto = new MembershipPlanCreateDto
            {
                Name = "Premium Paket",
                MaxBorrowLimit = 10,
                Price = 200,
                DurationInDays = 30
            };

            _service.Add(dto);

            _repositoryMock.Verify(
                x => x.Add(It.IsAny<MembershipPlan>()),
                Times.Once);
        }

        [Fact]
        public void Update_ShouldUpdateMembershipPlan_WhenPlanExists()
        {
            var plan = CreateMembershipPlan();

            _repositoryMock
                .Setup(x => x.GetById(plan.Id))
                .Returns(plan);

            var dto = new MembershipPlanUpdateDto
            {
                Name = "Premium Paket",
                MaxBorrowLimit = 10,
                Price = 200,
                DurationInDays = 60
            };

            _service.Update(plan.Id, dto);

            Assert.Equal("Premium Paket", plan.Name);
            Assert.Equal(10, plan.MaxBorrowLimit);
            Assert.Equal(200, plan.Price);
            Assert.Equal(60, plan.DurationInDays);

            _repositoryMock.Verify(
                x => x.Update(plan),
                Times.Once);
        }

        [Fact]
        public void Update_ShouldThrowException_WhenPlanDoesNotExist()
        {
            var id = Guid.NewGuid();

            _repositoryMock
                .Setup(x => x.GetById(id))
                .Returns((MembershipPlan?)null);

            var dto = new MembershipPlanUpdateDto
            {
                Name = "Premium Paket",
                MaxBorrowLimit = 10,
                Price = 200,
                DurationInDays = 60
            };

            Assert.Throws<KeyNotFoundException>(
                () => _service.Update(id, dto));
        }

        [Fact]
        public void Delete_ShouldCallRepositoryDelete_WhenPlanExists()
        {
            var plan = CreateMembershipPlan();

            _repositoryMock
                .Setup(x => x.GetById(plan.Id))
                .Returns(plan);

            _service.Delete(plan.Id);

            _repositoryMock.Verify(
                x => x.Delete(plan),
                Times.Once);
        }

        [Fact]
        public void Delete_ShouldThrowException_WhenPlanDoesNotExist()
        {
            var id = Guid.NewGuid();

            _repositoryMock
                .Setup(x => x.GetById(id))
                .Returns((MembershipPlan?)null);

            Assert.Throws<KeyNotFoundException>(
                () => _service.Delete(id));
        }
    }
}