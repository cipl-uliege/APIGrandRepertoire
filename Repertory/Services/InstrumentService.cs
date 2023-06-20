using Microsoft.AspNetCore.Mvc;
using Repertory.Dtos;
using Repertory.Repositories;
using Repertory.Utils;

namespace Repertory.Services {
    public class InstrumentService {

        private readonly InstrumentRepository _instrumentRepository;
        public InstrumentService(InstrumentRepository instrumentRepository)
        {
            _instrumentRepository = instrumentRepository;
        }

        public async Task<IEnumerable<Instrument>> GetFullNameOfCompositionAsync(string composition) {
            IEnumerable<string> abreviations = await InstrumentTools.RetrieveOnlyAbreviationAsync(composition);
            return await _instrumentRepository.GetFullNameOfCompositionAsync(abreviations.ToList()).ToListAsync();
        }
    }
}
