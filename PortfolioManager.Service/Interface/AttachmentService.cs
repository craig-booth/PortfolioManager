using System;
using System.Threading.Tasks;
using System.IO;

namespace PortfolioManager.Service.Interface
{
    public interface IAttachmentService : IPortfolioService
    {
        Task<AddAttachmentResponse> AddAttachment(string name, Stream stream);
        Task<GetAttachmentResponse> GetAttachment(Guid id);
        Task<ServiceResponce> DeleteAttachment(Guid id);
    }

    public class AddAttachmentResponse : ServiceResponce
    {
        public Guid Id { get; set; }
   
        public AddAttachmentResponse()
            : base()
        {

        }
    }

    public class GetAttachmentResponse : ServiceResponce
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Stream Data { get; set; }

        public GetAttachmentResponse()
            : base()
        {
           
        }
    }

}
