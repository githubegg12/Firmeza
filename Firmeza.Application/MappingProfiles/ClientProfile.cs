using AutoMapper;
using Firmeza.Domain.Entities;

namespace Firmeza.Application.MappingProfiles;

/// <summary>
/// AutoMapper profile for Client entity mappings
/// </summary>
public class ClientProfile : Profile
{
    public ClientProfile()
    {
        // Client -> ClientDto
        CreateMap<Client, Application.DTOs.ClientDto>();
        
        // CreateClientDto -> Client
        CreateMap<Application.DTOs.Client.CreateClientDto, Client>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Sales, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore());
        
        // UpdateClientDto -> Client
        CreateMap<Application.DTOs.Client.UpdateClientDto, Client>()
            .ForMember(dest => dest.Sales, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore());
    }
}

