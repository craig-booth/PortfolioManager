using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Service
{
    public class AttachmentService
    {
        private Dictionary<Guid, Attachment> _Attachments;

        internal AttachmentService()
        {
            _Attachments = new Dictionary<Guid, Attachment>();
        }

        public Attachment CreateAttachment(Stream stream)
        {
            var attachment = new Attachment();

            stream.CopyTo(attachment.Data);
            
            return attachment;
        }

        public Attachment CreateAttachment(string fileName)
        {
            var file = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            var attachement = CreateAttachment(file);

            file.Close();

            return attachement;
        }

        public void DeleteAttachment(Guid id)
        {
            _Attachments.Remove(id);
        }

        public Attachment GetAttachment(Guid id)
        {
            if (_Attachments.ContainsKey(id))
                return _Attachments[id];
            else
                throw new RecordNotFoundException(id);
        }
    }
}
