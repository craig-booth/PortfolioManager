using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Service.Obsolete
{
    class AttachmentService
    {
        private readonly IPortfolioDatabase _PortfolioDatabase;

        internal AttachmentService(IPortfolioDatabase portfolioDatabase)
        {
            _PortfolioDatabase = portfolioDatabase;
        }

        public Attachment CreateAttachment(string extension, Stream stream)
        {
            var attachment = new Attachment()
            {
                Extension = extension
            };            

            stream.CopyTo(attachment.Data);
            attachment.Data.Seek(0, SeekOrigin.Begin);

            using (var unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                unitOfWork.AttachmentRepository.Add(attachment);

                unitOfWork.Save();
            }

            return attachment;
        }

        public Attachment CreateAttachment(string fileName)
        {
            var file = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            var attachement = CreateAttachment(Path.GetExtension(fileName), file);

            file.Close();

            return attachement;
        }

        public void DeleteAttachment(Guid id)
        {
            using (var unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                unitOfWork.AttachmentRepository.Delete(id);

                unitOfWork.Save();
            }
        }

        public Attachment GetAttachment(Guid id)
        {
            Attachment attachment;

            using (var unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                attachment = unitOfWork.AttachmentRepository.Get(id);
            }

            return attachment;
        }
    }
}
