using Repertory.Utils.Generics;

namespace Repertory.Utils {
    public static class PaginationTools {
        public static int CorrectResultsPerPage(int resultsPerPage) {
            if (resultsPerPage < 1) {
                resultsPerPage = PaginationParameters.DEFAULT_RESULT;
            } else if (resultsPerPage > PaginationParameters.MAX_RESULT)
                resultsPerPage = PaginationParameters.MAX_RESULT;

            return resultsPerPage;
        }

        public static int CorrectPage(int page, int maxNbrOfPages) {
            if (page < 1)
                return 1;

            if (page > maxNbrOfPages)
                return maxNbrOfPages;

            return page;
        }

        public static int GetMaxNbrOfPages(int totalNbrOfResults, int resultsPerPage) {
            return (int)Math.Ceiling(totalNbrOfResults / (double)resultsPerPage);
        }
    }
}
