using AutoMapper;
using SunNext.Data.Models;
using SunNext.Services.Data.Prototypes.Simulation;

namespace SunNext.Web;

public class SimulatoionProfile : Profile
{
    public SimulatoionProfile()
    {
        CreateMap<SolarSimulationData, SolarSimulationDataPrototype>();
    }
}