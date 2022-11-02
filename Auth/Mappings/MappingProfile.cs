using AutoMapper;
using Flamma.Auth.Models;
using Google.Protobuf.WellKnownTypes;

namespace Flamma.Auth.Mappings;

/// <summary>
///     Application mapping profile
/// </summary>
public class MappingProfile : Profile
{
    /// <summary>
    ///     .ctor
    /// </summary>
    public MappingProfile()
    {
        // Grpc user info to BL user information
        CreateMap<UserInfo, UserInformation>()
            .ForMember(dest => dest.PrimaryLocationId, mo => mo.MapFrom(src => Guid.Parse(src.PrimaryLocationId)))
            .ForMember(dest => dest.BirthDate, mo => mo.MapFrom(src => src.BirthDate.ToDateTime().ToUniversalTime()));

        // Grpc registration request to BL registration request
        CreateMap<RegisterRequest, Models.RegisterRequest>()
            .ForMember(dest => dest.UserInformation, mo => mo.MapFrom(src => src.UserInfo));

        // BL registration result to Grpc reply
        CreateMap<Models.RegisterResult, RegisterReply>()
            .ForMember(dest => dest.Status,
                mo => mo.MapFrom(src => src.Success ? CommonStatus.Success : CommonStatus.Fail));

        // BL registration information to Data.Access entity
        CreateMap<Models.UserInformation, Data.Access.Models.AdditionalUserInformation>();
        CreateMap<Models.RegisterRequest, Data.Access.Models.UserData>()
            .ForMember(dest => dest.AdditionalUserInformation, mo => mo.MapFrom(src => src.UserInformation))
            .ForMember(dest => dest.PasswordHash, mo => mo.Ignore());

        CreateMap<Models.LoginResult, LoginResponse>()
            .ForMember(dest => dest.Status,
                mo => mo.MapFrom(src => src.Success ? CommonStatus.Success : CommonStatus.Fail));
        CreateMap<Models.JwtTokenInfo, TokenInfo>()
            .ForMember(dest => dest.TokenValidTo,
                mo => mo.MapFrom(src => src.TokenValidTo.ToTimestamp()));
    }
}