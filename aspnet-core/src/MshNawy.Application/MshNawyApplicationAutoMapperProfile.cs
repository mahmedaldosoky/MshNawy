using AutoMapper;
using MshNawy.Application.Contracts.Identity;
using MshNawy.Application.Contracts.Identity.Admin;
using MshNawy.Domain.Identity;

namespace MshNawy.Application;

public class MshNawyApplicationAutoMapperProfile : Profile
{
    public MshNawyApplicationAutoMapperProfile()
    {
        CreateMap<AppUser, KycStatusResponseDto>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.KycStatus))
            .ForMember(d => d.RejectionReason, o => o.MapFrom(s => s.KycRejectionReason))
            .ForMember(d => d.SubmittedAt, o => o.MapFrom(s => s.KycSubmittedAt));

        CreateMap<AppUser, AdminKycSubmissionDto>()
            .ForMember(d => d.UserId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.KycStatus))
            .ForMember(d => d.SubmittedAt, o => o.MapFrom(s => s.KycSubmittedAt))
            .ForMember(d => d.RejectionReason, o => o.MapFrom(s => s.KycRejectionReason))
            .ForMember(d => d.NationalIdFrontToken, o => o.MapFrom(s => s.NationalIdFrontImagePath))
            .ForMember(d => d.NationalIdBackToken, o => o.MapFrom(s => s.NationalIdBackImagePath))
            .ForMember(d => d.SelfieToken, o => o.MapFrom(s => s.SelfiePath));
    }
}
