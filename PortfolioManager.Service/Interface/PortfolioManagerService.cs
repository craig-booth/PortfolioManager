namespace PortfolioManager.Service.Interface
{
    public interface IPortfolioService
    {

    }

    public interface IPortfolioManagerService
    {
        T GetService<T>() where T : IPortfolioService;
    }

}
