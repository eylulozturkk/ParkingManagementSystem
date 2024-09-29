using AutoMapper;
using ParkingManagementSystem.BL.Dto.Request;
using ParkingManagementSystem.BL.Dto.Response;
using ParkingManagementSystem.DAL.Entity;
using System;

namespace ParkingManagementSystem.BL.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            #region VehicleParkingSpotMappings
            CreateMap<VehicleParkingSpotMapping, ParkingSpotVehicleMappingRequest>().ReverseMap();
            CreateMap<VehicleParkingSpotMapping, ParkingSpotVehicleMappingResponse>().ReverseMap();

            CreateMap<ParkingSpotVehicleMappingRequest, ParkingSpotVehicleMappingResponse>().ReverseMap();
            #endregion 

            #region Vehicles
            CreateMap<Vehicle, VehicleRequest>().ReverseMap();
            CreateMap<Vehicle, VehicleResponse>().ReverseMap();

            CreateMap<VehicleRequest, VehicleResponse>().ReverseMap();
            #endregion

            #region PriceParkingSpotMappings

            CreateMap<PriceParkingSpotMapping, ParkingSpotPriceRequest>().ReverseMap();
            CreateMap<PriceParkingSpotMapping, ParkingSpotPriceResponse>().ReverseMap();

            CreateMap<ParkingSpotPriceRequest, ParkingSpotPriceResponse>().ReverseMap();
            #endregion

            #region ParkingSpots
            CreateMap<ParkingSpot, ParkingSpotRequest>().ReverseMap();
            CreateMap<ParkingSpot, ParkingSpotResponse>().ReverseMap();

            CreateMap<ParkingSpotRequest, ParkingSpotResponse>().ReverseMap();
            #endregion
        }
    }
}
