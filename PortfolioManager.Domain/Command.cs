using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.Domain
{
    public interface ICommand
    {
    }

    public interface ICommandHandler<T> where T : ICommand
    {
        void Execute(T command);
    }
}
