using Microsoft.AspNetCore.Mvc;
using Repertory.Dtos;
using Repertory.Repositories;

namespace Repertory.Services {
    public class GroupService {

        private readonly GroupRepository _groupRepository;

        public GroupService(GroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        public async Task<IEnumerable<Group>> GetFromCategoryAsync(int id) {
            return await _groupRepository.GetFromCategoryAsync(id).ToListAsync();
        }

        public async Task<string> GetCompositionByIdAsync(long groupId) {
            return await _groupRepository.GetCompositionByIdAsync(groupId);
        }
    }
}
