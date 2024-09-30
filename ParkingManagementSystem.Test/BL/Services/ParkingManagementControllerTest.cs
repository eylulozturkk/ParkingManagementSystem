using AutoFixture;
using AutoMapper;
using Moq;
using ParkingManagementSystem.BL.Dto.Request;
using ParkingManagementSystem.BL.Dto.Response;
using ParkingManagementSystem.BL.Interface;
using ParkingManagementSystem.DAL.Entity;
using ParkingManagementSystem.DAL.Enums;
using ParkingManagementSystem.DAL.GenericRepository;
using ParkingManagementSystem.DAL.UOW;
using Xunit;

namespace ParkingManagementSystem.Test.BL.Services
{
    public class ParkingManagementControllerTest
    {
        ParkingManagementSystem.BL.Services.ParkingSpotService _ps;
        ParkingManagementSystem.BL.Services.VehicleService _v;

        Mock<IUnitOfWork> _uow = new Mock<IUnitOfWork>();
        Mock<IMapper> _mapper = new Mock<IMapper>();
        Mock<IGenericRepository<ParkingSpot>> _parkingSpotRepository = new Mock<IGenericRepository<ParkingSpot>>();
        Mock<IGenericRepository<Vehicle>> _vehicleRepository = new Mock<IGenericRepository<Vehicle>>();
        Mock<IRedisCacheService> _redisCacheService = new Mock<IRedisCacheService>();
        Mock<ILogger> _logger = new Mock<ILogger>();
        Mock<IParkingSpotService> _parkingSpotService = new Mock<IParkingSpotService>();

        Fixture _fixture = new Fixture();

        long _vehicleId;
        string _licensePlate;
        VehicleRequest _vehicleRequest;
        Vehicle _vehicle;

        long _parkingSpotId;
        VehicleSizeType _type;
        ParkingSpotRequest _parkingSpotRequest;
        ParkingSpot _parkingSpot;

        public ParkingManagementControllerTest()
        {
            _vehicleId = _fixture.Create<long>();
            _licensePlate = _fixture.Create<string>();
            _vehicleRequest = _fixture.Create<VehicleRequest>();
            _vehicle = _fixture.Create<Vehicle>();

            _parkingSpotId = _fixture.Create<long>();
            _type = _fixture.Create<VehicleSizeType>();
            _parkingSpotRequest = _fixture.Create<ParkingSpotRequest>();
            _parkingSpot = _fixture.Create<ParkingSpot>();

            _ps = new ParkingManagementSystem.BL.Services.ParkingSpotService(_mapper.Object, _uow.Object, _redisCacheService.Object, _logger.Object);
            _v = new ParkingManagementSystem.BL.Services.VehicleService(_mapper.Object, _uow.Object, _parkingSpotService.Object, _redisCacheService.Object, _logger.Object);

        }

        [Fact]
        public void GetParkingSpotByParkingSpotId_HappyPath()
        {
            // Arrange
            var expectedParkingSpot = _fixture.Create<ParkingSpotResponse>();

            _parkingSpotRepository.Setup(x => x.GetAsync(_parkingSpotId)).ReturnsAsync(_parkingSpot);
            _uow.Setup(x => x.GetRepository<ParkingSpot>()).Returns(_parkingSpotRepository.Object);
            _mapper.Setup(x => x.Map<ParkingSpotResponse>(_parkingSpot)).Returns(expectedParkingSpot);

            // Act
            var result = _ps.GetParkingSpotByIdAsync(_parkingSpotId).GetAwaiter().GetResult();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedParkingSpot, result);
            _parkingSpotRepository.Verify(x => x.GetAsync(_parkingSpotId), Times.Once);
            _mapper.Verify(x => x.Map<ParkingSpotResponse>(_parkingSpot), Times.Once);
        }

        [Fact]
        public void GetVehicleByVehicleId_HappyPath()
        {
            // Arrange
            var expectedVehicle = _fixture.Create<VehicleResponse>();

            _vehicleRepository.Setup(x => x.GetAsync(_vehicleId)).ReturnsAsync(_vehicle);
            _uow.Setup(x => x.GetRepository<Vehicle>()).Returns(_vehicleRepository.Object);
            _mapper.Setup(x => x.Map<VehicleResponse>(_vehicle)).Returns(expectedVehicle);

            // Act
            var result = _v.GetVehicleByIdAsync(_vehicleId).GetAwaiter().GetResult();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedVehicle, result);
            _vehicleRepository.Verify(x => x.GetAsync(_vehicleId), Times.Once);
            _mapper.Verify(x => x.Map<VehicleResponse>(_vehicle), Times.Once);
        }

        [Fact]
        public void GetTotalParkFeeByVehicleId_HappyPath()
        {
            // Arrange
            var expectedVehicle = _fixture.Create<VehicleResponse>();

            _vehicleRepository.Setup(x => x.GetAsync(_vehicleId)).ReturnsAsync(_vehicle);
            _uow.Setup(x => x.GetRepository<Vehicle>()).Returns(_vehicleRepository.Object);
            _mapper.Setup(x => x.Map<VehicleResponse>(_vehicle)).Returns(expectedVehicle);

            // Act
            var result = _v.GetTotalParkingSpotPriceVehicleIdAsync(_vehicleId).GetAwaiter().GetResult();

            // Assert
            Assert.NotNull(result);
            _vehicleRepository.Verify(x => x.GetAsync(_vehicleId), Times.Once);
            _mapper.Verify(x => x.Map<VehicleResponse>(_vehicle), Times.Once);
        }

        [Fact]
        public void CreateVehicle_HappyPath()
        {
            // Arrange
            _vehicleRepository.Setup(x => x.FindAsync(v => v.LicensePlate == _vehicleRequest.LicensePlate))
                              .Returns(Task.FromResult<Vehicle>(null));
            _vehicleRepository.Setup(x => x.AddAsync(It.IsAny<Vehicle>())).ReturnsAsync(_vehicle);
            _mapper.Setup(x => x.Map<Vehicle>(_vehicleRequest)).Returns(_vehicle);
            _mapper.Setup(x => x.Map<VehicleRequest>(_vehicle)).Returns(_vehicleRequest);
            _uow.Setup(x => x.GetRepository<Vehicle>()).Returns(_vehicleRepository.Object);

            // Act
            var result = _v.CreateVehicleAsync(_vehicleRequest).GetAwaiter().GetResult();

            // Assert
            Assert.NotNull(result);
            _vehicleRepository.Verify(x => x.AddAsync(It.IsAny<Vehicle>()), Times.Once);
            _mapper.Verify(x => x.Map<Vehicle>(_vehicleRequest), Times.Once);
            _mapper.Verify(x => x.Map<VehicleRequest>(_vehicle), Times.Once);
        }
    }
}
