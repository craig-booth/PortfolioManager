using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Service.Interface;

namespace PortfolioManager.Service.Rest
{
    class AttachmentService : IAttachmentService
    {
        private Local.LocalPortfolioManagerService _LocalService;
        private string _PortfolioDatabasePath;
        private string _StockDatabasePath;

        public AttachmentService(Local.LocalPortfolioManagerService localService, string portfolioDatabasePath, string stockDatabasePath)
        {
            _LocalService = localService;
            _PortfolioDatabasePath = portfolioDatabasePath;
            _StockDatabasePath = stockDatabasePath;
        }

        public async Task<AddAttachmentResponse> AddAttachment(string name, Stream stream)
        {
            await _LocalService.Connect(_PortfolioDatabasePath, _StockDatabasePath);

            var service = _LocalService.GetService<IAttachmentService>();

            return await service.AddAttachment(name, stream);
        }

        public async Task<ServiceResponce> DeleteAttachment(Guid id)
        {
            await _LocalService.Connect(_PortfolioDatabasePath, _StockDatabasePath);

            var service = _LocalService.GetService<IAttachmentService>();

            return await service.DeleteAttachment(id);
        }

        public async Task<GetAttachmentResponse> GetAttachment(Guid id)
        {
            await _LocalService.Connect(_PortfolioDatabasePath, _StockDatabasePath);

            var service = _LocalService.GetService<IAttachmentService>();

            return await service.GetAttachment(id);
        }
    }
}
