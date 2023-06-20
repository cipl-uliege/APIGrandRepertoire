using Repertory.Dtos;
using Repertory.Repositories;
using Repertory.Utils;
using Repertory.Utils.Generics;

namespace Repertory.Services {
    public class CategoryService {

        private readonly CategoryRepository _categoryRepository;
        public CategoryService(CategoryRepository categoryRepository) {
            _categoryRepository = categoryRepository;
        }

        public async Task<PaginatedResults<IEnumerable<Category>>> GetSelectedCategoriesAsync(bool includeOrchestraEnsemble, int nbrInstrumentalists, int page, int resultsPerPage, string instrumentFamiliesToInclude) {
            string[] instrumentFamilies = instrumentFamiliesToInclude.Split('_');
            PaginatedResults<IEnumerable<Category>> paginatedResults = new PaginatedResults<IEnumerable<Category>>() {
                TotalNbrOfResults = await _categoryRepository.CountSelectedCategoriesAsync(includeOrchestraEnsemble, nbrInstrumentalists, instrumentFamilies),
                ResultsPerPage = PaginationTools.CorrectResultsPerPage(resultsPerPage)
            };

            //Si aucun résultat, je retourne un Enumerable empty.
            if(paginatedResults.TotalNbrOfResults == 0) {
                await _categoryRepository.DisposeConnectionAsync();
                paginatedResults.Results = Enumerable.Empty<Category>();
                return paginatedResults;
            }

            paginatedResults.MaxNbrOfPages = (int)Math.Ceiling(paginatedResults.TotalNbrOfResults / (double)paginatedResults.ResultsPerPage);
            paginatedResults.Page = PaginationTools.CorrectPage(page, paginatedResults.MaxNbrOfPages);

            paginatedResults.Results = await _categoryRepository.GetSelectedCategoriesAsync(includeOrchestraEnsemble, nbrInstrumentalists, paginatedResults.Page, paginatedResults.ResultsPerPage, instrumentFamilies).ToListAsync();
            
            return paginatedResults;
        }
    }
}
