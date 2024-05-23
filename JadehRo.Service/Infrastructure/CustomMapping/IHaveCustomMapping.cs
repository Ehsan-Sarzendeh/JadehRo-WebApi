using AutoMapper;

namespace JadehRo.Service.Infrastructure.CustomMapping;

public interface IHaveCustomMapping
{
    void CreateMappings(Profile profile);
}