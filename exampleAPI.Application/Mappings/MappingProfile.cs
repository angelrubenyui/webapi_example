using AutoMapper;
using MiApi.Application.Commands;
using MiApi.Application.DTOs;
using MiApi.Domain.Entities;

namespace MiApi.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        
        CreateMap<Employee, EmployeeDto>()
            .MaxDepth(3)
            .ReverseMap();
        CreateMap<CreateEmployeeCommand, Employee>();
        CreateMap<UpdateEmployeeCommand, Employee>();
    }
}