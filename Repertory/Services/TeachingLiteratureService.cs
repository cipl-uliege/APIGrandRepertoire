using Repertory.Dtos;
using Repertory.Repositories;
using Repertory.Utils;
using Repertory.Utils.Generics;
using System.Text.RegularExpressions;

namespace Repertory.Services {
    public class TeachingLiteratureService {
        private readonly TeachingLiteratureRepository _teachingLiteratureRepository;

        public TeachingLiteratureService(TeachingLiteratureRepository teachingLiteratureRepository)
        {
            _teachingLiteratureRepository = teachingLiteratureRepository;
        }

        public async Task<IEnumerable<TeachingLiteratureCategory>> GetCategoryAndCountTitle() {
            return await _teachingLiteratureRepository.GetCategoryAndCountTitle().ToListAsync();
        }

        public async Task<PaginatedResults<IEnumerable<TeachingLiterature>>> GetFromGroupAsync(long groupId, int page, int resultsPerPage, string author, string title) {
            PaginatedResults<IEnumerable<TeachingLiterature>> paginatedResults = new PaginatedResults<IEnumerable<TeachingLiterature>>() {
                TotalNbrOfResults = await _teachingLiteratureRepository.CountSheetMusicFromGroupAsync(author, title, groupId),
                ResultsPerPage = PaginationTools.CorrectResultsPerPage(resultsPerPage)
            };

            //Si aucun résultat, je retourne un Enumerable empty.
            if (paginatedResults.TotalNbrOfResults == 0) {
                await _teachingLiteratureRepository.DisposeConnectionAsync();
                paginatedResults.Results = Enumerable.Empty<TeachingLiterature>();
                return paginatedResults;
            }

            paginatedResults.MaxNbrOfPages = PaginationTools.GetMaxNbrOfPages(paginatedResults.TotalNbrOfResults, paginatedResults.ResultsPerPage);
            paginatedResults.Page = PaginationTools.CorrectPage(page, paginatedResults.MaxNbrOfPages);

            paginatedResults.Results = await _teachingLiteratureRepository.GetFromGroupAsync(groupId, paginatedResults.Page, paginatedResults.ResultsPerPage, author, title).ToListAsync();

            return paginatedResults;
        }
    }
}
