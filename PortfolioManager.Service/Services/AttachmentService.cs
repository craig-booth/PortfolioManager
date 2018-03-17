using System;
using System.Threading.Tasks;
using System.IO;
using PortfolioManager.Service.Interface;
using PortfolioManager.Data.Portfolios;

namespace PortfolioManager.Service.Services
{
    public class AttachmentService : IAttachmentService
    {
        private readonly IPortfolioDatabase _PortfolioDatabase;

        public AttachmentService(IPortfolioDatabase portfolioDatabase)
        {
            _PortfolioDatabase = portfolioDatabase;
        }

        public Task<AddAttachmentResponse> AddAttachment(string name, Stream stream)
        {
            var responce = new AddAttachmentResponse();

            var attachment = new Attachment()
            {
                Extension = Path.GetExtension(name)
            };

            stream.CopyTo(attachment.Data);
            attachment.Data.Seek(0, SeekOrigin.Begin);

            using (var unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                unitOfWork.AttachmentRepository.Add(attachment);

                unitOfWork.Save();
            }

            responce.Id = attachment.Id;
            responce.SetStatusToSuccessfull();

            return Task.FromResult<AddAttachmentResponse>(responce);

        }

        public Task<GetAttachmentResponse> GetAttachment(Guid id)
        {
            var responce = new GetAttachmentResponse();

            Attachment attachment;

            using (var unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                attachment = unitOfWork.AttachmentRepository.Get(id);
            }

            responce.Id = attachment.Id;
            responce.Name = attachment.Extension;
            responce.Data = attachment.Data;

            responce.SetStatusToSuccessfull();

            return Task.FromResult<GetAttachmentResponse>(responce);
        }

        public Task<ServiceResponce> DeleteAttachment(Guid id)
        {
            var responce = new ServiceResponce();

            using (var unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                unitOfWork.AttachmentRepository.Delete(id);

                unitOfWork.Save();
            }

            responce.SetStatusToSuccessfull();

            return Task.FromResult<ServiceResponce>(responce);

        }

    }
}
