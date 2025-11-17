using Repo.Core.Models.api;
using Repo.Core.Models.user.annoucement;

namespace Repo.Server.UserManagmentModule.Interfaces;

public interface IAnnoucementService
{
    Task<Response<List<AnnoucementDTO>>> GetAllAnnouncements(int groupId);
    Task<Response<AnnoucementDTO>> AddAnnouncement(CreateAnnoucementDTO announcement);
    Task<Response<AnnoucementDTO>> UpdateAnnouncement(UpdateAnnoucementDTO announcement);
    Task<Response<bool>> DeleteAnnouncement(int id);
}