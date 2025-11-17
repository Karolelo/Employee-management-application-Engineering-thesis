using Repo.Core.Models;
using Repo.Core.Models.api;
using Repo.Core.Models.user.annoucement;
using Repo.Server.UserManagmentModule.Interfaces;

namespace Repo.Server.UserManagmentModule.Services;

public class AnnouncementService : IAnnoucementService
{
    private readonly IAnnoucementRepository _announcementRepository;
    private readonly IGroupRepository _groupRepository;
    
    public AnnouncementService(IAnnoucementRepository announcementRepository,IGroupRepository groupRepository)
    {
        _announcementRepository = announcementRepository;
        _groupRepository = groupRepository;
    }
    
    public async Task<Response<List<AnnoucementDTO>>> GetAllAnnouncements(int groupId)
    {
        try
        {
            if(await _groupRepository.GetGroupById(groupId) is null)
                return Response<List<AnnoucementDTO>>.Fail("Group does not exist");
            var announcements = await _announcementRepository.GetAllAnnouncements(groupId);
            var result = announcements.Select(MapAnnoucementToDTO).ToList();
            
            return Response<List<AnnoucementDTO>>.Ok(result);
        }
        catch (Exception ex)
        {
            return Response<List<AnnoucementDTO>>.Fail(ex.Message);
        }
    }

    public async Task<Response<AnnoucementDTO>> AddAnnouncement(CreateAnnoucementDTO announcement)
    {
        try
        {
            var annoucementToCreate = new Announcement()
            {
                Title = announcement.Title,
                Group_ID = announcement.Group_ID,
                Text = announcement.Text,
            };  
            
          var result = await _announcementRepository.AddAnnouncement(annoucementToCreate);
          return Response<AnnoucementDTO>.Ok(MapAnnoucementToDTO(result));
        }
        catch(Exception ex)
        {
            return Response<AnnoucementDTO>.Fail(ex.Message); 
        }
    }

    public async Task<Response<AnnoucementDTO>> UpdateAnnouncement(UpdateAnnoucementDTO announcement)
    {
        try
        {
            var annoucementToUpdate = await _announcementRepository.GetAnnoucementById(announcement.ID);
            if (annoucementToUpdate == null)
                return Response<AnnoucementDTO>.Fail("No annoucement found");
            
            annoucementToUpdate.Title = announcement.Title;
            annoucementToUpdate.Text = announcement.Text;
            var result = await _announcementRepository.UpdateAnnouncement(annoucementToUpdate);
            
            return Response<AnnoucementDTO>.Ok(MapAnnoucementToDTO(result));
        }
        catch (Exception ex)
        {
            return Response<AnnoucementDTO>.Fail(ex.Message);
        }
    }

    public async Task<Response<bool>> DeleteAnnouncement(int id)
    {
        try
        {
            var result = await _announcementRepository.DeleteAnnouncement(id);
            
            return Response<bool>.Ok(result);
        }
        catch (Exception e)
        {
            return Response<bool>.Fail(e.Message);
        }
    }

    private AnnoucementDTO MapAnnoucementToDTO(Announcement announcement)
    {
        return new AnnoucementDTO()
        {
            ID = announcement.ID,
            Title = announcement.Title ?? "",
            Group_ID = announcement.Group_ID??0,
            Text = announcement.Text?? "" , 
        };
    }
}