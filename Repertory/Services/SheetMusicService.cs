using Microsoft.AspNetCore.Mvc;
using Repertory.Dtos;
using Repertory.Repositories;
using Repertory.Utils.Generics;
using Repertory.Utils;
using System.Text.RegularExpressions;

namespace Repertory.Services {
    public class SheetMusicService {

        private readonly SheetMusicRepository _sheetMusicRepository;

        public SheetMusicService(SheetMusicRepository sheetMusicRepository) {
            _sheetMusicRepository = sheetMusicRepository;
        }
        public async Task<PaginatedResults<IEnumerable<SheetMusic>>> GetFromGroupAsync(long groupId, int page, string author, string title, int resultsPerPage) {
            PaginatedResults<IEnumerable<SheetMusic>> paginatedResults = new PaginatedResults<IEnumerable<SheetMusic>>() {
                TotalNbrOfResults = await _sheetMusicRepository.CountSheetMusicFromGroupAsync(groupId, author, title),
                ResultsPerPage = PaginationTools.CorrectResultsPerPage(resultsPerPage)
            };

            //Si aucun résultat, je retourne un Enumerable empty.
            if (paginatedResults.TotalNbrOfResults == 0) {
                await _sheetMusicRepository.DisposeConnectionAsync();
                paginatedResults.Results = Enumerable.Empty<SheetMusic>();
                return paginatedResults;
            }

            paginatedResults.MaxNbrOfPages = PaginationTools.GetMaxNbrOfPages(paginatedResults.TotalNbrOfResults, paginatedResults.ResultsPerPage);
            paginatedResults.Page = PaginationTools.CorrectPage(page, paginatedResults.MaxNbrOfPages);

            paginatedResults.Results = await _sheetMusicRepository.GetFromGroupAsync(groupId, paginatedResults.Page, author, title, paginatedResults.ResultsPerPage).ToListAsync();

            return paginatedResults;
        }

        public async Task<PaginatedResults<IEnumerable<SheetMusicWithComposition>>> GetFromAuthorOrTitleAsync(string author, string title, int resultsPerPage, int page, int nbrInstrumentalists) {
            PaginatedResults<IEnumerable<SheetMusicWithComposition>> paginatedResults = new PaginatedResults<IEnumerable<SheetMusicWithComposition>>() {
                TotalNbrOfResults = await _sheetMusicRepository.CountSheetMusicFromAuthorOrtitle(author, title, nbrInstrumentalists),
                ResultsPerPage = PaginationTools.CorrectResultsPerPage(resultsPerPage)
            };

            //Si aucun résultat, je retourne un Enumerable empty.
            if (paginatedResults.TotalNbrOfResults == 0) {
                await _sheetMusicRepository.DisposeConnectionAsync();
                paginatedResults.Results = Enumerable.Empty<SheetMusicWithComposition>();
                return paginatedResults;
            }

            paginatedResults.MaxNbrOfPages = PaginationTools.GetMaxNbrOfPages(paginatedResults.TotalNbrOfResults, paginatedResults.ResultsPerPage);
            paginatedResults.Page = PaginationTools.CorrectPage(page, paginatedResults.MaxNbrOfPages);

            paginatedResults.Results = await _sheetMusicRepository.GetFromAuthorOrTitle(paginatedResults.Page, author, title, paginatedResults.ResultsPerPage, nbrInstrumentalists).ToListAsync();

            return paginatedResults;
        }

        public async Task<PaginatedResults<IEnumerable<SheetMusicWithComposition>>> GetFavoritesAsync(List<int> favoriteIds, int page, int resultsPerPage, string author, string title, int nbrInstrumentalists) {
            favoriteIds = favoriteIds.Distinct().ToList();
            PaginatedResults<IEnumerable<SheetMusicWithComposition>> paginatedResults = new PaginatedResults<IEnumerable<SheetMusicWithComposition>>() {
                TotalNbrOfResults = await _sheetMusicRepository.CountSheetMusicFromFavorites(favoriteIds, author, title, nbrInstrumentalists),
                ResultsPerPage = PaginationTools.CorrectResultsPerPage(resultsPerPage)
            };

            //Si aucun résultat, je retourne un Enumerable empty.
            if (paginatedResults.TotalNbrOfResults == 0) {
                await _sheetMusicRepository.DisposeConnectionAsync();
                paginatedResults.Results = Enumerable.Empty<SheetMusicWithComposition>();
                return paginatedResults;
            }

            paginatedResults.MaxNbrOfPages = PaginationTools.GetMaxNbrOfPages(paginatedResults.TotalNbrOfResults, paginatedResults.ResultsPerPage);
            paginatedResults.Page = PaginationTools.CorrectPage(page, paginatedResults.MaxNbrOfPages);
            paginatedResults.Results = await _sheetMusicRepository.GetFavorites(favoriteIds, paginatedResults.Page, paginatedResults.ResultsPerPage, author, title, nbrInstrumentalists).ToListAsync();
            return paginatedResults;
        }
    }
}
